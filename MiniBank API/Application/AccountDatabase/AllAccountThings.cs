using Microsoft.Identity.Client;

namespace MiniBank_API.Application.AccountDatabase
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string HashedPassword { get; set; } = string.Empty;
        public Account? AccountDetails { get; set; }
    }

    public abstract class Account
    {
        public int Id { get; set; }
        public long AccountNumber { get; set; }
        public int CustomerId { get; set; }
        public decimal Balance { get; set; }
        public DateOnly CreatedOn { get; set; }
        public AccountType Type { get; set; }
      

        public virtual decimal CalculateInterest()
        {
            return 0;
        }

    }

    public class SavingsAccount : Account
    {
        public decimal InterestRate { get; set; }

        override public decimal CalculateInterest()
        {
            return Balance * InterestRate;
        }
    }

    public class CurrentAccount : Account
    {
        public decimal OverdraftLimit { get; set; }
        override public decimal CalculateInterest()
        {
            return 0;
        }
    }

    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }   
        public DateTime Timestamp { get; set; }
        public string Reference { get; set; } = String.Empty;
    }

    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        TransferIn,
        TransferOut
    }

    public enum AccountType
    {
        Savings,
        Current
    }

}
