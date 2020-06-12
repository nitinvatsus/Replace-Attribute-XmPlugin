using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceAttributeXmPlugin.Helper
{
    public class XmlResults
    {
        public XmlResults(bool pubResult)
        {
            IsPublish = pubResult;
            LayoutXml = string.Empty;
            FetchXml = string.Empty;
            oldFetchXml = string.Empty;
        }
        public string LayoutXml { get; set; }
        public string FetchXml { get; set; }
        public string oldFetchXml { get; set; }
        public bool IsPublish { get; set; }
    }
}
