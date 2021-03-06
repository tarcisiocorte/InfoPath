using Microsoft.Office.InfoPath;
using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace InfoPathRepeatingTable
{
    public partial class FormCode
    {
        // Member variables are not supported in browser-enabled forms.
        // Instead, write and read these values from the FormState
        // dictionary using code such as the following:
        //
        // private object _memberVariable
        // {
        //     get
        //     {
        //         return FormState["_memberVariable"];
        //     }
        //     set
        //     {
        //         FormState["_memberVariable"] = value;
        //     }
        // }

        // NOTE: The following procedure is required by Microsoft InfoPath.
        // It can be modified using Microsoft InfoPath.
        public void InternalStartup()
        {
            ((ButtonEvent)EventManager.ControlEvents["btnDeleteBody1"]).Clicked += new ClickedEventHandler(btnDeleteBody1_Clicked);
        }

        public void btnDeleteBody1_Clicked(object sender, ClickedEventArgs e)
        {
            DeleteDestinationRows();

            XPathNavigator root = MainDataSource.CreateNavigator();
            XPathNavigator sas_OrgCountyArea = MainDataSource.CreateNavigator().SelectSingleNode("/my:myFields/my:SecUF_UpdatedFields/my:sas_OrgCountyAreaNew", NamespaceManager);
            XPathNodeIterator OrgCountyAreaCoverage = sas_OrgCountyArea.Select("./my:OrgCountyAreaCoverageNew", NamespaceManager);

            //for (int i = OrgCountyAreaCoverage.Count; i > 0; i--)
            for (int i = 1; i <= OrgCountyAreaCoverage.Count; i++)
            {
                XPathNavigator OrgCountyAreaCoverageItem = sas_OrgCountyArea.CreateNavigator().SelectSingleNode("my:OrgCountyAreaCoverageNew[position() = " + i + "]", NamespaceManager);

                //Text Field
                XPathNavigator sas_CountiesCoverage = OrgCountyAreaCoverageItem.SelectSingleNode("my:sas_CountiesCoverageNew", NamespaceManager);
                string sas_CountiesCoverageValue = sas_CountiesCoverage.Value;
                List<string[]> valuesArray = new List<string[]>();

                //Repeating Table
                XPathNavigator AreaCoverageAddition = OrgCountyAreaCoverageItem.SelectSingleNode("my:AreaCoverageAdditionNew", NamespaceManager);
                XPathNodeIterator AreaCoverageAddTown = AreaCoverageAddition.Select("./my:AreaCoverageAddTownNew", NamespaceManager);
                for (int j = 1; j <= AreaCoverageAddTown.Count; j++)
                {
                    XPathNavigator AreaCoverageAddTownItem = AreaCoverageAddition.CreateNavigator().SelectSingleNode("my:AreaCoverageAddTownNew[position() = " + j + "]", NamespaceManager);

                    // Text Field
                    XPathNavigator sas_AreaCoverageTowns = AreaCoverageAddTownItem.SelectSingleNode("my:sas_AreaCoverageTownsNew", NamespaceManager);
                    XPathNavigator sas_AreaCoverageInfoNew = AreaCoverageAddTownItem.SelectSingleNode("my:sas_AreaCoverageInfoNew", NamespaceManager);

                    string[] values = { sas_AreaCoverageTowns.Value, sas_AreaCoverageInfoNew.Value };
                    valuesArray.Add(values);
                }

                SaveSection4(sas_CountiesCoverageValue, valuesArray);
            }
        }

        private void SaveSection4(string countiesCoverage, List<string[]> valuesArray)
        {
            try
            {
                XPathNavigator mainNav = this.MainDataSource.CreateNavigator();
                XPathNavigator section4 = mainNav.SelectSingleNode("/my:myFields/my:Sec04_PropAreaOfCaverageOrg/my:sas_OrgCountyArea", NamespaceManager);

                XmlDocument doc = new XmlDocument();
                XmlNode OrgCountyAreaCoverageElement = doc.CreateElement("OrgCountyAreaCoverage", NamespaceManager.LookupNamespace("my"));

                XmlNode fieldToAdd = doc.CreateElement("sas_CountiesCoverage", NamespaceManager.LookupNamespace("my"));
                XmlNode nodeToAdd = OrgCountyAreaCoverageElement.AppendChild(fieldToAdd);
                nodeToAdd.InnerText = countiesCoverage;

                fieldToAdd = doc.CreateElement("my:AreaCoverageAddition", NamespaceManager.LookupNamespace("my"));
                XmlNode areaCoverageAdditionElement = OrgCountyAreaCoverageElement.AppendChild(fieldToAdd);

                if (valuesArray.Count > 0)
                {
                    foreach (string[] valueArray in valuesArray)
                    {
                        AddAreaCoverage(valueArray[0], valueArray[1], doc, areaCoverageAdditionElement, OrgCountyAreaCoverageElement);
                    }
                }

                doc.AppendChild(OrgCountyAreaCoverageElement);
                section4.AppendChild(doc.DocumentElement.CreateNavigator());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddAreaCoverage(string field3, string field4, XmlDocument doc, XmlNode areaCoverageAdditionElement, XmlNode orgCountyAreaCoverageElement)
        {

            XmlNode group4Element = doc.CreateElement("AreaCoverageAddTown", NamespaceManager.LookupNamespace("my"));
            areaCoverageAdditionElement.AppendChild(group4Element);

            XmlNode fieldToAdd = doc.CreateElement("sas_AreaCoverageTowns", NamespaceManager.LookupNamespace("my"));
            XmlNode nodeToAdd = group4Element.AppendChild(fieldToAdd);
            nodeToAdd.InnerText = field3;

            fieldToAdd = doc.CreateElement("sas_AreaCoverageInfo", NamespaceManager.LookupNamespace("my"));
            nodeToAdd = group4Element.AppendChild(fieldToAdd);
            nodeToAdd.InnerText = field4;

            orgCountyAreaCoverageElement.AppendChild(areaCoverageAdditionElement);
        }

        private void DeleteDestinationRows()
        {
            XPathNavigator sas_OrgCountyArea = MainDataSource.CreateNavigator().SelectSingleNode("/my:myFields/my:Sec04_PropAreaOfCaverageOrg/my:sas_OrgCountyArea", NamespaceManager);
            XPathNodeIterator OrgCountyAreaCoverageItems = sas_OrgCountyArea.Select("./my:OrgCountyAreaCoverage", NamespaceManager);

            if (OrgCountyAreaCoverageItems.Count > 0)
            {
                for (int i = OrgCountyAreaCoverageItems.Count; i > 0; i--)
                {

                    XPathNavigator OrgCountyAreaCoverageItem = sas_OrgCountyArea.CreateNavigator().SelectSingleNode("my:OrgCountyAreaCoverage[position() = " + i + "]", NamespaceManager);

                    OrgCountyAreaCoverageItem.DeleteSelf();
                }
            }
        }
    }
}
