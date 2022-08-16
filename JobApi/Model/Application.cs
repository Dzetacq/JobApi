using System.ComponentModel.DataAnnotations.Schema;

namespace JobApi.Model
{
    public partial class Application
    {
        public string UserId { get; set; }
        public int JobId { get; set; }
        [Column(TypeName = "nvarchar(3000)")]
        public string Description { get; set; } = null!;

        public virtual Job? Job { get; set; } = null!;
        public virtual User? User { get; set; } = null!;
    }
}
