using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace P137Pronia.Models
{
    public class AppUser:IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
