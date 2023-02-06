using System.ComponentModel.DataAnnotations;

namespace Personal_Collection_Manager.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        
        [EmailAddress]
        public string Email { get; set; }
        public string Role { get; set; }
        public bool Blocked { get; set; }
    }
}
