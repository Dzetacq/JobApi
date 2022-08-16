using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobApi.Model
{
    public partial class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
