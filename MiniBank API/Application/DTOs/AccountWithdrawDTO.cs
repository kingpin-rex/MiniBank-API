using MiniBank_API.Application.AccountDatabase;

namespace MiniBank_API.Application.DTOs
{
    public record AccountWithdrawDTO
    (
        decimal Amount,
        AccountType newType
    );
}
