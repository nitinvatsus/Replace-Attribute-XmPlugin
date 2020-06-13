namespace ReplaceAttributeXmPlugin
{
    partial class FromUserView
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
            System.Windows.Forms.ListViewGroup listViewGroup21 = new System.Windows.Forms.ListViewGroup("Custom", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup22 = new System.Windows.Forms.ListViewGroup("System", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FromUserView));
            this.lblProgress = new System.Windows.Forms.Label();
            this.ViewName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CmdCheckAllViews = new System.Windows.Forms.Button();
            this.listViewView = new System.Windows.Forms.ListView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmdCheckAllUsers = new System.Windows.Forms.Button();
            this.TxtSearchUsersList = new System.Windows.Forms.TextBox();
            this.listViewSystemUsers = new System.Windows.Forms.ListView();
            this.colFullName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.domainname = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.islicensed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbDeleteSelectedDependency = new System.Windows.Forms.ToolStripButton();
            this.tbReplaceSelectedDependency = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.toolStripMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblProgress
            // 
            this.lblProgress.Location = new System.Drawing.Point(667, 841);
            this.lblProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(413, 37);
            this.lblProgress.TabIndex = 33;
            this.lblProgress.Text = "label1";
            // 
            // ViewName
            // 
            this.ViewName.Tag = "ViewName";
            this.ViewName.Text = "View Name";
            this.ViewName.Width = 350;
            // 
            // CmdCheckAllViews
            // 
            this.CmdCheckAllViews.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CmdCheckAllViews.Location = new System.Drawing.Point(624, 13);
            this.CmdCheckAllViews.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CmdCheckAllViews.Name = "CmdCheckAllViews";
            this.CmdCheckAllViews.Size = new System.Drawing.Size(124, 43);
            this.CmdCheckAllViews.TabIndex = 22;
            this.CmdCheckAllViews.Text = "Check All";
            this.CmdCheckAllViews.UseVisualStyleBackColor = true;
            this.CmdCheckAllViews.Click += new System.EventHandler(this.CmdCheckAllViews_Click);
            // 
            // listViewView
            // 
            this.listViewView.CheckBoxes = true;
            this.listViewView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ViewName});
            this.listViewView.FullRowSelect = true;
            this.listViewView.HideSelection = false;
            this.listViewView.Location = new System.Drawing.Point(8, 64);
            this.listViewView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listViewView.MultiSelect = false;
            this.listViewView.Name = "listViewView";
            this.listViewView.Size = new System.Drawing.Size(740, 484);
            this.listViewView.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.listViewView.TabIndex = 10;
            this.listViewView.Tag = "0";
            this.listViewView.UseCompatibleStateImageBehavior = false;
            this.listViewView.View = System.Windows.Forms.View.Details;
            this.listViewView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewView_ColumnClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CmdCheckAllViews);
            this.groupBox1.Controls.Add(this.listViewView);
            this.groupBox1.Location = new System.Drawing.Point(513, 36);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(759, 558);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Dependency";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmdCheckAllUsers);
            this.groupBox2.Controls.Add(this.TxtSearchUsersList);
            this.groupBox2.Controls.Add(this.listViewSystemUsers);
            this.groupBox2.Location = new System.Drawing.Point(14, 36);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox2.Size = new System.Drawing.Size(490, 558);
            this.groupBox2.TabIndex = 36;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Search System Users";
            // 
            // cmdCheckAllUsers
            // 
            this.cmdCheckAllUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCheckAllUsers.Location = new System.Drawing.Point(355, 29);
            this.cmdCheckAllUsers.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmdCheckAllUsers.Name = "cmdCheckAllUsers";
            this.cmdCheckAllUsers.Size = new System.Drawing.Size(124, 43);
            this.cmdCheckAllUsers.TabIndex = 23;
            this.cmdCheckAllUsers.Text = "Check All";
            this.cmdCheckAllUsers.UseVisualStyleBackColor = true;
            this.cmdCheckAllUsers.Click += new System.EventHandler(this.cmdCheckAllUsers_Click);
            // 
            // TxtSearchUsersList
            // 
            this.TxtSearchUsersList.Location = new System.Drawing.Point(12, 78);
            this.TxtSearchUsersList.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.TxtSearchUsersList.Name = "TxtSearchUsersList";
            this.TxtSearchUsersList.Size = new System.Drawing.Size(467, 23);
            this.TxtSearchUsersList.TabIndex = 3;
            this.TxtSearchUsersList.TextChanged += new System.EventHandler(this.TxtSearchUsersList_TextChanged);
            // 
            // listViewSystemUsers
            // 
            this.listViewSystemUsers.CheckBoxes = true;
            this.listViewSystemUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFullName,
            this.domainname,
            this.islicensed});
            this.listViewSystemUsers.FullRowSelect = true;
            listViewGroup21.Header = "Custom";
            listViewGroup21.Name = "Custom";
            listViewGroup21.Tag = "Custom";
            listViewGroup22.Header = "System";
            listViewGroup22.Name = "System";
            listViewGroup22.Tag = "System";
            this.listViewSystemUsers.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup21,
            listViewGroup22});
            this.listViewSystemUsers.HideSelection = false;
            this.listViewSystemUsers.Location = new System.Drawing.Point(10, 112);
            this.listViewSystemUsers.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.listViewSystemUsers.MultiSelect = false;
            this.listViewSystemUsers.Name = "listViewSystemUsers";
            this.listViewSystemUsers.Size = new System.Drawing.Size(469, 436);
            this.listViewSystemUsers.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.listViewSystemUsers.TabIndex = 4;
            this.listViewSystemUsers.Tag = "0";
            this.listViewSystemUsers.UseCompatibleStateImageBehavior = false;
            this.listViewSystemUsers.View = System.Windows.Forms.View.Details;
            this.listViewSystemUsers.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewSystemUsers_ColumnClick);
            // 
            // colFullName
            // 
            this.colFullName.Tag = "FullName";
            this.colFullName.Text = "Full Name";
            this.colFullName.Width = 200;
            // 
            // domainname
            // 
            this.domainname.Tag = "domainname";
            this.domainname.Text = "Domain Name";
            this.domainname.Width = 102;
            // 
            // islicensed
            // 
            this.islicensed.Tag = "islicensed";
            this.islicensed.Text = "Is Licensed";
            this.islicensed.Width = 100;
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.toolStripButton2,
            this.tssSeparator1,
            this.tbDeleteSelectedDependency,
            this.tbReplaceSelectedDependency});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1290, 31);
            this.toolStripMenu.TabIndex = 37;
            this.toolStripMenu.Text = "toolStrip1";
            this.toolStripMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStripMenu_ItemClicked);
            // 
            // tsbClose
            // 
            this.tsbClose.Image = ((System.Drawing.Image)(resources.GetObject("tsbClose.Image")));
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(110, 28);
            this.tsbClose.Text = "Close this tool";
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // tbDeleteSelectedDependency
            // 
            this.tbDeleteSelectedDependency.Image = ((System.Drawing.Image)(resources.GetObject("tbDeleteSelectedDependency.Image")));
            this.tbDeleteSelectedDependency.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbDeleteSelectedDependency.Name = "tbDeleteSelectedDependency";
            this.tbDeleteSelectedDependency.Size = new System.Drawing.Size(184, 28);
            this.tbDeleteSelectedDependency.Text = "Delete Selected Dependency";
            this.tbDeleteSelectedDependency.Click += new System.EventHandler(this.tbDeleteSelectedDependency_Click);
            // 
            // tbReplaceSelectedDependency
            // 
            this.tbReplaceSelectedDependency.Image = ((System.Drawing.Image)(resources.GetObject("tbReplaceSelectedDependency.Image")));
            this.tbReplaceSelectedDependency.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbReplaceSelectedDependency.Name = "tbReplaceSelectedDependency";
            this.tbReplaceSelectedDependency.Size = new System.Drawing.Size(192, 28);
            this.tbReplaceSelectedDependency.Text = "Replace Selected Dependency";
            this.tbReplaceSelectedDependency.Click += new System.EventHandler(this.tbReplaceSelectedDependency_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = global::ReplaceAttributeXmPlugin.Properties.Resources.cv_IconWeekListTeam32;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(115, 28);
            this.toolStripButton2.Text = "Load User View";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // FromUserView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1290, 604);
            this.Controls.Add(this.toolStripMenu);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FromUserView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FromUserView";
            this.Load += new System.EventHandler(this.FromUserView_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ColumnHeader ViewName;
        private System.Windows.Forms.Button CmdCheckAllViews;
        private System.Windows.Forms.ListView listViewView;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdCheckAllUsers;
        private System.Windows.Forms.TextBox TxtSearchUsersList;
        private System.Windows.Forms.ListView listViewSystemUsers;
        private System.Windows.Forms.ColumnHeader colFullName;
        private System.Windows.Forms.ColumnHeader domainname;
        private System.Windows.Forms.ColumnHeader islicensed;
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.ToolStripButton tbDeleteSelectedDependency;
        private System.Windows.Forms.ToolStripButton tbReplaceSelectedDependency;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
    }
}