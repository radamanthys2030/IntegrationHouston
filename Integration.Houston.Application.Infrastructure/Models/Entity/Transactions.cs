using Microsoft.EntityFrameworkCore;
using System;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Integration.Houston.Application.Infrastructure.Models.Entity
{
    public class Transactions
    {

        [Key]
        [Column("id")]
        public Guid Id { get; set; }  // UUID en PostgreSQL → Guid en C#

        [Column("monto")]
        [Precision(12, 2)]
        [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser mayor o igual a 0.")]
        public decimal Monto { get; set; }

        [Column("last_four")]
        [MaxLength(4)]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "last_four debe tener 4 dígitos.")]
        public string LastFour { get; set; } = default!;

        [Column("transaction_id")]
        [MaxLength(100)]
        public string? TransactionId { get; set; }

        [Column("merchant_id")]
        [MaxLength(100)]
        public string? MerchantId { get; set; }

        [Column("status")]
        [MaxLength(20)]
        public string? Status { get; set; }

        [Column("error_message")]
        public string? ErrorMessage { get; set; }

        public string merchanttransactionid { get; set; }

    }
}
