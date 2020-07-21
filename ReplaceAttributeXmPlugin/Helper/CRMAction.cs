using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ReplaceAttributeXmPlugin.Helper
{
    public static class CrmAction
    {
        public static List<EntityMetadata> GetAllEntities(IOrganizationService service)
        {
            var req = new RetrieveAllEntitiesRequest()
            {
                EntityFilters = EntityFilters.Entity,
                RetrieveAsIfPublished = true
            };
            var resp = (RetrieveAllEntitiesResponse)service.Execute(req);
            var entities = resp.EntityMetadata.Where(x => x.IsCustomizable.Value).ToList();
            return entities;
        }
        public static IEnumerable<Entity> GetAllSystemForms(IOrganizationService service, int? objectTypeCode, string attributeName)
        {
            var fetch = $@"<fetch>
	                                <entity name='systemform' >
		                                <attribute name='name' />
		                                <attribute name='formxml' />
		                                <attribute name='objecttypecode' />
                                         <attribute name='formactivationstate' />
                                         <attribute name='type' />
                                         <attribute name='ismanaged' />
		                                <filter type='and' >
			                                <condition attribute='objecttypecode' operator='eq' value='{objectTypeCode}' />
			                                <condition attribute='formxml' operator='like' value='%{attributeName}%' />
		                                </filter>
	                                </entity>
                                </fetch>";
            var result = service.RetrieveMultiple(new FetchExpression(fetch));
            return result.Entities;
        }
        public static IEnumerable<Entity> GetAllSystemViews(IOrganizationService service, int? objectTypeCode, string attributeName)
        {
            var fetch = string.Format(@"<fetch>
	                                        <entity name='savedquery' >
		                                        <attribute name='savedqueryid' />
		                                        <attribute name='conditionalformatting' />
		                                        <attribute name='name' />
		                                        <attribute name='iscustomizable' />
		                                        <attribute name='fetchxml' />
		                                        <attribute name='ismanaged' />
		                                        <attribute name='returnedtypecode' />
		                                        <attribute name='layoutxml' />
		                                        <filter type='and' >
			                                        <condition attribute='returnedtypecode' operator='eq' value='{0}' />
			                                        <filter type='or' >
				                                        <condition attribute='layoutxml' operator='like' value='%{1}%' />
				                                        <condition attribute='fetchxml' operator='like' value='%{1}%' />
			                                        </filter>
		                                        </filter>
	                                        </entity>
                                        </fetch>", objectTypeCode, attributeName);
            var result = service.RetrieveMultiple(new FetchExpression(fetch));
            return result.Entities;
        }
        public static IEnumerable<Entity> GetAllSystemUsersViews(IOrganizationService service)
        {
            var fetch = @"<fetch>
	                                        <entity name='savedquery' >
		                                        <attribute name='savedqueryid' />
		                                        <attribute name='conditionalformatting' />
		                                        <attribute name='name' />
		                                        <attribute name='iscustomizable' />
		                                        <attribute name='fetchxml' />
		                                        <attribute name='ismanaged' />
		                                        <attribute name='returnedtypecode' />
		                                        <attribute name='layoutxml' />
		                                        <filter type='and' >
			                                        <condition attribute='returnedtypecode' operator='eq' value='8' />
		                                        </filter>
	                                        </entity>
                                        </fetch>";
            var result = service.RetrieveMultiple(new FetchExpression(fetch));
            return result.Entities;
        }
        public static EntityMetadata RetrieveEntityAttributeMeta(IOrganizationService service, string logicalName)
        {
            // Retrieve the attribute metadata
            var req = new RetrieveEntityRequest()
            {
                LogicalName = logicalName,
                EntityFilters = EntityFilters.Attributes,
                RetrieveAsIfPublished = true
            };
            var resp = (RetrieveEntityResponse)service.Execute(req);

            return resp.EntityMetadata;
        }
        public static IEnumerable<Entity> GetAllUserViews(IOrganizationService service, int? objectTypeCode, string attributeName)
        {
            try
            {
                var fetch = string.Format(@"<fetch>
	                                        <entity name='userquery' >
		                                        <attribute name='userqueryid' />
		                                        <attribute name='name' />
		                                        <attribute name='fetchxml' />
		                                        <attribute name='returnedtypecode' />
		                                        <attribute name='layoutxml' />
                                                <attribute name='createdby' />
                                                <attribute name='ownerid' />
		                                        <filter type='and' >
			                                        <condition attribute='returnedtypecode' operator='eq' value='{0}' />
			                                        <filter type='or' >
				                                        <condition attribute='layoutxml' operator='like' value='%{1}%' />
				                                        <condition attribute='fetchxml' operator='like' value='%{1}%' />
			                                        </filter>
		                                        </filter>
	                                        </entity>
                                        </fetch>", objectTypeCode, attributeName);
                var result = service.RetrieveMultiple(new FetchExpression(fetch));
                return result.Entities;
            }
            catch
            {
                return null;
            }
        }
        public static IEnumerable<Entity> GetAllSystemUsers(IOrganizationService service)
        {
            const string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                              <entity name='systemuser'>
                                                <attribute name='fullname' />
                                                <attribute name='businessunitid' />
                                                <attribute name='systemuserid' />
                                                <attribute name='domainname' />
                                                <attribute name='islicensed' />
                                                <order attribute='fullname' descending='false' />
                                                <filter type='and'>
                                                  <condition attribute='isdisabled' operator='eq' value='0' />
                                                  <condition attribute='accessmode' operator='not-in'>
                                                    <value>5</value>
                                                    <value>4</value>
                                                  </condition>
                                                </filter>
                                              </entity>
                                            </fetch>";
            var result = service.RetrieveMultiple(new FetchExpression(fetch));
            return result.Entities;
        }
        
        public static IEnumerable<Entity> GetAllSystemUsers(IOrganizationService service, string fetchxml)
        {
            var result = service.RetrieveMultiple(new FetchExpression(ModifyFetchXml(fetchxml)));
            return result.Entities;
        }
        private static string ModifyFetchXml(string requestXml)
        {
	        var doc = new XmlDocument();
	        doc.LoadXml(requestXml);
	        if (doc.DocumentElement == null) return doc.InnerXml;
	        var node = doc.DocumentElement.SelectSingleNode("entity");
	        AddElementInXml(doc, node, "fullname");
	        AddElementInXml(doc, node, "domainname");
	        AddElementInXml(doc, node, "islicensed");
	        return doc.InnerXml;
        }
        private static void  AddElementInXml(XmlDocument doc, XmlNode node, string attributeName)
        {
	        var elem = doc.CreateElement("attribute");
	        elem.SetAttribute("name", attributeName);
	        node?.AppendChild(elem);
        }
    }
}
