using System.Net;
using Microsoft.EntityFrameworkCore;

using Bankount.Database;
using Bankount.Exceptions;
using Bankount.Transaction.Dto;
using Bankount.Transaction.Model;

namespace Bankount.Transaction.Service;

public class TransactionService(DatabaseContext context)
{
	private readonly DatabaseContext _context = context;

	public async Task<ResponseTransactionDto> CreateTransactionAsync(
		BaseTransactionDto baseTransactionDto, TransactionType transactionType)
	{
		var originAccount = await _context.Accounts.FindAsync(baseTransactionDto.AccountId)
			?? throw new HttpResponseException(HttpStatusCode.NotFound, "Origin account not found.");

		if (transactionType is TransactionType.Withdrawal)
		{
			var currentBalance = await GetAccountBalanceAsync(baseTransactionDto.AccountId);
			if (currentBalance < baseTransactionDto.Value)
				throw new HttpResponseException(HttpStatusCode.BadRequest, "Insufficient balance.");
		}

		var transaction = new TransactionsModel
		{
			Value = baseTransactionDto.Value,
			Type = transactionType,
			AccountId = baseTransactionDto.AccountId
		};

		_context.Transactions.Add(transaction);
		await _context.SaveChangesAsync();

		return new ResponseTransactionDto
		{
			Id = transaction.Id,
			Value = transaction.Value,
			Type = transaction.Type,
			AccountId = transaction.AccountId,
			TargetAccountId = transaction.TargetAccountId,
			CreatedAt = transaction.CreatedAt
		};
	}

	public async Task<ResponseTransactionDto> CreateTransferTransactionAsync(TransferTransactionDto dto)
	{
		var originAccount = await _context.Accounts.FindAsync(dto.AccountId)
			?? throw new HttpResponseException(HttpStatusCode.NotFound, "Origin account not found.");

		var targetAccount = await _context.Accounts.FindAsync(dto.TargetAccountId)
			?? throw new HttpResponseException(HttpStatusCode.NotFound, "Target account not found.");

		var currentBalance = await GetAccountBalanceAsync(dto.AccountId);
		if (currentBalance < dto.Value)
			throw new HttpResponseException(HttpStatusCode.BadRequest, "Insufficient balance.");

		var debitTransaction = new TransactionsModel
		{
			Value = dto.Value,
			Type = TransactionType.Transfer,
			AccountId = dto.AccountId,
			TargetAccountId = dto.TargetAccountId
		};

		var creditTransaction = new TransactionsModel
		{
			Value = dto.Value,
			Type = TransactionType.Deposit,
			AccountId = dto.TargetAccountId
		};

		_context.Transactions.AddRange(debitTransaction, creditTransaction);
		await _context.SaveChangesAsync();

		return new ResponseTransactionDto
		{
			Id = debitTransaction.Id,
			Value = debitTransaction.Value,
			Type = debitTransaction.Type,
			AccountId = debitTransaction.AccountId,
			TargetAccountId = debitTransaction.TargetAccountId,
			CreatedAt = debitTransaction.CreatedAt
		};
	}

	public async Task<IEnumerable<TransactionsModel>> GetExtractByAccountIdAsync(long accountId)
	{
		return await _context.Transactions
			.Where(t => t.AccountId == accountId || t.TargetAccountId == accountId)
			.ToListAsync();
	}

	public async Task<decimal> GetAccountBalanceAsync(long accountId)
	{
		var depositsAndReceived = await _context.Transactions
			.Where(t => (t.AccountId == accountId && t.Type == TransactionType.Deposit)
					|| (t.TargetAccountId == accountId && t.Type == TransactionType.Transfer))
			.SumAsync(t => t.Value);

		var withdrawalsAndSent = await _context.Transactions
			.Where(t => (t.AccountId == accountId && t.Type == TransactionType.Withdrawal)
					|| (t.AccountId == accountId && t.Type == TransactionType.Transfer))
			.SumAsync(t => t.Value);

		return depositsAndReceived - withdrawalsAndSent;
	}
}
