﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;

namespace ReplaceAttributeXmPlugin.Helper
{
    public class CRMAction
    {
        public static List<EntityMetadata> GetAllEntities(IOrganizationService service)
        {
            var req = new RetrieveAllEntitiesRequest()
            {
                EntityFilters = EntityFilters.Entity,
                RetrieveAsIfPublished = true
            };
            var resp = (RetrieveAllEntitiesResponse)service.Execute(req);
            var entities = resp.EntityMetadata.Where(x => x.IsCustomizable.Value == true).ToList();
            return entities;
        }

        public static IEnumerable<Entity> GetAllSystemForms(IOrganizationService service, int? objectTypeCode, string attributeName)
        {
            string fetch = string.Format(@"<fetch>
	                                <entity name='systemform' >
		                                <attribute name='name' />
		                                <attribute name='formxml' />
		                                <attribute name='objecttypecode' />
                                         <attribute name='formactivationstate' />
                                         <attribute name='type' />
                                         <attribute name='ismanaged' />
		                                <filter type='and' >
			                                <condition attribute='objecttypecode' operator='eq' value='{0}' />
			                                <condition attribute='formxml' operator='like' value='%{1}%' />
		                                </filter>
	                                </entity>
                                </fetch>", objectTypeCode, attributeName);
            EntityCollection result = service.RetrieveMultiple(new FetchExpression(fetch));
            return result.Entities;
        }
        public static IEnumerable<Entity> GetAllSystemViews(IOrganizationService service, int? objectTypeCode, string attributeName)
        {
            string fetch = string.Format(@"<fetch>
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
            EntityCollection result = service.RetrieveMultiple(new FetchExpression(fetch));
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
    }
}