﻿namespace ReplaceAttributeXmPlugin
{
    partial class ReplaceAttributeControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReplaceAttributeControl));
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Custom", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("System", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("Custom", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup8 = new System.Windows.Forms.ListViewGroup("System", System.Windows.Forms.HorizontalAlignment.Left);
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.tbLoadDependency = new System.Windows.Forms.ToolStripButton();
            this.tbDeleteSelectedDependency = new System.Windows.Forms.ToolStripButton();
            this.tbReplaceSelectedDependency = new System.Windows.Forms.ToolStripButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TxtSearchEnityList = new System.Windows.Forms.TextBox();
            this.listViewEntities = new System.Windows.Forms.ListView();
            this.colDisplayName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colState = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.TxtSearchAttributeList = new System.Windows.Forms.TextBox();
            this.listViewAttributes = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.TxtSearchReplaceAttributeList = new System.Windows.Forms.TextBox();
            this.listViewAttributesReplaced = new System.Windows.Forms.ListView();
            this.AttrDisplayName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AttributeName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.StateAttribute = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CmdCheckAllForms = new System.Windows.Forms.Button();
            this.CmdCheckAllViews = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.listViewView = new System.Windows.Forms.ListView();
            this.ViewName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ViewState = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.listViewForms = new System.Windows.Forms.ListView();
            this.FormName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.State = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripMenu.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1,
            this.toolStripButton1,
            this.tbLoadDependency,
            this.tbDeleteSelectedDependency,
            this.tbReplaceSelectedDependency,
            this.toolStripButton2});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1799, 31);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.Image = ((System.Drawing.Image)(resources.GetObject("tsbClose.Image")));
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(110, 28);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(102, 28);
            this.toolStripButton1.Text = "Load Entities";
            this.toolStripButton1.Click += new System.EventHandler(this.ToolStripButton1_Click);
            // 
            // tbLoadDependency
            // 
            this.tbLoadDependency.Enabled = false;
            this.tbLoadDependency.Image = global::ReplaceAttributeXmPlugin.Properties.Resources.icon_032;
            this.tbLoadDependency.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbLoadDependency.Name = "tbLoadDependency";
            this.tbLoadDependency.Size = new System.Drawing.Size(130, 28);
            this.tbLoadDependency.Text = "Load Dependency";
            this.tbLoadDependency.Click += new System.EventHandler(this.TbLoadDependency_Click);
            // 
            // tbDeleteSelectedDependency
            // 
            this.tbDeleteSelectedDependency.Enabled = false;
            this.tbDeleteSelectedDependency.Image = ((System.Drawing.Image)(resources.GetObject("tbDeleteSelectedDependency.Image")));
            this.tbDeleteSelectedDependency.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbDeleteSelectedDependency.Name = "tbDeleteSelectedDependency";
            this.tbDeleteSelectedDependency.Size = new System.Drawing.Size(184, 28);
            this.tbDeleteSelectedDependency.Text = "Delete Selected Dependency";
            this.tbDeleteSelectedDependency.Click += new System.EventHandler(this.TbDeleteSelectedDependency_Click);
            // 
            // tbReplaceSelectedDependency
            // 
            this.tbReplaceSelectedDependency.Enabled = false;
            this.tbReplaceSelectedDependency.Image = ((System.Drawing.Image)(resources.GetObject("tbReplaceSelectedDependency.Image")));
            this.tbReplaceSelectedDependency.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbReplaceSelectedDependency.Name = "tbReplaceSelectedDependency";
            this.tbReplaceSelectedDependency.Size = new System.Drawing.Size(192, 28);
            this.tbReplaceSelectedDependency.Text = "Replace Selected Dependency";
            this.tbReplaceSelectedDependency.Click += new System.EventHandler(this.TbReplaceSelectedDependency_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.TxtSearchEnityList);
            this.groupBox2.Controls.Add(this.listViewEntities);
            this.groupBox2.Location = new System.Drawing.Point(3, 36);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(427, 753);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Search Entities";
            // 
            // TxtSearchEnityList
            // 
            this.TxtSearchEnityList.Location = new System.Drawing.Point(11, 19);
            this.TxtSearchEnityList.Margin = new System.Windows.Forms.Padding(5);
            this.TxtSearchEnityList.Name = "TxtSearchEnityList";
            this.TxtSearchEnityList.Size = new System.Drawing.Size(408, 23);
            this.TxtSearchEnityList.TabIndex = 3;
            this.TxtSearchEnityList.TextChanged += new System.EventHandler(this.TxtSearchEnityList_TextChanged);
            // 
            // listViewEntities
            // 
            this.listViewEntities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDisplayName,
            this.colName,
            this.colState});
            this.listViewEntities.FullRowSelect = true;
            listViewGroup5.Header = "Custom";
            listViewGroup5.Name = "Custom";
            listViewGroup5.Tag = "Custom";
            listViewGroup6.Header = "System";
            listViewGroup6.Name = "System";
            listViewGroup6.Tag = "System";
            this.listViewEntities.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup5,
            listViewGroup6});
            this.listViewEntities.HideSelection = false;
            this.listViewEntities.Location = new System.Drawing.Point(11, 44);
            this.listViewEntities.Margin = new System.Windows.Forms.Padding(4);
            this.listViewEntities.MultiSelect = false;
            this.listViewEntities.Name = "listViewEntities";
            this.listViewEntities.Size = new System.Drawing.Size(408, 699);
            this.listViewEntities.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.listViewEntities.TabIndex = 4;
            this.listViewEntities.Tag = "0";
            this.listViewEntities.UseCompatibleStateImageBehavior = false;
            this.listViewEntities.View = System.Windows.Forms.View.Details;
            this.listViewEntities.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListViewEntities_ColumnClick);
            this.listViewEntities.SelectedIndexChanged += new System.EventHandler(this.ListViewEntities_SelectedIndexChanged);
            // 
            // colDisplayName
            // 
            this.colDisplayName.Tag = "DisplayName";
            this.colDisplayName.Text = "Display Name";
            this.colDisplayName.Width = 200;
            // 
            // colName
            // 
            this.colName.Tag = "Name";
            this.colName.Text = "Name";
            this.colName.Width = 100;
            // 
            // colState
            // 
            this.colState.Tag = "State";
            this.colState.Text = "State";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.TxtSearchAttributeList);
            this.groupBox3.Controls.Add(this.listViewAttributes);
            this.groupBox3.Location = new System.Drawing.Point(437, 36);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(427, 753);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Search Attributes";
            // 
            // TxtSearchAttributeList
            // 
            this.TxtSearchAttributeList.Location = new System.Drawing.Point(9, 19);
            this.TxtSearchAttributeList.Margin = new System.Windows.Forms.Padding(5);
            this.TxtSearchAttributeList.Name = "TxtSearchAttributeList";
            this.TxtSearchAttributeList.Size = new System.Drawing.Size(408, 23);
            this.TxtSearchAttributeList.TabIndex = 7;
            this.TxtSearchAttributeList.TextChanged += new System.EventHandler(this.TxtSearchAttributeList_TextChanged);
            // 
            // listViewAttributes
            // 
            this.listViewAttributes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listViewAttributes.FullRowSelect = true;
            listViewGroup7.Header = "Custom";
            listViewGroup7.Name = "Custom";
            listViewGroup7.Tag = "Custom";
            listViewGroup8.Header = "System";
            listViewGroup8.Name = "System";
            listViewGroup8.Tag = "System";
            this.listViewAttributes.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup7,
            listViewGroup8});
            this.listViewAttributes.HideSelection = false;
            this.listViewAttributes.Location = new System.Drawing.Point(9, 44);
            this.listViewAttributes.Margin = new System.Windows.Forms.Padding(4);
            this.listViewAttributes.MultiSelect = false;
            this.listViewAttributes.Name = "listViewAttributes";
            this.listViewAttributes.Size = new System.Drawing.Size(408, 699);
            this.listViewAttributes.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.listViewAttributes.TabIndex = 6;
            this.listViewAttributes.Tag = "0";
            this.listViewAttributes.UseCompatibleStateImageBehavior = false;
            this.listViewAttributes.View = System.Windows.Forms.View.Details;
            this.listViewAttributes.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListViewAttributes_ColumnClick);
            this.listViewAttributes.SelectedIndexChanged += new System.EventHandler(this.ListViewAttributes_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Tag = "DisplayName";
            this.columnHeader1.Text = "Display Name";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Tag = "Name";
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Tag = "SchemaName";
            this.columnHeader3.Text = "Type";
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Tag = "State";
            this.columnHeader4.Text = "State";
            this.columnHeader4.Width = 100;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.TxtSearchReplaceAttributeList);
            this.groupBox6.Controls.Add(this.listViewAttributesReplaced);
            this.groupBox6.Location = new System.Drawing.Point(871, 36);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox6.Size = new System.Drawing.Size(427, 753);
            this.groupBox6.TabIndex = 22;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Search Replace Attribuite";
            // 
            // TxtSearchReplaceAttributeList
            // 
            this.TxtSearchReplaceAttributeList.Location = new System.Drawing.Point(5, 19);
            this.TxtSearchReplaceAttributeList.Margin = new System.Windows.Forms.Padding(5);
            this.TxtSearchReplaceAttributeList.Name = "TxtSearchReplaceAttributeList";
            this.TxtSearchReplaceAttributeList.Size = new System.Drawing.Size(408, 23);
            this.TxtSearchReplaceAttributeList.TabIndex = 17;
            this.TxtSearchReplaceAttributeList.TextChanged += new System.EventHandler(this.TxtSearchReplaceAttributeList_TextChanged);
            // 
            // listViewAttributesReplaced
            // 
            this.listViewAttributesReplaced.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.AttrDisplayName,
            this.AttributeName,
            this.StateAttribute});
            this.listViewAttributesReplaced.FullRowSelect = true;
            this.listViewAttributesReplaced.HideSelection = false;
            this.listViewAttributesReplaced.Location = new System.Drawing.Point(5, 44);
            this.listViewAttributesReplaced.Margin = new System.Windows.Forms.Padding(4);
            this.listViewAttributesReplaced.MultiSelect = false;
            this.listViewAttributesReplaced.Name = "listViewAttributesReplaced";
            this.listViewAttributesReplaced.Size = new System.Drawing.Size(408, 699);
            this.listViewAttributesReplaced.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.listViewAttributesReplaced.TabIndex = 16;
            this.listViewAttributesReplaced.Tag = "0";
            this.listViewAttributesReplaced.UseCompatibleStateImageBehavior = false;
            this.listViewAttributesReplaced.View = System.Windows.Forms.View.Details;
            this.listViewAttributesReplaced.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListViewAttributesReplaced_ColumnClick);
            this.listViewAttributesReplaced.SelectedIndexChanged += new System.EventHandler(this.ListViewAttributesReplaced_SelectedIndexChanged);
            // 
            // AttrDisplayName
            // 
            this.AttrDisplayName.Tag = "DisplayName";
            this.AttrDisplayName.Text = "Display Name";
            this.AttrDisplayName.Width = 200;
            // 
            // AttributeName
            // 
            this.AttributeName.Tag = "Name";
            this.AttributeName.Text = "Name";
            this.AttributeName.Width = 100;
            // 
            // StateAttribute
            // 
            this.StateAttribute.Tag = "State";
            this.StateAttribute.Text = "State";
            this.StateAttribute.Width = 100;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CmdCheckAllForms);
            this.groupBox1.Controls.Add(this.CmdCheckAllViews);
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Location = new System.Drawing.Point(1305, 36);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(485, 751);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Dependency";
            // 
            // CmdCheckAllForms
            // 
            this.CmdCheckAllForms.BackColor = System.Drawing.SystemColors.Menu;
            this.CmdCheckAllForms.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CmdCheckAllForms.Location = new System.Drawing.Point(382, 30);
            this.CmdCheckAllForms.Margin = new System.Windows.Forms.Padding(4);
            this.CmdCheckAllForms.Name = "CmdCheckAllForms";
            this.CmdCheckAllForms.Size = new System.Drawing.Size(95, 26);
            this.CmdCheckAllForms.TabIndex = 21;
            this.CmdCheckAllForms.Text = "Check All";
            this.CmdCheckAllForms.UseVisualStyleBackColor = false;
            this.CmdCheckAllForms.Click += new System.EventHandler(this.CmdCheckAllForms_Click);
            // 
            // CmdCheckAllViews
            // 
            this.CmdCheckAllViews.BackColor = System.Drawing.SystemColors.Menu;
            this.CmdCheckAllViews.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CmdCheckAllViews.Location = new System.Drawing.Point(381, 391);
            this.CmdCheckAllViews.Margin = new System.Windows.Forms.Padding(4);
            this.CmdCheckAllViews.Name = "CmdCheckAllViews";
            this.CmdCheckAllViews.Size = new System.Drawing.Size(95, 26);
            this.CmdCheckAllViews.TabIndex = 22;
            this.CmdCheckAllViews.Text = "Check All";
            this.CmdCheckAllViews.UseVisualStyleBackColor = false;
            this.CmdCheckAllViews.Click += new System.EventHandler(this.CmdCheckAllViews_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.listViewView);
            this.groupBox5.Location = new System.Drawing.Point(8, 414);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox5.Size = new System.Drawing.Size(469, 328);
            this.groupBox5.TabIndex = 20;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Found in System View";
            // 
            // listViewView
            // 
            this.listViewView.CheckBoxes = true;
            this.listViewView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ViewName,
            this.ViewState});
            this.listViewView.FullRowSelect = true;
            this.listViewView.HideSelection = false;
            this.listViewView.Location = new System.Drawing.Point(8, 21);
            this.listViewView.Margin = new System.Windows.Forms.Padding(4);
            this.listViewView.MultiSelect = false;
            this.listViewView.Name = "listViewView";
            this.listViewView.Size = new System.Drawing.Size(453, 299);
            this.listViewView.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.listViewView.TabIndex = 10;
            this.listViewView.Tag = "0";
            this.listViewView.UseCompatibleStateImageBehavior = false;
            this.listViewView.View = System.Windows.Forms.View.Details;
            this.listViewView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ListViewView_ItemChecked);
            // 
            // ViewName
            // 
            this.ViewName.Tag = "ViewName";
            this.ViewName.Text = "View Name";
            this.ViewName.Width = 300;
            // 
            // ViewState
            // 
            this.ViewState.Tag = "State";
            this.ViewState.Text = "State";
            this.ViewState.Width = 100;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.listViewForms);
            this.groupBox4.Location = new System.Drawing.Point(8, 52);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(469, 328);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Found in System Forms";
            // 
            // listViewForms
            // 
            this.listViewForms.CheckBoxes = true;
            this.listViewForms.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FormName,
            this.State});
            this.listViewForms.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewForms.FullRowSelect = true;
            this.listViewForms.HideSelection = false;
            this.listViewForms.Location = new System.Drawing.Point(8, 21);
            this.listViewForms.Margin = new System.Windows.Forms.Padding(4);
            this.listViewForms.MultiSelect = false;
            this.listViewForms.Name = "listViewForms";
            this.listViewForms.Size = new System.Drawing.Size(453, 299);
            this.listViewForms.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.listViewForms.TabIndex = 8;
            this.listViewForms.Tag = "0";
            this.listViewForms.UseCompatibleStateImageBehavior = false;
            this.listViewForms.View = System.Windows.Forms.View.Details;
            this.listViewForms.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ListViewForms_ItemChecked);
            // 
            // FormName
            // 
            this.FormName.Tag = "FormName";
            this.FormName.Text = "Form Name";
            this.FormName.Width = 300;
            // 
            // State
            // 
            this.State.Tag = "State";
            this.State.Text = "State";
            this.State.Width = 100;
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
            // ReplaceAttributeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.toolStripMenu);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ReplaceAttributeControl";
            this.Size = new System.Drawing.Size(1799, 797);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox TxtSearchEnityList;
        private System.Windows.Forms.ListView listViewEntities;
        private System.Windows.Forms.ColumnHeader colDisplayName;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colState;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox TxtSearchAttributeList;
        private System.Windows.Forms.ListView listViewAttributes;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox TxtSearchReplaceAttributeList;
        private System.Windows.Forms.ListView listViewAttributesReplaced;
        private System.Windows.Forms.ColumnHeader AttrDisplayName;
        private System.Windows.Forms.ColumnHeader AttributeName;
        private System.Windows.Forms.ColumnHeader StateAttribute;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button CmdCheckAllViews;
        private System.Windows.Forms.ListView listViewView;
        private System.Windows.Forms.ColumnHeader ViewName;
        private System.Windows.Forms.ColumnHeader ViewState;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button CmdCheckAllForms;
        private System.Windows.Forms.ListView listViewForms;
        private System.Windows.Forms.ColumnHeader FormName;
        private System.Windows.Forms.ColumnHeader State;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton tbLoadDependency;
        private System.Windows.Forms.ToolStripButton tbDeleteSelectedDependency;
        private System.Windows.Forms.ToolStripButton tbReplaceSelectedDependency;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
    }
}
