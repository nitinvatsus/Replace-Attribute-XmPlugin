using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using ReplaceAttributeXmPlugin.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace ReplaceAttributeXmPlugin
{
    public partial class FromUserView : Form
    {
        private readonly IOrganizationService _serviceProxy;
        private readonly PluginControlBase _objPlugin;
        private readonly EntityMetadata _objEntityMetadata;
        private List<ListViewItem> _usersListViewItemsColl;
        private readonly int? _objectTypeCode;
        private List<ListViewGroup> _viewListViewGroup;
        private List<ListViewItem> _viewListViewItemsColl;

        private readonly string _oldAttribuetName;
        private readonly string _newAttribuetName;

        public FromUserView(UserViewRequest request)
        {
            InitializeComponent();
            _oldAttribuetName = request.AttributeSelected;
            _newAttribuetName = request.NewAttributeName;
            Text = "Delete Attribute Name: " + request.OldAttributeDisplayName;
            if (!string.IsNullOrEmpty(_newAttribuetName))
            {
                Text = "Replace Attribute " + request.OldAttributeDisplayName + " with " + request.NewAttributeDisplayName;
            }
            _serviceProxy = request.ServiceProxy;
            if (request.ObjEntity != null)
            {
                _objectTypeCode = request.ObjEntity.ObjectTypeCode;
                _objEntityMetadata = request.ObjEntity;
            }
            _objPlugin = request.Plugin;
        }

        [Localizable(false)]
        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }


        private void FromUserView_Load(object sender, EventArgs e)
        {
            var userEntities = CrmAction.GetAllSystemUsers(_serviceProxy);
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
        }

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

        private void ListViewSystemUsers_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listViewSystemUsers.Sorting = ((listViewSystemUsers.Sorting == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending);
            listViewSystemUsers.ListViewItemSorter = new ListViewItemComparer(e.Column, listViewSystemUsers.Sorting);
        }

        private void ListViewView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listViewViewUsers.Sorting = ((listViewViewUsers.Sorting == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending);
            listViewViewUsers.ListViewItemSorter = new ListViewItemComparer(e.Column, listViewViewUsers.Sorting);
        }

        private void CmdCheckAllUsers_Click(object sender, EventArgs e)
        {
            if (cmdCheckAllUsers.Text == @"Check All")
            {
                foreach (ListViewItem item in listViewSystemUsers.Items)
                {
                    item.Checked = true;
                }
                tbLoadUserView.Enabled = true;
                cmdCheckAllUsers.Text = @"Uncheck All";
            }
            else
            {
                foreach (ListViewItem item in listViewSystemUsers.Items)
                {
                    item.Checked = false;
                }
                tbLoadUserView.Enabled = false;
                cmdCheckAllUsers.Text = @"Check All";
            }
        }

        private void CmdCheckAllViews_Click(object sender, EventArgs e)
        {
            if (CmdCheckAllViews.Text == @"Check All")
            {
                foreach (ListViewItem item in listViewViewUsers.Items)
                {
                    item.Checked = true;
                }
                CmdCheckAllViews.Text = @"Uncheck All";
            }
            else
            {
                foreach (ListViewItem item in listViewViewUsers.Items)
                {
                    item.Checked = false;
                }
                CmdCheckAllViews.Text = @"Check All";
            }
        }

        private void TxtSearchUsersList_TextChanged(object sender, EventArgs e)
        {
            var searchList = _usersListViewItemsColl.Where(l => l.SubItems[0].Text.ToUpper().Contains(TxtSearchUsersList.Text.ToUpper()));
            listViewSystemUsers.Items.Clear();
            listViewSystemUsers.Items.AddRange(searchList.ToArray());
        }

       

        private void LoadUserView(IReadOnlyCollection<ClsViiewItemsChecked> checkedItems)
        {
            if (checkedItems.All(c => c.CheckItemProcessed)) return;
            {
                var objListItem = checkedItems.FirstOrDefault(c => c.CheckItemProcessed == false);
                if (objListItem != null)
                {
                    var systemUser = (Entity)objListItem.CheckedItem.Tag;
                    ((CrmServiceClient)_serviceProxy).CallerId = systemUser.Id;
                    var itemProcssed = checkedItems.Count(c => c.CheckItemProcessed);
                    var totalItems = checkedItems.Count;
                    var itemsRemain = totalItems - itemProcssed;

                    _objPlugin.WorkAsync(new WorkAsyncInfo
                    {
                        Message =
                            $"Fetching User View  for {objListItem.CheckedItem.Text}\n Total Users: {totalItems}, Process:{itemProcssed} Remaining:{itemsRemain} ",
                        Work = (bw, e) =>
                        {
                            e.Result = CrmAction.GetAllUserViews(_serviceProxy, _objectTypeCode, _oldAttribuetName);
                        },
                        PostWorkCallBack = e =>
                        {
                            if (e.Error != null)
                            {
                                MessageBox.Show(_objPlugin.ParentForm, e.Error.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (e.Result != null)
                                    AddUserViews((IEnumerable<Entity>)e.Result, systemUser);
                                objListItem.CheckItemProcessed = true;
                                LoadUserView(checkedItems);
                                if (checkedItems.Any(c => c.CheckItemProcessed == false)) return;
                                listViewViewUsers.Groups.AddRange(_viewListViewGroup.ToArray<ListViewGroup>());
                                listViewViewUsers.Items.AddRange(_viewListViewItemsColl.ToArray<ListViewItem>());
                                SortList(listViewViewUsers);
                                Opacity = 1;
                            }
                        }
                    });
                }
            }
        }

        private void AddUserViews(IEnumerable<Entity> userViews,  Entity systemUser)
        {
            var userFullName = systemUser.Attributes.Contains("fullname") ? (string)systemUser["fullname"] : "";
            foreach (var objViewEntity in userViews)
            {
                var layoutXml = (string)objViewEntity["layoutxml"];
                var isFound = XmlOperation.FindXmlControl(layoutXml, _oldAttribuetName, "cell", "name");
                if (!isFound)
                {
                    layoutXml = (string)objViewEntity["fetchxml"];
                    isFound = XmlOperation.FindXmlControl(layoutXml, _oldAttribuetName, "attribute", "name");
                    if (!isFound)
                        isFound = XmlOperation.FindXmlControl(layoutXml, _oldAttribuetName, "condition", "attribute");
                }

                if (!isFound) continue;
                var displayName = "";
                if (objViewEntity.Attributes.Contains("name"))
                {
                    displayName = (string)objViewEntity["name"];
                }
                ListViewGroup group;
                if (_viewListViewGroup.All(o => o.Name != userFullName))
                {
                    group = new ListViewGroup(userFullName)
                    {
                        Name = userFullName,
                        Tag = systemUser
                    };
                    _viewListViewGroup.Add(group);
                }
                else
                {
                    group = _viewListViewGroup.FirstOrDefault(o => o.Name == userFullName);
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
                _viewListViewItemsColl.Add(lvItem);
            }
        }

        private static List<ClsViiewItemsChecked> ConvertListViweItems(IEnumerable<ListViewItem> checkedItems)
        {
            return checkedItems.Select(item => new ClsViiewItemsChecked {CheckedItem = item, CheckItemProcessed = false}).ToList();
        }

        private void TbDeleteSelectedDependency_Click(object sender, EventArgs e)
        {
            Opacity = 0;
            var objRequest = new XmlRequest()
            {
                IsUserView = true,
                ObjPlugin = _objPlugin,
                ServiceProxy = _serviceProxy,
                OldAttributeName = _oldAttribuetName,
                ReplaceAttributeName = string.Empty,
                IsViewDependency = false,
                CheckedFromItems = null,
                CheckedItemsViews = null,
                ObjAttDataReplace = null,
                Objentity = _objEntityMetadata,
            };
            if (listViewViewUsers.CheckedItems.Count <= 0) return;
            objRequest.CheckedItemsViews = listViewViewUsers.CheckedItems.Cast<ListViewItem>().ToList();
            XmlOperation.TaskCompletedCallBack callback = CallBackAfterDelete;
            XmlOperation.DeleteViewDependency(objRequest, callback);
        }

        private void CallBackAfterDelete(XmlRequest request)
        {
            LoadUsersViews();
        }
        private void CallBackAfterReplace(XmlRequest request)
        {
            LoadUsersViews();
        }
        private void TbReplaceSelectedDependency_Click(object sender, EventArgs e)
        {
            Opacity = 0;
            var objRequest = new XmlRequest()
            {
                IsUserView = true,
                ObjPlugin = _objPlugin,
                ServiceProxy = _serviceProxy,
                OldAttributeName = _oldAttribuetName,
                ReplaceAttributeName = _newAttribuetName,
                IsViewDependency = false,
                CheckedFromItems = null,
                CheckedItemsViews = null,
                ObjAttDataReplace = null,
                Objentity = _objEntityMetadata
            };
            if (listViewViewUsers.CheckedItems.Count <= 0) return;
            objRequest.CheckedItemsViews = listViewViewUsers.CheckedItems.Cast<ListViewItem>().ToList();
            XmlOperation.TaskCompletedCallBack callback = CallBackAfterReplace;
            XmlOperation.ReplaceViewDependency(objRequest, callback);
        }

        private void TbLoadUserView_Click(object sender, EventArgs e)
        {
            LoadUsersViews();
        }

        private void LoadUsersViews()
        {
            CmdCheckAllViews.Text = @"Check All";
            _viewListViewGroup = new List<ListViewGroup>();
            _viewListViewItemsColl = new List<ListViewItem>();
            listViewViewUsers.Items.Clear();

            StartPosition = FormStartPosition.CenterParent;
            Opacity = 0;

            var checkedItemsProcess = ConvertListViweItems(listViewSystemUsers.CheckedItems.Cast<ListViewItem>().ToList());
            LoadUserView(checkedItemsProcess);
        }

        private void TsbClose_Click(object sender, EventArgs e)
        {
           Close();
        }

        private void ListViewSystemUsers_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            tbLoadUserView.Enabled = listViewSystemUsers.CheckedItems.Count > 0;
        }

        private void ListViewSystemUsers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ListViewView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (listViewSystemUsers.CheckedItems.Count > 0)
            {
                tbDeleteSelectedDependency.Enabled = true;
                if (!string.IsNullOrEmpty(_newAttribuetName))
                    tbReplaceSelectedDependency.Enabled = true;
            }
            else
            {
                tbDeleteSelectedDependency.Enabled = false;
                tbReplaceSelectedDependency.Enabled = false;
            }
        }

        private void ToolStripMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
