using Microsoft.AspNetCore.Mvc;

using Bankount.Exceptions;
using Bankount.Transaction.Dto;
using Bankount.Transaction.Model;
using Bankount.Transaction.Service;

namespace Bankount.Transaction.Controller;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(TransactionService transactionService) : ControllerBase
{
	private readonly TransactionService _transactionService = transactionService;

	[HttpPost("deposit")]
	public async Task<IActionResult> Deposit([FromBody] BaseTransactionDto baseTransactionDto)
	{
		var result = await _transactionService.CreateTransactionAsync(
			baseTransactionDto, TransactionType.Deposit
		);

		return CreatedAtAction(nameof(GetExtract), new { accountId = result.AccountId }, result);
	}

	[HttpPost("withdraw")]
	public async Task<IActionResult> Withdraw([FromBody] BaseTransactionDto dto)
	{
		var result = await _transactionService.CreateTransactionAsync(dto, TransactionType.Withdrawal);
		return CreatedAtAction(nameof(GetExtract), new { accountId = result.AccountId }, result);
	}

	[HttpPost("transfer")]
	public async Task<IActionResult> Transfer([FromBody] TransferTransactionDto dto)
	{
		try
		{
			var result = await _transactionService.CreateTransferTransactionAsync(dto);
			return CreatedAtAction(nameof(GetExtract), new { accountId = result.AccountId }, result);
		}
		catch (HttpResponseException ex)
		{
			return StatusCode(ex.StatusCode, ex.Value);
		}
	}

	[HttpGet("accounts/{accountId}/extract")]
	public async Task<IActionResult> GetExtract(long accountId)
	{
		var transactions = await _transactionService.GetExtractByAccountIdAsync(accountId);

		return Ok(transactions);
	}

	[HttpGet("accounts/{accountId}/balance")]
	public async Task<IActionResult> GetBalance(long accountId)
	{
		var balance = await _transactionService.GetAccountBalanceAsync(accountId);
		return Ok(new { accountId, balance });
	}
}
