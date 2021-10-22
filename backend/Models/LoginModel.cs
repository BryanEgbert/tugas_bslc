using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "NIM missing")]
        public uint NIM { get; set; }
        public string Name { get; set; }

        [Required(ErrorMessage = "Email missing")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password missing")]
        public string Password { get; set; }

        public string Role { get; set; }
    }
}