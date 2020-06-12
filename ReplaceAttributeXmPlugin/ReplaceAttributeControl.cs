using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using ReplaceAttributeXmPlugin.Helper;
using Microsoft.Xrm.Sdk.Metadata;
using System.Xml;
using Microsoft.Crm.Sdk.Messages;

namespace ReplaceAttributeXmPlugin
{
    public partial class ReplaceAttributeControl : PluginControlBase
    {
        private Settings mySettings;
        private List<ListViewItem> _entitiesListViewItemsColl = null;
        private List<ListViewItem> _AttributeListViewItemsColl = null;
        private List<ListViewItem> _AttributeListViewItemsCollReplace = null;
        private List<ListViewItem> _FormsListViewItemsColl = null;
        private List<ListViewItem> _ViewListViewItemsColl = null;
        private List<ListViewGroup> _FormListViewGroup = null;
        private List<ListViewGroup> _ViewListViewGroup = null;

        private EntityMetadata _attributesMetadata;

        #region Default Methods
        public ReplaceAttributeControl()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
           
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }
        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }
        #endregion

        #region Load Metadat Information
        
        private void LoadEntities()
        {
            TxtSearchEnityList.Text = "";
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading entities...",
                Work = (bw, e) =>
                {
                    e.Result = CRMAction.GetAllEntities(Service);
                },
                PostWorkCallBack = e =>
                {
                    if (e.Error != null)
                    {
                        MessageBox.Show(ParentForm, e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        listViewEntities.Items.Clear();
                        _entitiesListViewItemsColl = new List<ListViewItem>();
                        foreach (EntityMetadata entity in (List<EntityMetadata>)e.Result)
                        {
                            LocalizedLabel localLabel = null;
                            if (entity.DisplayName.LocalizedLabels.Count > 0)
                            {
                                localLabel = entity.DisplayName.LocalizedLabels.Where(l => l.LanguageCode == 1033).First();
                            }

                            var displayName = (localLabel != null) ? localLabel.Label : entity.SchemaName;
                            var entityType = (entity.IsCustomEntity.Value) ? "Custom" : "System";
                            var lvItem = new ListViewItem()
                            {
                                Name = "Display Name",
                                ImageIndex = 0,
                                StateImageIndex = 0,
                                Text = displayName,
                                Tag = entity,  // stash the template here so we can view details later
                                Group = listViewEntities.Groups[entityType]
                            };
                            lvItem.Tag = entity;

                            var state = (entity.IsManaged.Value) ? "Managed" : "Unmanaged";
                            lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, entity.LogicalName) { Tag = "Name", Name = "Name" });
                            lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, state) { Tag = "State", Name = "State" });
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
            if (listViewEntities.SelectedItems.Count > 0)
            {
                TxtSearchAttributeList.Text = "";
                var Item = listViewEntities.SelectedItems[0];
                var entityName = Item.SubItems[1].Text;
                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Loading Attributes...",
                    Work = (bw, e) =>
                    {
                        e.Result = CRMAction.RetrieveEntityAttributeMeta(Service, entityName);
                        //bw.ReportProgress()
                    },
                    //ProgressChanged = (e) =>
                    //{
                    //    SetWorkingMessage("");
                    //},
                    PostWorkCallBack = e =>
                    {
                        if (e.Error != null)
                        {
                            MessageBox.Show(ParentForm, e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            _attributesMetadata = (EntityMetadata)e.Result;
                            listViewAttributes.Items.Clear();
                            _AttributeListViewItemsColl = new List<ListViewItem>();
                            foreach (AttributeMetadata attribute in _attributesMetadata.Attributes)
                            {
                                if (attribute.AttributeType == AttributeTypeCode.Virtual)
                                    continue;

                                LocalizedLabel localLabel = null;
                                if (attribute.DisplayName.LocalizedLabels.Count > 0)
                                {
                                    localLabel = attribute.DisplayName.LocalizedLabels.Where(l => l.LanguageCode == 1033).First();
                                }
                                var displayName = (localLabel != null) ? localLabel.Label : attribute.SchemaName;
                                var attributeType = (attribute.IsCustomizable.Value) ? "Custom" : "System";
                                _AttributeListViewItemsColl.Add(GetAttribuetListItem(attribute, displayName, attributeType));
                            }
                            SortList(listViewAttributes);
                            listViewAttributes.Items.AddRange(_AttributeListViewItemsColl.ToArray<ListViewItem>());
                            TxtSearchAttributeList.Focus();
                            
                        }
                    }
                });
            }
        }
        private void LoadFormDependency()
        {
            if (listViewAttributes.SelectedItems.Count > 0)
            {
                var EntityItem = (EntityMetadata)listViewEntities.SelectedItems[0].Tag;
                var ObjectTypeCode = EntityItem.ObjectTypeCode;
                var Item = listViewAttributes.SelectedItems[0];
                var subitem = Item.SubItems[1].Text;
                var attribute = _attributesMetadata.Attributes.Where(k => k.LogicalName == subitem).FirstOrDefault();
                if (attribute != null)
                {
                    WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Loading Forms Dependency...",
                        Work = (bw, e) =>
                        {
                            e.Result = CRMAction.GetAllSystemForms(Service, ObjectTypeCode, attribute.LogicalName);
                        },
                        PostWorkCallBack = e =>
                        {
                            if (e.Error != null)
                            {
                                MessageBox.Show(ParentForm, e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                var formEntity = (IEnumerable<Entity>)e.Result;
                                listViewForms.Items.Clear();
                                _FormsListViewItemsColl = new List<ListViewItem>();
                                _FormListViewGroup = new List<ListViewGroup>();
                                foreach (Entity objFormEntity in formEntity)
                                {
                                    string layoutXml = (string)objFormEntity["formxml"];
                                    if (FindXMLControl(layoutXml, attribute.LogicalName, "control", "id"))
                                    {
                                        var displayName = "";
                                        if (objFormEntity.Attributes.Contains("name"))
                                        {
                                            displayName = (string)objFormEntity["name"];
                                        }
                                        ListViewGroup group = null;
                                        if (objFormEntity.FormattedValues.Contains("type"))
                                        {
                                            string formType = (string)objFormEntity.FormattedValues["type"];
                                            if (_FormListViewGroup.Where(o => o.Name == formType).Count() == 0)
                                            {
                                                group = new ListViewGroup(formType)
                                                {
                                                    Name = formType,
                                                    Tag = "FormType"
                                                };
                                                _FormListViewGroup.Add(group);
                                            }
                                            else
                                            {
                                                group = _FormListViewGroup.Where(o => o.Name == formType).FirstOrDefault();
                                            }
                                        }
                                        var lvItem = new ListViewItem()
                                        {
                                            Name = "Name",
                                            ImageIndex = 0,
                                            StateImageIndex = 0,
                                            Text = displayName,
                                            Tag = objFormEntity,  // stash the template here so we can view details later
                                            Group = group
                                        };
                                        var state = ((bool)objFormEntity["ismanaged"]) ? "Managed" : "Unmanaged";
                                        lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, state) { Tag = "State", Name = "State" });
                                        _FormsListViewItemsColl.Add(lvItem);
                                    }
                                }
                                listViewForms.Groups.AddRange(_FormListViewGroup.ToArray<ListViewGroup>());
                                listViewForms.Items.AddRange(_FormsListViewItemsColl.ToArray<ListViewItem>());
                                ExecuteMethod(LoadViewDependency);
                            }
                        }
                    });
                }
            }
        }
        private void LoadViewDependency()
        {
            if (listViewAttributes.SelectedItems.Count > 0)
            {
                var EntityItem = (EntityMetadata)listViewEntities.SelectedItems[0].Tag;
                var ObjectTypeCode = EntityItem.ObjectTypeCode;
                var Item = listViewAttributes.SelectedItems[0];
                var subitem = Item.SubItems[1].Text;
                var attribute = _attributesMetadata.Attributes.Where(k => k.LogicalName == subitem).FirstOrDefault();
                if (attribute != null)
                {
                    WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Loading View Dependency...",
                        Work = (bw, e) =>
                        {
                            e.Result = CRMAction.GetAllSystemViews(Service, ObjectTypeCode, attribute.LogicalName);
                        },
                        PostWorkCallBack = e =>
                        {
                            if (e.Error != null)
                            {
                                MessageBox.Show(ParentForm, e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                var viewEntities = (IEnumerable<Entity>)e.Result;
                                listViewView.Items.Clear();
                                _ViewListViewItemsColl = new List<ListViewItem>();
                                _ViewListViewGroup = new List<ListViewGroup>();
                                string attributeName = attribute.LogicalName;

                                foreach (Entity objViewEntity in viewEntities)
                                {
                                    string layoutXml = (string)objViewEntity["layoutxml"];
                                    bool IsFound = FindXMLControl(layoutXml, attributeName, "cell", "name");
                                    if (!IsFound)
                                    {
                                        layoutXml = (string)objViewEntity["fetchxml"];
                                        IsFound = FindXMLControl(layoutXml, attributeName, "attribute", "name");
                                        if (!IsFound)
                                            IsFound = FindXMLControl(layoutXml, attributeName, "condition", "attribute");
                                    }
                                    if (IsFound)
                                    {
                                        var displayName = "";
                                        if (objViewEntity.Attributes.Contains("name"))
                                        {
                                            displayName = (string)objViewEntity["name"];
                                        }
                                        ListViewGroup group = null;
                                        if (objViewEntity.Attributes.Contains("layoutxml"))
                                        {
                                            string columnXml = (string)objViewEntity.Attributes["layoutxml"];
                                            if (columnXml.IndexOf(attributeName) >= 0)
                                            {
                                                if (_ViewListViewGroup.Where(o => o.Name == "LayoutXML").Count() == 0)
                                                {
                                                    group = new ListViewGroup("Layout XML")
                                                    {
                                                        Name = "LayoutXML",
                                                        Tag = "LayoutXML"
                                                    };
                                                    _ViewListViewGroup.Add(group);
                                                }
                                                else
                                                {
                                                    group = _ViewListViewGroup.Where(o => o.Name == "LayoutXML").FirstOrDefault();
                                                }
                                            }
                                            else
                                            {
                                                if (_ViewListViewGroup.Where(o => o.Name == "FetchXML").Count() == 0)
                                                {
                                                    group = new ListViewGroup("Fetch XML")
                                                    {
                                                        Name = "FetchXML",
                                                        Tag = "FetchXML"
                                                    };
                                                    _ViewListViewGroup.Add(group);
                                                }
                                                else
                                                {
                                                    group = _ViewListViewGroup.Where(o => o.Name == "FetchXML").FirstOrDefault();
                                                }
                                            }
                                        }
                                        var lvItem = new ListViewItem()
                                        {
                                            Name = "Name",
                                            ImageIndex = 0,
                                            StateImageIndex = 0,
                                            Text = displayName,
                                            Tag = objViewEntity,  // stash the template here so we can view details later
                                            Group = group
                                        };
                                        var state = ((bool)objViewEntity["ismanaged"]) ? "Managed" : "Unmanaged";
                                        lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, state) { Tag = "State", Name = "State" });
                                        _ViewListViewItemsColl.Add(lvItem);
                                    }
                                }

                                listViewView.Groups.AddRange(_ViewListViewGroup.ToArray<ListViewGroup>());
                                listViewView.Items.AddRange(_ViewListViewItemsColl.ToArray<ListViewItem>());

                            }
                        }
                    });
                }
            }
        }
        private bool FindXMLControl(string formXml, string attributeName, string elementTagName, string searchElementProperty)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(formXml);
            var allControls = doc.GetElementsByTagName(elementTagName);
            var control = allControls.Cast<XmlNode>().Where(p => p.Attributes[searchElementProperty].InnerText == attributeName);
            if (control != null && control.Count() > 0)
                return true;
            else
                return false;
        }
        private ListViewItem GetAttribuetListItem(AttributeMetadata attribute, string displayName, string attributeType)
        {
            var lvItem = new ListViewItem()
            {
                Name = "Display Name",
                ImageIndex = 0,
                StateImageIndex = 0,
                Text = displayName,
                Tag = attribute,  // stash the template here so we can view details later
                Group = listViewAttributes.Groups[attributeType]
            };
            lvItem.Tag = attribute;

            var state = (attribute.IsManaged.Value) ? "Managed" : "Unmanaged";
            lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, attribute.LogicalName) { Tag = "Name", Name = "Name" });
            lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, attribute.AttributeType.Value.ToString()) { Tag = "Type", Name = "Type" });
            lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, state) { Tag = "State", Name = "State" });
            return lvItem;
        }
       
        #region Default Sorting in ListView Load
        private void SortList(ListView objListView)
        {
            int.TryParse(objListView.Tag.ToString(), out int currCol);
            SortList(currCol, objListView);
        }
        private void SortList(int column, ListView objListView)
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
            ExecuteMethod(LoadEntities);
        }
        private void ListViewEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteMethod(LoadAttributes);
        }
        private void ListViewAttributes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewAttributes.SelectedItems.Count > 0)
            {
                tbLoadDependency.Enabled = true;
                TxtSearchReplaceAttributeList.Text = "";
                listViewAttributesReplaced.Items.Clear();
                var Item = listViewAttributes.SelectedItems[0];
                var attributeCurrent = (AttributeMetadata)Item.Tag;
                _AttributeListViewItemsCollReplace = new List<ListViewItem>();
                List<ListViewGroup> typeGroups = new List<ListViewGroup>();
                bool isGroupAdd = false;

                foreach (var attribute in _attributesMetadata.Attributes)
                {
                    LocalizedLabel localLabel = null;
                    if (attribute.DisplayName.LocalizedLabels.Count > 0)
                    {
                        localLabel = attribute.DisplayName.LocalizedLabels.Where(l => l.LanguageCode == 1033).First();
                    }

                    var displayName = (localLabel != null) ? localLabel.Label : attribute.SchemaName;
                    var attributeType = (attribute.IsCustomizable.Value) ? "Custom" : "System";
                    ListViewGroup group = new ListViewGroup(attribute.AttributeTypeName.Value)
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
                        Name = "Display Name",
                        ImageIndex = 0,
                        StateImageIndex = 0,
                        Text = displayName,
                        Tag = attribute,  // stash the template here so we can view details later
                        Group = typeGroups.Where(k => k.Name == attribute.AttributeTypeName.Value).FirstOrDefault()
                    };

                    lvItem.Tag = attribute;

                    var state = (attribute.IsManaged.Value) ? "Managed" : "Unmanaged";
                    lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, attribute.LogicalName) { Tag = "Name", Name = "Name" });
                    lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, state) { Tag = "State", Name = "State" });
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
                        _AttributeListViewItemsCollReplace.Add(lvItem);
                }
                SortList(listViewAttributesReplaced);

                foreach (var group in typeGroups)
                {
                    if (group.Name != attributeCurrent.AttributeTypeName.Value)
                    {
                        listViewAttributesReplaced.Groups.Add(group);
                    }
                }
                listViewAttributesReplaced.Items.AddRange(_AttributeListViewItemsCollReplace.ToArray<ListViewItem>());
            }
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
            var searchList = _AttributeListViewItemsColl.Where(l => l.SubItems[0].Text.ToUpper().Contains(TxtSearchAttributeList.Text.ToUpper()));
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
            var searchList = _AttributeListViewItemsCollReplace.Where(l => l.SubItems[0].Text.ToUpper().Contains(TxtSearchReplaceAttributeList.Text.ToUpper()));
            listViewAttributesReplaced.Items.Clear();
            listViewAttributesReplaced.Items.AddRange(searchList.ToArray());
        }
        #endregion

        #region Select All Options
        private void CmdCheckAllForms_Click(object sender, EventArgs e)
        {
            if (CmdCheckAllForms.Text == "Check All")
            {
                foreach (ListViewItem item in listViewForms.Items)
                {
                    item.Checked = true;
                }
                CmdCheckAllForms.Text = "Uncheck All";
            }
            else
            {
                foreach (ListViewItem item in listViewForms.Items)
                {
                    item.Checked = false;
                }
                CmdCheckAllForms.Text = "Check All";
            }
        }

        private void CmdCheckAllViews_Click(object sender, EventArgs e)
        {
            if (CmdCheckAllViews.Text == "Check All")
            {
                foreach (ListViewItem item in listViewView.Items)
                {
                    item.Checked = true;
                }
                CmdCheckAllViews.Text = "Uncheck All";
            }
            else
            {
                foreach (ListViewItem item in listViewView.Items)
                {
                    item.Checked = false;
                }
                CmdCheckAllViews.Text = "Check All";
            }
        }
        #endregion

        #region Find Control in Xml Document
        private XmlOperationResult PerformXMLOperation(string xmlString, string attributeName, string elementTagName, string searchElementProperty)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            var allControls = doc.GetElementsByTagName(elementTagName);
            var control = allControls.Cast<XmlNode>().Where(p => p.Attributes[searchElementProperty].InnerText == attributeName);
            if (control != null && control.Count() > 0)
            {
                control.FirstOrDefault().ParentNode.RemoveChild(control.FirstOrDefault());
                return (new XmlOperationResult(doc.InnerXml, true));
            }
            return (new XmlOperationResult("", false));
        }

        private XmlOperationResult PerformXMLOperation(string formXml, string attributeName, string elementTagName, string searchElementProperty, string searchNodeName)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(formXml);
            var allControls = doc.GetElementsByTagName(elementTagName);
            var control = allControls.Cast<XmlNode>().Where(p => p.Attributes[searchElementProperty].InnerText == attributeName);
            if (control != null && control.Count() > 0)
            {
                if (control.FirstOrDefault().ParentNode != null && control.FirstOrDefault().ParentNode.Name == searchNodeName)
                {
                    control.FirstOrDefault().ParentNode.RemoveChild(control.FirstOrDefault());
                    return (new XmlOperationResult(doc.InnerXml, true));
                }
                else
                {
                    XmlNode findNode = FindControlParentNode(control.FirstOrDefault(), searchNodeName);
                    if (findNode != null && findNode.Name == searchNodeName)
                    {
                        findNode.ParentNode.RemoveChild(findNode);
                        return (new XmlOperationResult(doc.InnerXml, true));
                    }
                }
            }
            return (new XmlOperationResult("", false));
        }
        private XmlNode FindControlParentNode(XmlNode ControlNode, string searchNodeName)
        {
            XmlNode findNode = null;
            if (ControlNode != null)
            {
                findNode = ControlNode.ParentNode;
                int parentCount = 0;
                do
                {
                    if (findNode == null)
                        break;
                    if (parentCount > 5)
                        break;

                    findNode = findNode.ParentNode;
                    parentCount++;
                } while (findNode.Name != searchNodeName);
            }
            return findNode;
        }

        #endregion

        #region Delete CRM Dependency from CRM SystemForm and SystemView
        private bool DeleteFormDependency(string attributeName)
        {
            bool IsPublish = false;
            foreach (ListViewItem Item in listViewForms.CheckedItems)
            {
                Entity objEnt = (Entity)Item.Tag;
                if (objEnt.Attributes.Contains("formxml"))
                {
                    string layoutXml = (string)objEnt["formxml"];
                    XmlOperationResult objResult = PerformXMLOperation(layoutXml, attributeName, "control", "id", "row");
                    objEnt["formxml"] = objResult.PublishXml;
                    if (objResult.IsPublish)
                    {
                        WorkAsync(new WorkAsyncInfo
                        {
                            Message = "Updaing Form Xml for " + Item.Text,

                            Work = (bw, e) =>
                            {
                                Service.Update(objEnt);
                            },
                            PostWorkCallBack = e =>
                            {
                                if (e.Error != null)
                                {
                                    MessageBox.Show(ParentForm, e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        });
                    }
                    IsPublish = objResult.IsPublish;
                }
            }
            return IsPublish;
        }
        private bool DeleteViewDependency(string attributeName)
        {
            bool isPublish = false;
            foreach (ListViewItem Item in listViewView.CheckedItems)
            {
                XmlResults objResult = new XmlResults(false);
                Entity objEnt = (Entity)Item.Tag;
                if (objEnt.Attributes.Contains("layoutxml"))
                {
                    string layoutXml = (string)objEnt["layoutxml"];
                    ////Remove Column from View
                    XmlOperationResult opResult = PerformXMLOperation(layoutXml, attributeName, "cell", "name");
                    objResult.IsPublish = opResult.IsPublish;
                    objResult.LayoutXml = opResult.PublishXml;
                }
                if (objEnt.Attributes.Contains("fetchxml"))
                {
                    //Remove Attribute from fetchxml
                    string layoutXml = (string)objEnt["fetchxml"];
                    XmlOperationResult opResult = PerformXMLOperation(layoutXml, attributeName, "attribute", "name");
                    if (!objResult.IsPublish)
                        objResult.IsPublish = opResult.IsPublish;
                    objResult.FetchXml = opResult.PublishXml;
                    //Remove Condition from fetchxml
                    opResult = PerformXMLOperation(layoutXml, attributeName, "condition", "attribute");
                    if (!objResult.IsPublish)
                        objResult.IsPublish = opResult.IsPublish;
                    objResult.FetchXml = opResult.PublishXml;
                }
                if (objResult.IsPublish)
                {
                    if (objResult.LayoutXml != string.Empty)
                        objEnt["layoutxml"] = objResult.LayoutXml;
                    if (objResult.FetchXml != string.Empty)
                        objEnt["fetchxml"] = objResult.FetchXml;
                    
                    WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Updaing View Xml for " + Item.Text,

                        Work = (bw, e) =>
                        {
                            Service.Update(objEnt);
                        },
                        PostWorkCallBack = e =>
                        {
                            if (e.Error != null)
                            {
                                MessageBox.Show(ParentForm, e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    });
                }
                isPublish = objResult.IsPublish;
            }
            return isPublish;
        }
        private void TbDeleteSelectedDependency_Click(object sender, EventArgs e)
        {
            if (listViewAttributes.SelectedItems.Count > 0)
            {
                var EntityItem = (EntityMetadata)listViewEntities.SelectedItems[0].Tag;
                if (listViewAttributes.SelectedItems.Count > 0)
                {
                    var lstItem = listViewAttributes.SelectedItems[0];
                    var subitem = lstItem.SubItems[1].Text;

                    bool IsPublish = DeleteFormDependency(subitem);

                    if (!IsPublish)
                        IsPublish = DeleteViewDependency(subitem);
                    else
                        DeleteViewDependency(subitem);

                    if (IsPublish)
                    {
                        string paramXml = string.Format(" <importexportxml><entities><entity>{0}</entity></entities><nodes/><securityroles/><settings/><workflows/></importexportxml>", EntityItem.LogicalName);
                        WorkAsync(new WorkAsyncInfo
                        {
                            Message = "Publishing Entity  " + EntityItem.LogicalName,
                            Work = (bw, ex) =>
                            {
                                Service.Execute(new PublishXmlRequest
                                {
                                    ParameterXml = paramXml
                                });
                            },
                            PostWorkCallBack = ex =>
                            {
                                if (ex.Error != null)
                                {
                                    MessageBox.Show(ParentForm, ex.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        });
                    }
                }
                else
                {
                    MessageBox.Show("Please Select Attribute First");
                }
            }
            else
            {
                MessageBox.Show("Please Select Entity First");
            }
        }
        #endregion

        #region Enable Depenency Operation Button
        private void EnableDependencyOperation()
        {
          
            if (listViewForms.Items.Cast<ListViewItem>().Where(k => k.Checked == true).Count() > 0)
                tbDeleteSelectedDependency.Enabled = true;
            else if (listViewView.Items.Cast<ListViewItem>().Where(k => k.Checked == true).Count() > 0)
                tbDeleteSelectedDependency.Enabled = true;
            else
            {
                tbReplaceSelectedDependency.Enabled = false;
                tbDeleteSelectedDependency.Enabled = false;
            }
           
            
            if (tbDeleteSelectedDependency.Enabled)
            {
                if (listViewAttributesReplaced.SelectedItems.Count > 0)
                    tbReplaceSelectedDependency.Enabled = true;
            }
        }
        private void ListViewForms_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            EnableDependencyOperation();
        }
        private void listViewView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            EnableDependencyOperation();
        }
        private void ListViewAttributesReplaced_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableDependencyOperation();
        }
        #endregion

        private void tbReplaceSelectedDependency_Click(object sender, EventArgs e)
        {

        }
    }
}