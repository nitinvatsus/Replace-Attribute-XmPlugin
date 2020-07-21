using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using ReplaceAttributeXmPlugin.Helper;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;

namespace ReplaceAttributeXmPlugin
{
    public partial class ReplaceAttributeControl : PluginControlBase
    {
        private List<ListViewItem> _entitiesListViewItemsColl;
        private List<ListViewItem> _attributeListViewItemsColl;
        private List<ListViewItem> _attributeListViewItemsCollReplace;
        private List<ListViewItem> _formsListViewItemsColl;
        private List<ListViewItem> _viewListViewItemsColl;
        private List<ListViewGroup> _formListViewGroup;
        private List<ListViewGroup> _viewListViewGroup;

        private List<ListViewItem> _usersListViewItemsColl;
        private List<ListViewGroup> _viewListUserViewGroup;
        private List<ListViewItem> _viewListUserViewItemsColl;
        private EntityMetadata _attributesMetadata;
        private bool _itemCheckedEvnt;


        private CheckBox _chkCheckAllSystemForms;
        private CheckBox _chkCheckAllSystemViews;
        private CheckBox _chkCheckAllSystemUsers;
        private CheckBox _chkCheckAllUserViews;


        private IEnumerable<Entity> _systemViewOnUserEntity;

        #region Default Methods
        public ReplaceAttributeControl()
        {
            InitializeComponent();
        }

        private void TsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void ResetAllCheckBox()
        {
            if (_chkCheckAllSystemForms != null)
            {
                _chkCheckAllSystemForms.Text = CheckBoxTitleStrings.AllSystemForms;
                _chkCheckAllSystemForms.Checked = false;
                _chkCheckAllSystemForms.Enabled = false;
            }
            if (_chkCheckAllSystemViews != null)
            {
                _chkCheckAllSystemViews.Text = CheckBoxTitleStrings.AllSystemViews;
                _chkCheckAllSystemViews.Checked = false;
                _chkCheckAllSystemViews.Enabled = false;
            }
            if (_chkCheckAllSystemUsers != null)
            {
                _chkCheckAllSystemUsers.Text = CheckBoxTitleStrings.AllSystemUsers;
                _chkCheckAllSystemUsers.Checked = false;
                _chkCheckAllSystemUsers.Visible = false;
            }
            if (_chkCheckAllUserViews == null) return;
            _chkCheckAllUserViews.Text = CheckBoxTitleStrings.AllSystemUsersView;
            _chkCheckAllUserViews.Checked = false;
            _chkCheckAllUserViews.Enabled = false;
            _chkCheckAllUserViews.Visible = false;
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);
            listViewEntities.Items.Clear();
            listViewAttributes.Items.Clear();
            listViewAttributesReplaced.Items.Clear();
            listViewView.Items.Clear();
            listViewForms.Items.Clear();
            ResetAllCheckBox();
        }
        #endregion

        #region Load Metadata Information
        
