using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Generic;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace ReplaceAttributeXmPlugin.Helper
{
    public class XmlRequest
    {
        public string OldAttributeName { get; set; }
        public string ReplaceAttributeName { get; set; }
        public List<ListViewItem> CheckedFromItems { get; set; }
        public IOrganizationService ServiceProxy { get; set; }
        public PluginControlBase ObjPlugin { get; set; }
        public EntityMetadata Objentity { get; set; }
        public bool IsViewDependency { get; set; }
        public List<ListViewItem> CheckedItemsViews { get; set; }
        public bool IsUserView { get; set; }

        public AttributeMetadata ObjAttDataReplace { get; set; }

    }
}
