using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Web.Security;
using System.Web.Profile;
using WebApp.Models;

namespace WebApp.Util
{
    class LegacyUser
    {
        public string userName, name, country, emailAddress, affiliation, research; 
    }

    public class LegacyUserImporter
    {
        /// <summary>
        /// Imports legacy users from an XML file into the database
        /// </summary>
        /// <param name="fileStream">the file stream to the XML file. The XML file has to be exported
        /// from Microsoft Access with default settings.</param>
        /// <returns></returns>
        public static Boolean Import(Stream fileStream)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileStream);

            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes("/dataroot/Contacts_x0020_Inputlog");

            List<LegacyUser> users = new List<LegacyUser>();

            foreach (XmlNode node in nodes)
            {
                LegacyUser user = new LegacyUser();
                XmlNode subNode;
                user.name = node.SelectSingleNode("Name").InnerText;
                user.userName = user.name.Trim().Replace(" ", "");
                subNode = node.SelectSingleNode("Country");
                user.country = subNode!= null? subNode.InnerText : "NA";
                subNode = node.SelectSingleNode("EmailAddress");
                user.emailAddress = subNode !=  null ? subNode.InnerText : "NA";
                subNode = node.SelectSingleNode("Organization");
                user.affiliation = subNode != null? subNode.InnerText : "NA";
                subNode = node.SelectSingleNode("DateJoined");
                user.research += subNode != null? "Date joined: " + DateTime.Parse(subNode.InnerText).
                    ToShortDateString() + ". \n" : "";
                subNode = node.SelectSingleNode("Via");
                user.research += subNode != null? "Via: "+ subNode.InnerText + ". \n" : "";
                subNode = node.SelectSingleNode("Notes");
                user.research += subNode != null? subNode.InnerText : "";

                users.Add(user);
            }

            var service = new AccountMembershipService();

            if (users.Count == 0)
                return false;

            foreach(LegacyUser user in users){
                MembershipCreateStatus createStatus = service.CreateUser(user.userName, "tempPassw", user.emailAddress);

                if (createStatus == MembershipCreateStatus.Success) {
                 Roles.AddUserToRole(user.name, "Default");
                 var validationCode = Guid.NewGuid().ToString();
                 ProfileBase profile = ProfileBase.Create(user.userName);
                 profile.SetPropertyValue("ValidationCode", validationCode);
                 profile.SetPropertyValue("AdminValidation", true);
                 profile.SetPropertyValue("EmailNotifications", true);
                 profile.SetPropertyValue("Name", user.name);
                 profile.SetPropertyValue("Affiliation", user.affiliation);
                 profile.SetPropertyValue("Address", "NA");
                 profile.SetPropertyValue("Postal", "NA");
                 profile.SetPropertyValue("City", "NA");
                 profile.SetPropertyValue("Country", user.country);
                 profile.SetPropertyValue("Research", user.research);
                 profile.Save();
                }
            }
            
            return true;
        }
    }
}