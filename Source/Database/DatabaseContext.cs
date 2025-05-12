using Microsoft.EntityFrameworkCore;

using Bankount.Account.Model;
using Bankount.Transaction.Model;

namespace Bankount.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
	public DbSet<AccountModel> Accounts { get; set; }
	public DbSet<TransactionsModel> Transactions { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<AccountModel>().HasIndex(a => a.Cnpj).IsUnique();
		modelBuilder.Entity<AccountModel>().HasIndex(a => a.AccountNumber).IsUnique();
		modelBuilder.Entity<AccountModel>().HasIndex(a => a.Agency).IsUnique();

		modelBuilder.Entity<TransactionsModel>()
			.HasOne(t => t.Account)
			.WithMany(a => a.Transactions)
			.HasForeignKey(t => t.AccountId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<TransactionsModel>()
			.HasOne(t => t.TargetAccount)
			.WithMany()
			.HasForeignKey(t => t.TargetAccountId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}