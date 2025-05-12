using System.ComponentModel.DataAnnotations;

namespace Bankount.Transaction.Dto;

public class BaseTransactionDto
{
	[Required(ErrorMessage = "Transaction amount is required.")]
	[Range(0.01, double.MaxValue, ErrorMessage = "The value must be greater than zero.")]
	public decimal Value { get; set; }

	[Required(ErrorMessage = "Source account is required.")]
	public long AccountId { get; set; }
}

public class TransferTransactionDto : BaseTransactionDto
{
	[Required(ErrorMessage = "Target account is required for transfers.")]
	public long TargetAccountId { get; set; }
}
