using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace backend.Models
{
    public class AdminModel
    {
        public string FileName { get; set; }
        public IFormFile File { get; set; }
    }
}