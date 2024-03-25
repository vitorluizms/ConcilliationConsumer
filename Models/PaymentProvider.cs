using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcilliationConsumer.Models
{
    public class PaymentProvider : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        public required string Token { get; set; }
        [Required]
        public required string Webhook { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public new DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PixKeys>? PixKeys { get; set; }
        public ICollection<Account>? Accounts { get; set; }
    }
}
