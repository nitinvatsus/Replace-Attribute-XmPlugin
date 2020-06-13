using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using ReplaceAttributeXmPlugin.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace ReplaceAttributeXmPlugin
{
    public partial class FromUserView : Form
    {
        readonly OrganizationServiceProxy _serviceProxy;
        private readonly PluginControlBase ObjPlugin;
        private List<ListViewItem> _UsersListViewItemsColl = null;
        private List<ListViewItem> _CheckedUsersListViewItemsColl = null;
        private readonly int? objectTypeCode;
        private List<ListViewGroup> _ViewListViewGroup = null;
        private List<ListViewItem> _ViewListViewItemsColl = null;

        private readonly string _oldAttribuetName = string.Empty;
        private readonly string _NewAttribuetName = string.Empty;

        public FromUserView(string AttributeSelected, string newAttributeName, int? objectType, OrganizationServiceProxy serviceProxy, PluginControlBase Plugin)
        {
            InitializeComponent();
            _oldAttribuetName = AttributeSelected;
            _NewAttribuetName = newAttributeName;
            _serviceProxy = serviceProxy;
            objectTypeCode = objectType;
            ObjPlugin = Plugin;
        }

        private void toolStripMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void FromUserView_Load(object sender, EventArgs e)
        {
            var userEntities = CRMAction.GetAllSystemUsers(_serviceProxy);
            _UsersListViewItemsColl = new List<ListViewItem>();
            foreach (Entity objUserEntity in userEntities)
            {
                var displayName = "";
                if (objUserEntity.Attributes.Contains("fullname"))
                {
                    displayName = (string)objUserEntity["fullname"];
                }
                var lvItem = new ListViewItem()
                {
                    Name = "Name",
                    ImageIndex = 0,
                    StateImageIndex = 0,
                    Text = displayName,
                    Tag = objUserEntity
                };

                if (objUserEntity.Attributes.Contains("domainname"))
                {
                    lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, (string)objUserEntity["domainname"]) { Tag = "domainname", Name = "domainname" });
                }
                if (objUserEntity.Attributes.Contains("islicensed"))
                {
                    var state = ((bool)objUserEntity["islicensed"]) ? "Licensed" : "Unlicensed";
                    lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, state) { Tag = "islicensed", Name = "islicensed" });
                }
                _UsersListViewItemsColl.Add(lvItem);
            }
            listViewSystemUsers.Items.AddRange(_UsersListViewItemsColl.ToArray<ListViewItem>());
            SortList(listViewSystemUsers);
        }

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

        private void listViewSystemUsers_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listViewSystemUsers.Sorting = ((listViewSystemUsers.Sorting == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending);
            listViewSystemUsers.ListViewItemSorter = new ListViewItemComparer(e.Column, listViewSystemUsers.Sorting);
        }

        private void listViewView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listViewView.Sorting = ((listViewView.Sorting == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending);
            listViewView.ListViewItemSorter = new ListViewItemComparer(e.Column, listViewView.Sorting);
        }

        private void cmdCheckAllUsers_Click(object sender, EventArgs e)
        {
            if (cmdCheckAllUsers.Text == "Check All")
            {
                foreach (ListViewItem item in listViewSystemUsers.Items)
                {
                    item.Checked = true;
                }
                cmdCheckAllUsers.Text = "Uncheck All";
            }
            else
            {
                foreach (ListViewItem item in listViewSystemUsers.Items)
                {
                    item.Checked = false;
                }
                cmdCheckAllUsers.Text = "Check All";
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

        private void TxtSearchUsersList_TextChanged(object sender, EventArgs e)
        {
            var searchList = _ViewListViewItemsColl.Where(l => l.SubItems[0].Text.ToUpper().Contains(TxtSearchUsersList.Text.ToUpper()));
            listViewSystemUsers.Items.Clear();
            listViewSystemUsers.Items.AddRange(searchList.ToArray());
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            _CheckedUsersListViewItemsColl = new List<ListViewItem>();
            _CheckedUsersListViewItemsColl.AddRange(listViewSystemUsers.CheckedItems.Cast<ListViewItem>().ToArray());
            List<ClsViiewItemsChecked> checkedItemsProcess = ConvertListViweItems(listViewSystemUsers.CheckedItems.Cast<ListViewItem>().ToList());
            LoadUserView(checkedItemsProcess);
        }

        private void LoadUserView(List<ClsViiewItemsChecked> checkedItems)
        {
            if (checkedItems.Where(c => c.checkItemProcessed == false).Any())
            {
                ClsViiewItemsChecked objListItem = checkedItems.Where(c => c.checkItemProcessed == false).FirstOrDefault();
                Entity systemUser = (Entity)objListItem.checkedItem.Tag;
                _serviceProxy.CallerId = systemUser.Id;
                int itemProcssed = checkedItems.Where(c => c.checkItemProcessed).Count();
                int TotalItems = checkedItems.Where(c => c.checkItemProcessed).Count();
                int ItemsRemain = TotalItems - itemProcssed;

                ObjPlugin.WorkAsync(new WorkAsyncInfo
                {
                    Message = string.Format("Fetching User View  for {0};/n Total Users: {1}, Process:{2} Remaining:{3} ", objListItem.checkedItem.Text, TotalItems, itemProcssed, ItemsRemain),
                    Work = (bw, e) =>
                    {
                        e.Result = CRMAction.GetAllUserViews(_serviceProxy, objectTypeCode, _oldAttribuetName);
                    },
                    PostWorkCallBack = e =>
                    {
                        if (e.Error != null)
                        {
                            MessageBox.Show(ObjPlugin.ParentForm, e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            objListItem.checkItemProcessed = true;
                            LoadUserView(checkedItems);
                            if (!checkedItems.Where(c => c.checkItemProcessed == false).Any())
                            {
                                listViewView.Groups.AddRange(_ViewListViewGroup.ToArray<ListViewGroup>());
                                listViewView.Items.AddRange(_ViewListViewItemsColl.ToArray<ListViewItem>());
                                SortList(listViewView);
                            }
                        }
                    }
                });

            }
        }

        public void AddUserViews(IEnumerable<Entity> userViews, string userFullName, Entity systemUser)
        {
            foreach (var objViewEntity in userViews)
            {
                string layoutXml = (string)objViewEntity["layoutxml"];
                bool IsFound = XmlOperation.FindXMLControl(layoutXml, _oldAttribuetName, "cell", "name");
                if (!IsFound)
                {
                    layoutXml = (string)objViewEntity["fetchxml"];
                    IsFound = XmlOperation.FindXMLControl(layoutXml, _oldAttribuetName, "attribute", "name");
                    if (!IsFound)
                        IsFound = XmlOperation.FindXMLControl(layoutXml, _oldAttribuetName, "condition", "attribute");
                }
                if (IsFound)
                {
                    var displayName = "";
                    if (objViewEntity.Attributes.Contains("name"))
                    {
                        displayName = (string)objViewEntity["name"];
                    }
                    ListViewGroup group = null;
                    if (_ViewListViewGroup.Where(o => o.Name == userFullName).Count() == 0)
                    {
                        group = new ListViewGroup(userFullName)
                        {
                            Name = userFullName,
                            Tag = systemUser
                        };
                        _ViewListViewGroup.Add(group);
                    }
                    else
                    {
                        group = _ViewListViewGroup.Where(o => o.Name == userFullName).FirstOrDefault();
                    }

                    var lvItem = new ListViewItem()
                    {
                        Name = "ViewName",
                        ImageIndex = 0,
                        StateImageIndex = 0,
                        Text = displayName,
                        Tag = objViewEntity,  // stash the template here so we can view details later
                        Group = group
                    };
                    _ViewListViewItemsColl.Add(lvItem);
                }
            }
        }

        private static List<ClsViiewItemsChecked> ConvertListViweItems(List<ListViewItem> checkedItems)
        {
            List<ClsViiewItemsChecked> checkedItemsProcess = new List<ClsViiewItemsChecked>();
            foreach (var Item in checkedItems)
            {
                ClsViiewItemsChecked processItem = new ClsViiewItemsChecked
                {
                    checkedItem = Item,
                    checkItemProcessed = false
                };
                checkedItemsProcess.Add(processItem);
            }
            return checkedItemsProcess;
        }

        private void tbDeleteSelectedDependency_Click(object sender, EventArgs e)
        {
            XmlRequest objRequest = new XmlRequest()
            {
                IsUserView = true,
                ObjPlugin = ObjPlugin,
                ServiceProxy = _serviceProxy,
                OldAttributeName = _oldAttribuetName,
                ReplaceAttributeName = string.Empty,
                IsViewDependency = false,
                CheckedFromItems = null,
                CheckedItemsViews = null,
                ObjAttDataReplace = null
            };
            if (listViewView.CheckedItems.Count > 0)
            {
                objRequest.CheckedItemsViews = listViewView.CheckedItems.Cast<ListViewItem>().ToList();
                XmlOperation.DeleteViewDependency(objRequest);
            }
        }

        private void tbReplaceSelectedDependency_Click(object sender, EventArgs e)
        {
            XmlRequest objRequest = new XmlRequest()
            {
                IsUserView = true,
                ObjPlugin = ObjPlugin,
                ServiceProxy = _serviceProxy,
                OldAttributeName = _oldAttribuetName,
                ReplaceAttributeName = _NewAttribuetName,
                IsViewDependency = false,
                CheckedFromItems = null,
                CheckedItemsViews = null,
                ObjAttDataReplace = null
            };
            if (listViewView.CheckedItems.Count > 0)
            {
                objRequest.CheckedItemsViews = listViewView.CheckedItems.Cast<ListViewItem>().ToList();
                XmlOperation.ReplaceViewDependency(objRequest);
            }
        }
    }
}
