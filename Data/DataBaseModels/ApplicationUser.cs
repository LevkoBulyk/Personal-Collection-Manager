using Microsoft.AspNetCore.Identity;

namespace Personal_Collection_Manager.Data.DataBaseModels
{
    public class ApplicationUser : IdentityUser
    {
        public bool Blocked { get; set; }
    }
}
