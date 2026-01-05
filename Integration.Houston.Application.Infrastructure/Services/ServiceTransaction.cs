using AutoMapper;
using Integration.Houston.Application.Contract.Models;
using Integration.Houston.Application.Contract.Models.Dto;
using Integration.Houston.Application.Infrastructure.Models.Entity;
using Integration.Houston.Application.Infrastructure.Repositorie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Houston.Application.Infrastructure.Services
{
    public interface IserviceTransaction
    {
        public Task<CreditcardTransaction> AddTransaction(CreditCardCommand T);

        public Task<CreditcardTransaction> GetTransaction(Guid transactionId);

        public Task<CryptoTransaction> AddCryotoTransaction(CryptoCommand T);

        public Task<CryptoTransaction> GetCryptoTransaction(Guid transactionId);
    }
    public class ServiceTransaction : IserviceTransaction
    {
        private ITransactionRepositorie _transactionRepositorie;
        private IMapper _mapper;
        private ICryptoTransactionRepositories _cryptoTransactionRepositories;

        public ServiceTransaction(ITransactionRepositorie transactionRepositorie, IMapper mapper,ICryptoTransactionRepositories cryptoTransactionRepositories)
        {
            _transactionRepositorie = transactionRepositorie;
            _mapper = mapper;
            _cryptoTransactionRepositories = cryptoTransactionRepositories;
        }

        public async Task<CryptoTransaction> AddCryotoTransaction(CryptoCommand T)
        {
            var crypto = await _cryptoTransactionRepositories.AddTransaction(new TransactionsCrypto()
            { 
                 Id = Guid.NewGuid(),
                 Monto = T.Monto,
                 merchanttransactionid =T.ReferenceCode ,
                 CreatedAt = DateTime.UtcNow,
                 UpdatedAt = DateTime.UtcNow

            });
            return _mapper.Map<CryptoTransaction>(crypto);
        }

        public async Task<CreditcardTransaction> AddTransaction(CreditCardCommand T)
        {
            var addtra = await _transactionRepositorie.AddTransaction(new Transactions()
            {
               ErrorMessage = string.Empty,
               Id = Guid.NewGuid(),
               LastFour = T.transaction.creditCard.number.Substring(T.transaction.creditCard.number.Length - 4),
               MerchantId = string.Empty,
               Monto = T.transaction.order.additionalValues.TX_VALUE.value,
               Status ="Created",
               TransactionId = string.Empty ,
               merchanttransactionid = T.transaction.order.referenceCode 
            });

            return _mapper.Map<CreditcardTransaction>(addtra);
        }

        public async Task<CryptoTransaction> GetCryptoTransaction(Guid transactionId)
        {
            var trans = await _cryptoTransactionRepositories.GetTransaction(transactionId);
            return _mapper.Map<CryptoTransaction>(trans);
        }

        public async Task<CreditcardTransaction> GetTransaction(Guid transactionId)
        {
            var trans = await  _transactionRepositorie.GetTransaction(transactionId);
            return _mapper.Map<CreditcardTransaction>(trans); 
        }
    }
}
