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
        [MaxLength(10)]
        public int NIM { get; set; }

        [Required(ErrorMessage = "Email missing")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password missing")]
        public string Password { get; set; }
    }
}