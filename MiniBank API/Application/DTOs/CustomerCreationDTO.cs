using MiniBank_API.Application.AccountDatabase;

namespace MiniBank_API.Application.DTOs
{
    public record CustomerCreationDTO
    (
        string FirstName,
        string LastName,
        string EmailAddress,
        DateOnly DateOfBirth,
        decimal InitialDeposit,
        DateOnly CreatedOn,
        AccountType Type,
        decimal Overdraftlimit,
        decimal interestRate,
        string Password


    );
}
