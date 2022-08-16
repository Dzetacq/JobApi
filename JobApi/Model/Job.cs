using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobApi.Model
{
    public partial class Job
    {
        public Job()
        {
            Applications = new HashSet<Application>();
            Categories = new HashSet<Category>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; } = null!;
        [Column(TypeName = "nvarchar(3000)")]
        public string Description { get; set; } = null!;
        public DateTime? Deadline { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string? ContractType { get; set; } = null!;
        [Column(TypeName = "nvarchar(100)")]
        public string? Location { get; set; } = null!;
        [Column(TypeName = "nvarchar(100)")]
        public string? Salary { get; set; }
        public int CompanyId { get; set; }
        public int? SectorId { get; set; }

        public virtual Company? Company { get; set; } = null!;
        public virtual Sector? Sector { get; set; }
        public virtual ICollection<Application> Applications { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? Profile { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? Offer { get; set; }
    }
}
