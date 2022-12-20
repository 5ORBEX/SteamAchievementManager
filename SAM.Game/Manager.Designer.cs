using SAM.API;
using SAM.API.Resources;
using System.Drawing;
using System.Windows.Forms;

namespace SAM.Game
{
	partial class Manager
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSeparator _ToolStripSeparator1;
            System.Windows.Forms.ToolStripSeparator _ToolStripSeparator2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Manager));
            this._MainToolStrip = new System.Windows.Forms.ToolStrip();
            this._StoreButton = new System.Windows.Forms.ToolStripButton();
            this._ReloadButton = new System.Windows.Forms.ToolStripButton();
            this._ResetButton = new System.Windows.Forms.ToolStripButton();
            this._AchievementImageList = new System.Windows.Forms.ImageList(this.components);
            this._MainStatusStrip = new System.Windows.Forms.StatusStrip();
            this._CountryStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._GameStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._DownloadStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._CallbackTimer = new System.Windows.Forms.Timer(this.components);
            this._MainTabControl = new System.Windows.Forms.TabControl();
            this._AchievementsTabPage = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._SettingsDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this._SettingsAppLangsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._SettingsAppLangEnMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._SettingsAppLangRuMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._SettingsAppLangUaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._SettingsGameLangsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._SettingsGameLangEnMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._SettingsGameLangRuMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._SettingsGameLangUaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._AchievementsToolStrip = new System.Windows.Forms.ToolStrip();
            this._LockAllButton = new System.Windows.Forms.ToolStripButton();
            this._InvertAllButton = new System.Windows.Forms.ToolStripButton();
            this._UnlockAllButton = new System.Windows.Forms.ToolStripButton();
            this._StatisticsTabPage = new System.Windows.Forms.TabPage();
            this._EnableStatsEditingCheckBox = new System.Windows.Forms.CheckBox();
            this._StatisticsDataGridView = new System.Windows.Forms.DataGridView();
            this._AchievementListView = new SAM.Game.DoubleBufferedListView();
            this._AchievementNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._AchievementDescriptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            _ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            _ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._MainToolStrip.SuspendLayout();
            this._MainStatusStrip.SuspendLayout();
            this._MainTabControl.SuspendLayout();
            this._AchievementsTabPage.SuspendLayout();
            this._AchievementsToolStrip.SuspendLayout();
            this._StatisticsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._StatisticsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // _ToolStripSeparator1
            // 
            _ToolStripSeparator1.Name = "_ToolStripSeparator1";
            _ToolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // _ToolStripSeparator2
            // 
            _ToolStripSeparator2.Name = "_ToolStripSeparator2";
            _ToolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // _MainToolStrip
            // 
            this._MainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._StoreButton,
            this._ReloadButton,
            _ToolStripSeparator1,
            this._ResetButton,
            _ToolStripSeparator2,
            _SettingsDropDownButton});
            this._MainToolStrip.Location = new System.Drawing.Point(0, 0);
            this._MainToolStrip.Name = "_MainToolStrip";
            this._MainToolStrip.Size = new System.Drawing.Size(632, 25);
            this._MainToolStrip.TabIndex = 1;
            // 
            // _StoreButton
            // 
            this._StoreButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._StoreButton.Enabled = false;
            this._StoreButton.Image = global::SAM.Game.Resources.Save;
            this._StoreButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._StoreButton.Name = "_StoreButton";
            this._StoreButton.Size = new System.Drawing.Size(120, 22);
            this._StoreButton.Text = ResourcesUI.COMMIT;
            this._StoreButton.ToolTipText = ResourcesUI.COMMIT_TOOLTIP;
            this._StoreButton.Click += new System.EventHandler(this.OnStore);
            // 
            // _ReloadButton
            // 
            this._ReloadButton.Enabled = false;
            this._ReloadButton.Image = global::SAM.Game.Resources.Refresh;
            this._ReloadButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._ReloadButton.Name = "_ReloadButton";
            this._ReloadButton.Size = new System.Drawing.Size(66, 22);
            this._ReloadButton.Text = ResourcesUI.REFRESH_ACHIEVS;
            this._ReloadButton.ToolTipText = ResourcesUI.REFRESH_ACHIEVS_TOOLTIP;
            this._ReloadButton.Click += new System.EventHandler(this.OnRefresh);
            // 
            // _ResetButton
            // 
            this._ResetButton.Image = global::SAM.Game.Resources.Reset;
            this._ResetButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._ResetButton.Name = "_ResetButton";
            this._ResetButton.Size = new System.Drawing.Size(55, 22);
            this._ResetButton.Text = ResourcesUI.RESET;
            this._ResetButton.ToolTipText = ResourcesUI.RESET_TOOLTIP;
            this._ResetButton.Click += new System.EventHandler(this.OnResetAllStats);
            // 
            // _SettingsDropDownButton
            // 
            this._SettingsDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._SettingsDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._SettingsAppLangsMenuItem,
                this._SettingsGameLangsMenuItem
            });
            this._SettingsDropDownButton.Image = global::SAM.Game.Resources.Settings;
            this._SettingsDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._SettingsDropDownButton.Name = "_SettingsDropDownButton";
            this._SettingsDropDownButton.Size = new System.Drawing.Size(29, 22);
            this._SettingsDropDownButton.Text = ResourcesUI.SETTINGS;
            // 
            // _SettingsAppLangsMenuItem
            // 
            this._SettingsAppLangsMenuItem.CheckOnClick = false;
            this._SettingsAppLangsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                _SettingsAppLangEnMenuItem,
                _SettingsAppLangRuMenuItem,
                _SettingsAppLangUaMenuItem
            });
            this._SettingsAppLangsMenuItem.Name = "_SettingsAppLangsMenuItem";
            this._SettingsAppLangsMenuItem.Size = new System.Drawing.Size(150, 22);
            this._SettingsAppLangsMenuItem.Text = ResourcesUI.APP_LANG;
            // 
            // _SettingsAppLangEnMenuItem
            // 
            this._SettingsAppLangEnMenuItem.CheckOnClick = true;
            this._SettingsAppLangEnMenuItem.Name = "_SettingsAppLangEnMenuItem";
            this._SettingsAppLangEnMenuItem.Size = new System.Drawing.Size(150, 22);
            this._SettingsAppLangEnMenuItem.Text = ResourcesUI.ENG_LANG;
            this._SettingsAppLangEnMenuItem.Click += new System.EventHandler(this.OnAppLanguageUpdate);
            // 
            // _SettingsAppLangRuMenuItem
            // 
            this._SettingsAppLangRuMenuItem.CheckOnClick = true;
            this._SettingsAppLangRuMenuItem.Name = "_SettingsAppLangRuMenuItem";
            this._SettingsAppLangRuMenuItem.Size = new System.Drawing.Size(150, 22);
            this._SettingsAppLangRuMenuItem.Text = ResourcesUI.RUS_LANG;
            this._SettingsAppLangRuMenuItem.Click += new System.EventHandler(this.OnAppLanguageUpdate);
            // 
            // _SettingsAppLangUaMenuItem
            // 
            this._SettingsAppLangUaMenuItem.CheckOnClick = true;
            this._SettingsAppLangUaMenuItem.Name = "_SettingsAppLangUaMenuItem";
            this._SettingsAppLangUaMenuItem.Size = new System.Drawing.Size(150, 22);
            this._SettingsAppLangUaMenuItem.Text = ResourcesUI.UKR_LANG;
            this._SettingsAppLangUaMenuItem.Click += new System.EventHandler(this.OnAppLanguageUpdate);
            // 
            // _SettingsGameLangsMenuItem
            // 
            this._SettingsGameLangsMenuItem.CheckOnClick = false;
            this._SettingsGameLangsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                _SettingsGameLangEnMenuItem,
                _SettingsGameLangRuMenuItem,
                _SettingsGameLangUaMenuItem
            });
            this._SettingsGameLangsMenuItem.Name = "_SettingsGameLangsMenuItem";
            this._SettingsGameLangsMenuItem.Size = new System.Drawing.Size(150, 22);
            this._SettingsGameLangsMenuItem.Text = ResourcesUI.GAME_LANG;
            // 
            // _SettingsGameLangEnMenuItem
            // 
            this._SettingsGameLangEnMenuItem.CheckOnClick = true;
            this._SettingsGameLangEnMenuItem.Name = "_SettingsGameLangEnMenuItem";
            this._SettingsGameLangEnMenuItem.Size = new System.Drawing.Size(150, 22);
            this._SettingsGameLangEnMenuItem.Text = ResourcesUI.ENG_LANG;
            this._SettingsGameLangEnMenuItem.Click += new System.EventHandler(this.OnGameLanguageUpdate);
            // 
            // _SettingsGameLangRuMenuItem
            // 
            this._SettingsGameLangRuMenuItem.CheckOnClick = true;
            this._SettingsGameLangRuMenuItem.Name = "_SettingsGameLangRuMenuItem";
            this._SettingsGameLangRuMenuItem.Size = new System.Drawing.Size(150, 22);
            this._SettingsGameLangRuMenuItem.Text = ResourcesUI.RUS_LANG;
            this._SettingsGameLangRuMenuItem.Click += new System.EventHandler(this.OnGameLanguageUpdate);
            // 
            // _SettingsGameLangUaMenuItem
            // 
            this._SettingsGameLangUaMenuItem.CheckOnClick = true;
            this._SettingsGameLangUaMenuItem.Name = "_SettingsGameLangUaMenuItem";
            this._SettingsGameLangUaMenuItem.Size = new System.Drawing.Size(150, 22);
            this._SettingsGameLangUaMenuItem.Text = ResourcesUI.UKR_LANG;
            this._SettingsGameLangUaMenuItem.Click += new System.EventHandler(this.OnGameLanguageUpdate);
            // 
            // _AchievementImageList
            // 
            this._AchievementImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this._AchievementImageList.ImageSize = new System.Drawing.Size(64, 64);
            this._AchievementImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // _MainStatusStrip
            // 
            this._MainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._CountryStatusLabel,
            this._GameStatusLabel,
            this._DownloadStatusLabel});
            this._MainStatusStrip.Location = new System.Drawing.Point(0, 370);
            this._MainStatusStrip.Name = "_MainStatusStrip";
            this._MainStatusStrip.Size = new System.Drawing.Size(632, 22);
            this._MainStatusStrip.TabIndex = 4;
            this._MainStatusStrip.Text = "statusStrip1";
            // 
            // _CountryStatusLabel
            // 
            this._CountryStatusLabel.Name = "_CountryStatusLabel";
            this._CountryStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // _GameStatusLabel
            // 
            this._GameStatusLabel.Name = "_GameStatusLabel";
            this._GameStatusLabel.Size = new System.Drawing.Size(617, 17);
            this._GameStatusLabel.Spring = true;
            this._GameStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _DownloadStatusLabel
            // 
            this._DownloadStatusLabel.Image = global::SAM.Game.Resources.Download;
            this._DownloadStatusLabel.Name = "_DownloadStatusLabel";
            this._DownloadStatusLabel.Size = new System.Drawing.Size(111, 17);
            this._DownloadStatusLabel.Text = ResourcesUI.DOWNLOAD;
            this._DownloadStatusLabel.Visible = false;
            // 
            // _CallbackTimer
            // 
            this._CallbackTimer.Enabled = true;
            this._CallbackTimer.Tick += new System.EventHandler(this.OnTimer);
            // 
            // _MainTabControl
            // 
            this._MainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._MainTabControl.Controls.Add(this._AchievementsTabPage);
            this._MainTabControl.Controls.Add(this._StatisticsTabPage);
            this._MainTabControl.Location = new System.Drawing.Point(8, 33);
            this._MainTabControl.Name = "_MainTabControl";
            this._MainTabControl.SelectedIndex = 0;
            this._MainTabControl.Size = new System.Drawing.Size(616, 334);
            this._MainTabControl.TabIndex = 5;
            // 
            // _AchievementsTabPage
            //
            this._AchievementsTabPage.Controls.Add(this.label1);
            this._AchievementsTabPage.Controls.Add(this.textBox1);
            this._AchievementsTabPage.Controls.Add(this._AchievementListView);
            this._AchievementsTabPage.Controls.Add(this._AchievementsToolStrip);
            this._AchievementsTabPage.Location = new System.Drawing.Point(4, 22);
            this._AchievementsTabPage.Name = "_AchievementsTabPage";
            this._AchievementsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._AchievementsTabPage.Size = new System.Drawing.Size(608, 308);
            this._AchievementsTabPage.TabIndex = 0;
            this._AchievementsTabPage.Text = ResourcesUI.ACHIEVS_HEAD;
            this._AchievementsTabPage.UseVisualStyleBackColor = false;
            this._AchievementsTabPage.BackColor = Color.FromArgb(230, 230, 230);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(94, 8);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.MaximumSize = new System.Drawing.Size(45, 13);
            this.label1.MinimumSize = new System.Drawing.Size(45, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 6;
            this.label1.BackColor = Color.FromArgb(248, 248, 248);
            this.label1.Text = ResourcesUI.SEARCH_LABEL;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(147, 5);
            this.textBox1.Margin = new System.Windows.Forms.Padding(0);
            this.textBox1.MaximumSize = new System.Drawing.Size(150, 20);
            this.textBox1.MinimumSize = new System.Drawing.Size(150, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(150, 20);
            this.textBox1.TabIndex = 7;
            this.textBox1.TextChanged += new System.EventHandler(this.OnTextBox1Changed);
            // 
            // _AchievementsToolStrip
            // 
            this._AchievementsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._LockAllButton,
            this._InvertAllButton,
            this._UnlockAllButton});
            this._AchievementsToolStrip.Location = new System.Drawing.Point(3, 3);
            this._AchievementsToolStrip.Name = "_AchievementsToolStrip";
            this._AchievementsToolStrip.Size = new System.Drawing.Size(602, 25);
            this._AchievementsToolStrip.TabIndex = 5;
            // 
            // _LockAllButton
            // 
            this._LockAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._LockAllButton.Image = global::SAM.Game.Resources.Lock;
            this._LockAllButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._LockAllButton.Name = "_LockAllButton";
            this._LockAllButton.Size = new System.Drawing.Size(23, 22);
            this._LockAllButton.Text = ResourcesUI.LOCK_ACHIEVS;
            this._LockAllButton.ToolTipText = ResourcesUI.LOCK_ACHIEVS_TOOLTIP;
            this._LockAllButton.Click += new System.EventHandler(this.OnLockAll);
            // 
            // _InvertAllButton
            // 
            this._InvertAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._InvertAllButton.Image = global::SAM.Game.Resources.Invert;
            this._InvertAllButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._InvertAllButton.Name = "_InvertAllButton";
            this._InvertAllButton.Size = new System.Drawing.Size(23, 22);
            this._InvertAllButton.Text = ResourcesUI.INVERT_ACHIEVS;
            this._InvertAllButton.ToolTipText = ResourcesUI.INVERT_ACHIEVS_TOOLTIP;
            this._InvertAllButton.Click += new System.EventHandler(this.OnInvertAll);
            // 
            // _UnlockAllButton
            // 
            this._UnlockAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._UnlockAllButton.Image = global::SAM.Game.Resources.Unlock;
            this._UnlockAllButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._UnlockAllButton.Name = "_UnlockAllButton";
            this._UnlockAllButton.Size = new System.Drawing.Size(23, 22);
            this._UnlockAllButton.Text = ResourcesUI.UNLOCK_ACHIEVS;
            this._UnlockAllButton.ToolTipText = ResourcesUI.UNLOCK_ACHIEVS_TOOLTIP;
            this._UnlockAllButton.Click += new System.EventHandler(this.OnUnlockAll);
            // 
            // _StatisticsTabPage
            // 
            this._StatisticsTabPage.Controls.Add(this._EnableStatsEditingCheckBox);
            this._StatisticsTabPage.Controls.Add(this._StatisticsDataGridView);
            this._StatisticsTabPage.Location = new System.Drawing.Point(4, 22);
            this._StatisticsTabPage.Name = "_StatisticsTabPage";
            this._StatisticsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._StatisticsTabPage.Size = new System.Drawing.Size(608, 308);
            this._StatisticsTabPage.TabIndex = 1;
            this._StatisticsTabPage.Text = ResourcesUI.STATS_HEAD;
            this._StatisticsTabPage.UseVisualStyleBackColor = true;
            // 
            // _EnableStatsEditingCheckBox
            // 
            this._EnableStatsEditingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._EnableStatsEditingCheckBox.AutoSize = true;
            this._EnableStatsEditingCheckBox.Location = new System.Drawing.Point(6, 285);
            this._EnableStatsEditingCheckBox.Name = "_EnableStatsEditingCheckBox";
            this._EnableStatsEditingCheckBox.Size = new System.Drawing.Size(512, 17);
            this._EnableStatsEditingCheckBox.TabIndex = 1;
            this._EnableStatsEditingCheckBox.Text = ResourcesUI.UNDERSTAND_MSG;
            this._EnableStatsEditingCheckBox.UseVisualStyleBackColor = true;
            this._EnableStatsEditingCheckBox.CheckedChanged += new System.EventHandler(this.OnStatAgreementChecked);
            // 
            // _StatisticsDataGridView
            // 
            this._StatisticsDataGridView.AllowUserToAddRows = false;
            this._StatisticsDataGridView.AllowUserToDeleteRows = false;
            this._StatisticsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._StatisticsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._StatisticsDataGridView.Location = new System.Drawing.Point(6, 6);
            this._StatisticsDataGridView.Name = "_StatisticsDataGridView";
            this._StatisticsDataGridView.Size = new System.Drawing.Size(596, 273);
            this._StatisticsDataGridView.TabIndex = 0;
            this._StatisticsDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnStatCellEndEdit);
            this._StatisticsDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.OnStatDataError);
            // 
            // _AchievementListView
            // 
            this._AchievementListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this._AchievementListView.BackColor = System.Drawing.Color.Black;
            this._AchievementListView.BackgroundImageTiled = true;
            this._AchievementListView.CheckBoxes = true;
            this._AchievementListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._AchievementNameColumnHeader,
            this._AchievementDescriptionColumnHeader});
            this._AchievementListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._AchievementListView.ForeColor = System.Drawing.Color.White;
            this._AchievementListView.FullRowSelect = true;
            this._AchievementListView.GridLines = true;
            this._AchievementListView.HideSelection = false;
            this._AchievementListView.LargeImageList = this._AchievementImageList;
            this._AchievementListView.Location = new System.Drawing.Point(3, 28);
            this._AchievementListView.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this._AchievementListView.Name = "_AchievementListView";
            this._AchievementListView.Size = new System.Drawing.Size(602, 277);
            this._AchievementListView.SmallImageList = this._AchievementImageList;
            this._AchievementListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this._AchievementListView.TabIndex = 4;
            this._AchievementListView.UseCompatibleStateImageBehavior = false;
            this._AchievementListView.View = System.Windows.Forms.View.Details;
            this._AchievementListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this._AchievementListView_ColumnClick);
            this._AchievementListView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.OnCheckAchievement);
            // 
            // _AchievementNameColumnHeader
            // 
            this._AchievementNameColumnHeader.Text = ResourcesUI.ACHIEVS_NAME_HEAD;
            this._AchievementNameColumnHeader.Width = 200;
            // 
            // _AchievementDescriptionColumnHeader
            // 
            this._AchievementDescriptionColumnHeader.Text = ResourcesUI.ACHIEVS_DESCR_HEAD;
            this._AchievementDescriptionColumnHeader.Width = 380;
            // 
            // Manager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 392);
            this.Controls.Add(this._MainToolStrip);
            this.Controls.Add(this._MainTabControl);
            this.Controls.Add(this._MainStatusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(640, 50);
            this.Name = "Manager";
            this.Text = "Steam Achievement Manager 7.0.27";
            this._MainToolStrip.ResumeLayout(false);
            this._MainToolStrip.PerformLayout();
            this._MainStatusStrip.ResumeLayout(false);
            this._MainStatusStrip.PerformLayout();
            this._MainTabControl.ResumeLayout(false);
            this._AchievementsTabPage.ResumeLayout(false);
            this._AchievementsTabPage.PerformLayout();
            this._AchievementsToolStrip.ResumeLayout(false);
            this._AchievementsToolStrip.PerformLayout();
            this._StatisticsTabPage.ResumeLayout(false);
            this._StatisticsTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._StatisticsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

        #endregion

        private System.Windows.Forms.ToolStrip _MainToolStrip;
		private System.Windows.Forms.ToolStripButton _StoreButton;
        private System.Windows.Forms.ToolStripButton _ReloadButton;
		private System.Windows.Forms.StatusStrip _MainStatusStrip;
		private System.Windows.Forms.ToolStripStatusLabel _CountryStatusLabel;
		private System.Windows.Forms.ToolStripStatusLabel _GameStatusLabel;
		private System.Windows.Forms.ImageList _AchievementImageList;
        private System.Windows.Forms.Timer _CallbackTimer;
        private System.Windows.Forms.TabControl _MainTabControl;
        private System.Windows.Forms.TabPage _AchievementsTabPage;
        private System.Windows.Forms.TabPage _StatisticsTabPage;
        private DoubleBufferedListView _AchievementListView;
        private System.Windows.Forms.ColumnHeader _AchievementNameColumnHeader;
        private System.Windows.Forms.ColumnHeader _AchievementDescriptionColumnHeader;
        private System.Windows.Forms.ToolStrip _AchievementsToolStrip;
        private System.Windows.Forms.ToolStripButton _LockAllButton;
        private System.Windows.Forms.ToolStripButton _InvertAllButton;
        private System.Windows.Forms.ToolStripButton _UnlockAllButton;
        private System.Windows.Forms.DataGridView _StatisticsDataGridView;
        public System.Windows.Forms.CheckBox _EnableStatsEditingCheckBox;
        private System.Windows.Forms.ToolStripButton _ResetButton;
        private System.Windows.Forms.ToolStripStatusLabel _DownloadStatusLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripDropDownButton _SettingsDropDownButton;

        private System.Windows.Forms.ToolStripMenuItem _SettingsAppLangsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _SettingsAppLangEnMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _SettingsAppLangRuMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _SettingsAppLangUaMenuItem;

        private System.Windows.Forms.ToolStripMenuItem _SettingsGameLangsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _SettingsGameLangEnMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _SettingsGameLangRuMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _SettingsGameLangUaMenuItem;
    }
}

