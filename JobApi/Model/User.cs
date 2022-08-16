using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace JobApi.Model
{
    public class User : IdentityUser
    {
        [PersonalData]
        public string? Firstname { get; set; }
        [PersonalData]
        public string? Lastname { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool IsAdmin => Companies is { Count: > 0 };

        public bool IsSuper { get; set; }
        [PersonalData]
        public string? Address { get; set; }
        [PersonalData]
        public string? LinkedIn { get; set; }

        public virtual ICollection<Application>? Applications { get; set; }
        public virtual ICollection<Company>? Companies { get; set; }
    }
}
