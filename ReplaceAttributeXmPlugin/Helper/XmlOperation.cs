using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using XrmToolBox.Extensibility;

namespace ReplaceAttributeXmPlugin.Helper
{
    public static class XmlOperation
    {
        public delegate void TaskCompletedCallBack(XmlRequest request);

        public static void DeleteFormDependency(XmlRequest request, TaskCompletedCallBack taskCompletedCallBack)
        {
            var checkedItemsProcess = ConvertListViweItems(request.CheckedFromItems);
            DeleteFormDependencyRecuricive(request, checkedItemsProcess, taskCompletedCallBack);
        }
       
        private static void DeleteFormDependencyRecuricive(XmlRequest request, List<ClsViiewItemsChecked> checkedItems, TaskCompletedCallBack taskCompletedCallBack)
        {
            if (checkedItems.All(c => c.CheckItemProcessed)) return;
            {
                var objListItem = checkedItems.FirstOrDefault(c => c.CheckItemProcessed == false);
                if (objListItem == null) return;
                {
                    var objEnt = (Entity)objListItem.CheckedItem.Tag;
                    if (!objEnt.Attributes.Contains("formxml")) return;
                    var layoutXml = (string)objEnt["formxml"];
                    var objResult = PerformXmlOperation(layoutXml, request.OldAttributeName, "control", "id", "row");
                    objEnt["formxml"] = objResult.PublishXml;
                    request.ObjPlugin.WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Deleting Dependency from  " + objListItem.CheckedItem.Text + " for Attribute Name " + request.OldAttributeName,
                        Work = (bw, e) =>
                        {
                            try
                            {
                                request.ServiceProxy.Update(objEnt);
                            }
                            catch(Exception exc)
                            {
                                MessageBox.Show(request.ObjPlugin.ParentForm, exc.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        },
                        PostWorkCallBack = e =>
                        {
                            if (e.Error != null)
                            {
                                MessageBox.Show(request.ObjPlugin.ParentForm, e.Error.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                objListItem.CheckItemProcessed = true;
                                DeleteFormDependencyRecuricive(request, checkedItems, taskCompletedCallBack);
                                if (checkedItems.Any(c => c.CheckItemProcessed == false)) return;
                                if (request.IsViewDependency)
                                {
                                    DeleteViewDependency(request, taskCompletedCallBack);
                                }
                                else
                                {
                                    PublishEntity(request.ObjPlugin, request.ServiceProxy, request.Objentity);
                                    taskCompletedCallBack?.Invoke(request);
                                }
                            }
                        }
                    });
                }
            }
        }

        public static void DeleteViewDependency(XmlRequest request, TaskCompletedCallBack taskCompletedCallBack)
        {
            var checkedItemsProcess = ConvertListViweItems(request.CheckedItemsViews);
            DeleteViewDependencyRecuricive(request, checkedItemsProcess, taskCompletedCallBack);
        }

        private static void DeleteViewDependencyRecuricive(XmlRequest request, List<ClsViiewItemsChecked> checkedItems, TaskCompletedCallBack taskCompletedCallBack)
        {
            if (checkedItems.Any(c => c.CheckItemProcessed == false))
            {
                var objListItem = checkedItems.FirstOrDefault(c => c.CheckItemProcessed == false);
                if (objListItem == null) return;
                {
                    var objEnt = (Entity)objListItem.CheckedItem.Tag;
                    if (request.IsUserView)
                    {
                        ((CrmServiceClient)request.ServiceProxy).CallerId = ((EntityReference)objEnt.Attributes["ownerid"]).Id;
                    }
                    var objResult = InternalDeleteViewDependency(request.OldAttributeName, objListItem.CheckedItem);
                    objResult.OldFetchXml = (string)objEnt["fetchxml"];
                    if (objResult.LayoutXml != string.Empty)
                        objEnt["layoutxml"] = objResult.LayoutXml;
                    if (objResult.FetchXml != string.Empty)
                        objEnt["fetchxml"] = objResult.FetchXml;
                    request.ObjPlugin.WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Deleting Dependency View  " + objListItem.CheckedItem.Text + " for Attribute Name " + request.OldAttributeName,
                        Work = (bw, e) =>
                        {
                            try
                            {
                                request.ServiceProxy.Update(objEnt);
                            }catch(Exception exc)
                            {
                                MessageBox.Show(request.ObjPlugin.ParentForm, exc.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        },
                        PostWorkCallBack = e =>
                        {
                            if (e.Error != null)
                            {
                                MessageBox.Show(request.ObjPlugin.ParentForm, e.Error.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                objListItem.CheckItemProcessed = true;
                                DeleteViewDependencyRecuricive(request, checkedItems, taskCompletedCallBack);
                                if (checkedItems.Any(c => c.CheckItemProcessed == false)) return;
                                if (!request.IsUserView)
                                    PublishEntity(request.ObjPlugin, request.ServiceProxy, request.Objentity);
                                taskCompletedCallBack?.Invoke(request);
                            }
                        }
                    });
                }
            }
        }


        public static void ReplaceFormDependency(XmlRequest request, TaskCompletedCallBack taskCompletedCallBack)
        {
            var checkedItemsProcess = ConvertListViweItems(request.CheckedFromItems);
            ReplaceFormDependencyRecuricive(request, checkedItemsProcess, taskCompletedCallBack);
        }
        private static void ReplaceFormDependencyRecuricive(XmlRequest request, IReadOnlyCollection<ClsViiewItemsChecked> checkedItems, TaskCompletedCallBack taskCompletedCallBack)
        {
            if (checkedItems.All(c => c.CheckItemProcessed)) return;
            {
                var objListItem = checkedItems.FirstOrDefault(c => c.CheckItemProcessed == false);
                if (objListItem == null) return;
                {
                    var objEnt = (Entity)objListItem.CheckedItem.Tag;
                    if (!objEnt.Attributes.Contains("formxml")) return;
                    var layoutXml = (string)objEnt["formxml"];
                    if (layoutXml.IndexOf(request.OldAttributeName, StringComparison.Ordinal) < 0) return;
                    var doc = new XmlDocument();
                    doc.LoadXml(layoutXml);
                    var allControls = doc.GetElementsByTagName("control");
                    var control = allControls.Cast<XmlNode>().FirstOrDefault(p => p.Attributes != null && p.Attributes["id"].InnerText == request.OldAttributeName);
                    if (control == null) return;
                    {
                        var controlParentCell = control.ParentNode;
                        var labelNode = controlParentCell?.SelectNodes(@"labels//label");
                        if (labelNode != null && labelNode.Count > 0)
                        {
                            var xmlAttributeCollection = labelNode[0].Attributes;
                            if (xmlAttributeCollection != null)
                                xmlAttributeCollection["description"].InnerText = request.ObjAttDataReplace.DisplayName
                                    .LocalizedLabels.FirstOrDefault(l => l.LanguageCode == 1033)
                                    ?.Label ?? string.Empty;
                        }
                        var findSimilerAttribute = request.Objentity.Attributes.Where(k => k.AttributeType == request.ObjAttDataReplace.AttributeType);
                        XmlNode similerClassControl = null;
                        foreach (var attSimilierControl in findSimilerAttribute)
                        {
                            similerClassControl = allControls.Cast<XmlNode>().FirstOrDefault(p => p.Attributes != null && p.Attributes["id"].InnerText == attSimilierControl.LogicalName);
                            if (similerClassControl != null)
                                break;
                        }
                        if (similerClassControl != null)
                        {
                            if (control.Attributes != null)
                            {
                                control.Attributes["id"].InnerText = request.ReplaceAttributeName;
                                if (similerClassControl.Attributes != null)
                                {
                                    control.Attributes["classid"].InnerText =
                                        similerClassControl.Attributes["classid"].InnerText;
                                    control.Attributes["datafieldname"].InnerText = request.ReplaceAttributeName;
                                }
                            }

                            objEnt["formxml"] = doc.InnerXml;
                            request.ObjPlugin.WorkAsync(new WorkAsyncInfo
                            {
                                Message = "Replacing Dependency Form  " + objListItem.CheckedItem.Text + "\n Old Attribute Name " + request.OldAttributeName + " with New Attribute Name: " + request.ReplaceAttributeName,
                                Work = (bw, e) =>
                                {
                                    try
                                    {
                                        request.ServiceProxy.Update(objEnt);
                                    }
                                    catch(Exception exc)
                                    {
                                        MessageBox.Show(request.ObjPlugin.ParentForm, exc.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                },
                                PostWorkCallBack = e =>
                                {
                                    if (e.Error != null)
                                    {
                                        MessageBox.Show(request.ObjPlugin.ParentForm, e.Error.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        objListItem.CheckItemProcessed = true;
                                        ReplaceFormDependencyRecuricive(request, checkedItems, taskCompletedCallBack);
                                        if (checkedItems.Any(c => c.CheckItemProcessed == false)) return;
                                        if (request.IsViewDependency)
                                        {
                                            ReplaceViewDependency(request, taskCompletedCallBack);
                                        }
                                        else
                                        {
                                            PublishEntity(request.ObjPlugin, request.ServiceProxy, request.Objentity);
                                            taskCompletedCallBack?.Invoke(request);
                                        }
                                    }
                                }
                            });
                        }
                        else
                        {
                            MessageBox.Show(@"Replace Attribute data type shold be same or same datatype control should be availbale on from : " + objListItem.CheckedItem.Text);
                        }
                    }
                }
            }
        }

        public static void ReplaceViewDependency(XmlRequest request, TaskCompletedCallBack taskCompletedCallBack)
        {
            var checkedItemsProcess = ConvertListViweItems(request.CheckedItemsViews);
            ReplaceViewDependencyRecuricive(request, checkedItemsProcess, taskCompletedCallBack);
        }
        private static void ReplaceViewDependencyRecuricive(XmlRequest request, List<ClsViiewItemsChecked> checkedItems, TaskCompletedCallBack taskCompletedCallBack)
        {
            if (checkedItems.All(c => c.CheckItemProcessed)) return;
            {
                var objListItem = checkedItems.FirstOrDefault(c => c.CheckItemProcessed == false);
                if (objListItem == null) return;
                {
                    var objEnt = (Entity)objListItem.CheckedItem.Tag;
                    if (request.IsUserView)
                    {
                        ((CrmServiceClient)request.ServiceProxy).CallerId = ((EntityReference)objEnt.Attributes["ownerid"]).Id;
                    }
                    var objResult = InternalReplaceViewDependency(request.OldAttributeName, request.ReplaceAttributeName, objListItem.CheckedItem);
                    if (!objResult.IsPublish) return;
                    if (objResult.LayoutXml != string.Empty)
                        objEnt["layoutxml"] = objResult.LayoutXml;
                    if (objResult.FetchXml != string.Empty)
                        objEnt["fetchxml"] = objResult.FetchXml;
                    request.ObjPlugin.WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Replacing Dependency View  " + objListItem.CheckedItem.Text + "\n Old Attribute Name " + request.OldAttributeName + " with New Attribute Name: " + request.ReplaceAttributeName,
                        Work = (bw, e) =>
                        {
                            try
                            {
                                request.ServiceProxy.Update(objEnt);
                            }
                            catch(Exception exc)
                            {
                                MessageBox.Show(request.ObjPlugin.ParentForm, @"Error in Fetch Xml Update 
                                                                                     " + exc.Message + @"
                                                                                     Try Old Fetch Xml and New Layout Xml Update !!! 
                                                                                     Replacing Dependency View  " + @"
                                                                                     Old Attribute Name " + request.OldAttributeName + @" with New Attribute Name: " + request.ReplaceAttributeName
                                                                                                                        , @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                try
                                {
                                    objEnt["fetchxml"] = objResult.OldFetchXml;
                                    request.ServiceProxy.Update(objEnt);
                                }
                                catch {
                                    MessageBox.Show(request.ObjPlugin.ParentForm, @"Error in Old Fetch Xml Update 
                                                                                         " + exc.Message + @"
                                                                                         Replacing Dependency View  " + @"
                                                                                         Old Attribute Name " + request.OldAttributeName + @" with New Attribute Name: " + request.ReplaceAttributeName
                                                                                                                                , @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        },
                        PostWorkCallBack = e =>
                        {
                            if (e.Error != null)
                            {
                                MessageBox.Show(request.ObjPlugin.ParentForm, e.Error.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                objListItem.CheckItemProcessed = true;
                                ReplaceViewDependencyRecuricive(request, checkedItems, taskCompletedCallBack);
                                if (checkedItems.All(c => c.CheckItemProcessed))
                                {
                                    if (!request.IsUserView)
                                    {
                                        PublishEntity(request.ObjPlugin, request.ServiceProxy, request.Objentity);
                                        taskCompletedCallBack?.Invoke(request);
                                    }
                                    else
                                        taskCompletedCallBack?.Invoke(request);
                                }
                            }
                        }
                    });
                }
            }
        }
        

        public static bool FindXmlControl(string formXml, string attributeName, string elementTagName, string searchElementProperty)
        {
            var doc = new XmlDocument();
            doc.LoadXml(formXml);
            var allControls = doc.GetElementsByTagName(elementTagName);
            var control = allControls.Cast<XmlNode>().Where(p => p.Attributes != null && p.Attributes[searchElementProperty].InnerText == attributeName);
            return control.Any();
        }

        private static XmlResults InternalReplaceViewDependency(string oldAttributeName, string replaceAttributeName, ListViewItem item)
        {
            var objResult = new XmlResults(false);
            var objEnt = (Entity)item.Tag;
            if (objEnt.Attributes.Contains("layoutxml"))
            {
                var layoutXml = (string)objEnt["layoutxml"];
                if (layoutXml.IndexOf(oldAttributeName, StringComparison.Ordinal) >= 0)
                {
                    objResult.IsPublish = true;
                    objResult.LayoutXml = layoutXml.Replace(oldAttributeName, replaceAttributeName);
                }
            }

            if (!objEnt.Attributes.Contains("fetchxml")) return objResult;
            {
                var layoutXml = (string)objEnt["fetchxml"];
                if (layoutXml.IndexOf(oldAttributeName, StringComparison.Ordinal) < 0) return objResult;
                objResult.IsPublish = true;
                objResult.OldFetchXml = layoutXml;
                objResult.FetchXml = layoutXml.Replace(oldAttributeName, replaceAttributeName);
            }
            return objResult;
        }

        private static XmlResults InternalDeleteViewDependency(string attributeName, ListViewItem item)
        {
            var objResult = new XmlResults(false);
            var objEnt = (Entity)item.Tag;
            if (objEnt.Attributes.Contains("layoutxml"))
            {
                var layoutXml = (string)objEnt["layoutxml"];

                ////Remove Column from View
                var opResult = PerformXmlOperation(layoutXml, attributeName, "cell", "name");
                objResult.IsPublish = opResult.IsPublish;
                objResult.LayoutXml = opResult.PublishXml;
            }

            if (!objEnt.Attributes.Contains("fetchxml")) return objResult;
            {
                //Remove Attribute from fetch xml
                var layoutXml = (string)objEnt["fetchxml"];
                var opResult = PerformXmlOperation(layoutXml, attributeName, "attribute", "name");
                if (!objResult.IsPublish)
                    objResult.IsPublish = opResult.IsPublish;
                objResult.FetchXml = opResult.PublishXml;

                //Remove Condition from fetch xml
                opResult = PerformXmlOperation(objResult.FetchXml, attributeName, "condition", "attribute");
                if (!objResult.IsPublish)
                    objResult.IsPublish = opResult.IsPublish;
                objResult.FetchXml = opResult.PublishXml;
            }
            return objResult;
        }
        private static XmlOperationResult PerformXmlOperation(string xmlString, string attributeName, string elementTagName, string searchElementProperty)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlString);
            var allControls = doc.GetElementsByTagName(elementTagName);
            var control = allControls.Cast<XmlNode>().Where(p => p.Attributes != null && p.Attributes[searchElementProperty].InnerText == attributeName);
            var xmlNodes = control as XmlNode[] ?? control.ToArray();
            if (!xmlNodes.Any()) return (new XmlOperationResult(doc.InnerXml, false));
            var parentNode = xmlNodes.FirstOrDefault()?.ParentNode;
            parentNode?.RemoveChild(xmlNodes.FirstOrDefault() ?? throw new InvalidOperationException());
            return (new XmlOperationResult(doc.InnerXml, true));
        }
        private static XmlOperationResult PerformXmlOperation(string formXml, string attributeName, string elementTagName, string searchElementProperty, string searchNodeName)
        {
            var doc = new XmlDocument();
            doc.LoadXml(formXml);
            var allControls = doc.GetElementsByTagName(elementTagName);
            var control = allControls.Cast<XmlNode>().Where(p => p.Attributes != null && p.Attributes[searchElementProperty].InnerText == attributeName);
            var xmlNodes = control as XmlNode[] ?? control.ToArray();
            if (!xmlNodes.Any()) return (new XmlOperationResult("", false));
            var parentNode = xmlNodes.FirstOrDefault()?.ParentNode;
            if (parentNode != null && parentNode.Name == searchNodeName)
            {
                var xmlNode = xmlNodes.FirstOrDefault()?.ParentNode;
                xmlNode?.RemoveChild(xmlNodes.FirstOrDefault() ?? throw new InvalidOperationException());
                return (new XmlOperationResult(doc.InnerXml, true));
            }
            else
            {
                var findNode = FindControlParentNode(xmlNodes.FirstOrDefault(), searchNodeName);
                if (findNode == null || findNode.Name != searchNodeName) return (new XmlOperationResult("", false));
                findNode.ParentNode?.RemoveChild(findNode);
                return (new XmlOperationResult(doc.InnerXml, true));
            }
        }
        private static XmlNode FindControlParentNode(XmlNode controlNode, string searchNodeName)
        {
            if (controlNode == null) return null;
            var findNode = controlNode.ParentNode;
            var parentCount = 0;
            do
            {
                if (findNode == null)
                    break;
                if (parentCount > 5)
                    break;

                findNode = findNode.ParentNode;
                parentCount++;
            } while (findNode != null && findNode.Name != searchNodeName);
            return findNode;
        }

        private static void PublishEntity(PluginControlBase objPlugin, IOrganizationService serviceProxy, EntityMetadata entityItem)
        {
            string paramXml =
                $" <importexportxml><entities><entity>{entityItem.LogicalName}</entity></entities><nodes/><securityroles/><settings/><workflows/></importexportxml>";
            objPlugin.WorkAsync(new WorkAsyncInfo
            {
                Message = "Publishing Entity  " + entityItem.LogicalName,
                Work = (bw, ex) =>
                {
                    serviceProxy.Execute(new PublishXmlRequest
                    {
                        ParameterXml = paramXml
                    });
                },
                PostWorkCallBack = ex =>
                {
                    if (ex.Error != null)
                    {
                        MessageBox.Show(objPlugin.ParentForm, ex.Error.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            });
        }

        private static List<ClsViiewItemsChecked> ConvertListViweItems(IEnumerable<ListViewItem> checkedItems)
        {
            return checkedItems.Select(item => new ClsViiewItemsChecked {CheckedItem = item, CheckItemProcessed = false}).ToList();
        }
    }
}
