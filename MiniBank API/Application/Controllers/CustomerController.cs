using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniBank_API.Application.AccountDatabase;
using MiniBank_API.Application.DTOs;
using MiniBank_API.Application.Utilities;


namespace MiniBank_API.Application.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Customers : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerCreationDTO data, [FromServices] AccountDbContext db)
        {
            
            if (data.Type == AccountType.Savings && decimal.IsPositive(data.InitialDeposit))
            {
                long? accountNumber = await AccountNumberGeneration.GenerateAccount(db);

                SavingsAccount newAccount = new SavingsAccount();
                newAccount.AccountNumber = accountNumber.Value;
                newAccount.Balance = data.InitialDeposit;
                newAccount.CreatedOn = data.CreatedOn;
                newAccount.InterestRate = data.interestRate;
                newAccount.Type = data.Type;
                

                Customer newCustomer = new Customer();
                newCustomer.FirstName = data.FirstName;
                newCustomer.LastName = data.LastName;
                newCustomer.Email = data.EmailAddress;
                newCustomer.DateOfBirth = data.DateOfBirth;
                newCustomer.AccountDetails = newAccount;
                db.Customers.Add(newCustomer);
                await db.SaveChangesAsync();
                return Ok(new { accounts = await db.Accounts.ToListAsync() , Customers = await db.Customers.ToListAsync() });
            }
            else if (data.Type == AccountType.Current && decimal.IsPositive(data.InitialDeposit))
            {
                long? accountNumber = await AccountNumberGeneration.GenerateAccount(db);
                CurrentAccount newAccount = new CurrentAccount();
                newAccount.AccountNumber = accountNumber.Value;
                newAccount.Balance = data.InitialDeposit;
                newAccount.CreatedOn = data.CreatedOn;
                newAccount.OverdraftLimit = data.Overdraftlimit;
                newAccount.Type = data.Type;
                
                Customer newCustomer = new Customer();
                newCustomer.FirstName = data.FirstName;
                newCustomer.LastName = data.LastName;
                newCustomer.Email = data.EmailAddress;
                newCustomer.DateOfBirth = data.DateOfBirth;
                newCustomer.AccountDetails = newAccount;
                db.Customers.Add(newCustomer);
                await db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> DiscoverCustomer([FromServices] AccountDbContext db, int id)
        {
            var newCustomer = await db.Customers.Where(h => h.Id == id).FirstOrDefaultAsync();

            if (newCustomer != null)
            {
                return Ok(new { customer = newCustomer });
            }
            else
            {
                return NotFound("Customer not found");
            }

        }

        [HttpGet]
        public async Task<IActionResult> LoginCustomer()
    }
}
