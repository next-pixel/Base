using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace SoftinuxBase.Security.ViewModels.Permissions
{
    public class ReadRoleViewModel
    {
        public IdentityRole<string> Role { get; set; }
        public List<string> SelectedExtensions {get; set;}
        public List<string> AvailableExtensions {get; set;}

    }
}