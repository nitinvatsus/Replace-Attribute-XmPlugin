using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
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
    public static class XmlOperationss
    {
        public delegate void TaskCompletedCallBack(XmlRequest request);

        public static void DeleteFormDependency(XmlRequest request, TaskCompletedCallBack taskCompletedCallBack)
        {
            var checkedItemsProcess = ConvertListViweItems(request.CheckedFromItems);
            DeleteFormDependencyRecuricive(request, checkedItemsProcess, taskCompletedCallBack);
        }
       
        private static void DeleteFormDependencyRecuricive(XmlRequest Request, List<ClsViiewItemsChecked> checkedItems, TaskCompletedCallBack taskCompletedCallBack)
        {
            if (checkedItems.Where(c => c.checkItemProcessed == false).Any())
            {
                ClsViiewItemsChecked objListItem = checkedItems.Where(c => c.checkItemProcessed == false).FirstOrDefault();
                Entity objEnt = (Entity)objListItem.checkedItem.Tag;
                if (objEnt.Attributes.Contains("formxml"))
                {
                    string layoutXml = (string)objEnt["formxml"];
                    XmlOperationResult objResult = PerformXMLOperation(layoutXml, Request.OldAttributeName, "control", "id", "row");
                    objEnt["formxml"] = objResult.PublishXml;
                    Request.ObjPlugin.WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Deleting Dependency from  " + objListItem.checkedItem.Text + " for Attribute Name " + Request.OldAttributeName,
                        Work = (bw, e) =>
                        {
                            try
                            {
                                Request.ServiceProxy.Update(objEnt);
                            }
                            catch(Exception exc)
                            {
                                MessageBox.Show(Request.ObjPlugin.ParentForm, exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        },
                        PostWorkCallBack = e =>
                        {
                            if (e.Error != null)
                            {
                                MessageBox.Show(Request.ObjPlugin.ParentForm, e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                objListItem.checkItemProcessed = true;
                                DeleteFormDependencyRecuricive(Request, checkedItems, taskCompletedCallBack);
                                if (!checkedItems.Where(c => c.checkItemProcessed == false).Any())
                                {
                                    if (Request.IsViewDependency)
                                    {
                                        DeleteViewDependency(Request, taskCompletedCallBack);
                                    }
                                    else
                                    {
                                        PublishEntity(Request.ObjPlugin, Request.ServiceProxy, Request.Objentity);
                                        taskCompletedCallBack?.Invoke(Request);
                                    }
                                }
                            }
                        }
                    });
                }
            }
        }

        public static void DeleteViewDependency(XmlRequest Request, TaskCompletedCallBack taskCompletedCallBack)
        {
            List<ClsViiewItemsChecked> checkedItemsProcess = ConvertListViweItems(Request.CheckedItemsViews);
            DeleteViewDependencyRecuricive(Request, checkedItemsProcess, taskCompletedCallBack);
        }

        private static void DeleteViewDependencyRecuricive(XmlRequest Request, List<ClsViiewItemsChecked> checkedItems, TaskCompletedCallBack taskCompletedCallBack)
        {
            if (checkedItems.Where(c => c.checkItemProcessed == false).Any())
            {
                ClsViiewItemsChecked objListItem = checkedItems.Where(c => c.checkItemProcessed == false).FirstOrDefault();
                Entity objEnt = (Entity)objListItem.checkedItem.Tag;
                if (Request.IsUserView)
                {
                    ((CrmServiceClient)Request.ServiceProxy).CallerId = ((EntityReference)objEnt.Attributes["ownerid"]).Id;
                }
                XmlResults objResult = InternalDeleteViewDependency(Request.OldAttributeName, objListItem.checkedItem);
                objResult.oldFetchXml = (string)objEnt["fetchxml"];
                if (objResult.LayoutXml != string.Empty)
                    objEnt["layoutxml"] = objResult.LayoutXml;
                if (objResult.FetchXml != string.Empty)
                    objEnt["fetchxml"] = objResult.FetchXml;
                Request.ObjPlugin.WorkAsync(new WorkAsyncInfo
                {
                    Message = "Deleting Dependency View  " + objListItem.checkedItem.Text + " for Attribute Name " + Request.OldAttributeName,
                    Work = (bw, e) =>
                    {
                        try
                        {
                            Request.ServiceProxy.Update(objEnt);
                        }catch(Exception exc)
                        {
                            MessageBox.Show(Request.ObjPlugin.ParentForm, exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    },
                    PostWorkCallBack = e =>
                    {
                        if (e.Error != null)
                        {
                            MessageBox.Show(Request.ObjPlugin.ParentForm, e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            objListItem.checkItemProcessed = true;
                            DeleteViewDependencyRecuricive(Request, checkedItems, taskCompletedCallBack);
                            if (!checkedItems.Where(c => c.checkItemProcessed == false).Any())
                            {
                                if (!Request.IsUserView)
                                    PublishEntity(Request.ObjPlugin, Request.ServiceProxy, Request.Objentity);
                                taskCompletedCallBack?.Invoke(Request);
                            }
                        }
                    }
                });
            }
        }


        public static void ReplaceFormDependency(XmlRequest request, TaskCompletedCallBack taskCompletedCallBack)
        {
            List<ClsViiewItemsChecked> checkedItemsProcess = ConvertListViweItems(request.CheckedFromItems);
            ReplaceFormDependencyRecuricive(request, checkedItemsProcess, taskCompletedCallBack);
        }
        private static void ReplaceFormDependencyRecuricive(XmlRequest request, List<ClsViiewItemsChecked> checkedItems, TaskCompletedCallBack taskCompletedCallBack)
        {
            if (checkedItems.Where(c => c.checkItemProcessed == false).Any())
            {
                ClsViiewItemsChecked objListItem = checkedItems.Where(c => c.checkItemProcessed == false).FirstOrDefault();
                Entity objEnt = (Entity)objListItem.checkedItem.Tag;
                if (objEnt.Attributes.Contains("formxml"))
                {
                    string layoutXml = (string)objEnt["formxml"];
                    if (layoutXml.IndexOf(request.OldAttributeName) >= 0)
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(layoutXml);
                        var allControls = doc.GetElementsByTagName("control");
                        var control = allControls.Cast<XmlNode>().Where(p => p.Attributes["id"].InnerText == request.OldAttributeName).FirstOrDefault();
                        if (control != null)
                        {
                            XmlNode controlParentCell = control.ParentNode;
                            if (controlParentCell != null)
                            {
                                var labelNode = controlParentCell.SelectNodes(@"labels//label");
                                if (labelNode != null && labelNode.Count > 0)
                                {
                                    labelNode[0].Attributes["description"].InnerText = request.ObjAttDataReplace.DisplayName.LocalizedLabels.Where(l => l.LanguageCode == 1033).FirstOrDefault().Label;
                                }
                            }
                            var findSimilerAttribute = request.Objentity.Attributes.Where(K => K.AttributeType == request.ObjAttDataReplace.AttributeType);
                            XmlNode similerClassControl = null;
                            foreach (var attSimilierControl in findSimilerAttribute)
                            {
                                similerClassControl = allControls.Cast<XmlNode>().Where(p => p.Attributes["id"].InnerText == attSimilierControl.LogicalName).FirstOrDefault();
                                if (similerClassControl != null)
                                    break;
                            }
                            if (similerClassControl != null)
                            {
                                control.Attributes["id"].InnerText = request.ReplaceAttributeName;
                                control.Attributes["classid"].InnerText = similerClassControl.Attributes["classid"].InnerText;
                                control.Attributes["datafieldname"].InnerText = request.ReplaceAttributeName;
                                objEnt["formxml"] = doc.InnerXml;
                                request.ObjPlugin.WorkAsync(new WorkAsyncInfo
                                {
                                    Message = "Replacing Dependency Form  " + objListItem.checkedItem.Text + "\n Old Attribute Name " + request.OldAttributeName + " with New Attribute Name: " + request.ReplaceAttributeName,
                                    Work = (bw, e) =>
                                    {
                                        try
                                        {
                                            request.ServiceProxy.Update(objEnt);
                                        }
                                        catch(Exception exc)
                                        {
                                            MessageBox.Show(request.ObjPlugin.ParentForm, exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    },
                                    PostWorkCallBack = e =>
                                    {
                                        if (e.Error != null)
                                        {
                                            MessageBox.Show(request.ObjPlugin.ParentForm, e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                        else
                                        {
                                            objListItem.checkItemProcessed = true;
                                            ReplaceFormDependencyRecuricive(request, checkedItems, taskCompletedCallBack);
                                            if (!checkedItems.Where(c => c.checkItemProcessed == false).Any())
                                            {
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
                                    }
                                });
                            }
                            else
                            {
                                MessageBox.Show("Replace Attribute data type shold be same or same datatype control should be availbale on from : " + objListItem.checkedItem.Text);
                            }
                        }
                    }
                }
            }
        }

        public static void ReplaceViewDependency(XmlRequest request, TaskCompletedCallBack taskCompletedCallBack)
        {
            List<ClsViiewItemsChecked> checkedItemsProcess = ConvertListViweItems(request.CheckedItemsViews);
            ReplaceViewDependencyRecuricive(request, checkedItemsProcess, taskCompletedCallBack);
        }
        private static void ReplaceViewDependencyRecuricive(XmlRequest request, List<ClsViiewItemsChecked> checkedItems, TaskCompletedCallBack taskCompletedCallBack)
        {
            if (checkedItems.Where(c => c.checkItemProcessed == false).Any())
            {
                ClsViiewItemsChecked objListItem = checkedItems.Where(c => c.checkItemProcessed == false).FirstOrDefault();
                Entity objEnt = (Entity)objListItem.checkedItem.Tag;
                if (request.IsUserView)
                {
                    ((CrmServiceClient)request.ServiceProxy).CallerId = ((EntityReference)objEnt.Attributes["ownerid"]).Id;
                }
                XmlResults objResult = InternalReplaceViewDependency(request.OldAttributeName, request.ReplaceAttributeName, objListItem.checkedItem);
                if (objResult.IsPublish)
                {
                    if (objResult.LayoutXml != string.Empty)
                        objEnt["layoutxml"] = objResult.LayoutXml;
                    if (objResult.FetchXml != string.Empty)
                        objEnt["fetchxml"] = objResult.FetchXml;
                    request.ObjPlugin.WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Replacing Dependency View  " + objListItem.checkedItem.Text + "\n Old Attribute Name " + request.OldAttributeName + " with New Attribute Name: " + request.ReplaceAttributeName,
                        Work = (bw, e) =>
                        {
                            try
                            {
                                request.ServiceProxy.Update(objEnt);
                            }
                            catch(Exception exc)
                            {
                                MessageBox.Show(request.ObjPlugin.ParentForm, "Error in Fetch Xml Update \n " + exc.Message + "\n Try Old Fetch Xml and New Layout Xml Update !!! \n Replacing Dependency View  " + "\n Old Attribute Name " + request.OldAttributeName + " with New Attribute Name: " + request.ReplaceAttributeName
                                    , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                try
                                {
                                    objEnt["fetchxml"] = objResult.oldFetchXml;
                                    request.ServiceProxy.Update(objEnt);
                                }
                                catch {
                                    MessageBox.Show(request.ObjPlugin.ParentForm, "Error in Old Fetch Xml Update \n " + exc.Message + "\n Replacing Dependency View  " + "\n Old Attribute Name " + request.OldAttributeName + " with New Attribute Name: " + request.ReplaceAttributeName
                                       , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        },
                        PostWorkCallBack = e =>
                        {
                            if (e.Error != null)
                            {
                                MessageBox.Show(request.ObjPlugin.ParentForm, e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                objListItem.checkItemProcessed = true;
                                ReplaceViewDependencyRecuricive(request, checkedItems, taskCompletedCallBack);
                                if (checkedItems.All(c => c.checkItemProcessed))
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

        private static XmlResults InternalReplaceViewDependency(string oldAttributeName, string replaceAttributeName, ListViewItem Item)
        {
            XmlResults objResult = new XmlResults(false);
            Entity objEnt = (Entity)Item.Tag;
            if (objEnt.Attributes.Contains("layoutxml"))
            {
                string layoutXml = (string)objEnt["layoutxml"];
                if (layoutXml.IndexOf(oldAttributeName) >= 0)
                {
                    objResult.IsPublish = true;
                    objResult.LayoutXml = layoutXml.Replace(oldAttributeName, replaceAttributeName);
                }
            }
            if (objEnt.Attributes.Contains("fetchxml"))
            {
                string layoutXml = (string)objEnt["fetchxml"];
                if (layoutXml.IndexOf(oldAttributeName) >= 0)
                {
                    objResult.IsPublish = true;
                    objResult.oldFetchXml = layoutXml;
                    objResult.FetchXml = layoutXml.Replace(oldAttributeName, replaceAttributeName);
                }
            }
            return objResult;
        }

        private static XmlResults InternalDeleteViewDependency(string attributeName, ListViewItem Item)
        {
            XmlResults objResult = new XmlResults(false);
            Entity objEnt = (Entity)Item.Tag;
            if (objEnt.Attributes.Contains("layoutxml"))
            {
                string layoutXml = (string)objEnt["layoutxml"];

                ////Remove Column from View
                XmlOperationResult opResult = PerformXMLOperation(layoutXml, attributeName, "cell", "name");
                objResult.IsPublish = opResult.IsPublish;
                objResult.LayoutXml = opResult.PublishXml;
            }
            if (objEnt.Attributes.Contains("fetchxml"))
            {
                //Remove Attribute from fetchxml
                string layoutXml = (string)objEnt["fetchxml"];
                XmlOperationResult opResult = PerformXMLOperation(layoutXml, attributeName, "attribute", "name");
                if (!objResult.IsPublish)
                    objResult.IsPublish = opResult.IsPublish;
                objResult.FetchXml = opResult.PublishXml;

                //Remove Condition from fetchxml
                opResult = PerformXMLOperation(objResult.FetchXml, attributeName, "condition", "attribute");
                if (!objResult.IsPublish)
                    objResult.IsPublish = opResult.IsPublish;
                objResult.FetchXml = opResult.PublishXml;
            }
            return objResult;
        }
        private static XmlOperationResult PerformXMLOperation(string xmlString, string attributeName, string elementTagName, string searchElementProperty)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            var allControls = doc.GetElementsByTagName(elementTagName);
            var control = allControls.Cast<XmlNode>().Where(p => p.Attributes[searchElementProperty].InnerText == attributeName);
            if (control != null && control.Count() > 0)
            {
                control.FirstOrDefault().ParentNode.RemoveChild(control.FirstOrDefault());
                return (new XmlOperationResult(doc.InnerXml, true));
            }
            return (new XmlOperationResult(doc.InnerXml, false));
        }
        private static XmlOperationResult PerformXMLOperation(string formXml, string attributeName, string elementTagName, string searchElementProperty, string searchNodeName)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(formXml);
            var allControls = doc.GetElementsByTagName(elementTagName);
            var control = allControls.Cast<XmlNode>().Where(p => p.Attributes[searchElementProperty].InnerText == attributeName);
            if (control != null && control.Count() > 0)
            {
                if (control.FirstOrDefault().ParentNode != null && control.FirstOrDefault().ParentNode.Name == searchNodeName)
                {
                    control.FirstOrDefault().ParentNode.RemoveChild(control.FirstOrDefault());
                    return (new XmlOperationResult(doc.InnerXml, true));
                }
                else
                {
                    XmlNode findNode = FindControlParentNode(control.FirstOrDefault(), searchNodeName);
                    if (findNode != null && findNode.Name == searchNodeName)
                    {
                        findNode.ParentNode.RemoveChild(findNode);
                        return (new XmlOperationResult(doc.InnerXml, true));
                    }
                }
            }
            return (new XmlOperationResult("", false));
        }
        private static XmlNode FindControlParentNode(XmlNode ControlNode, string searchNodeName)
        {
            XmlNode findNode = null;
            if (ControlNode != null)
            {
                findNode = ControlNode.ParentNode;
                int parentCount = 0;
                do
                {
                    if (findNode == null)
                        break;
                    if (parentCount > 5)
                        break;

                    findNode = findNode.ParentNode;
                    parentCount++;
                } while (findNode.Name != searchNodeName);
            }
            return findNode;
        }

        private static void PublishEntity(PluginControlBase objPlugin, IOrganizationService _serviceProxy, EntityMetadata EntityItem)
        {
            string paramXml = string.Format(" <importexportxml><entities><entity>{0}</entity></entities><nodes/><securityroles/><settings/><workflows/></importexportxml>", EntityItem.LogicalName);
            objPlugin.WorkAsync(new WorkAsyncInfo
            {
                Message = "Publishing Entity  " + EntityItem.LogicalName,
                Work = (bw, ex) =>
                {
                    _serviceProxy.Execute(new PublishXmlRequest
                    {
                        ParameterXml = paramXml
                    });
                },
                PostWorkCallBack = ex =>
                {
                    if (ex.Error != null)
                    {
                        MessageBox.Show(objPlugin.ParentForm, ex.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            });
        }

        private static List<ClsViiewItemsChecked> ConvertListViweItems(List<ListViewItem> checkedItems)
        {
            List<ClsViiewItemsChecked> checkedItemsProcess = new List<ClsViiewItemsChecked>();
            foreach (var Item in checkedItems)
            {
                ClsViiewItemsChecked processItem = new ClsViiewItemsChecked
                {
                    checkedItem = Item,
                    checkItemProcessed = false
                };
                checkedItemsProcess.Add(processItem);
            }
            return checkedItemsProcess;
        }
    }
}
