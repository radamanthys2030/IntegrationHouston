using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Houston.Application.Contract.Models.Dto
{
    public class CreditcardTransaction
    {

     
        public Guid Id { get; set; }  // UUID en PostgreSQL → Guid en C#

       
        public decimal Monto { get; set; }

 
        public string LastFour { get; set; } = default!;

       
        public string? TransactionId { get; set; }

      
        public string? MerchantId { get; set; }

      
        public string? Status { get; set; }
   
        public string? ErrorMessage { get; set; }
    }
}
