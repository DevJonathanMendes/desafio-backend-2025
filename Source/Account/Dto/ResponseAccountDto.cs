namespace Bankount.Account.Dto;

public class ResponseAccountDto
{
	public long Id { get; set; }
	public string Name { get; set; } = default!;
	public string Cnpj { get; set; } = default!;
	public string AccountNumber { get; set; } = default!;
	public string Agency { get; set; } = default!;
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
