using Microsoft.AspNetCore.Identity;

namespace backend.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public uint NIM { get; set; }
    }
}