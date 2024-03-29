﻿/* Copyright (c) 2019 Rick (rick 'at' gibbed 'dot' us)
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

using SAM.API;
using SAM.API.Resources;
using SAM.API.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Windows.Threading;
using APITypes = SAM.API.Types;

namespace SAM.Game
{
    internal partial class Manager : Form
    {
        private readonly long _GameId;
        private readonly API.Client _SteamClient;

        private readonly WebClient _IconDownloader = new WebClient();

        private readonly List<Stats.AchievementInfo> _IconQueue = new List<Stats.AchievementInfo>();
        private readonly List<Stats.StatDefinition> _StatDefinitions = new List<Stats.StatDefinition>();

        private readonly List<Stats.AchievementDefinition> _AchievementDefinitions =
            new List<Stats.AchievementDefinition>();

        private readonly BindingList<Stats.StatInfo> _Statistics = new BindingList<Stats.StatInfo>();

        // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
        private readonly API.Callbacks.UserStatsReceived _UserStatsReceivedCallback;
        // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

        //private API.Callback<APITypes.UserStatsStored> UserStatsStoredCallback;

        public Manager(long gameId, API.Client client)
        {
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

            this._MainTabControl.SelectedTab = this._AchievementsTabPage;
            //this.statisticsList.Enabled = this.checkBox1.Checked;

            this._AchievementImageList.Images.Add("Blank", new Bitmap(64, 64));

            this._StatisticsDataGridView.AutoGenerateColumns = false;

            this._StatisticsDataGridView.Columns.Add("name", ResourcesUI.NAME_HEAD);
            this._StatisticsDataGridView.Columns[0].ReadOnly = true;
            this._StatisticsDataGridView.Columns[0].Width = 200;
            this._StatisticsDataGridView.Columns[0].DataPropertyName = "DisplayName";

            this._StatisticsDataGridView.Columns.Add("value", ResourcesUI.VALUE_HEAD);
            this._StatisticsDataGridView.Columns[1].ReadOnly = this._EnableStatsEditingCheckBox.Checked == false;
            this._StatisticsDataGridView.Columns[1].Width = 90;
            this._StatisticsDataGridView.Columns[1].DataPropertyName = "Value";

            this._StatisticsDataGridView.Columns.Add("extra", ResourcesUI.EXTRA_HEAD);
            this._StatisticsDataGridView.Columns[2].ReadOnly = true;
            this._StatisticsDataGridView.Columns[2].Width = 200;
            this._StatisticsDataGridView.Columns[2].DataPropertyName = "Extra";

            this._StatisticsDataGridView.DataSource = new BindingSource
            {
                DataSource = this._Statistics,
            };

            this._GameId = gameId;
            this._SteamClient = client;

            this._IconDownloader.DownloadDataCompleted += this.OnIconDownload;

            string name = this._SteamClient.SteamApps001.GetAppData((uint)this._GameId, "name");
            if (name != null)
            {
                base.Text += " | " + name;
            }
            else
            {
                base.Text += " | " + this._GameId.ToString(CultureInfo.InvariantCulture);
            }

            this._UserStatsReceivedCallback = client.CreateAndRegisterCallback<API.Callbacks.UserStatsReceived>();
            this._UserStatsReceivedCallback.OnRun += this.OnUserStatsReceived;

            //this.UserStatsStoredCallback = new API.Callback(1102, new API.Callback.CallbackFunction(this.OnUserStatsStored));
            this.RefreshStats();
        }

        private void AddAchievementIcon(Stats.AchievementInfo info, Image icon)
        {
            if (icon == null)
            {
                info.ImageIndex = 0;
            }
            else
            {
                info.ImageIndex = this._AchievementImageList.Images.Count;
                this._AchievementImageList.Images.Add(info.IsAchieved == true ? info.IconNormal : info.IconLocked, icon);
            }
        }

        private void OnIconDownload(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled == false)
            {
                var info = e.UserState as Stats.AchievementInfo;

                Bitmap bitmap;

                try
                {
                    using (var stream = new MemoryStream())
                    {
                        stream.Write(e.Result, 0, e.Result.Length);
                        bitmap = new Bitmap(stream);
                    }
                }
                catch (Exception)
                {
                    bitmap = null;
                }

                this.AddAchievementIcon(info, bitmap);
                this._AchievementListView.Update();
            }

            this.DownloadNextIcon();
        }

        private void DownloadNextIcon()
        {
            if (this._IconQueue.Count == 0)
            {
                this._DownloadStatusLabel.Visible = false;
                return;
            }

            if (this._IconDownloader.IsBusy == true)
            {
                return;
            }

            this._DownloadStatusLabel.Text = string.Format(ResourcesUI.DOWNLOAD_ACHIEVS_ICONS, this._IconQueue.Count);
            this._DownloadStatusLabel.Visible = true;

            var info = this._IconQueue[0];
            this._IconQueue.RemoveAt(0);


            this._IconDownloader.DownloadDataAsync(
                new Uri(string.Format(
                    CultureInfo.InvariantCulture,
                    "http://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/{0}/{1}",
                    this._GameId,
                    info.IsAchieved == true ? info.IconNormal : info.IconLocked)),
                info);
        }

        private static string TranslateError(int id)
        {
            switch (id)
            {
                case 2:
                {
                    return "generic error -- this usually means you don't own the game";
                }
            }

            return id.ToString(CultureInfo.InvariantCulture);
        }

        private static string GetLocalizedString(KeyValue kv, string language, string defaultValue)
        {
            var name = kv[language].AsString("");
            if (string.IsNullOrEmpty(name) == false)
            {
                return name;
            }

            if (language != "english")
            {
                name = kv["english"].AsString("");
                if (string.IsNullOrEmpty(name) == false)
                {
                    return name;
                }
            }

            name = kv.AsString("");
            if (string.IsNullOrEmpty(name) == false)
            {
                return name;
            }

            return defaultValue;
        }

        private bool LoadUserGameStatsSchema()
        {
            string path;

            try
            {
                path = API.Steam.GetInstallPath();
                path = Path.Combine(path, "appcache");
                path = Path.Combine(path, "stats");
                path = Path.Combine(path, string.Format(
                    CultureInfo.InvariantCulture,
                    "UserGameStatsSchema_{0}.bin",
                    this._GameId));

                if (File.Exists(path) == false)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            var kv = KeyValue.LoadAsBinary(path);

            if (kv == null)
            {
                return false;
            }

            var currentLanguage = ResourcesUI.GameLanguage.GetLangName();

            this._AchievementDefinitions.Clear();
            this._StatDefinitions.Clear();

            var stats = kv[this._GameId.ToString(CultureInfo.InvariantCulture)]["stats"];
            if (stats.Valid == false ||
                stats.Children == null)
            {
                return false;
            }

            foreach (var stat in stats.Children)
            {
                if (stat.Valid == false)
                {
                    continue;
                }

                var rawType = stat["type_int"].Valid
                                  ? stat["type_int"].AsInteger(0)
                                  : stat["type"].AsInteger(0);
                var type = (APITypes.UserStatType)rawType;
                switch (type)
                {
                    case APITypes.UserStatType.Invalid:
                    {
                        break;
                    }

                    case APITypes.UserStatType.Integer:
                    {
                        var id = stat["name"].AsString("");
                        string name = GetLocalizedString(stat["display"]["name"], currentLanguage, id);

                        this._StatDefinitions.Add(new Stats.IntegerStatDefinition()
                        {
                            Id = stat["name"].AsString(""),
                            DisplayName = name,
                            MinValue = stat["min"].AsInteger(int.MinValue),
                            MaxValue = stat["max"].AsInteger(int.MaxValue),
                            MaxChange = stat["maxchange"].AsInteger(0),
                            IncrementOnly = stat["incrementonly"].AsBoolean(false),
                            DefaultValue = stat["default"].AsInteger(0),
                            Permission = stat["permission"].AsInteger(0),
                        });
                        break;
                    }

                    case APITypes.UserStatType.Float:
                    case APITypes.UserStatType.AverageRate:
                    {
                        var id = stat["name"].AsString("");
                        string name = GetLocalizedString(stat["display"]["name"], currentLanguage, id);

                        this._StatDefinitions.Add(new Stats.FloatStatDefinition()
                        {
                            Id = stat["name"].AsString(""),
                            DisplayName = name,
                            MinValue = stat["min"].AsFloat(float.MinValue),
                            MaxValue = stat["max"].AsFloat(float.MaxValue),
                            MaxChange = stat["maxchange"].AsFloat(0.0f),
                            IncrementOnly = stat["incrementonly"].AsBoolean(false),
                            DefaultValue = stat["default"].AsFloat(0.0f),
                            Permission = stat["permission"].AsInteger(0),
                        });
                        break;
                    }

                    case APITypes.UserStatType.Achievements:
                    case APITypes.UserStatType.GroupAchievements:
                    {
                        if (stat.Children != null)
                        {
                            foreach (var bits in stat.Children.Where(
                                b => string.Compare(b.Name, "bits", StringComparison.InvariantCultureIgnoreCase) == 0))
                            {
                                if (bits.Valid == false ||
                                    bits.Children == null)
                                {
                                    continue;
                                }

                                foreach (var bit in bits.Children)
                                {
                                    string id = bit["name"].AsString("");
                                    string name = GetLocalizedString(bit["display"]["name"], currentLanguage, id);
                                    string desc = GetLocalizedString(bit["display"]["desc"], currentLanguage, "");

                                    this._AchievementDefinitions.Add(new Stats.AchievementDefinition()
                                    {
                                        Id = id,
                                        Name = name,
                                        Description = desc,
                                        IconNormal = bit["display"]["icon"].AsString(""),
                                        IconLocked = bit["display"]["icon_gray"].AsString(""),
                                        IsHidden = bit["display"]["hidden"].AsBoolean(false),
                                        Permission = bit["permission"].AsInteger(0),
                                    });
                                }
                            }
                        }

                        break;
                    }

                    default:
                    {
                        throw new InvalidOperationException("invalid stat type");
                    }
                }
            }

            return true;
        }

        private void OnUserStatsReceived(APITypes.UserStatsReceived param)
        {
            if (param.Result != 1)
            {
                this._GameStatusLabel.Text = string.Format(ResourcesUI.GAME_STATUS_RETRIEVE_ERROR, param.Result.ToString());
                this.EnableInput();
                return;
            }

            if (this.LoadUserGameStatsSchema() == false)
            {
                this._GameStatusLabel.Text = ResourcesUI.GAME_STATUS_SCHEMA_ERROR;
                this.EnableInput();
                return;
            }

            try
            {
                this.GetAchievements();
                this.GetStatistics();
            }
            catch (Exception e)
            {
                this._GameStatusLabel.Text = ResourcesUI.GAME_STATUS_STATS_ERROR;
                this.EnableInput();
                MessageBox.Show(
                    ResourcesUI.GAME_STATUS_STATS_ERROR + "\n" + e,
                    "Steam Achievement Manager",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            this._GameStatusLabel.Text = string.Format(ResourcesUI.GAME_STATUS_RETRIEVE_MSG,
                this._AchievementListView.Items.Count,
                this._StatisticsDataGridView.Rows.Count);
            this.EnableInput();
        }

        private void RefreshStats()
        {
            this._AchievementListView.Items.Clear();
            this._StatisticsDataGridView.Rows.Clear();

            if (this._SteamClient.SteamUserStats.RequestCurrentStats() == false)
            {
                MessageBox.Show(this, ResourcesUI.DLG_LOAD_STATS_ERROR, "Steam Achievement Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.textBox1.Text = "";
            this._GameStatusLabel.Text = ResourcesUI.GAME_STATUS_RETRIEVING_PROCESS;
            this.DisableInput();
        }

        private bool _IsUpdatingAchievementList;

        private void GetAchievements()
        {
            this._IsUpdatingAchievementList = true;

            this._AchievementListView.Items.Clear();
            this._AchievementListView.BeginUpdate();
            //this.Achievements.Clear();

            foreach (var def in this._AchievementDefinitions)
            {
                if (string.IsNullOrEmpty(def.Id) == true)
                {
                    continue;
                }

                bool isAchieved;
                if (this._SteamClient.SteamUserStats.GetAchievementState(def.Id, out isAchieved) == false)
                {
                    continue;
                }

                var info = new Stats.AchievementInfo()
                {
                    Id = def.Id,
                    IsAchieved = isAchieved,
                    IconNormal = string.IsNullOrEmpty(def.IconNormal) ? null : def.IconNormal,
                    IconLocked = string.IsNullOrEmpty(def.IconLocked) ? def.IconNormal : def.IconLocked,
                    Permission = def.Permission,
                    Name = def.Name,
                    Description = def.Description,
                };

                var item = new ListViewItem()
                {
                    Checked = isAchieved,
                    Tag = info,
                    Text = info.Name,
                    BackColor = (def.Permission & 3) == 0 ? Color.Black : Color.FromArgb(64, 0, 0),
                };

                info.Item = item;

                if (item.Text.StartsWith("#", StringComparison.InvariantCulture) == true)
                {
                    item.Text = info.Id;
                }
                else
                {
                    item.SubItems.Add(info.Description);
                }

                info.ImageIndex = 0;

                this.AddAchievementToIconQueue(info, false);
                this._AchievementListView.Items.Add(item);
                //this.Achievements.Add(info.Id, info);
            }
            this._AchievementListView.EndUpdate();
            this._IsUpdatingAchievementList = false;

            this.DownloadNextIcon();
        }

        private void GetStatistics()
        {
            this._Statistics.Clear();
            foreach (var rdef in this._StatDefinitions)
            {
                if (string.IsNullOrEmpty(rdef.Id) == true)
                {
                    continue;
                }

                if (rdef is Stats.IntegerStatDefinition)
                {
                    var def = (Stats.IntegerStatDefinition)rdef;

                    int value;
                    if (this._SteamClient.SteamUserStats.GetStatValue(def.Id, out value))
                    {
                        this._Statistics.Add(new Stats.IntStatInfo()
                        {
                            Id = def.Id,
                            DisplayName = def.DisplayName,
                            IntValue = value,
                            OriginalValue = value,
                            IsIncrementOnly = def.IncrementOnly,
                            Permission = def.Permission,
                        });
                    }
                }
                else if (rdef is Stats.FloatStatDefinition)
                {
                    var def = (Stats.FloatStatDefinition)rdef;

                    float value;
                    if (this._SteamClient.SteamUserStats.GetStatValue(def.Id, out value))
                    {
                        this._Statistics.Add(new Stats.FloatStatInfo()
                        {
                            Id = def.Id,
                            DisplayName = def.DisplayName,
                            FloatValue = value,
                            OriginalValue = value,
                            IsIncrementOnly = def.IncrementOnly,
                            Permission = def.Permission,
                        });
                    }
                }
            }
        }

        private void AddAchievementToIconQueue(Stats.AchievementInfo info, bool startDownload)
        {
            int imageIndex = this._AchievementImageList.Images.IndexOfKey(
                info.IsAchieved == true ? info.IconNormal : info.IconLocked);

            if (imageIndex >= 0)
            {
                info.ImageIndex = imageIndex;
            }
            else
            {
                this._IconQueue.Add(info);

                if (startDownload == true)
                {
                    this.DownloadNextIcon();
                }
            }
        }

        private int StoreAchievements()
        {
            if (this._AchievementListView.Items.Count == 0)
            {
                return 0;
            }

            var achievements = new List<Stats.AchievementInfo>();
            foreach (ListViewItem item in this._AchievementListView.Items)
            {
                var achievementInfo = item.Tag as Stats.AchievementInfo;
                if (achievementInfo != null &&
                    achievementInfo.IsAchieved != item.Checked)
                {
                    achievementInfo.IsAchieved = item.Checked;
                    achievements.Add(item.Tag as Stats.AchievementInfo);
                }
            }

            if (achievements.Count == 0)
            {
                return 0;
            }

            foreach (Stats.AchievementInfo info in achievements)
            {
                if (this._SteamClient.SteamUserStats.SetAchievement(info.Id, info.IsAchieved) == false)
                {
                    MessageBox.Show(
                        this,
                        string.Format(
                            CultureInfo.CurrentCulture,
                            ResourcesUI.DLG_STORE_ACHIEV_STATE_ERROR,
                            info.Id),
                        "Steam Achievement Manager",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return -1;
                }
            }

            return achievements.Count;
        }

        private int StoreStatistics()
        {
            if (this._Statistics.Count == 0)
            {
                return 0;
            }

            var statistics = this._Statistics.Where(stat => stat.IsModified == true).ToList();
            if (statistics.Count == 0)
            {
                return 0;
            }

            foreach (Stats.StatInfo stat in statistics)
            {
                if (stat is Stats.IntStatInfo)
                {
                    var intStat = (Stats.IntStatInfo)stat;
                    if (this._SteamClient.SteamUserStats.SetStatValue(
                        intStat.Id,
                        intStat.IntValue) == false)
                    {
                        MessageBox.Show(
                            this,
                            string.Format(
                                CultureInfo.CurrentCulture,
                                ResourcesUI.DLG_STORE_STAT_STATE_ERROR,
                                stat.Id),
                            "Steam Achievement Manager",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return -1;
                    }
                }
                else if (stat is Stats.FloatStatInfo)
                {
                    var floatStat = (Stats.FloatStatInfo)stat;
                    if (this._SteamClient.SteamUserStats.SetStatValue(
                        floatStat.Id,
                        floatStat.FloatValue) == false)
                    {
                        MessageBox.Show(
                            this,
                            string.Format(
                                CultureInfo.CurrentCulture,
                                ResourcesUI.DLG_STORE_STAT_STATE_ERROR,
                                stat.Id),
                            "Steam Achievement Manager",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return -1;
                    }
                }
                else
                {
                    throw new InvalidOperationException("unsupported stat type");
                }
            }

            return statistics.Count;
        }

        private void DisableInput()
        {
            this._ReloadButton.Enabled = false;
            this._StoreButton.Enabled = false;
        }

        private void EnableInput()
        {
            this._ReloadButton.Enabled = true;
            this._StoreButton.Enabled = true;
        }

        private void OnTimer(object sender, EventArgs e)
        {
            this._CallbackTimer.Enabled = false;
            this._SteamClient.RunCallbacks(false);
            this._CallbackTimer.Enabled = true;
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            this.RefreshStats();
        }

        private void OnLockAll(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this._AchievementListView.Items)
            {
                item.Checked = false;
            }
        }

        private void OnInvertAll(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this._AchievementListView.Items)
            {
                item.Checked = !item.Checked;
            }
        }

        private void OnUnlockAll(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this._AchievementListView.Items)
            {
                item.Checked = true;
            }
        }

        private bool Store()
        {
            if (this._SteamClient.SteamUserStats.StoreStats() == false)
            {
                MessageBox.Show(
                    this,
                    ResourcesUI.DLG_STORE_ERROR,
                    "Steam Achievement Manager",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void OnStore(object sender, EventArgs e)
        {
            int achievements = this.StoreAchievements();
            if (achievements < 0)
            {
                this.RefreshStats();
                return;
            }

            int stats = this.StoreStatistics();
            if (stats < 0)
            {
                this.RefreshStats();
                return;
            }

            if (this.Store() == false)
            {
                this.RefreshStats();
                return;
            }

            MessageBox.Show(
                this,
                string.Format(
                    CultureInfo.CurrentCulture,
                    ResourcesUI.DLG_STORE_MSG,
                    achievements,
                    stats),
                "Steam Achievement Manager",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            this.RefreshStats();
        }

        private void OnStatDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Context == DataGridViewDataErrorContexts.Commit)
            {
                var view = (DataGridView)sender;
                if (e.Exception is Stats.StatIsProtectedException)
                {
                    e.ThrowException = false;
                    e.Cancel = true;
                    view.Rows[e.RowIndex].ErrorText = "Stat is protected! -- you can't modify it";
                }
                else
                {
                    e.ThrowException = false;
                    e.Cancel = true;
                    view.Rows[e.RowIndex].ErrorText = "Invalid value";
                }
            }
        }

        private void OnStatAgreementChecked(object sender, EventArgs e)
        {
            this._StatisticsDataGridView.Columns[1].ReadOnly = this._EnableStatsEditingCheckBox.Checked == false;
        }

        private void OnStatCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var view = (DataGridView)sender;
            view.Rows[e.RowIndex].ErrorText = "";
        }

        private void OnResetAllStats(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                ResourcesUI.DLG_RESET_STATS_MSG,
                "Steam Achievement Manager",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            bool achievementsToo = DialogResult.Yes == MessageBox.Show(
                ResourcesUI.DLG_RESET_ACHIEVES_MSG,
                "Steam Achievement Manager",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (MessageBox.Show(
                ResourcesUI.DLG_CONFIRM_RESET_MSG,
                "Steam Achievement Manager",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }

            if (this._SteamClient.SteamUserStats.ResetAllStats(achievementsToo) == false)
            {
                MessageBox.Show(this, ResourcesUI.DLG_FAILED_TO_RESET_ERROR, "Steam Achievement Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.RefreshStats();
        }

        private void OnCheckAchievement(object sender, ItemCheckEventArgs e)
        {
            if (sender != this._AchievementListView)
            {
                return;
            }

            if (this._IsUpdatingAchievementList == true)
            {
                return;
            }

            var info = this._AchievementListView.Items[e.Index].Tag
                       as Stats.AchievementInfo;
            if (info == null)
            {
                return;
            }

            if ((info.Permission & 3) != 0)
            {
                MessageBox.Show(
                    this,
                    ResourcesUI.DLG_PROTECTED_ACHIEV_ERROR,
                    "Steam Achievement Manager",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                e.NewValue = e.CurrentValue;
            }
        }

        private int sortColumn = -1;

        private void _AchievementListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != sortColumn)
            {
                sortColumn = e.Column;
                this._AchievementListView.Sorting = SortOrder.Ascending;
            }
            else
            {
                if (this._AchievementListView.Sorting == SortOrder.Ascending)
                {
                    this._AchievementListView.Sorting = SortOrder.Descending;
                }
                else
                {
                    this._AchievementListView.Sorting = SortOrder.Ascending;
                }
            }

            this._AchievementListView.Sort();
            this._AchievementListView.ListViewItemSorter = new ListViewItemComparer(e.Column, this._AchievementListView.Sorting);
        }

        private void searchData(string filterText)
        {
            this._IsUpdatingAchievementList = true;

            this._AchievementListView.Items.Clear();
            this._AchievementListView.BeginUpdate();
            //this.Achievements.Clear();

            foreach (var def in this._AchievementDefinitions)
            {
                if (string.IsNullOrEmpty(def.Id) == true)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(filterText) && !string.IsNullOrWhiteSpace(filterText))
                {
                    if (!def.Name.ToLower().Contains(filterText.ToLower()) && !def.Description.ToLower().Contains(filterText.ToLower()))
                    {
                        continue;
                    }
                }

                bool isAchieved;
                if (this._SteamClient.SteamUserStats.GetAchievementState(def.Id, out isAchieved) == false)
                {
                    continue;
                }

                var info = new Stats.AchievementInfo()
                {
                    Id = def.Id,
                    IsAchieved = isAchieved,
                    IconNormal = string.IsNullOrEmpty(def.IconNormal) ? null : def.IconNormal,
                    IconLocked = string.IsNullOrEmpty(def.IconLocked) ? def.IconNormal : def.IconLocked,
                    Permission = def.Permission,
                    Name = def.Name,
                    Description = def.Description,
                };

                var item = new ListViewItem()
                {
                    Checked = isAchieved,
                    Tag = info,
                    Text = info.Name,
                    BackColor = (def.Permission & 3) == 0 ? Color.Black : Color.FromArgb(64, 0, 0),
                };

                info.Item = item;

                if (item.Text.StartsWith("#", StringComparison.InvariantCulture) == true)
                {
                    item.Text = info.Id;
                }
                else
                {
                    item.SubItems.Add(info.Description);
                }

                info.ImageIndex = 0;

                this.AddAchievementToIconQueue(info, false);
                this._AchievementListView.Items.Add(item);
                //this.Achievements.Add(info.Id, info);
            }

            this._AchievementListView.EndUpdate();
            this._IsUpdatingAchievementList = false;

            this.DownloadNextIcon();
        }

        private void OnTextBox1Changed(object sender, System.EventArgs e)
        {
            searchData(this.textBox1.Text);
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

            this._StatisticsDataGridView.Columns[0].HeaderText = ResourcesUI.NAME_HEAD;
            this._StatisticsDataGridView.Columns[1].HeaderText = ResourcesUI.VALUE_HEAD;
            this._StatisticsDataGridView.Columns[2].HeaderText = ResourcesUI.EXTRA_HEAD;
            this._DownloadStatusLabel.Text = string.Format(ResourcesUI.DOWNLOAD_ACHIEVS_ICONS, this._IconQueue.Count);
            this._StoreButton.Text = ResourcesUI.COMMIT;
            this._StoreButton.ToolTipText = ResourcesUI.COMMIT_TOOLTIP;
            this._ReloadButton.Text = ResourcesUI.REFRESH_ACHIEVS;
            this._ReloadButton.ToolTipText = ResourcesUI.REFRESH_ACHIEVS_TOOLTIP;
            this._ResetButton.Text = ResourcesUI.RESET;
            this._ResetButton.ToolTipText = ResourcesUI.RESET_TOOLTIP;
            this._DownloadStatusLabel.Text = ResourcesUI.DOWNLOAD;
            this._AchievementsTabPage.Text = ResourcesUI.ACHIEVS_HEAD;
            this._LockAllButton.Text = ResourcesUI.LOCK_ACHIEVS;
            this._LockAllButton.ToolTipText = ResourcesUI.LOCK_ACHIEVS_TOOLTIP;
            this._InvertAllButton.Text = ResourcesUI.INVERT_ACHIEVS;
            this._InvertAllButton.ToolTipText = ResourcesUI.INVERT_ACHIEVS_TOOLTIP;
            this._UnlockAllButton.Text = ResourcesUI.UNLOCK_ACHIEVS;
            this._UnlockAllButton.ToolTipText = ResourcesUI.UNLOCK_ACHIEVS_TOOLTIP;
            this._StatisticsTabPage.Text = ResourcesUI.STATS_HEAD;
            this._EnableStatsEditingCheckBox.Text = ResourcesUI.UNDERSTAND_MSG;
            this._AchievementNameColumnHeader.Text = ResourcesUI.ACHIEVS_NAME_HEAD;
            this._AchievementDescriptionColumnHeader.Text = ResourcesUI.ACHIEVS_DESCR_HEAD;
            this.label1.Text = ResourcesUI.SEARCH_LABEL;
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

            this.textBox1.Text = string.Empty;
            this.RefreshStats();
        }

        private static LangType currentAppLanguage;
        private static LangType currentGameLanguage;
        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
    }
}
