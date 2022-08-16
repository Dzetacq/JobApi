using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobApi.Model
{
    public partial class EditInput
    {
        public string? Username { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? LinkedIn { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
