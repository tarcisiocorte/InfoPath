using Microsoft.Office.InfoPath;
using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.SharePoint;

namespace SP2016FormMessages
{
    public partial class FormCode
    {
        public void InternalStartup()
        {
            EventManager.FormEvents.Submit += new SubmitEventHandler(FormEvents_Submit);
            EventManager.FormEvents.Loading += new LoadingEventHandler(FormEvents_Loading);
        }

        public void FormEvents_Submit(object sender, SubmitEventArgs e)
        {

            FileSubmitConnection fc = (FileSubmitConnection)this.DataConnections["SharePoint Library Submit"];
            fc.Filename.SetStringValue("Message - " + DateTime.Now.ToString("yyyymmddhhmmss"));
            fc.Execute();

            e.CancelableArgs.Cancel = false;
            
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite elevatedSite = new SPSite("http://pobalfba"))
                {
                    using (SPWeb elevatedWeb = elevatedSite.OpenWeb())
                    {
                        elevatedWeb.AllowUnsafeUpdates = true;

                        SPList logsList = elevatedWeb.Lists.TryGetList("Logs");
                        SPListItem newItem = logsList.AddItem();
                        newItem["Title"] = string.Format("User: {0}; Date/Time: {1}", elevatedWeb.CurrentUser.LoginName, DateTime.Now.ToLongDateString());
                        newItem.Update();

                        elevatedWeb.AllowUnsafeUpdates = true;
                    }
                }
            });
        }

        public void FormEvents_Loading(object sender, LoadingEventArgs e)
        {
            
        }
    }
}
