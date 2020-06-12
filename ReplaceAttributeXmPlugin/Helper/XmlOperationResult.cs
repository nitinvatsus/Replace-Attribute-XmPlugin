using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceAttributeXmPlugin.Helper
{
    public class XmlOperationResult
    {
        public XmlOperationResult(string docXml, bool publishState)
        {
            PublishXml = docXml;
            IsPublish = publishState;
        }
        public bool IsPublish { get; }
        public string PublishXml { get; }
    }
}
