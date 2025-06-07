using lootboxes_db.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace lootboxes_db.Context;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<LootBox> LootBoxes { get; set; }

    public virtual DbSet<Rarity> Rarities { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserInventory> UserInventories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost; Database=postgres; Username=postgres; Password=123;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Categories_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Items_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Items_CategoryId_fkey");

            entity.HasOne(d => d.Rarity).WithMany(p => p.Items)
                .HasForeignKey(d => d.RarityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Items_RarityId_fkey");

            entity.HasMany(d => d.LootBoxes).WithMany(p => p.Items)
                .UsingEntity<Dictionary<string, object>>(
                    "LootBoxItem",
                    r => r.HasOne<LootBox>().WithMany()
                        .HasForeignKey("LootBoxId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("LootBoxItems_LootBoxId_fkey"),
                    l => l.HasOne<Item>().WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("LootBoxItems_ItemId_fkey"),
                    j =>
                    {
                        j.HasKey("ItemId", "LootBoxId").HasName("LootBoxItems_pkey");
                        j.ToTable("LootBoxItems");
                    });
        });

        modelBuilder.Entity<LootBox>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("LootBoxes_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<Rarity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Rarities_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.HasIndex(e => e.Login, "Users_Login_key").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<UserInventory>(entity =>
        {
            entity.HasKey(e => e.UserInventoryId).HasName("UserInventory_pkey");

            entity.ToTable("UserInventory");

            entity.Property(e => e.UserInventoryId).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Item).WithMany(p => p.UserInventories)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserInventory_ItemId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserInventories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserInventory_UserId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
