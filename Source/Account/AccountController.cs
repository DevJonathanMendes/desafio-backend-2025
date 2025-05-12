using Microsoft.AspNetCore.Mvc;

using Bankount.Exceptions;
using Bankount.Account.Dto;
using Bankount.Account.Model;

namespace Bankount.Account.Controller;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IAccountService service) : ControllerBase
{
	private readonly IAccountService _service = service;

	[HttpPost]
	[Consumes("multipart/form-data")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
	[ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
	public async Task<IActionResult> CreateAccount([FromForm] CreateAccountDto createAccountDto)
	{
		using var ms = new MemoryStream();
		await createAccountDto.DocumentImage.CopyToAsync(ms);
		var base64Image = Convert.ToBase64String(ms.ToArray());

		try
		{
			var account = await _service.CreateAccountAsync(createAccountDto, base64Image);

			account.DocumentImageBase64 = "Saved successfully.";

			return CreatedAtAction(nameof(GetAccountById), new { id = account.Id }, account);
		}
		catch (HttpResponseException ex)
		{
			return StatusCode(ex.StatusCode, ex.Value);
		}
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<AccountModel>> GetAccounts()
	{
		var account = await _service.GetAccountsAsync();

		return account is not null ? Ok(account) : NotFound();
	}

	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetAccountById(long id)
	{
		try
		{
			return Ok(await _service.GetAccountByIdAsync(id));
		}
		catch (HttpResponseException ex)
		{
			return StatusCode(ex.StatusCode, ex.Value);
		}
	}

	[HttpPatch("{id}")]
	[Consumes("multipart/form-data")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	public async Task<IActionResult> PatchAccountById(long id, [FromForm] PatchAccountDto patchAccountDto)
	{
		string? base64Image = null;

		if (patchAccountDto.DocumentImage is not null && patchAccountDto.DocumentImage.Length > 0)
		{
			using var ms = new MemoryStream();
			await patchAccountDto.DocumentImage.CopyToAsync(ms);
			base64Image = Convert.ToBase64String(ms.ToArray());
		}

		try
		{
			await _service.PatchAccountByIdAsync(id, patchAccountDto, base64Image ?? "");
			return Ok();
		}
		catch (HttpResponseException ex)
		{
			return StatusCode(ex.StatusCode, ex.Value);
		}
	}

	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteAccountById(long id)
	{
		try
		{
			await this._service.DeleteAccountByIdAsync(id);
			return NoContent();
		}
		catch (HttpResponseException ex)
		{
			return StatusCode(ex.StatusCode, ex.Value);
		}
	}
}
