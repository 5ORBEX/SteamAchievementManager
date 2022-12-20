/* Copyright (c) 2019 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using SAM.API.Resources;
using SAM.API.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml.XPath;
using APITypes = SAM.API.Types;

namespace SAM.Picker
{
    internal partial class GamePicker : Form
    {
        private readonly API.Client _SteamClient;

        private readonly Dictionary<uint, GameInfo> _Games;
        private readonly List<GameInfo> _FilteredGames;
        private int _SelectedGameIndex;

        private readonly List<string> _LogosAttempted;
        private readonly ConcurrentQueue<GameInfo> _LogoQueue;

        // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
        private readonly API.Callbacks.AppDataChanged _AppDataChangedCallback;
        // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

        public GamePicker(API.Client client)
        {
            this._Games = new Dictionary<uint, GameInfo>();
            this._FilteredGames = new List<GameInfo>();
            this._SelectedGameIndex = -1;
            this._LogosAttempted = new List<string>();
            this._LogoQueue = new ConcurrentQueue<GameInfo>();

            currentAppLanguage = ResourcesUI.AppLanguage;
            currentGameLanguage = ResourcesUI.GameLanguage;

            this.InitializeComponent();

            this._SettingsAppLangEnMenuItem.Checked = currentAppLanguage == LangType.EN;
            this._SettingsAppLangRuMenuItem.Checked = currentAppLanguage == LangType.RU;
            this._SettingsAppLangUaMenuItem.Checked = currentAppLanguage == LangType.UA;

            this._SettingsGameLangEnMenuItem.Checked = currentGameLanguage == LangType.EN;
            this._SettingsGameLangRuMenuItem.Checked = currentGameLanguage == LangType.RU;
            this._SettingsGameLangUaMenuItem.Checked = currentGameLanguage == LangType.UA;

            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = Path.GetDirectoryName(strExeFilePath);

            var watcher = new FileSystemWatcher(strWorkPath);
            watcher.NotifyFilter = NotifyFilters.Attributes
                | NotifyFilters.CreationTime
                | NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.Security
                | NotifyFilters.Size;

            watcher.Changed += OnFileChanged;
            watcher.Created += OnFileCreated;
            watcher.Filter = ResourcesUI.SettingsFileName;
            watcher.EnableRaisingEvents = true;

            var blank = new Bitmap(this._LogoImageList.ImageSize.Width, this._LogoImageList.ImageSize.Height);
            using (var g = Graphics.FromImage(blank))
            {
                g.Clear(Color.DimGray);
            }

            this._LogoImageList.Images.Add("Blank", blank);

            this._SteamClient = client;

            this._AppDataChangedCallback = client.CreateAndRegisterCallback<API.Callbacks.AppDataChanged>();
            this._AppDataChangedCallback.OnRun += this.OnAppDataChanged;

            this.AddGames();
        }

        private void OnAppDataChanged(APITypes.AppDataChanged param)
        {
            if (param.Result == true && this._Games.ContainsKey(param.Id))
            {
                var game = this._Games[param.Id];

                game.Name = this._SteamClient.SteamApps001.GetAppData(game.Id, "name");
                this.AddGameToLogoQueue(game);
                this.DownloadNextLogo();
            }
        }

        private void DoDownloadList(object sender, DoWorkEventArgs e)
        {
            var pairs = new List<KeyValuePair<uint, string>>();
            byte[] bytes;
            using (var downloader = new WebClient())
            {
                bytes = downloader.DownloadData(new Uri("http://gib.me/sam/games.xml"));
            }
            using (var stream = new MemoryStream(bytes, false))
            {
                var document = new XPathDocument(stream);
                var navigator = document.CreateNavigator();
                var nodes = navigator.Select("/games/game");
                while (nodes.MoveNext())
                {
                    string type = nodes.Current.GetAttribute("type", "");
                    if (string.IsNullOrEmpty(type) == true)
                    {
                        type = "normal";
                    }
                    pairs.Add(new KeyValuePair<uint, string>((uint)nodes.Current.ValueAsLong, type));
                }
            }

            foreach (var kv in pairs)
            {
                this.AddGame(kv.Key, kv.Value);
            }
        }

        private void OnDownloadList(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled)
            {
                this.AddDefaultGames();
            }

            this.RefreshGames();
            this._RefreshGamesButton.Enabled = true;
            this.DownloadNextLogo();
        }

        private void RefreshGames()
        {
            this._SelectedGameIndex = -1;
            this._FilteredGames.Clear();
            foreach (var info in this._Games.Values.OrderBy(gi => gi.Name))
            {
                if (info.Type == "normal" && _FilterGamesMenuItem.Checked == false)
                {
                    continue;
                }
                if (info.Type == "demo" && this._FilterDemosMenuItem.Checked == false)
                {
                    continue;
                }
                if (info.Type == "mod" && this._FilterModsMenuItem.Checked == false)
                {
                    continue;
                }
                if (info.Type == "junk" && this._FilterJunkMenuItem.Checked == false)
                {
                    continue;
                }
                this._FilteredGames.Add(info);
                this.AddGameToLogoQueue(info);
            }

            this._GameListView.VirtualListSize = this._FilteredGames.Count;
            this._PickerStatusLabel.Text = string.Format(ResourcesUI.DISPLAY_MSG, this._GameListView.Items.Count, this._Games.Count);
        }

        private void OnGameListViewRetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var info = this._FilteredGames[e.ItemIndex];
            e.Item = new ListViewItem()
            {
                Text = info.Name,
                ImageIndex = info.ImageIndex,
            };
        }

        private void OnGameListViewSearchForVirtualItem(object sender, SearchForVirtualItemEventArgs e)
        {
            if (e.Direction != SearchDirectionHint.Down || e.IsTextSearch == false)
            {
                return;
            }

            var count = this._FilteredGames.Count;
            if (count < 2)
            {
                return;
            }

            var text = e.Text;
            int startIndex = e.StartIndex;

            Predicate<GameInfo> predicate;
            /*if (e.IsPrefixSearch == true)*/
            {
                predicate = gi => gi.Name != null && gi.Name.StartsWith(text, StringComparison.CurrentCultureIgnoreCase);
            }
            /*else
            {
                predicate = gi => gi.Name != null && string.Compare(gi.Name, text, StringComparison.CurrentCultureIgnoreCase) == 0;
            }*/

            int index;
            if (e.StartIndex >= count)
            {
                // starting from the last item in the list
                index = this._FilteredGames.FindIndex(0, startIndex - 1, predicate);
            }
            else if (startIndex <= 0)
            {
                // starting from the first item in the list
                index = this._FilteredGames.FindIndex(0, count, predicate);
            }
            else
            {
                index = this._FilteredGames.FindIndex(startIndex, count - startIndex, predicate);
                if (index < 0)
                {
                    index = this._FilteredGames.FindIndex(0, startIndex - 1, predicate);
                }
            }

            e.Index = index < 0 ? -1 : index;
        }

        private void DoDownloadLogo(object sender, DoWorkEventArgs e)
        {
            var info = (GameInfo)e.Argument;
            var logoPath = string.Format(
                CultureInfo.InvariantCulture,
                "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/{0}/{1}.jpg",
                info.Id,
                info.Logo);
            using (var downloader = new WebClient())
            {
                try
                {
                    var data = downloader.DownloadData(new Uri(logoPath));
                    using (var stream = new MemoryStream(data, false))
                    {
                        var bitmap = new Bitmap(stream);
                        e.Result = new LogoInfo(info.Id, bitmap);
                    }
                }
                catch (Exception)
                {
                    e.Result = new LogoInfo(info.Id, null);
                }
            }
        }

        private void OnDownloadLogo(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled == true)
            {
                return;
            }

            var logoInfo = (LogoInfo)e.Result;
            if (logoInfo.Bitmap != null && this._Games.TryGetValue(logoInfo.Id, out var gameInfo))
            {
                this._GameListView.BeginUpdate();
                var imageIndex = this._LogoImageList.Images.Count;
                this._LogoImageList.Images.Add(gameInfo.Logo, logoInfo.Bitmap);
                gameInfo.ImageIndex = imageIndex;
                this._GameListView.EndUpdate();
            }

            this.DownloadNextLogo();
        }

        private void DownloadNextLogo()
        {
            if (this._LogoWorker.IsBusy == true)
            {
                return;
            }

            GameInfo info;
            if (this._LogoQueue.TryDequeue(out info) == false)
            {
                this._DownloadStatusLabel.Visible = false;
                return;
            }

            this._DownloadStatusLabel.Text = string.Format(ResourcesUI.DOWNLOAD_GAMES_ICONS, this._LogoQueue.Count);
            this._DownloadStatusLabel.Visible = true;

            this._LogoWorker.RunWorkerAsync(info);
        }

        private void AddGameToLogoQueue(GameInfo info)
        {
            string logo = this._SteamClient.SteamApps001.GetAppData(info.Id, "logo");

            if (logo == null)
            {
                return;
            }

            info.Logo = logo;

            int imageIndex = this._LogoImageList.Images.IndexOfKey(logo);
            if (imageIndex >= 0)
            {
                info.ImageIndex = imageIndex;
            }
            else if (this._LogosAttempted.Contains(logo) == false)
            {
                this._LogosAttempted.Add(logo);
                this._LogoQueue.Enqueue(info);
            }
        }

        private bool OwnsGame(uint id)
        {
            return this._SteamClient.SteamApps008.IsSubscribedApp(id);
        }

        private void AddGame(uint id, string type)
        {
            if (this._Games.ContainsKey(id))
            {
                return;
            }

            if (this.OwnsGame(id) == false)
            {
                return;
            }

            var info = new GameInfo(id, type);
            info.Name = this._SteamClient.SteamApps001.GetAppData(info.Id, "name");

            this._Games.Add(id, info);
        }

        private void AddGames()
        {
            this._Games.Clear();
            this._RefreshGamesButton.Enabled = false;
            this._ListWorker.RunWorkerAsync();
        }

        private void AddDefaultGames()
        {
            this.AddGame(480, "normal"); // Spacewar
        }

        private void OnTimer(object sender, EventArgs e)
        {
            this._CallbackTimer.Enabled = false;
            this._SteamClient.RunCallbacks(false);
            this._CallbackTimer.Enabled = true;
        }

        private void OnSelectGame(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            this._SelectedGameIndex = e.ItemIndex;
        }

        private void OnActivateGame(object sender, EventArgs e)
        {
            var index = this._SelectedGameIndex;
            if (index < 0 || index >= this._FilteredGames.Count)
            {
                return;
            }

            var info = this._FilteredGames[index];
            if (info == null)
            {
                return;
            }

            try
            {
                Process.Start("SAM.Game.exe", info.Id.ToString(CultureInfo.InvariantCulture));
            }
            catch (Win32Exception)
            {
                MessageBox.Show(
                    this,
                    ResourcesUI.DLG_FAILED_START_SAM_GAME_ERROR,
                    "Steam Achievement Manager",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            this._AddGameTextBox.Text = "";
            this.AddGames();
        }

        private void OnAddGame(object sender, EventArgs e)
        {
            uint id;

            if (uint.TryParse(this._AddGameTextBox.Text, out id) == false)
            {
                MessageBox.Show(
                    this,
                    ResourcesUI.DLG_INVALID_ID_ERROR,
                    "Steam Achievement Manager",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (this.OwnsGame(id) == false)
            {
                MessageBox.Show(this, ResourcesUI.DLG_OWN_GAME_ERROR, "Steam Achievement Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            while (this._LogoQueue.TryDequeue(out var logo))
            {
                // clear the download queue because we will be showing only one app
                // TODO: https://github.com/gibbed/SteamAchievementManager/issues/106
                this._LogosAttempted.Remove(logo.Logo);
            }

            this._AddGameTextBox.Text = "";
            this._Games.Clear();
            this.AddGame(id, "normal");
            this._FilterGamesMenuItem.Checked = true;
            this.RefreshGames();
            this.DownloadNextLogo();
        }

        private void OnFilterUpdate(object sender, EventArgs e)
        {
            this.RefreshGames();
        }

        private void OnAppLanguageUpdate(object sender, EventArgs e)
        {
            foreach (var item in this._SettingsAppLangsMenuItem.DropDownItems)
            {
                if (item == sender)
                {
                    (item as ToolStripMenuItem).CheckState = CheckState.Checked;
                }
                else
                {
                    (item as ToolStripMenuItem).CheckState = CheckState.Unchecked;
                }
            }

            if (sender == this._SettingsAppLangRuMenuItem)
            {
                currentAppLanguage = LangType.RU;
            }
            else if (sender == this._SettingsAppLangUaMenuItem)
            {
                currentAppLanguage = LangType.UA;
            }
            else
            {
                currentAppLanguage = LangType.EN;
            }

            if (ResourcesUI.AppLanguage != currentAppLanguage)
            {
                ResourcesUI.AppLanguage = currentAppLanguage;
            }
        }

        private void OnGameLanguageUpdate(object sender, EventArgs e)
        {
            foreach (var item in this._SettingsGameLangsMenuItem.DropDownItems)
            {
                if (item == sender)
                {
                    (item as ToolStripMenuItem).CheckState = CheckState.Checked;
                }
                else
                {
                    (item as ToolStripMenuItem).CheckState = CheckState.Unchecked;
                }
            }

            if (sender == this._SettingsGameLangRuMenuItem)
            {
                currentGameLanguage = LangType.RU;
            }
            else if (sender == this._SettingsGameLangUaMenuItem)
            {
                currentGameLanguage = LangType.UA;
            }
            else
            {
                currentGameLanguage = LangType.EN;
            }

            if (ResourcesUI.GameLanguage != currentGameLanguage)
            {
                ResourcesUI.GameLanguage = currentGameLanguage;
            }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            dispatcher?.Invoke(() => {
                ResourcesUI.UpdateSettings();
                OnAppLanguageChanged();
                OnGameLanguageChanged();
            });
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            dispatcher?.Invoke(() => {
                ResourcesUI.UpdateSettings();
                OnAppLanguageChanged();
                OnGameLanguageChanged();
            });
        }

        private void OnAppLanguageChanged()
        {
            currentAppLanguage = ResourcesUI.AppLanguage;

            switch (currentAppLanguage)
            {
                case LangType.RU:
                    this._SettingsAppLangEnMenuItem.CheckState = CheckState.Unchecked;
                    this._SettingsAppLangRuMenuItem.CheckState = CheckState.Checked;
                    this._SettingsAppLangUaMenuItem.CheckState = CheckState.Unchecked;
                    break;
                case LangType.UA:
                    this._SettingsAppLangEnMenuItem.CheckState = CheckState.Unchecked;
                    this._SettingsAppLangRuMenuItem.CheckState = CheckState.Unchecked;
                    this._SettingsAppLangUaMenuItem.CheckState = CheckState.Checked;
                    break;
                default:
                    this._SettingsAppLangEnMenuItem.CheckState = CheckState.Checked;
                    this._SettingsAppLangRuMenuItem.CheckState = CheckState.Unchecked;
                    this._SettingsAppLangUaMenuItem.CheckState = CheckState.Unchecked;
                    break;
            }

            this._PickerStatusLabel.Text = string.Format(ResourcesUI.DISPLAY_MSG, this._GameListView.Items.Count, this._Games.Count);
            this._DownloadStatusLabel.Text = string.Format(ResourcesUI.DOWNLOAD_GAMES_ICONS, this._LogoQueue.Count);
            this._RefreshGamesButton.Text = ResourcesUI.REFRESH_GAMES;
            this._RefreshGamesButton.ToolTipText = ResourcesUI.REFRESH_GAMES_TOOLTIP;
            this._AddGameButton.Text = ResourcesUI.SEARCH_GAME;
            this._AddGameButton.ToolTipText = ResourcesUI.SEARCH_GAME_TOOLTIP;
            this._FilterDropDownButton.Text = ResourcesUI.GAME_FILTER;
            this._FilterGamesMenuItem.Text = ResourcesUI.SHOW_GAMES;
            this._FilterDemosMenuItem.Text = ResourcesUI.SHOW_DEMOS;
            this._FilterModsMenuItem.Text = ResourcesUI.SHOW_MODS;
            this._FilterJunkMenuItem.Text = ResourcesUI.SHOW_JUNK;
            this._SettingsDropDownButton.Text = ResourcesUI.SETTINGS;
            this._SettingsAppLangsMenuItem.Text = ResourcesUI.APP_LANG;
            this._SettingsAppLangEnMenuItem.Text = ResourcesUI.ENG_LANG;
            this._SettingsAppLangRuMenuItem.Text = ResourcesUI.RUS_LANG;
            this._SettingsAppLangUaMenuItem.Text = ResourcesUI.UKR_LANG;
            this._SettingsGameLangsMenuItem.Text = ResourcesUI.GAME_LANG;
            this._SettingsGameLangEnMenuItem.Text = ResourcesUI.ENG_LANG;
            this._SettingsGameLangRuMenuItem.Text = ResourcesUI.RUS_LANG;
            this._SettingsGameLangUaMenuItem.Text = ResourcesUI.UKR_LANG;
            this._DownloadStatusLabel.Text = ResourcesUI.DOWNLOAD;
        }

        private void OnGameLanguageChanged()
        {
            currentGameLanguage = ResourcesUI.GameLanguage;

            switch (currentGameLanguage)
            {
                case LangType.RU:
                    this._SettingsGameLangEnMenuItem.CheckState = CheckState.Unchecked;
                    this._SettingsGameLangRuMenuItem.CheckState = CheckState.Checked;
                    this._SettingsGameLangUaMenuItem.CheckState = CheckState.Unchecked;
                    break;
                case LangType.UA:
                    this._SettingsGameLangEnMenuItem.CheckState = CheckState.Unchecked;
                    this._SettingsGameLangRuMenuItem.CheckState = CheckState.Unchecked;
                    this._SettingsGameLangUaMenuItem.CheckState = CheckState.Checked;
                    break;
                default:
                    this._SettingsGameLangEnMenuItem.CheckState = CheckState.Checked;
                    this._SettingsGameLangRuMenuItem.CheckState = CheckState.Unchecked;
                    this._SettingsGameLangUaMenuItem.CheckState = CheckState.Unchecked;
                    break;
            }
        }

        private static LangType currentAppLanguage;
        private static LangType currentGameLanguage;
        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
    }
}
