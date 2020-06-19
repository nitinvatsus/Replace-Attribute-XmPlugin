using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;

namespace ReplaceAttributeXmPlugin.Helper
{
    public class UserViewRequest
    {
        public string AttributeSelected { get; set; }
        public string NewAttributeName { get; set; }
        public string OldAttributeDisplayName { get; set; }
        public string NewAttributeDisplayName { get; set; }
        public EntityMetadata ObjEntity { get; set; }
        public IOrganizationService ServiceProxy { get; set; }
        public PluginControlBase Plugin { get; set; }
    }
}
