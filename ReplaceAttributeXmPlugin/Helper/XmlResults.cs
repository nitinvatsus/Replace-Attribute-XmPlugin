namespace ReplaceAttributeXmPlugin.Helper
{
    public class XmlResults
    {
        public XmlResults(bool pubResult)
        {
            IsPublish = pubResult;
            LayoutXml = string.Empty;
            FetchXml = string.Empty;
            OldFetchXml = string.Empty;
        }
        public string LayoutXml { get; set; }
        public string FetchXml { get; set; }
        public string OldFetchXml { get; set; }
        public bool IsPublish { get; set; }
    }
}
