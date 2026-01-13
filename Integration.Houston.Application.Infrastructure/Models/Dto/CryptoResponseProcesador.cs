using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Houston.Application.Infrastructure.Models.Dto
{
    public class CryptoResponseProcesador
    {
        public string OrderId { get; internal set; }
        public string PayAddress { get; internal set; }
        public decimal PayAmount { get; internal set; }
        public decimal OrderAmount { get; internal set; }
        public decimal BufferAmount { get; internal set; }
        public string QrCodeUrl { get; internal set; }
        public DateTime ExpireAt { get; internal set; }
    }
}
