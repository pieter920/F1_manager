using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace F1_managerApi.Models;

public partial class F1_ManagerDbContext : DbContext
{
    public F1_ManagerDbContext()
    {
    }

    public F1_ManagerDbContext(DbContextOptions<F1_ManagerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Auto> Autos { get; set; }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<Raceweekend> Raceweekends { get; set; }

    public virtual DbSet<Raceweekendhasdriver> Raceweekendhasdrivers { get; set; }

    public virtual DbSet<Seizoen> Seizoens { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<Teamhasseizoen> Teamhasseizoens { get; set; }

    public virtual DbSet<Track> Tracks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=f1_manager;user=root;password=1234", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.43-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Auto>(entity =>
        {
            entity.HasKey(e => e.Idauto).HasName("PRIMARY");

            entity.ToTable("auto");

            entity.HasIndex(e => e.Fkteam, "FKTeam");

            entity.Property(e => e.Idauto).HasColumnName("IDAuto");
            entity.Property(e => e.Fkteam).HasColumnName("FKTeam");
            entity.Property(e => e.NaamAuto).HasMaxLength(64);

            entity.HasOne(d => d.FkteamNavigation).WithMany(p => p.Autos)
                .HasForeignKey(d => d.Fkteam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("auto_ibfk_1");
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.Iddriver).HasName("PRIMARY");

            entity.ToTable("driver");

            entity.HasIndex(e => e.Fkteam, "FKTeam");

            entity.Property(e => e.Iddriver).HasColumnName("IDDriver");
            entity.Property(e => e.AchternaamDriver).HasMaxLength(64);
            entity.Property(e => e.Fkteam).HasColumnName("FKTeam");
            entity.Property(e => e.NationaliteitDriver).HasMaxLength(64);
            entity.Property(e => e.VoornaamDriver).HasMaxLength(64);

            entity.HasOne(d => d.FkteamNavigation).WithMany(p => p.Drivers)
                .HasForeignKey(d => d.Fkteam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("driver_ibfk_1");
        });

        modelBuilder.Entity<Raceweekend>(entity =>
        {
            entity.HasKey(e => e.IdraceWeekend).HasName("PRIMARY");

            entity.ToTable("raceweekend");

            entity.HasIndex(e => e.Fkseizoen, "FKSeizoen");

            entity.HasIndex(e => e.Fktrack, "FKTrack");

            entity.HasIndex(e => e.Fkuser, "FKUser");

            entity.Property(e => e.IdraceWeekend).HasColumnName("IDRaceWeekend");
            entity.Property(e => e.Fkseizoen).HasColumnName("FKSeizoen");
            entity.Property(e => e.Fktrack).HasColumnName("FKTrack");
            entity.Property(e => e.Fkuser).HasColumnName("FKUser");

            entity.HasOne(d => d.FkseizoenNavigation).WithMany(p => p.Raceweekends)
                .HasForeignKey(d => d.Fkseizoen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("raceweekend_ibfk_1");

            entity.HasOne(d => d.FktrackNavigation).WithMany(p => p.Raceweekends)
                .HasForeignKey(d => d.Fktrack)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("raceweekend_ibfk_2");

            entity.HasOne(d => d.FkuserNavigation).WithMany(p => p.Raceweekends)
                .HasForeignKey(d => d.Fkuser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("raceweekend_ibfk_3");
        });

        modelBuilder.Entity<Raceweekendhasdriver>(entity =>
        {
            entity.HasKey(e => e.IdraceWeekendHasDriver).HasName("PRIMARY");

            entity.ToTable("raceweekendhasdriver");

            entity.HasIndex(e => e.Fkdriver, "FKDriver");

            entity.HasIndex(e => e.FkraceWeekend, "FKRaceWeekend");

            entity.Property(e => e.IdraceWeekendHasDriver).HasColumnName("IDRaceWeekendHasDriver");
            entity.Property(e => e.Fkdriver).HasColumnName("FKDriver");
            entity.Property(e => e.FkraceWeekend).HasColumnName("FKRaceWeekend");
            entity.Property(e => e.Positie).HasColumnName("positie");
            entity.Property(e => e.Punten).HasColumnName("punten");

            entity.HasOne(d => d.FkdriverNavigation).WithMany(p => p.Raceweekendhasdrivers)
                .HasForeignKey(d => d.Fkdriver)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("raceweekendhasdriver_ibfk_1");

            entity.HasOne(d => d.FkraceWeekendNavigation).WithMany(p => p.Raceweekendhasdrivers)
                .HasForeignKey(d => d.FkraceWeekend)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("raceweekendhasdriver_ibfk_2");
        });

        modelBuilder.Entity<Seizoen>(entity =>
        {
            entity.HasKey(e => e.Idseizoen).HasName("PRIMARY");

            entity.ToTable("seizoen");

            entity.HasIndex(e => e.Fkuser, "FKUser");

            entity.Property(e => e.Idseizoen).HasColumnName("IDSeizoen");
            entity.Property(e => e.Fkuser).HasColumnName("FKUser");
            entity.Property(e => e.NaamSeizoen).HasMaxLength(64);

            entity.HasOne(d => d.FkuserNavigation).WithMany(p => p.Seizoens)
                .HasForeignKey(d => d.Fkuser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("seizoen_ibfk_1");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Idteam).HasName("PRIMARY");

            entity.ToTable("team");

            entity.Property(e => e.Idteam).HasColumnName("IDTeam");
            entity.Property(e => e.NaamTeam).HasMaxLength(128);
            entity.Property(e => e.NationaliteitTeam).HasMaxLength(64);
        });

        modelBuilder.Entity<Teamhasseizoen>(entity =>
        {
            entity.HasKey(e => e.IdteamHasSeizoen).HasName("PRIMARY");

            entity.ToTable("teamhasseizoen");

            entity.HasIndex(e => e.Fkseizoen, "FKSeizoen");

            entity.HasIndex(e => e.Fkteam, "FKTeam");

            entity.Property(e => e.IdteamHasSeizoen).HasColumnName("IDTeamHasSeizoen");
            entity.Property(e => e.Fkseizoen).HasColumnName("FKSeizoen");
            entity.Property(e => e.Fkteam).HasColumnName("FKTeam");

            entity.HasOne(d => d.FkseizoenNavigation).WithMany(p => p.Teamhasseizoens)
                .HasForeignKey(d => d.Fkseizoen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("teamhasseizoen_ibfk_1");

            entity.HasOne(d => d.FkteamNavigation).WithMany(p => p.Teamhasseizoens)
                .HasForeignKey(d => d.Fkteam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("teamhasseizoen_ibfk_2");
        });

        modelBuilder.Entity<Track>(entity =>
        {
            entity.HasKey(e => e.Idtrack).HasName("PRIMARY");

            entity.ToTable("track");

            entity.Property(e => e.Idtrack).HasColumnName("IDtrack");
            entity.Property(e => e.LandTrack).HasMaxLength(64);
            entity.Property(e => e.NaamTrack).HasMaxLength(64);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Iduser).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.Fkteam, "FKTeam");

            entity.HasIndex(e => e.NameUser, "nameUser").IsUnique();

            entity.Property(e => e.Iduser).HasColumnName("IDUser");
            entity.Property(e => e.Fkteam).HasColumnName("FKTeam");
            entity.Property(e => e.NameUser)
                .HasMaxLength(60)
                .HasColumnName("nameUser");
            entity.Property(e => e.PassWordUser)
                .HasMaxLength(60)
                .HasColumnName("passWordUser");

            entity.HasOne(d => d.FkteamNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Fkteam)
                .HasConstraintName("user_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
