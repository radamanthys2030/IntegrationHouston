using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Houston.Application.Infrastructure.Models.Entity
{
    public class TransactionsCrypto
    {

        public Guid? Id { get; set; }                   
        public decimal? Monto { get; set; }            
        public string? UsdtAddres { get; set; }         
        public string? TransactionId { get; set; }      
        public string? MerchantId { get; set; }         
        public string? Status { get; set; }            
        public string? ErrorMessage { get; set; }       
        public DateTimeOffset? CreatedAt { get; set; }  
        public DateTimeOffset? UpdatedAt { get; set; } 
        public string merchanttransactionid { get; set; }

    }
}
