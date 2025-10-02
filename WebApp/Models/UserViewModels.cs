using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Security;
using System.Collections.Generic;
using System.Web.Profile;

namespace WebApp.Models
{

    public class UserIndexViewModel
    {
        public readonly IEnumerable<MembershipUser> Users;
        public readonly bool ShowDisabled;

        public UserIndexViewModel(IEnumerable<MembershipUser> users, bool showDisabled)
        {
            Users = users;
            ShowDisabled = showDisabled;
        }
    }

    public class UserDetailsViewModel
    {
        public string UserName { get; private set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email Address")]
        public string Email { get; set; }

        #region InputlogFields
        [Required]
        [DataType(DataType.Text)]
        [DisplayName("Full name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [DisplayName("Affiliation")]
        public string Affiliation { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [DisplayName("Address")]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [DisplayName("Postal Code")]
        public string Postal { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [DisplayName("City")]
        public string City { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [DisplayName("Country")]
        public string Country { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [DisplayName("Short description of your research")]
        public string Research { get; set; }
        #endregion

        [DisplayName("Email Notifications")]
        public bool EmailNotifications { get; set; }

        public List<TaskModel> Tasks { get; private set; }

        public UserDetailsViewModel()
        {
        }

        public UserDetailsViewModel(MembershipUser user, ProfileBase profile, List<TaskModel> tasks)
        {
            UserName = user.UserName;
            Email = user.Email;
            EmailNotifications = bool.Parse(profile.GetPropertyValue("EmailNotifications").ToString());
            Name = profile.GetPropertyValue("Name").ToString();
            Affiliation = profile.GetPropertyValue("Affiliation").ToString();
            Address = profile.GetPropertyValue("Address").ToString();
            Postal = profile.GetPropertyValue("Postal").ToString();
            City = profile.GetPropertyValue("City").ToString();
            Country = profile.GetPropertyValue("Country").ToString();
            Research = profile.GetPropertyValue("Research").ToString();
            Tasks = tasks;
        }

        public UserDetailsViewModel(MembershipUser user, ProfileBase profile)
        {
            UserName = user.UserName;
            Email = user.Email;
            EmailNotifications = bool.Parse(profile.GetPropertyValue("EmailNotifications").ToString());
            Name = profile.GetPropertyValue("Name").ToString();
            Affiliation = profile.GetPropertyValue("Affiliation").ToString();
            Address = profile.GetPropertyValue("Address").ToString();
            Postal = profile.GetPropertyValue("Postal").ToString();
            City = profile.GetPropertyValue("City").ToString();
            Country = profile.GetPropertyValue("Country").ToString();
            Research = profile.GetPropertyValue("Research").ToString();
            Tasks = new List<TaskModel>();
        }
    }
}