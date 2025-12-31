using Integration.Houston.Application.Infrastructure.Models;
using Integration.Houston.Application.Infrastructure.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Houston.Application.Infrastructure.EntityFramework
{
    public class TransactiondbContext : DbContext
    {

        public DbSet<Transactions> Transactions { get; set; }

        public TransactiondbContext(DbContextOptions<TransactiondbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Host=tramway.proxy.rlwy.net;Port=53151;Database=tm_fibex_2;Username=dixon;Password=awhKnshJbs2Oplxn2axcv;Timeout=30";
            optionsBuilder.UseNpgsql(connectionString);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transactions>(builder =>
            {
                builder.HasKey(x => x.Id);


                builder.ToTable("transactions");

                builder.HasKey(t => t.Id);

                builder.Property(t => t.Id)
                       .HasColumnName("id")
                       .HasColumnType("uuid");

                builder.Property(t => t.Monto)
                       .HasColumnName("monto")
                       .HasPrecision(12, 2)
                       .IsRequired();

                builder.Property(t => t.LastFour)
                       .HasColumnName("last_four")
                       .HasMaxLength(4)
                       .IsRequired();

                builder.Property(t => t.TransactionId)
                       .HasColumnName("transaction_id")
                       .HasMaxLength(100);

                builder.Property(t => t.MerchantId)
                       .HasColumnName("merchant_id")
                       .HasMaxLength(100);

                builder.Property(t => t.Status)
                       .HasColumnName("status")
                       .HasMaxLength(20);

                builder.Property(t => t.ErrorMessage)
                       .HasColumnName("error_message");




            });

           
        }
    }
}
