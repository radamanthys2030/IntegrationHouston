using Integration.Houston.Application.Contract.Models;
using Integration.Houston.Application.Contract.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Houston.Application.Contract
{
    public interface IApplicationContract
    {
        public Task<CreditcardTransaction> AddTransaction(CreditCardCommand T);

        public Task<CreditcardTransaction> GetTransaction(Guid transactionId);

        public Task<CryptoTransaction> AddTCryptoTransaction(CryptoCommand T);

        public Task<CryptoTransaction> GetCryptoTransaction(Guid transactionId);


    }
}
