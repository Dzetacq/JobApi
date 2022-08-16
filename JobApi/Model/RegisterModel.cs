using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobApi.Model
{
    public partial class RegisterInput
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? LinkedIn { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
