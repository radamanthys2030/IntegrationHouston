using Integration.Houston.Application.Infrastructure.EntityFramework;
using Integration.Houston.Application.Infrastructure.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Houston.Application.Infrastructure.Repositorie
{
    public interface ICryptoTransactionRepositories
    {
        public Task<TransactionsCrypto> AddTransaction(TransactionsCrypto T);

        public Task<TransactionsCrypto> GetTransaction(Guid transactionId);
    }
    public class CryptoTransactionRepositories : ICryptoTransactionRepositories
    {
        private TransactiondbContext _transactiondbContext;

        public CryptoTransactionRepositories(TransactiondbContext transactiondbContext)
        {
            _transactiondbContext = transactiondbContext;
        }
        public async  Task<TransactionsCrypto> AddTransaction(TransactionsCrypto T)
        {
            var tra = await _transactiondbContext.TransactionsCrypto.AddAsync(T);
            await _transactiondbContext.SaveChangesAsync();
            return T;
        }

        public async Task<TransactionsCrypto> GetTransaction(Guid transactionId)
        {
            var tra = await _transactiondbContext.TransactionsCrypto.Where(a => a.Id == transactionId).ToListAsync();

            return tra.FirstOrDefault();
        }
    }
}
