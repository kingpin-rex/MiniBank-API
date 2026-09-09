using MiniBank_API.Application.AccountDatabase;

namespace MiniBank_API.Application.DTOs
{
    public record AccountTransferDTO
    (
        decimal Amount,
        int RecipientAccountId
    );
}
