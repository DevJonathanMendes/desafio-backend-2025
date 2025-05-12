using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bankount.Account.Model;

namespace Bankount.Transaction.Model;

public enum TransactionType
{
	Deposit = 1,
	Withdrawal = 2,
	Transfer = 3
}

[Table("Transactions")]
public class TransactionsModel
{
	[Key]
	public long Id { get; set; }

	[Required]
	public decimal Value { get; set; }

	[Required]
	public TransactionType Type { get; set; }

	[Required]
	public long AccountId { get; set; }

	[ForeignKey(nameof(AccountId))]
	public AccountModel Account { get; set; } = null!;

	public long? TargetAccountId { get; set; }

	[ForeignKey(nameof(TargetAccountId))]
	public AccountModel? TargetAccount { get; set; }

	[Column(TypeName = "timestamptz")]
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	[Column(TypeName = "timestamptz")]
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}
