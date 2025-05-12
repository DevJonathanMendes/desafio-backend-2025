using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Bankount.Transaction.Model;

namespace Bankount.Account.Model;

[Table("Accounts")]
public class AccountModel
{
	[Key]
	public long Id { get; set; }

	[Required]
	[MaxLength(255)]
	public string Name { get; set; } = null!;

	[Required]
	[MaxLength(18)]
	public string Cnpj { get; set; } = null!;

	[Required]
	[MaxLength(10)]
	public string AccountNumber { get; set; } = null!;

	[Required]
	[MaxLength(4)]
	public string Agency { get; set; } = null!;

	[Required]
	public string DocumentImageBase64 { get; set; } = null!;

	[Column(TypeName = "timestamptz")]
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	[Column(TypeName = "timestamptz")]
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

	public ICollection<TransactionsModel> Transactions { get; set; } = new List<TransactionsModel>();
}
