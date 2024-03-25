using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcilliationConsumer.Models
{

    public enum PaymentStatus
    {
        PROCESSING,
        SUCCESS,
        FAILED
    }
    public class Payments : BaseEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PixKeyId { get; set; }

        [Required]
        public int PaymentProviderAccountId { get; set; }

        [Required]
        public int Amount { get; set; }
        public string? Description { get; set; }

        [EnumDataType(typeof(PaymentStatus))]
        public string Status { get; set; } = "PROCESSING";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public new DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public PixKeys? PixKeys { get; set; }
        public Account? Account { get; set; }
    }
}