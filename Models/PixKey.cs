using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcilliationConsumer.Models
{

    public enum PixKeyType
    {
        CPF,
        Email,
        Phone,
        Random
    }
    public class PixKeys : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public required string Value { get; set; }

        [Required]
        [EnumDataType(typeof(PixKeyType))]
        public required string Type { get; set; } // "CPF", "Email", "Phone", "Random"
        [Required]
        public int AccountId { get; set; }

        [Required]
        public int PaymentProviderId { get; set; }

        public PaymentProvider? PaymentProvider { get; set; }
        public Account? Account { get; set; }
        public ICollection<Payments>? Payments { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public new DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
