using DealerEcommerce.Domain.Addresses;
using DealerEcommerce.Domain.Dealers;
using DealerEcommerce.Domain.Integrations;
using DealerEcommerce.Domain.Orders;
using DealerEcommerce.Domain.Products;
using DealerEcommerce.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DealerEcommerce.Infrastructure.Persistence
{
    public class DealerEcommerceDbContext : DbContext
    {
        public DealerEcommerceDbContext(DbContextOptions<DealerEcommerceDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Dealer> Dealers => Set<Dealer>();
        public DbSet<DealerAddress> DealerAddresses => Set<DealerAddress>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderLine> OrderLines => Set<OrderLine>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        public DbSet<IntegrationLog> IntegrationLogs => Set<IntegrationLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUsers(modelBuilder);
            ConfigureDealers(modelBuilder);
            ConfigureDealerAddresses(modelBuilder);
            ConfigureProducts(modelBuilder);
            ConfigureOrders(modelBuilder);
            ConfigureOrderLines(modelBuilder);
            ConfigureOutboxMessages(modelBuilder);
            ConfigureIntegrationLogs(modelBuilder);
        }

        private static void ConfigureUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Username)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.FirstName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.SecondName)
                    .HasMaxLength(100);

                entity.Property(x => x.LastName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.SecondLastName)
                    .HasMaxLength(100);

                entity.Property(x => x.Email)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.PasswordHash)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(x => x.Role)
                .HasConversion(
                        value => ((int)value).ToString(CultureInfo.InvariantCulture),
                        value => (UserRole)int.Parse(value, CultureInfo.InvariantCulture))
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.DealerType)
                    //.HasColumnName("dealerType")
                    .HasConversion(
                        value => ((int)value).ToString(CultureInfo.InvariantCulture),
                        value => (UserDealerType)int.Parse(value, CultureInfo.InvariantCulture))
                    .HasMaxLength(50)
                    .IsRequired();

                entity.HasIndex(x => x.Username).IsUnique();
                entity.HasIndex(x => x.Email).IsUnique();
            });
        }

        private static void ConfigureDealers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dealer>(entity =>
            {
                entity.ToTable("Dealers");

                entity.HasKey(x => x.Id);

                //entity.Property(x => x.FirstName)
                //    .HasMaxLength(100)
                //    .IsRequired();

                //entity.Property(x => x.SecondName)
                //    .HasMaxLength(100);

                //entity.Property(x => x.LastName)
                //    .HasMaxLength(100)
                //    .IsRequired();

                //entity.Property(x => x.SecondLastName)
                //    .HasMaxLength(100);

                entity.Property(x => x.BusinessName)
                    .HasMaxLength(200);

                entity.Property(x => x.Email)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.DocumentNumber)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.HasIndex(x => x.DocumentNumber).IsUnique();

                entity.HasMany(x => x.Addresses)
                    .WithOne()
                    .HasForeignKey(x => x.DealerId);
            });
        }

        private static void ConfigureDealerAddresses(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DealerAddress>(entity =>
            {
                entity.ToTable("DealerAddresses");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Province)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.City)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.MainStreet)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.SecondaryStreet)
                    .HasMaxLength(200);

                entity.Property(x => x.Reference)
                    .HasMaxLength(300);
            });
        }

        private static void ConfigureProducts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Sku)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Name)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasMaxLength(1000);

                entity.Property(x => x.Price)
                    .HasPrecision(18, 2);

                entity.Property(x => x.StockQuantity)
                    .HasPrecision(18, 2);

                entity.HasIndex(x => x.Sku).IsUnique();
            });
        }

        private static void ConfigureOrders(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Subtotal)
                    .HasPrecision(18, 2);

                entity.Property(x => x.TaxTotal)
                    .HasPrecision(18, 2);

                entity.Property(x => x.Total)
                    .HasPrecision(18, 2);

                entity.HasMany(x => x.Lines)
                    .WithOne()
                    .HasForeignKey(x => x.OrderId);
            });
        }

        private static void ConfigureOrderLines(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderLine>(entity =>
            {
                entity.ToTable("OrderLines");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Sku)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.ProductName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Quantity)
                    .HasPrecision(18, 2);

                entity.Property(x => x.UnitPrice)
                    .HasPrecision(18, 2);

                entity.Property(x => x.LineTotal)
                    .HasPrecision(18, 2);
            });
        }

        private static void ConfigureOutboxMessages(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OutboxMessage>(entity =>
            {
                entity.ToTable("OutboxMessages");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.EventType)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.Payload)
                    .IsRequired();

                entity.Property(x => x.ErrorMessage)
                    .HasMaxLength(1000);
            });
        }

        private static void ConfigureIntegrationLogs(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IntegrationLog>(entity =>
            {
                entity.ToTable("IntegrationLogs");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.IntegrationName)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.Action)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.Request);

                entity.Property(x => x.Response);

                entity.Property(x => x.ErrorMessage)
                    .HasMaxLength(1000);
            });
        }
    }
}
