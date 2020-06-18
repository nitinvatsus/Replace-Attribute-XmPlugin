using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmToolBox.Extensibility;

namespace ReplaceAttributeXmPlugin.Helper
{
    public class UserViewRequest
    {
        public string AttributeSelected { get; set; }
        public string newAttributeName { get; set; }
        public string oldAttributeDisplayName { get; set; }
        public string newAttributeDisplayName { get; set; }
        public EntityMetadata objEntity { get; set; }
        public IOrganizationService serviceProxy { get; set; }
        public PluginControlBase Plugin { get; set; }
    }
}
