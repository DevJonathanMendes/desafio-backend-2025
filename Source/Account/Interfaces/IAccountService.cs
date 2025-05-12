using Bankount.Account.Dto;
using Bankount.Account.Model;

public interface IAccountService
{
	Task<AccountModel> CreateAccountAsync(CreateAccountDto createAccountDto, string documentImageBase64);
	Task<AccountModel> GetAccountByIdAsync(long id);
	Task<List<ResponseAccountDto>> GetAccountsAsync();
	Task<AccountModel> PatchAccountByIdAsync(long id, PatchAccountDto patchAccountDto, string documentImageBase64);
	Task DeleteAccountByIdAsync(long id);
}
