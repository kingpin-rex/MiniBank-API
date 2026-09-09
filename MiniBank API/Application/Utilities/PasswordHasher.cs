using Microsoft.AspNetCore.Identity;
using MiniBank_API.Application.AccountDatabase;

namespace MiniBank_API.Application.Utilities
{
    public class PasswordHasherClass
    {   
        private readonly PasswordHasher<Customer> Hasher = new PasswordHasher<Customer>();
        public string HashPassword(Customer customer, string Password)
        {
            return Hasher.HashPassword(customer, Password);
        }

        public bool VerifyPassword(Customer customer, string Password, string HashedPassword)
        {
            var VerifyPassword = Hasher.VerifyHashedPassword(customer, HashedPassword, Password);

            if (VerifyPassword == PasswordVerificationResult.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
