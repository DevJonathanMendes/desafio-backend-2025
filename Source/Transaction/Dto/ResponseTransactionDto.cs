using Bankount.Transaction.Model;

namespace Bankount.Transaction.Dto;

public class ResponseTransactionDto
{
	public long Id { get; set; }
	public decimal Value { get; set; }
	public TransactionType Type { get; set; }
	public long AccountId { get; set; }
	public long? TargetAccountId { get; set; }
	public DateTime CreatedAt { get; set; }
}
