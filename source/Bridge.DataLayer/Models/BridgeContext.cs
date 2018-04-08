using Microsoft.EntityFrameworkCore;

namespace Bridge.DataLayer.Models
{
    public partial class BridgeContext : DbContext
    {
        public virtual DbSet<Deal> Deal { get; set; }
        public virtual DbSet<DuplicateDeal> DuplicateDeal { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<MakeableContract> MakeableContract { get; set; }
        public virtual DbSet<Pair> Pair { get; set; }
        public virtual DbSet<SysEventType> SysEventType { get; set; }
        public virtual DbSet<SysPlayer> SysPlayer { get; set; }
        public virtual DbSet<SysVulnerability> SysVulnerability { get; set; }

        public BridgeContext(DbContextOptions<BridgeContext> options) : base(options)
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Deal>(entity =>
            {
                entity.Property(e => e.BestContract)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.BestContractDisplay)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.BestContractHandViewerInput).IsRequired();

                entity.Property(e => e.HandViewerInput).IsRequired();

                entity.Property(e => e.Pbnrepresentation)
                    .IsRequired()
                    .HasColumnName("PBNRepresentation");

                entity.HasOne(d => d.BestContractDeclarerNavigation)
                    .WithMany(p => p.Deal)
                    .HasForeignKey(d => d.BestContractDeclarer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Deal_SysPlayer");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Deal)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_Deal_Event");

                entity.HasOne(d => d.SysVulnerability)
                    .WithMany(p => p.Deal)
                    .HasForeignKey(d => d.SysVulnerabilityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Deal_SysVulnerability");
            });

            modelBuilder.Entity<DuplicateDeal>(entity =>
            {
                entity.Property(e => e.Contract)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ContractDisplay)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.EwpairId).HasColumnName("EWPairId");

                entity.Property(e => e.Ewpercentage).HasColumnName("EWPercentage");

                entity.Property(e => e.HandViewerInput).IsRequired();

                entity.Property(e => e.NspairId).HasColumnName("NSPairId");

                entity.Property(e => e.Nspercentage).HasColumnName("NSPercentage");

                entity.Property(e => e.Tricks)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasOne(d => d.Deal)
                    .WithMany(p => p.DuplicateDeal)
                    .HasForeignKey(d => d.DealId)
                    .HasConstraintName("FK_DuplicateDeal_Deal");

                entity.HasOne(d => d.DeclarerNavigation)
                    .WithMany(p => p.DuplicateDeal)
                    .HasForeignKey(d => d.Declarer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DuplicateDeal_Declarer");

                entity.HasOne(d => d.Ewpair)
                    .WithMany(p => p.DuplicateDealEwpair)
                    .HasForeignKey(d => d.EwpairId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DuplicateDeal_EWPair");

                entity.HasOne(d => d.Nspair)
                    .WithMany(p => p.DuplicateDealNspair)
                    .HasForeignKey(d => d.NspairId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DuplicateDeal_NSPair");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.SysEventType)
                    .WithMany(p => p.Event)
                    .HasForeignKey(d => d.SysEventTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Event_EventType");
            });

            modelBuilder.Entity<Events>(entity =>
            {
                entity.HasKey(e => e.InStoreId);

                entity.Property(e => e.EventBody).IsRequired();

                entity.Property(e => e.EventDescription)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.OccurredOn).HasColumnType("datetime");

                entity.Property(e => e.TypeName)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<MakeableContract>(entity =>
            {
                entity.Property(e => e.Contract)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.HandViewerInput).IsRequired();

                entity.HasOne(d => d.Deal)
                    .WithMany(p => p.MakeableContract)
                    .HasForeignKey(d => d.DealId)
                    .HasConstraintName("FK_MakeableContract_Deal");

                entity.HasOne(d => d.DeclarerNavigation)
                    .WithMany(p => p.MakeableContract)
                    .HasForeignKey(d => d.Declarer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MakeableContract_SysPlayer");
            });

            modelBuilder.Entity<Pair>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Player1Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Player2Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Score).HasColumnType("decimal(5, 2)");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Pair)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_Pair_Event");
            });

            modelBuilder.Entity<SysEventType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EventType)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SysPlayer>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Player)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SysVulnerability>(entity =>
            {
                entity.Property(e => e.SysVulnerabilityId).ValueGeneratedNever();

                entity.Property(e => e.SysVulnerabilityName)
                    .IsRequired()
                    .HasColumnType("nchar(50)");
            });
        }
    }
}
