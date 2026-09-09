using FluentValidation;
using Microsoft.AspNetCore.Http;
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
    public class transaction : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> DisplayAllTransaction([FromServices]AccountDbContext db)
        {
           
            var transactions = await db.Transactions.ToListAsync();
            if (transactions != null)
            { 
                return Ok(new { TransactionList = transactions }); 
            }else
            {
                return NotFound("No Transactions found");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> DisplayTransaction([FromServices] AccountDbContext db, int id)
        {
            var transactions = await db.Transactions.Where(h => h.Id == id).ToListAsync();

            if (transactions != null)
            {
                return Ok(new {TransactionList =  transactions});
            }
            else
            {
                return NotFound("No transactions for that Id");
            }
        }
    }


}
