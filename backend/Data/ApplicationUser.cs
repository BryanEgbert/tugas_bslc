using Microsoft.AspNetCore.Identity;

namespace backend.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public uint NIM { get; set; }
        [PersonalData]
        public string Password { get; set; }
        [PersonalData]
        public string Role { get; set; } = "user";
    }
}