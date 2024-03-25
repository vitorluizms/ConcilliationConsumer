using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcilliationConsumer.Models
{
    public class Users : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(14)] // CPF length
        public required string CPF { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public new DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Account>? Accounts { get; set; }
        public ICollection<Payments>? Payments { get; set; }
    }
}