using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobApi.Model
{
    public partial class Company
    {
        public Company()
        {
            Jobs = new HashSet<Job>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; } = null!;
        [Column(TypeName = "nvarchar(450)")]
        public string? AdminId { get; set; }

        public virtual User? Admin { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
        [NotMapped] 
        public int JobCount { get; set; } = 0;
        [Column(TypeName = "varchar(1000)")]
        public string? Description { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string? Address { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string? PhoneNumber { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? MailAddress { get; set; }
    }
}
