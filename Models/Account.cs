using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ConcilliationConsumer.Models
{
    public class Account : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PaymentProviderId { get; set; }

        [Required]
        public int Number { get; set; }

        [Required]
        public int Agency { get; set; }

        public Users? User { get; set; }

        public PaymentProvider? PaymentProvider { get; set; }

        public ICollection<PixKeys>? PixKeys { get; }
        public ICollection<Payments>? Payments { get; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public new DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
