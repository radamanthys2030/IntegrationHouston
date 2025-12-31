using Integration.Houston.Application.Contract.Models;
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

    public interface ITransactionRepositorie
    {
        public Task<Transactions> AddTransaction(Transactions T);

        public Task<Transactions> GetTransaction(Guid transactionId);
    }
    public class TransactionRepositorie : ITransactionRepositorie
    {
        private TransactiondbContext _transactiondbContext;

        public TransactionRepositorie(TransactiondbContext transactiondbContext)
        {
            _transactiondbContext = transactiondbContext;
        }
        public async Task<Transactions> AddTransaction(Transactions T)
        {
           var tra = await _transactiondbContext.Transactions.AddAsync(T);
            await _transactiondbContext.SaveChangesAsync();
            return T;
        }

        public async Task<Transactions> GetTransaction(Guid transactionId)
        {
            var tra = await _transactiondbContext.Transactions.Where(a => a.Id == transactionId).ToListAsync();

            return tra.FirstOrDefault();

        }
    }
}