        private void LoadEntities()
        {
            TxtSearchEnityList.Text = "";
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading entities...",
                Work = (bw, e) =>
                {
                    e.Result = CrmAction.GetAllEntities(Service);
                },
                PostWorkCallBack = e =>
                {
                    if (e.Error != null)
                    {
                        MessageBox.Show(ParentForm, e.Error.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        listViewEntities.Items.Clear();
                        _entitiesListViewItemsColl = new List<ListViewItem>();
                        foreach (var entity in (List<EntityMetadata>)e.Result)
                        {
                            LocalizedLabel localLabel = null;
                            if (entity.DisplayName.LocalizedLabels.Count > 0)
                            {
                                localLabel = entity.DisplayName.LocalizedLabels.First(l => l.LanguageCode == 1033);
                            }

                            var displayName = (localLabel != null) ? localLabel.Label : entity.SchemaName;
                            var entityType = entity.IsCustomEntity != null && (entity.IsCustomEntity.Value) ? "Custom" : "System";
                            var lvItem = new ListViewItem()
                            {
                                Name = @"Display Name",
                                ImageIndex = 0,
                                StateImageIndex = 0,
                                Text = displayName,
                                Tag = entity,  // stash the template here so we can view details later
                                Group = listViewEntities.Groups[entityType]
                            };
                            lvItem.Tag = entity;

                            var state = entity.IsManaged != null && (entity.IsManaged.Value) ? "Managed" : "Unmanaged";
                            lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, entity.LogicalName) { Tag = "Name", Name = @"Name" });
                            lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, state) { Tag = "State", Name = @"State" });
                            _entitiesListViewItemsColl.Add(lvItem);
                        }
                        SortList(listViewEntities);
                        listViewEntities.Items.AddRange(_entitiesListViewItemsColl.ToArray<ListViewItem>());
                        listViewEntities.ResumeLayout();
                        TxtSearchEnityList.Focus();
                    }
                }
            });
        }
        private void LoadAttributes()
        {
            TxtSearchAttributeList.Text = "";
            if (listViewEntities.SelectedItems.Count <= 0) return;
            TxtSearchAttributeList.Text = "";
            var item = listViewEntities.SelectedItems[0];
            var entityName = item.SubItems[1].Text;
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Attributes...",
                Work = (bw, e) =>
                {
                    e.Result = CrmAction.RetrieveEntityAttributeMeta(Service, entityName);
                },
                PostWorkCallBack = e =>
                {
                    if (e.Error != null)
                    {
                        MessageBox.Show(ParentForm, e.Error.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        _attributesMetadata = (EntityMetadata)e.Result;
                        listViewAttributes.Items.Clear();
                        _attributeListViewItemsColl = new List<ListViewItem>();
                        foreach (var attribute in _attributesMetadata.Attributes)
                        {
                            if (attribute.AttributeType == AttributeTypeCode.Virtual)
                                continue;

                            LocalizedLabel localLabel = null;
                            if (attribute.DisplayName.LocalizedLabels.Count > 0)
                            {
                                localLabel = attribute.DisplayName.LocalizedLabels.First(l => l.LanguageCode == 1033);
                            }
                            var displayName = (localLabel != null) ? localLabel.Label : attribute.SchemaName;
                            var attributeType = (attribute.IsCustomizable.Value) ? "Custom" : "System";
                            _attributeListViewItemsColl.Add(GetAttribuetListItem(attribute, displayName, attributeType));
                        }
                        SortList(listViewAttributes);
                        listViewAttributes.Items.AddRange(_attributeListViewItemsColl.ToArray<ListViewItem>());
                        TxtSearchAttributeList.Focus();
                            
                    }
                }
            });
        }
        private void LoadFormDependency()
        {
            if (listViewAttributes.SelectedItems.Count <= 0) return;
            var entityItem = (EntityMetadata)listViewEntities.SelectedItems[0].Tag;
            var objectTypeCode = entityItem.ObjectTypeCode;
            var item = listViewAttributes.SelectedItems[0];
            var subitem = item.SubItems[1].Text;
            var attribute = _attributesMetadata.Attributes.FirstOrDefault(k => k.LogicalName == subitem);
            if (attribute != null)
            {
                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Loading Forms Dependency...",
                    Work = (bw, e) =>
                    {
                        e.Result = CrmAction.GetAllSystemForms(Service, objectTypeCode, attribute.LogicalName);
                    },
                    PostWorkCallBack = e =>
                    {
                        if (e.Error != null)
                        {
                            MessageBox.Show(ParentForm, e.Error.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            var formEntity = (IEnumerable<Entity>)e.Result;
                            listViewForms.Items.Clear();
                            _formsListViewItemsColl = new List<ListViewItem>();
                            _formListViewGroup = new List<ListViewGroup>();
                            foreach (var objFormEntity in formEntity)
                            {
                                var layoutXml = (string)objFormEntity["formxml"];
                                if (!XmlOperation.FindXmlControl(layoutXml, attribute.LogicalName, "control", "id"))
                                    continue;
                                var displayName = "";
                                if (objFormEntity.Attributes.Contains("name"))
                                {
                                    displayName = (string)objFormEntity["name"];
                                }
                                ListViewGroup group = null;
                                if (objFormEntity.FormattedValues.Contains("type"))
                                {
                                    var formType = objFormEntity.FormattedValues["type"];
                                    if (_formListViewGroup.All(o => o.Name != formType))
                                    {
                                        group = new ListViewGroup(formType)
                                        {
                                            Name = formType,
                                            Tag = "FormType"
                                        };
                                        _formListViewGroup.Add(group);
                                    }
                                    else
                                    {
                                        group = _formListViewGroup.FirstOrDefault(o => o.Name == formType);
                                    }
                                }
                                var lvItem = new ListViewItem()
                                {
                                    Name = @"Name",
                                    ImageIndex = 0,
                                    StateImageIndex = 0,
                                    Text = displayName,
                                    Tag = objFormEntity,  // stash the template here so we can view details later
                                    Group = group
                                };
                                var state = ((bool)objFormEntity["ismanaged"]) ? "Managed" : "Unmanaged";
                                lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, state) { Tag = "State", Name = @"State" });
                                _formsListViewItemsColl.Add(lvItem);
                            }
                            listViewForms.Groups.AddRange(_formListViewGroup.ToArray<ListViewGroup>());
                            listViewForms.Items.AddRange(_formsListViewItemsColl.ToArray<ListViewItem>());
                            _chkCheckAllSystemForms.Enabled = _formsListViewItemsColl.Count > 0;
                            _chkCheckAllSystemForms.Visible = _formsListViewItemsColl.Count > 0;
                            tssSeparator6.Visible = _formsListViewItemsColl.Count > 0;
                            ExecuteMethod(LoadViewDependency);
                        }
                    }
                });
            }
        }
        private void LoadViewDependency()
        {
            if (listViewAttributes.SelectedItems.Count <= 0) return;
            var entityItem = (EntityMetadata)listViewEntities.SelectedItems[0].Tag;
            var objectTypeCode = entityItem.ObjectTypeCode;
            var item = listViewAttributes.SelectedItems[0];
            var subitem = item.SubItems[1].Text;
            var attribute = _attributesMetadata.Attributes.FirstOrDefault(k => k.LogicalName == subitem);
            if (attribute != null)
            {
                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Loading View Dependency...",
                    Work = (bw, e) =>
                    {
                        e.Result = CrmAction.GetAllSystemViews(Service, objectTypeCode, attribute.LogicalName);
                    },
                    PostWorkCallBack = e =>
                    {
                        if (e.Error != null)
                        {
                            MessageBox.Show(ParentForm, e.Error.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            var viewEntities = (IEnumerable<Entity>)e.Result;
                            listViewView.Items.Clear();
                            _viewListViewItemsColl = new List<ListViewItem>();
                            _viewListViewGroup = new List<ListViewGroup>();
                            var attributeName = attribute.LogicalName;

                            foreach (var objViewEntity in viewEntities)
                            {
                                string layoutXml;
                                var isFound = false;
                                if (objViewEntity.Attributes.Contains("layoutxml"))
                                {
                                    layoutXml = (string) objViewEntity["layoutxml"];
                                    isFound = XmlOperation.FindXmlControl(layoutXml, attributeName, "cell", "name");
                                }
                                if (!isFound)
                                {
                                    if (objViewEntity.Attributes.Contains("fetchxml"))
                                    {
                                        layoutXml = (string) objViewEntity["fetchxml"];
                                        isFound = XmlOperation.FindXmlControl(layoutXml, attributeName, "attribute",
                                            "name");
                                        if (!isFound)
                                            isFound = XmlOperation.FindXmlControl(layoutXml, attributeName, "condition",
                                                "attribute");
                                    }
                                }

                                if (!isFound) continue;
                                var displayName = "";
                                if (objViewEntity.Attributes.Contains("name"))
                                {
                                    displayName = (string)objViewEntity["name"];
                                }
                                ListViewGroup group = null;
                                if (objViewEntity.Attributes.Contains("layoutxml"))
                                {
                                    var columnXml = (string)objViewEntity.Attributes["layoutxml"];
                                    if (columnXml.IndexOf(attributeName, StringComparison.Ordinal) >= 0)
                                    {
                                        if (_viewListViewGroup.All(o => o.Name != "LayoutXML"))
                                        {
                                            group = new ListViewGroup("Layout XML")
                                            {
                                                Name = "LayoutXML",
                                                Tag = "LayoutXML"
                                            };
                                            _viewListViewGroup.Add(group);
                                        }
                                        else
                                        {
                                            group = _viewListViewGroup.FirstOrDefault(o => o.Name == "LayoutXML");
                                        }
                                    }
                                    else
                                    {
                                        if (_viewListViewGroup.All(o => o.Name != "FetchXML"))
                                        {
                                            group = new ListViewGroup("Fetch XML")
                                            {
                                                Name = "FetchXML",
                                                Tag = "FetchXML"
                                            };
                                            _viewListViewGroup.Add(group);
                                        }
                                        else
                                        {
                                            group = _viewListViewGroup.FirstOrDefault(o => o.Name == "FetchXML");
                                        }
                                    }
                                }
                                else if (objViewEntity.Attributes.Contains("fetchxml"))
                                {
                                    var columnXml = (string)objViewEntity.Attributes["fetchxml"];
                                    if (columnXml.IndexOf(attributeName, StringComparison.Ordinal) >= 0)
                                    {
                                        if (_viewListViewGroup.All(o => o.Name != "FetchXML"))
                                        {
                                            group = new ListViewGroup("Fetch XML")
                                            {
                                                Name = "FetchXML",
                                                Tag = "FetchXML"
                                            };
                                            _viewListViewGroup.Add(group);
                                        }
                                        else
                                        {
                                            group = _viewListViewGroup.FirstOrDefault(o => o.Name == "FetchXML");
                                        }
                                    }
                                }

                                var lvItem = new ListViewItem()
                                {
                                    Name = @"Name",
                                    ImageIndex = 0,
                                    StateImageIndex = 0,
                                    Text = displayName,
                                    Tag = objViewEntity,  // stash the template here so we can view details later
                                    Group = group
                                };
                                var state = ((bool)objViewEntity["ismanaged"]) ? "Managed" : "Unmanaged";
                                lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, state) { Tag = "State", Name = @"State" });
                                _viewListViewItemsColl.Add(lvItem);
                            }
                            _chkCheckAllSystemViews.Enabled = _viewListViewItemsColl.Count > 0;
                            _chkCheckAllSystemViews.Visible = _viewListViewItemsColl.Count > 0;
                            //tssSeparator7.Visible = _viewListViewItemsColl.Count > 0;
                            listViewView.Groups.AddRange(_viewListViewGroup.ToArray<ListViewGroup>());
                            listViewView.Items.AddRange(_viewListViewItemsColl.ToArray<ListViewItem>());

                        }
                    }
                });
            }
        }
        
        private ListViewItem GetAttribuetListItem(AttributeMetadata attribute, string displayName, string attributeType)
        {
            var lvItem = new ListViewItem()
            {
                Name = @"Display Name",
                ImageIndex = 0,
                StateImageIndex = 0,
                Text = displayName,
                Tag = attribute,  // stash the template here so we can view details later
                Group = listViewAttributes.Groups[attributeType]
            };
            lvItem.Tag = attribute;

            var state = attribute.IsManaged != null && (attribute.IsManaged.Value) ? "Managed" : "Unmanaged";
            lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, attribute.LogicalName) { Tag = "Name", Name = @"Name" });
            if (attribute.AttributeType != null)
                lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, attribute.AttributeType.Value.ToString())
                    {Tag = "Type", Name = @"Type"});
            lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, state) { Tag = "State", Name = @"State" });
            return lvItem;
        }
       
        #region Default Sorting in ListView Load
        private static void SortList(ListView objListView)
        {
            int.TryParse(objListView.Tag.ToString(), out int currCol);
            SortList(currCol, objListView);
        }
        private static void SortList(int column, ListView objListView)
        {
            var currSortCol = int.Parse(objListView.Tag.ToString());
            objListView.SuspendLayout();
            if (column == currSortCol)
            {
                objListView.Sorting = SortOrder.Ascending;
            }
            else
            {
                objListView.Tag = column;
            }
            objListView.ListViewItemSorter = new ListViewItemComparer(column, objListView.Sorting);
            objListView.ResumeLayout();
        }
        #endregion

        #region Form Events

        #region Column Sorting
        private void ListViewEntities_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listViewEntities.Sorting = ((listViewEntities.Sorting == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending);
            listViewEntities.ListViewItemSorter = new ListViewItemComparer(e.Column, listViewEntities.Sorting);
        }

        private void ListViewAttributes_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listViewAttributes.Sorting = ((listViewAttributes.Sorting == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending);
            listViewAttributes.ListViewItemSorter = new ListViewItemComparer(e.Column, listViewAttributes.Sorting);
        }

        private void ListViewAttributesReplaced_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listViewAttributesReplaced.Sorting = ((listViewAttributesReplaced.Sorting == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending);
            listViewAttributesReplaced.ListViewItemSorter = new ListViewItemComparer(e.Column, listViewAttributesReplaced.Sorting);
        }
        #endregion
        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            tbLoadDependency.Enabled = false;
            tbDeleteSelectedDependency.Enabled = false;
            tbReplaceSelectedDependency.Enabled = false;
            ResetAllCheckBox();
            listViewAttributes.Items.Clear();
            listViewAttributesReplaced.Items.Clear();
            listViewForms.Items.Clear();
            listViewView.Items.Clear();
            ExecuteMethod(LoadEntities);
        }
        private void ListViewEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabMainDetails.TabPages.Remove(tabUserView);
            tbLoadDependency.Enabled = false;
            tbDeleteSelectedDependency.Enabled = false;
            tbReplaceSelectedDependency.Enabled = false;
            listViewAttributes.Items.Clear();
            listViewAttributesReplaced.Items.Clear();
            listViewForms.Items.Clear();
            listViewView.Items.Clear();
            ResetAllCheckBox();
            ExecuteMethod(LoadAttributes);
        }
        private void ListViewAttributes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewAttributes.SelectedItems.Count <= 0) return;
            tbLoadDependency.Enabled = true;
            if (tabMainDetails.TabPages.Count == 1)
                tabMainDetails.TabPages.Add(tabUserView);

            TxtSearchReplaceAttributeList.Text = "";
            _chkCheckAllSystemForms.Text = CheckBoxTitleStrings.AllSystemForms;
            _chkCheckAllSystemViews.Text = CheckBoxTitleStrings.AllSystemViews;
            _chkCheckAllUserViews.Text = CheckBoxTitleStrings.AllSystemUsersView;

            _chkCheckAllSystemForms.Checked = false;
            _chkCheckAllSystemViews.Checked = false;
            _chkCheckAllUserViews.Checked = false;
            _chkCheckAllSystemUsers.Checked = false;
          
            _chkCheckAllSystemForms.Enabled = false;
            _chkCheckAllSystemViews.Enabled = false;
            _chkCheckAllUserViews.Enabled = false;

            listViewAttributesReplaced.Items.Clear();
            listViewForms.Items.Clear();
            listViewView.Items.Clear();
            listViewViewUsers.Items.Clear();

            tbDeleteSelectedDependency.Enabled = false;
            tbReplaceSelectedDependency.Enabled = false;
            tbDeleteSelectedUserDependency.Enabled = false;
            tbReplaceSelectedUserDependency.Enabled = false;


            var item = listViewAttributes.SelectedItems[0];
            var attributeCurrent = (AttributeMetadata)item.Tag;
            _attributeListViewItemsCollReplace = new List<ListViewItem>();
            var typeGroups = new List<ListViewGroup>();
            var isGroupAdd = false;

            foreach (var attribute in _attributesMetadata.Attributes)
            {
                if (attribute.AttributeType == AttributeTypeCode.Virtual)
                    continue;

                LocalizedLabel localLabel = null;
                if (attribute.DisplayName.LocalizedLabels.Count > 0)
                {
                    localLabel = attribute.DisplayName.LocalizedLabels.First(l => l.LanguageCode == 1033);
                }

                var displayName = (localLabel != null) ? localLabel.Label : attribute.SchemaName;
                var group = new ListViewGroup(attribute.AttributeTypeName.Value)
                {
                    Name = attribute.AttributeTypeName.Value,
                    Tag = "AttrType"
                };
                if (typeGroups.Count > 0)
                {
                    if (!typeGroups.Contains(group))
                    {
                        typeGroups.Add(group);
                    }
                }
                else
                {
                    typeGroups.Add(group);
                }

                var lvItem = new ListViewItem()
                {
                    Name = @"Display Name",
                    ImageIndex = 0,
                    StateImageIndex = 0,
                    Text = displayName,
                    Tag = attribute,  // stash the template here so we can view details later
                    Group = typeGroups.FirstOrDefault(k => k.Name == attribute.AttributeTypeName.Value)
                };

                lvItem.Tag = attribute;

                var state = attribute.IsManaged != null && (attribute.IsManaged.Value) ? "Managed" : "Unmanaged";
                lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, attribute.LogicalName) { Tag = "Name", Name = @"Name" });
                lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, state) { Tag = "State", Name = @"State" });
                if (attribute.AttributeTypeName.Value == attributeCurrent.AttributeTypeName.Value)
                {
                    if (!isGroupAdd)
                    {
                        listViewAttributesReplaced.Groups.Add(group);
                        isGroupAdd = true;
                    }
                    lvItem.BackColor = Color.GreenYellow;
                }
                if (lvItem.SubItems[1].Text != attributeCurrent.LogicalName)
                    _attributeListViewItemsCollReplace.Add(lvItem);
            }
            SortList(listViewAttributesReplaced);

            foreach (var group in typeGroups.Where(group => group.Name != attributeCurrent.AttributeTypeName.Value))
            {
                listViewAttributesReplaced.Groups.Add(group);
            }
            listViewAttributesReplaced.Items.AddRange(_attributeListViewItemsCollReplace.ToArray<ListViewItem>());
        }
        private void TbLoadDependency_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadFormDependency);
        }
       
        #endregion

        #endregion

        #region Search in List View By Text Changed
        private void TxtSearchAttributeList_TextChanged(object sender, EventArgs e)
        {
            var searchList = _attributeListViewItemsColl.Where(l => l.SubItems[0].Text.ToUpper().Contains(TxtSearchAttributeList.Text.ToUpper()));
            listViewAttributes.Items.Clear();
            listViewAttributes.Items.AddRange(searchList.ToArray());
        }
        private void TxtSearchEnityList_TextChanged(object sender, EventArgs e)
        {
            var searchList = _entitiesListViewItemsColl.Where(l => l.SubItems[0].Text.ToUpper().Contains(TxtSearchEnityList.Text.ToUpper()));
            listViewEntities.Items.Clear();
            listViewEntities.Items.AddRange(searchList.ToArray());
        }
        private void TxtSearchReplaceAttributeList_TextChanged(object sender, EventArgs e)
        {
            var searchList = _attributeListViewItemsCollReplace.Where(l => l.SubItems[0].Text.ToUpper().Contains(TxtSearchReplaceAttributeList.Text.ToUpper()));
            listViewAttributesReplaced.Items.Clear();
            listViewAttributesReplaced.Items.AddRange(searchList.ToArray());
        }
        #endregion

        

        #region Delete CRM Dependency from CRM SystemForm and SystemView
        
        private void TbDeleteSelectedDependency_Click(object sender, EventArgs e)
        {
            if (_attributesMetadata?.Attributes != null && _attributesMetadata.Attributes.Any())
            {
                if (listViewAttributes.SelectedItems.Count > 0)
                {
                    var lstItem = listViewAttributes.SelectedItems[0];
                    var subitem = lstItem.SubItems[1].Text;
                    var objRequest = new XmlRequest()
                    {
                        IsUserView = false,
                        Objentity = _attributesMetadata,
                        ObjPlugin = this,
                        ServiceProxy = Service,
                        OldAttributeName = subitem,
                        ReplaceAttributeName = string.Empty,
                        IsViewDependency = false,
                        CheckedFromItems = null,
                        CheckedItemsViews = null,
                        ObjAttDataReplace = null
                    };

                    if (listViewView.CheckedItems.Count > 0 && listViewForms.CheckedItems.Count > 0)
                    {
                        objRequest.CheckedFromItems = listViewForms.CheckedItems.Cast<ListViewItem>().ToList();
                        objRequest.CheckedItemsViews = listViewView.CheckedItems.Cast<ListViewItem>().ToList();
                        objRequest.IsViewDependency = true;
                        XmlOperation.DeleteFormDependency(objRequest, null);
                    }
                    else if (listViewForms.CheckedItems.Count > 0)
                    {
                        objRequest.CheckedFromItems = listViewForms.CheckedItems.Cast<ListViewItem>().ToList();
                        XmlOperation.DeleteFormDependency(objRequest, null);
                    }
                    else if (listViewView.CheckedItems.Count > 0)
                    {
                        objRequest.CheckedItemsViews = listViewView.CheckedItems.Cast<ListViewItem>().ToList();
                        XmlOperation.DeleteViewDependency(objRequest, null);
                    }
                }
                else
                {
                    MessageBox.Show(@"Please Select Attribute First");
                }
            }
            else
            {
                MessageBox.Show(@"Attribute Metadata not found", @"Information Missing", MessageBoxButtons.OK);
            }
        }
        #endregion

        #region Enable Depenency Operation Button
        private void EnableDependencyOperation()
        {
          
            if (listViewForms.Items.Cast<ListViewItem>().Any(k => k.Checked))
                tbDeleteSelectedDependency.Enabled = true;
            else if (listViewView.Items.Cast<ListViewItem>().Any(k => k.Checked))
                tbDeleteSelectedDependency.Enabled = true;
            else
            {
                tbReplaceSelectedDependency.Enabled = false;
                tbReplaceSelectedUserDependency.Enabled = false;
                tbDeleteSelectedDependency.Enabled = false;
            }
            if (!tbDeleteSelectedDependency.Enabled) return;
            if (listViewAttributesReplaced.SelectedItems.Count > 0)
            {
                tbReplaceSelectedDependency.Enabled = true;
                tbReplaceSelectedUserDependency.Enabled = true;
            }
        }
        private void ListViewForms_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            EnableDependencyOperation();
        }
        private void ListViewView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            EnableDependencyOperation();
        }
        private void ListViewAttributesReplaced_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableDependencyOperation();
        }
        #endregion

        private void TbReplaceSelectedDependency_Click(object sender, EventArgs e)
        {
            if (_attributesMetadata?.Attributes != null && _attributesMetadata.Attributes.Any())
            {
                if (listViewAttributesReplaced.SelectedItems.Count > 0)
                {
                    if (listViewAttributes.SelectedItems.Count > 0)
                    {
                        var lstItem = listViewAttributes.SelectedItems[0];
                        var subitem = lstItem.SubItems[1].Text;
                        var lstItemReplace = listViewAttributesReplaced.SelectedItems[0];
                        var subitemReplace = lstItemReplace.SubItems[1].Text;
                        var objAttData = (AttributeMetadata)lstItemReplace.Tag;

                        var objRequest = new XmlRequest
                        {
                            IsUserView = false,
                            Objentity = _attributesMetadata,
                            ObjPlugin = this,
                            ServiceProxy = Service,
                            OldAttributeName = subitem,
                            ReplaceAttributeName = subitemReplace,
                            ObjAttDataReplace = objAttData,
                            IsViewDependency = false,
                            CheckedFromItems = null,
                            CheckedItemsViews = null
                        };

                        if (listViewView.CheckedItems.Count > 0 && listViewForms.CheckedItems.Count > 0)
                        {
                            objRequest.CheckedFromItems = listViewForms.CheckedItems.Cast<ListViewItem>().ToList();
                            objRequest.CheckedItemsViews = listViewView.CheckedItems.Cast<ListViewItem>().ToList();
                            objRequest.IsViewDependency = true;
                            XmlOperation.ReplaceFormDependency(objRequest, null);
                        }
                        else if (listViewForms.CheckedItems.Count > 0)
                        {
                            objRequest.CheckedFromItems = listViewForms.CheckedItems.Cast<ListViewItem>().ToList();
                            XmlOperation.ReplaceFormDependency(objRequest, null);
                        }
                        else if (listViewView.CheckedItems.Count > 0)
                        {
                            objRequest.CheckedItemsViews = listViewView.CheckedItems.Cast<ListViewItem>().ToList();
                            XmlOperation.ReplaceViewDependency(objRequest, null);
                        }
                    }
                    else
                    {
                        MessageBox.Show(@"Please Select Attribute First");
                    }
                }
                else
                {
                    MessageBox.Show(@"Please Select Replace Attribute First");
                }
            }
            else
            {
                MessageBox.Show(@"Attribute Metadata not found", @"Information Missing", MessageBoxButtons.OK);
            }
        }

        private UserViewRequest GetUserViewRequest()
        {
            UserViewRequest userViewRequest = null;
            if (_attributesMetadata?.Attributes != null && _attributesMetadata.Attributes.Any())
            {
                if (listViewAttributes.SelectedItems.Count > 0)
                {
                    var item = listViewAttributes.SelectedItems[0];
                    var attributeCurrent = (AttributeMetadata)item.Tag;
                    userViewRequest = new UserViewRequest()
                    {
                        AttributeSelected = attributeCurrent.LogicalName,
                        OldAttributeDisplayName = item.Text,
                        Plugin = this,
                        ServiceProxy = Service,
                        ObjEntity = _attributesMetadata
                    };
                    if (listViewAttributesReplaced.SelectedItems.Count <= 0) return userViewRequest;
                    var itemReplace = listViewAttributesReplaced.SelectedItems[0];
                    var attributeCurrentReplace = (AttributeMetadata)itemReplace.Tag;
                    userViewRequest.NewAttributeDisplayName = itemReplace.Text;
                    userViewRequest.NewAttributeName = attributeCurrentReplace.LogicalName;
                }
                else
                {
                    MessageBox.Show(@"Please Select Attribute First");
                }
            }
            else
            {
                MessageBox.Show(@"Attribute Metadata not found", @"Information Missing", MessageBoxButtons.OK);
            }

            return userViewRequest;
        }

       

        private void ReplaceAttributeControl_Resize(object sender, EventArgs e)
        {
            groupBox1.Width =  Width - (groupBox2.Width + groupBox3.Width+groupBox6.Width + 20);
            groupBox4.Height = Convert.ToInt32(Height / 2.0 - 70);
            groupBox5.Height = Convert.ToInt32(Height / 2.0 -5);
        }

        private void LoadSystemUser()
        {
            TxtSearchUsersList.Text = "";
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading System Users...",
                Work = (bw, e) =>
                {
                    e.Result = CrmAction.GetAllSystemUsers(Service);
                },
                PostWorkCallBack = e =>
                {
                    if (e.Error != null)
                    {
                        MessageBox.Show(ParentForm, e.Error.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        listViewSystemUsers.Items.Clear();
                        var userEntities = (IEnumerable<Entity>)e.Result;
                        _usersListViewItemsColl = new List<ListViewItem>();
                        foreach (var objUserEntity in userEntities)
                        {
                            var displayName = "";
                            if (objUserEntity.Attributes.Contains("fullname"))
                            {
                                displayName = (string)objUserEntity["fullname"];
                            }
                            var lvItem = new ListViewItem()
                            {
                                Name = @"Name",
                                ImageIndex = 0,
                                StateImageIndex = 0,
                                Text = displayName,
                                Tag = objUserEntity
                            };

                            if (objUserEntity.Attributes.Contains("domainname"))
                            {
                                lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, (string)objUserEntity["domainname"]) { Tag = "domainname", Name = @"domainname" });
                            }
                            if (objUserEntity.Attributes.Contains("islicensed"))
                            {
                                var state = ((bool)objUserEntity["islicensed"]) ? "Licensed" : "Unlicensed";
                                lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, state) { Tag = "islicensed", Name = @"islicensed" });
                            }
                            _usersListViewItemsColl.Add(lvItem);
                        }
                        listViewSystemUsers.Items.AddRange(_usersListViewItemsColl.ToArray<ListViewItem>());

                        SortList(listViewSystemUsers);
                        _chkCheckAllSystemUsers.Visible = true;
                        _chkCheckAllSystemUsers.Enabled = true;
                        TbLoadUserView.Visible = true;
                        listViewSystemUsers.ResumeLayout();
                        TxtSearchUsersList.Focus();
                    }
                }
            });
        }

        private void LoadSystemUser(string fetchXml)
        {
            TxtSearchUsersList.Text = "";
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading System Users...",
                Work = (bw, e) =>
                {
                    e.Result = CrmAction.GetAllSystemUsers(Service, fetchXml);
                },
                PostWorkCallBack = e =>
                {
                    if (e.Error != null)
                    {
                        MessageBox.Show(ParentForm, e.Error.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        listViewSystemUsers.Items.Clear();
                        var userEntities = (IEnumerable<Entity>)e.Result;
                        _usersListViewItemsColl = new List<ListViewItem>();
                        foreach (var objUserEntity in userEntities)
                        {
                            var displayName = "";
                            if (objUserEntity.Attributes.Contains("fullname"))
                            {
                                displayName = (string)objUserEntity["fullname"];
                            }
                            var lvItem = new ListViewItem()
                            {
                                Name = @"Name",
                                ImageIndex = 0,
                                StateImageIndex = 0,
                                Text = displayName,
                                Tag = objUserEntity
                            };

                            if (objUserEntity.Attributes.Contains("domainname"))
                            {
                                lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, (string)objUserEntity["domainname"]) { Tag = "domainname", Name = @"domainname" });
                            }
                            if (objUserEntity.Attributes.Contains("islicensed"))
                            {
                                var state = ((bool)objUserEntity["islicensed"]) ? "Licensed" : "Unlicensed";
                                lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, state) { Tag = "islicensed", Name = @"islicensed" });
                            }
                            _usersListViewItemsColl.Add(lvItem);
                        }
                        listViewSystemUsers.Items.AddRange(_usersListViewItemsColl.ToArray<ListViewItem>());

                        SortList(listViewSystemUsers);
                        _chkCheckAllSystemUsers.Visible = true;
                        _chkCheckAllSystemUsers.Enabled = true;
                        TbLoadUserView.Visible = true;
                        listViewSystemUsers.ResumeLayout();
                        
                        TxtSearchUsersList.Focus();
                    }
                }
            });
        }
        private void LoadSystemUserSystemViews()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading System View for User Entity...",
                Work = (bw, e) =>
                {
                    e.Result = CrmAction.GetAllSystemUsersViews(Service);
                },
                PostWorkCallBack = e =>
                {
                    if (e.Error != null)
                    {
                        MessageBox.Show(ParentForm, e.Error.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        TblstUserView.DropDownItems.Clear();
                        _systemViewOnUserEntity = (IEnumerable<Entity>)e.Result;
                        var systemViewOnUserEntity = _systemViewOnUserEntity as Entity[] ?? _systemViewOnUserEntity.ToArray();
                        foreach (var objUserEntity in systemViewOnUserEntity)
                        {
                            var displayName = "";
                            if (objUserEntity.Attributes.Contains("name"))
                            {
                                displayName = (string)objUserEntity["name"];
                            }
                            TblstUserView.DropDownItems.Add(displayName);
                        }
                        TblstUserView.Enabled = systemViewOnUserEntity.Length > 0;
                    }
                }
            });
        }

        private void TabMainDetaiils_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabMainDetails.SelectedTab == tabUserView)
            {
                if (listViewSystemUsers.Items.Count == 0)
                {
                    ExecuteMethod(LoadSystemUser);
                    ExecuteMethod(LoadSystemUserSystemViews);
                    TblstUserView.Text = @"Select User View";
                    TblstUserView.Enabled = false;
                    _chkCheckAllSystemUsers.Enabled = true;
                    listViewViewUsers.Items.Clear();
                }
                ToggleTabButtonVisibility(false);
            }
            else
            {
                ToggleTabButtonVisibility(true);
            }
        }

        private void ToggleTabButtonVisibility(bool currentState)
        {
            tbLoadDependency.Visible = currentState;
            tbDeleteSelectedDependency.Visible = currentState;
            tbReplaceSelectedDependency.Visible = currentState;
            _chkCheckAllSystemForms.Visible = currentState;
            _chkCheckAllSystemViews.Visible = currentState;
            tssSeparator6.Visible = true;
            tssSeparator7.Visible = !currentState;
            tssSeparator8.Visible = true;
            TblstUserView.Visible = !currentState;
            TbLoadUserView.Visible = !currentState;
            tbDeleteSelectedUserDependency.Visible = !currentState;
            tbReplaceSelectedUserDependency.Visible = !currentState;
            _chkCheckAllSystemUsers.Visible = !currentState;
            _chkCheckAllUserViews.Visible = !currentState;
        }

        

        private void TbLoadUserView_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadUsersViews);
        }
        private void LoadUsersViews()
        {
            _viewListUserViewGroup = new List<ListViewGroup>();
            _viewListUserViewItemsColl = new List<ListViewItem>();
            listViewViewUsers.Items.Clear();

            var checkedItemsProcess = ConvertListViweItems(listViewSystemUsers.CheckedItems.Cast<ListViewItem>().ToList());
            LoadUserView(checkedItemsProcess);
        }
        private static List<ClsViiewItemsChecked> ConvertListViweItems(IEnumerable<ListViewItem> checkedItems)
        {
            return checkedItems.Select(item => new ClsViiewItemsChecked { CheckedItem = item, CheckItemProcessed = false }).ToList();
        }
        private void LoadUserView(IReadOnlyCollection<ClsViiewItemsChecked> checkedItems)
        {
            if (checkedItems.All(c => c.CheckItemProcessed)) return;
            {
                var objListItem = checkedItems.FirstOrDefault(c => c.CheckItemProcessed == false);
                if (objListItem == null) return;
                {
                    var systemUser = (Entity)objListItem.CheckedItem.Tag;
                    ((CrmServiceClient)Service).CallerId = systemUser.Id;
                    var itemProcssed = checkedItems.Count(c => c.CheckItemProcessed);
                    var totalItems = checkedItems.Count;
                    var itemsRemain = totalItems - itemProcssed;
                    var objUserViewRequest = GetUserViewRequest();
                    if (objUserViewRequest != null)
                    {
                        WorkAsync(new WorkAsyncInfo
                        {
                            Message =
                                $"Fetching User View  for {objListItem.CheckedItem.Text}\n Total Users: {totalItems}, Process:{itemProcssed} Remaining:{itemsRemain} ",
                            Work = (bw, e) =>
                            {
                                e.Result = CrmAction.GetAllUserViews(Service, objUserViewRequest.ObjEntity.ObjectTypeCode, objUserViewRequest.AttributeSelected);
                            },
                            PostWorkCallBack = e =>
                            {
                                if (e.Error != null)
                                {
                                    MessageBox.Show(ParentForm, e.Error.Message, @"Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                    if (e.Result != null)
                                        AddUserViews((IEnumerable<Entity>) e.Result, systemUser, objUserViewRequest);
                                    objListItem.CheckItemProcessed = true;
                                    LoadUserView(checkedItems);
                                    if (checkedItems.Any(c => c.CheckItemProcessed == false)) return;
                                    listViewViewUsers.Groups.AddRange(_viewListUserViewGroup.ToArray<ListViewGroup>());
                                    listViewViewUsers.Items.AddRange(_viewListUserViewItemsColl.ToArray<ListViewItem>());
                                    if (listViewViewUsers.Items.Count > 0)
                                    {
                                        _chkCheckAllUserViews.Text = CheckBoxTitleStrings.AllSystemUsersView;
                                        _chkCheckAllUserViews.Enabled = true;
                                    }
                                    SortList(listViewViewUsers);
                                }
                            }
                        });
                    }
                }
            }
        }

        private void AddUserViews(IEnumerable<Entity> userViews, Entity systemUser, UserViewRequest objUserViewRequest)
        {
            var userFullName = systemUser.Attributes.Contains("fullname") ? (string)systemUser["fullname"] : "";
            foreach (var objViewEntity in userViews)
            {
                var isFound = false;
                if (objViewEntity.Attributes.Contains("layoutxml"))
                {
                    var layoutXml = (string) objViewEntity["layoutxml"];
                    isFound = XmlOperation.FindXmlControl(layoutXml, objUserViewRequest.AttributeSelected, "cell",
                        "name");
                    if (!isFound)
                    {
                        layoutXml = (string) objViewEntity["fetchxml"];
                        isFound = XmlOperation.FindXmlControl(layoutXml, objUserViewRequest.AttributeSelected,
                            "attribute", "name");
                        if (!isFound)
                            isFound = XmlOperation.FindXmlControl(layoutXml, objUserViewRequest.AttributeSelected,
                                "condition", "attribute");
                    }
                }
                else if (objViewEntity.Attributes.Contains("fetchxml"))
                {
                    var layoutXml = (string) objViewEntity["fetchxml"];
                    isFound = XmlOperation.FindXmlControl(layoutXml, objUserViewRequest.AttributeSelected,
                        "attribute", "name");
                    if (!isFound)
                        isFound = XmlOperation.FindXmlControl(layoutXml, objUserViewRequest.AttributeSelected,
                            "condition", "attribute");
                }

                if (!isFound) continue;
                var displayName = "";
                if (objViewEntity.Attributes.Contains("name"))
                {
                    displayName = (string)objViewEntity["name"];
                }
                ListViewGroup group;
                if (_viewListUserViewGroup.All(o => o.Name != userFullName))
                {
                    group = new ListViewGroup(userFullName)
                    {
                        Name = userFullName,
                        Tag = systemUser
                    };
                    _viewListUserViewGroup.Add(group);
                }
                else
                {
                    group = _viewListUserViewGroup.FirstOrDefault(o => o.Name == userFullName);
                }

                var lvItem = new ListViewItem()
                {
                    Name = @"ViewName",
                    ImageIndex = 0,
                    StateImageIndex = 0,
                    Text = displayName,
                    Tag = objViewEntity,  // stash the template here so we can view details later
                    Group = group
                };
                _viewListUserViewItemsColl.Add(lvItem);
            }
        }

        private void ReplaceAttributeControl_Load(object sender, EventArgs e)
        {
            if (_chkCheckAllSystemForms == null)
            {
                _chkCheckAllSystemForms = BindCheckBox(CheckBoxTitleStrings.AllSystemForms, listViewForms, 14);
                _chkCheckAllSystemForms.Visible = false;
                tssSeparator6.Visible = false;
            }
            if (_chkCheckAllSystemViews == null)
            {
                _chkCheckAllSystemViews = BindCheckBox(CheckBoxTitleStrings.AllSystemViews, listViewView, 16);
                _chkCheckAllSystemViews.Visible = false;
                tssSeparator7.Visible = false;
            }

            if (_chkCheckAllSystemUsers == null)
            {
                _chkCheckAllSystemUsers = BindCheckBox(CheckBoxTitleStrings.AllSystemUsers, listViewSystemUsers, 17);
                _chkCheckAllSystemUsers.Visible = false;
            }

            if (_chkCheckAllUserViews == null)
            {
                _chkCheckAllUserViews = BindCheckBox(CheckBoxTitleStrings.AllSystemUsersView, listViewViewUsers, 19);
                _chkCheckAllUserViews.Visible = false;
            }
            tabMainDetails.TabPages.Remove(tabUserView);
        }

        private CheckBox BindCheckBox(string checkText, IDisposable listView, int position)
        {
            var chkControl = new CheckBox {Text = checkText};
            chkControl.CheckStateChanged += CheckStageChanged;
            chkControl.BackColor = Color.Transparent;
            chkControl.Padding = new Padding(5);
            chkControl.Tag = listView;
            chkControl.Enabled = false;
            //chkControl.Visible = false;
            var host = new ToolStripControlHost(chkControl);
            toolStripMenu.Items.Insert(position,host);
            return chkControl;
        }

        private void CheckStageChanged(object sender, EventArgs e)
        {
            var checkState = (CheckBox) sender;
            if (checkState.Text == CheckBoxTitleStrings.AllSystemUsers)
            {
                if (listViewSystemUsers.Items.Count > 0)
                    TbLoadUserView.Enabled = true;
            }
            else if (checkState.Text == CheckBoxTitleStrings.AllSystemUsersCheck)
            {
                TbLoadUserView.Enabled = false;
            }
            if (checkState.Text == CheckBoxTitleStrings.AllSystemUsersView)
            {
                if (listViewViewUsers.Items.Count > 0)
                {
                    if (listViewEntities.SelectedItems.Count > 0 && listViewAttributes.SelectedItems.Count > 0)
                        tbDeleteSelectedUserDependency.Enabled = true;
                    if (listViewAttributesReplaced.SelectedItems.Count > 0)
                        tbReplaceSelectedUserDependency.Enabled = true;
                }
            }
            else if (checkState.Text == CheckBoxTitleStrings.AllSystemUsersViewCheck)
            {
                tbDeleteSelectedUserDependency.Enabled = false;
                tbReplaceSelectedUserDependency.Enabled = false;
            }

            if (checkState.Checked)
            {
                CheckUnCheckToggle((ListView) checkState.Tag, true);
                checkState.Text = checkState.Text.Replace(@"Check", @"Uncheck");
            }
            else
            {
                CheckUnCheckToggle((ListView) checkState.Tag, false);
                checkState.Text = checkState.Text.Replace(@"Uncheck", @"Check");
            }

        }

        private void CheckUnCheckToggle(ListView listView,bool currentState)
        {
            _itemCheckedEvnt = true;
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = currentState;
            }
            _itemCheckedEvnt = false;
        }


        private void ListViewSystemUsers_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (_itemCheckedEvnt) return;
            if (listViewEntities.SelectedItems.Count > 0 && listViewAttributes.SelectedItems.Count > 0)
                TbLoadUserView.Enabled = listViewSystemUsers.CheckedItems.Count > 0;
        }

        private void TblstUserView_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var selectedView = _systemViewOnUserEntity?.Where(k => k.Attributes.Contains("name") && (string)k.Attributes["name"] == e.ClickedItem.Text).FirstOrDefault();
            TblstUserView.Text = e.ClickedItem.Text;
            if (selectedView == null) return;
            if (!selectedView.Attributes.Contains("fetchxml")) return;
            listViewViewUsers.Items.Clear();
            TbLoadUserView.Enabled = false;
            tbDeleteSelectedUserDependency.Enabled = false;
            tbReplaceSelectedUserDependency.Enabled = false;
            _chkCheckAllUserViews.Enabled = false;
            LoadSystemUser((string) selectedView["fetchxml"]);
        }

        private void ListViewViewUsers_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (_itemCheckedEvnt) return;
            if (listViewEntities.SelectedItems.Count > 0 && listViewAttributes.SelectedItems.Count > 0)
                tbDeleteSelectedUserDependency.Enabled = listViewViewUsers.CheckedItems.Count > 0;
            if (listViewAttributesReplaced.SelectedItems.Count > 0)
                tbReplaceSelectedUserDependency.Enabled = listViewViewUsers.CheckedItems.Count > 0;
        }

        private void TbDeleteSelectedUserDependency_Click(object sender, EventArgs e)
        {
            if (listViewEntities.SelectedItems.Count <= 0 || listViewAttributes.SelectedItems.Count <= 0) return;
            var objUserViewRequest = GetUserViewRequest();
            if(objUserViewRequest == null) return;

            var objRequest = new XmlRequest()
            {
                IsUserView = true,
                ServiceProxy = Service,
                ObjPlugin = this,
                OldAttributeName = objUserViewRequest.AttributeSelected,
                ReplaceAttributeName = string.Empty,
                IsViewDependency = false,
                CheckedFromItems = null,
                CheckedItemsViews = null,
                ObjAttDataReplace = null,
                Objentity = objUserViewRequest.ObjEntity
            };
            if (listViewViewUsers.CheckedItems.Count <= 0) return;
            objRequest.CheckedItemsViews = listViewViewUsers.CheckedItems.Cast<ListViewItem>().ToList();
            XmlOperation.TaskCompletedCallBack callback = CallBackAfterOperation;
            XmlOperation.DeleteViewDependency(objRequest, callback);
        }

        private void TbReplaceSelectedUserDependency_Click(object sender, EventArgs e)
        {
            var objUserViewRequest = GetUserViewRequest();
            if (objUserViewRequest == null) return;
            var objRequest = new XmlRequest()
            {
                IsUserView = true,
                ObjPlugin = this,
                ServiceProxy = Service,
                OldAttributeName = objUserViewRequest.AttributeSelected,
                ReplaceAttributeName = objUserViewRequest.NewAttributeName,
                IsViewDependency = false,
                CheckedFromItems = null,
                CheckedItemsViews = null,
                ObjAttDataReplace = null,
                Objentity = objUserViewRequest.ObjEntity
            };
            if (listViewViewUsers.CheckedItems.Count <= 0) return;
            objRequest.CheckedItemsViews = listViewViewUsers.CheckedItems.Cast<ListViewItem>().ToList();
            XmlOperation.TaskCompletedCallBack callback = CallBackAfterOperation;
            XmlOperation.ReplaceViewDependency(objRequest, callback);
        }

        private void CallBackAfterOperation(XmlRequest request)
        {
            MessageBox.Show(@"User View update complete");
        }

       

        private void TxtSearchUsersList_TextChanged(object sender, EventArgs e)
        {
            var searchList = _usersListViewItemsColl.Where(l => l.SubItems[0].Text.ToUpper().Contains(TxtSearchUsersList.Text.ToUpper()));
            listViewSystemUsers.Items.Clear();
            listViewSystemUsers.Items.AddRange(searchList.ToArray());
        }

        private void listViewSystemUsers_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listViewSystemUsers.Sorting = ((listViewSystemUsers.Sorting == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending);
            listViewSystemUsers.ListViewItemSorter = new ListViewItemComparer(e.Column, listViewSystemUsers.Sorting);
        }
    }
}