using Integration.Houston.Application.Contract;
using Integration.Houston.Application.Contract.Models;
using Integration.Houston.Application.Contract.Models.Dto;
using Integration.Houston.Application.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Houston.Application
{
    public class TransactionApplication : IApplicationContract
    {
        private IserviceTransaction _iserviceTransaction;

        public TransactionApplication(IserviceTransaction iserviceTransaction) {

            _iserviceTransaction = iserviceTransaction;


        }
        public async Task<CreditcardTransaction> AddTransaction(CreditCardCommand T)
        {
            return await _iserviceTransaction.AddTransaction(T); 


         }

        public async Task<CreditcardTransaction> GetTransaction(Guid transactionId)
        {
            return await _iserviceTransaction.GetTransaction(transactionId);
        }
    }
}
