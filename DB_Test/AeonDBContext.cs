using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DB_Test;

public partial class AeonDBContext : DbContext
{
	public AeonDBContext(DbContextOptions<AeonDBContext> options) : base(options)
	{
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
			entity.HasNoKey();

			entity.Property(e => e.Result)
				.HasMaxLength(32)
				.IsUnicode(false);

			entity.Property(e => e.RoundId).HasColumnName("RoundID");

			entity.HasOne(d => d.Round)
				.WithMany()
				.HasForeignKey(d => d.RoundId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Attacks__RoundID__34C8D9D1");
		});

		modelBuilder.Entity<Buy>(entity => {
			entity.HasNoKey();

			entity.Property(e => e.RoundId).HasColumnName("RoundID");

			entity.Property(e => e.StatId).HasColumnName("StatID");

			entity.HasOne(d => d.Round)
				.WithMany()
				.HasForeignKey(d => d.RoundId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Buys__RoundID__31EC6D26");

			entity.HasOne(d => d.Stat)
				.WithMany()
				.HasForeignKey(d => d.StatId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Buys__StatID__32E0915F");
		});

		modelBuilder.Entity<Game>(entity => {
			entity.Property(e => e.Id).HasColumnName("ID");

			entity.Property(e => e.Player1Id).HasColumnName("Player1ID");

			entity.Property(e => e.Player2Id).HasColumnName("Player2ID");

			entity.HasOne(d => d.Hero1Navigation)
				.WithMany(p => p.GameHero1Navigations)
				.HasForeignKey(d => d.Hero1)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Games__Hero1__2A4B4B5E");

			entity.HasOne(d => d.Hero2Navigation)
				.WithMany(p => p.GameHero2Navigations)
				.HasForeignKey(d => d.Hero2)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Games__Hero2__2B3F6F97");

			entity.HasOne(d => d.Player1)
				.WithMany(p => p.GamePlayer1s)
				.HasForeignKey(d => d.Player1Id)
				.HasConstraintName("FK__Games__Player1ID__286302EC");

			entity.HasOne(d => d.Player2)
				.WithMany(p => p.GamePlayer2s)
				.HasForeignKey(d => d.Player2Id)
				.HasConstraintName("FK__Games__Player2ID__29572725");
		});

		modelBuilder.Entity<Hero>(entity => {
			entity.Property(e => e.Id).HasColumnName("ID");

			entity.Property(e => e.AsmName)
				.HasMaxLength(128)
				.IsUnicode(false);
		});

		modelBuilder.Entity<Player>(entity => {
			entity.Property(e => e.Id).HasColumnName("ID");

			entity.Property(e => e.Nickname).HasMaxLength(50);

			entity.Property(e => e.Pwhash).HasColumnName("PWHash");

			entity.Property(e => e.ValueElo)
				.HasColumnType("decimal(9, 4)")
				.HasColumnName("ValueELO");
		});

		modelBuilder.Entity<Round>(entity => {
			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("ID");

			entity.Property(e => e.GameId).HasColumnName("GameID");

			entity.HasOne(d => d.Game)
				.WithMany(p => p.Rounds)
				.HasForeignKey(d => d.GameId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Rounds__GameID__2E1BDC42");
		});

		modelBuilder.Entity<Stat>(entity => {
			entity.Property(e => e.Id).HasColumnName("ID");

			entity.Property(e => e.AsmName)
				.HasMaxLength(128)
				.IsUnicode(false);
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
