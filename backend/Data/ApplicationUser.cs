using Microsoft.AspNetCore.Identity;

namespace backend.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public int NIM { get; set; }
        [PersonalData]
        public string Password { get; set; }
        [PersonalData]
        public bool Active { get; set; } = true;
    }
}