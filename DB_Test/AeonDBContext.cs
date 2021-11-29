using Microsoft.EntityFrameworkCore;

namespace DB_Test;

public partial class AeonDBContext : DbContext
{
	public AeonDBContext(DbContextOptions<AeonDBContext> options) : base(options)
	{
		Database.EnsureDeleted();
		Database.EnsureCreated();
	}

	public virtual DbSet<Attack> Attacks { get; set; } = null!;
	public virtual DbSet<Buy> Buys { get; set; } = null!;
	public virtual DbSet<Game> Games { get; set; } = null!;
	public virtual DbSet<Hero> Heroes { get; set; } = null!;
	public virtual DbSet<Player> Players { get; set; } = null!;
	public virtual DbSet<Round> Rounds { get; set; } = null!;
	public virtual DbSet<Stat> Stats { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Attack>(entity => {
			entity.HasKey(e => new { e.GameId, e.RoundNumber, e.Number });

			entity.Property(e => e.Result).HasMaxLength(32).IsUnicode(false);

			entity.HasOne(e => e.Round)
			.WithMany(r => r.Attacks)
			.HasForeignKey(e => new { e.GameId, e.RoundNumber });
		});

		modelBuilder.Entity<Buy>(entity => {
			entity.HasNoKey();

			entity.HasOne(d => d.Round).WithMany()
				.HasForeignKey(d => new { d.GameId, d.RoundNumber });

			entity.HasOne(d => d.Stat).WithMany()
				.HasForeignKey(d => d.StatId)
				.OnDelete(DeleteBehavior.ClientSetNull);
		});

		modelBuilder.Entity<Game>(entity => {
			entity.HasOne(e => e.Player1)
			.WithMany(p => p.GamesOnPos1)
			.HasForeignKey(e => e.Player1Id)
			.OnDelete(DeleteBehavior.ClientSetNull);

			entity.HasOne(e => e.Player2)
			.WithMany(p => p.GamesOnPos2)
			.HasForeignKey(e => e.Player2Id)
			.OnDelete(DeleteBehavior.ClientSetNull);

			entity.HasOne(e => e.Hero1)
			.WithMany(h => h.GamesPick1)
			.HasForeignKey(e => e.Hero1Id)
			.OnDelete(DeleteBehavior.ClientSetNull);

			entity.HasOne(e => e.Hero2)
			.WithMany(h => h.GamesPick2)
			.HasForeignKey(e => e.Hero2Id)
			.OnDelete(DeleteBehavior.ClientSetNull);
		});

		modelBuilder.Entity<Hero>(entity => {
			entity.HasAlternateKey(e => e.AsmName);
			entity.Property(e => e.AsmName).HasMaxLength(128).IsUnicode(false);
		});

		modelBuilder.Entity<Player>(entity => {
			entity.HasAlternateKey(e => e.Nickname);
			entity.Property(e => e.Nickname).HasMaxLength(50);
			entity.Property(e => e.ValueElo).HasColumnType("decimal(9, 4)");
		});

		modelBuilder.Entity<Round>(entity => {
			entity.HasKey(e => new { e.GameId, e.Number });

			entity.HasOne(d => d.Game)
				.WithMany(p => p.Rounds)
				.HasForeignKey(d => d.GameId);
		});

		modelBuilder.Entity<Stat>(entity => {
			entity.HasAlternateKey(e => e.AsmName);
			entity.Property(e => e.AsmName).HasMaxLength(128).IsUnicode(false);
		});
	}
}
