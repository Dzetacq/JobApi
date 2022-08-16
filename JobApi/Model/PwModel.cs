using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobApi.Model
{
    public partial class PwModel
    {
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string NewPassword { get; set; } = "";
    }
}
