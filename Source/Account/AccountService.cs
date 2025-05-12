using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

using Bankount.Database;
using Bankount.Exceptions;
using Bankount.Account.Dto;
using Bankount.Account.Model;

namespace Bankount.Account.Service;

public class AccountService(DatabaseContext context) : IAccountService
{
	private readonly DatabaseContext _context = context;

	public async Task<AccountModel> CreateAccountAsync(CreateAccountDto createAccountDto, string documentImageBase64)
	{
		await CheckAlreadyExist(createAccountDto);
		var name = await GetCompanyByCnpjAsync(createAccountDto.Cnpj);

		var account = new AccountModel
		{
			Name = name,
			Cnpj = createAccountDto.Cnpj,
			AccountNumber = createAccountDto.AccountNumber,
			Agency = createAccountDto.Agency,
			DocumentImageBase64 = documentImageBase64,
		};

		_context.Accounts.Add(account);
		await _context.SaveChangesAsync();
		return account;
	}

	public async Task<List<ResponseAccountDto>> GetAccountsAsync()
	{
		return await _context.Accounts
			.Select(a => new ResponseAccountDto
			{
				Id = a.Id,
				Name = a.Name,
				Cnpj = a.Cnpj,
				AccountNumber = a.AccountNumber,
				Agency = a.Agency,
				CreatedAt = a.CreatedAt,
				UpdatedAt = a.UpdatedAt
			})
			.ToListAsync();
	}

	public async Task<AccountModel> GetAccountByIdAsync(long id)
	{
		return await _context.Accounts.FindAsync(id) ??
			throw new HttpResponseException(
				HttpStatusCode.NotFound, "Account not found."
			);
	}

	public async Task<AccountModel> PatchAccountByIdAsync(long id, PatchAccountDto patchAccountDto, string base64Image)
	{
		var account = await this.GetAccountByIdAsync(id);

		await CheckAlreadyExist(patchAccountDto);

		if (!string.IsNullOrEmpty(patchAccountDto.Cnpj))
		{
			account.Name = await GetCompanyByCnpjAsync(patchAccountDto.Cnpj);
		}

		if (!string.IsNullOrEmpty(patchAccountDto.Cnpj))
		{
			account.Cnpj = patchAccountDto.Cnpj;
		}

		if (!string.IsNullOrEmpty(patchAccountDto.Agency))
		{
			account.Agency = patchAccountDto.Agency;
		}

		if (!string.IsNullOrEmpty(patchAccountDto.AccountNumber))
		{
			account.AccountNumber = patchAccountDto.AccountNumber;
		}

		if (!string.IsNullOrEmpty(base64Image))
		{
			account.DocumentImageBase64 = base64Image;
		}


		account.UpdatedAt = DateTime.UtcNow;
		await _context.SaveChangesAsync();

		return account;
	}

	public async Task DeleteAccountByIdAsync(long id)
	{
		var account = await this.GetAccountByIdAsync(id);

		_context.Accounts.Remove(account);
		await _context.SaveChangesAsync();
	}

	private async Task CheckAlreadyExist(IAccountValidationDto accountValidationDto)
	{
		var alreadyExists = await _context.Accounts
			.Where(a => a.Cnpj == accountValidationDto.Cnpj
				|| a.AccountNumber == accountValidationDto.AccountNumber
				|| a.Agency == accountValidationDto.Agency
			)
			.ToListAsync();

		var errors = new Dictionary<string, string[]>();

		if (alreadyExists.Any(e => e.Cnpj == accountValidationDto.Cnpj))
			errors["Cnpj"] = ["CNPJ already exists."];

		if (alreadyExists.Any(e => e.AccountNumber == accountValidationDto.AccountNumber))
			errors["AccountNumber"] = ["AccountNumber already exists."];

		if (alreadyExists.Any(e => e.Agency == accountValidationDto.Agency))
			errors["Agency"] = ["Agency already exists."];

		if (errors.Count != 0)
		{
			throw new HttpResponseException(
				HttpStatusCode.Conflict, new ValidationProblemDetails(errors)
			);
		}
	}

	record ReceitaWsResponse(string? Status, string? Nome);
	public async Task<string> GetCompanyByCnpjAsync(string cnpj)
	{
		using var http = new HttpClient();
		cnpj = Regex.Replace(cnpj, @"\D", "");

		var res = await http.GetFromJsonAsync<ReceitaWsResponse>($"https://receitaws.com.br/v1/cnpj/{cnpj}");

		if (res is null || res.Status?.ToLower() != "ok" || string.IsNullOrWhiteSpace(res.Nome))
		{
			throw new HttpResponseException(
				HttpStatusCode.BadRequest, new ValidationProblemDetails(
					new Dictionary<string, string[]>()
					{
						["cnpj"] = ["We were unable to find the company using the CNPJ provided, or please try again later."]
					}
				)
			);
		}

		return res.Nome;
	}

}