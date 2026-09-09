using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniBank_API.Application.AccountDatabase;
using MiniBank_API.Application.DTOs;
using MiniBank_API.Application.Utilities;
using System.Security.Principal;


namespace MiniBank_API.Application.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Accounts : ControllerBase
    {
        [HttpGet("{id:int}")]
        public async Task<IActionResult> DiscoverAccount([FromServices] AccountDbContext db, int id)
        {
            var newAccount = await db.Accounts.Where(h => h.Id == id).FirstOrDefaultAsync();

            if (newAccount != null)
            {
                return Ok(new { Account = newAccount });
            }
            else
            {
                return NotFound("Account not found");
            }
            
        }

        [HttpPut("{id:int}/deposit")]
        public async Task<IActionResult> DepositAccount([FromServices] AccountDbContext db,[FromBody] AccountDepositDTO data, int id)
        {
            var newAccount = await db.Accounts.Where(h => h.Id == id).FirstOrDefaultAsync();
            if (newAccount != null && decimal.IsPositive(data.Amount))
            {
                newAccount.Balance += data.Amount;

                Transaction newTransaction = new Transaction();
                newTransaction.AccountId = id;
                newTransaction.Type = TransactionType.Deposit;
                newTransaction.Amount = data.Amount;
                newTransaction.BalanceAfter = newAccount.Balance;
                newTransaction.Timestamp = DateTime.UtcNow;
                newTransaction.Reference = Convert.ToString(newAccount.AccountNumber);

                db.Transactions.Add(newTransaction);
                await db.SaveChangesAsync();
                return Ok(new { Account = newAccount.Balance , Transaction = newTransaction });
            }
            else if (decimal.IsNegative(data.Amount))
            {
                return BadRequest();
            }
            else
            {
                return NotFound("Account not found");
            }
        }

        [HttpPut("{id:int}/withdraw")]
        public async Task<IActionResult> WithdrawAccount ([FromServices] AccountDbContext db, int id, AccountWithdrawDTO data)
        {
            Transaction newTransaction = new Transaction();
            newTransaction.AccountId = id;
            newTransaction.Type = TransactionType.Withdrawal;
            newTransaction.Amount = data.Amount;
            newTransaction.Timestamp = DateTime.UtcNow;
            
            if (data.newType == AccountType.Current)
            {
                var newAccount = await db.CurrentAccounts.Where(h => h.Id == id).FirstOrDefaultAsync();
                newTransaction.Reference = Convert.ToString(newAccount.AccountNumber);

                if (newAccount != null && decimal.IsPositive(data.Amount) )
                {
                    if (newAccount.Balance > data.Amount)
                    {
                        newAccount.Balance -= data.Amount;

                        newTransaction.BalanceAfter = newAccount.Balance;
                        db.Transactions.Add(newTransaction);
                        await db.SaveChangesAsync();
                        return Ok(new { Account = newAccount.Balance });
                    }
                    else if (newAccount.Balance < data.Amount && newAccount.Type == AccountType.Current && (newAccount.Balance + newAccount.OverdraftLimit) > data.Amount)
                    {
                        decimal CurrentOverdraftBalance = (newAccount.Balance + newAccount.OverdraftLimit) - data.Amount;
                        newAccount.Balance = 0;
                        newAccount.OverdraftLimit = CurrentOverdraftBalance;
                        newTransaction.BalanceAfter = newAccount.Balance;
                        db.Transactions.Add(newTransaction);
                        await db.SaveChangesAsync();
                        return Ok(new { Account = newAccount.Balance, OverdraftBalance = newAccount.OverdraftLimit });
                    }
                    else if (newAccount.Balance < data.Amount && newAccount.Type == AccountType.Current && (newAccount.Balance + newAccount.OverdraftLimit) < data.Amount)
                    {
                        return BadRequest("Account does not qualify for withdrawl");
                    }
                    else
                    {
                        return BadRequest("Error");
                    }

                }
                else
                {
                    return NotFound("Account Not Found");
                }
            }

            else if (data.newType == AccountType.Savings && decimal.IsPositive(data.Amount))
            {
                var newAccount = await db.SavingsAccounts.Where(h => h.Id == id).FirstOrDefaultAsync();
                newTransaction.Reference = Convert.ToString(newAccount.AccountNumber);
                if (newAccount != null)
                {
                    if (newAccount.Balance > data.Amount)
                    {
                        newAccount.Balance -= data.Amount;
                        newTransaction.BalanceAfter = newAccount.Balance;
                        db.Transactions.Add(newTransaction);
                        await db.SaveChangesAsync();
                        return Ok(new { Account = newAccount.Balance });
                    }
                    else if(newAccount.Balance < data.Amount)
                    {
                        return BadRequest("Insufficient funds");
                    }
                    else
                    {
                        return BadRequest("Unknown error");
                    }
                }
                else
                {
                    return NotFound("Account Not found");
                }
            }
            else
            {
                return Ok("Unknown error");
            }
        }

        [HttpPut]
        public async Task<IActionResult>  TransferAccount([FromServices] AccountDbContext db, int id, AccountTransferDTO data)
        {
            var senderAccount = await db.Accounts.Where(h => h.Id == id).FirstOrDefaultAsync();
            var recipientAccount = await db.Accounts.Where(h => h.Id == data.RecipientAccountId).FirstOrDefaultAsync();
            Transaction SenderTransaction = new Transaction();
            SenderTransaction.AccountId = id;
            SenderTransaction.Type = TransactionType.TransferOut;
            SenderTransaction.Timestamp = DateTime.UtcNow;
            SenderTransaction.Amount = data.Amount;

            Transaction RecipientTransaction = new Transaction();
            RecipientTransaction.AccountId = data.RecipientAccountId;
            RecipientTransaction.Type = TransactionType.TransferIn;
            RecipientTransaction.Timestamp = DateTime.UtcNow;
            RecipientTransaction.Amount = data.Amount;
            if (senderAccount != null && recipientAccount != null && decimal.IsPositive(data.Amount))
            {
                if (senderAccount.Balance > data.Amount)
                {
                    senderAccount.Balance -= data.Amount;
                    recipientAccount.Balance += data.Amount;
                    SenderTransaction.BalanceAfter = senderAccount.Balance;
                    RecipientTransaction.BalanceAfter = recipientAccount.Balance;
                    db.Transactions.Add(SenderTransaction);
                    db.Transactions.Add(RecipientTransaction);
                    await db.SaveChangesAsync();
                    return Ok(new { SenderBalance = senderAccount.Balance, reciepientBalance = recipientAccount.Balance });
                }
                else if(senderAccount.Balance > data.Amount && decimal.IsPositive(data.Amount))
                {
                    return BadRequest("Insufficient funds");
                }
                else
                {
                    return BadRequest("Unknown Error");
                }
            }
            else
            {
                return NotFound("Wrong Accounts");
            }
        }

    }
}
