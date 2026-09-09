using Microsoft.EntityFrameworkCore;
using MiniBank_API.Application.AccountDatabase;

namespace MiniBank_API.Application.Utilities
{
    public class AccountNumberGeneration
    {
        public static async Task<long?> GenerateAccount(AccountDbContext db)
        {
            bool exists;
            long accountNumber;
            do
            {
                accountNumber = Random.Shared.NextInt64(1_000_000_000L, 10_000_000_000L);

                exists = await db.Accounts.AnyAsync(a => a.AccountNumber == accountNumber);

            } while (exists);

                return accountNumber;

        }
    }
}
