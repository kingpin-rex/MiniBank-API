using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;

namespace MiniBank_API.Application.AccountDatabase
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) { }

        // Database Tables:
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<SavingsAccount> SavingsAccounts => Set<SavingsAccount>();
        public DbSet<CurrentAccount> CurrentAccounts => Set<CurrentAccount>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<Account> Accounts => Set<Account>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure financial decimal precision
            modelBuilder.Entity<Account>().Property(a => a.Balance).HasPrecision(18, 2);
            modelBuilder.Entity<SavingsAccount>().Property(s => s.InterestRate).HasPrecision(5, 4);
            modelBuilder.Entity<CurrentAccount>().Property(c => c.OverdraftLimit).HasPrecision(18, 2);
            modelBuilder.Entity<Transaction>().Property(t => t.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Transaction>().Property(t => t.BalanceAfter).HasPrecision(18, 2);

            // Store TransactionType as readable string in SQL
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Type)
                .HasConversion<string>();
        }
    }
}
