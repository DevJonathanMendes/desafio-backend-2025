using System.ComponentModel.DataAnnotations;

namespace Bankount.Account.Dto;

public class PatchAccountDto : IAccountValidationDto
{
	[RegularExpression(@"^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$", ErrorMessage = "The CNPJ must be in the format 00.000.000/0000-00.")]
	public string? Cnpj { get; set; }

	[RegularExpression(@"^\d{5,10}$", ErrorMessage = "The account number must be between 5 and 10 digits long.")]
	public string? AccountNumber { get; set; }

	[RegularExpression(@"^\d{4}$", ErrorMessage = "The agency must contain exactly 4 digits.")]
	public string? Agency { get; set; }

	public IFormFile? DocumentImage { get; set; }
}
