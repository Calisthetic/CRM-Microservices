using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UsersAPI.Models;

public partial class CrmContext : DbContext
{
    public CrmContext()
    {
    }

    public CrmContext(DbContextOptions<CrmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Division> Divisions { get; set; }

    public virtual DbSet<LikeType> LikeTypes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<ProfileImage> ProfileImages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersLike> UsersLikes { get; set; }

    public virtual DbSet<UsersTimeOff> UsersTimeOffs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-GJJERNN;Initial Catalog=CRM;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.Property(e => e.CompanyName).HasMaxLength(40);
        });

        modelBuilder.Entity<Division>(entity =>
        {
            entity.Property(e => e.DivisionName).HasMaxLength(40);

            entity.HasOne(d => d.UpperDivision).WithMany(p => p.InverseUpperDivision)
                .HasForeignKey(d => d.UpperDivisionId)
                .HasConstraintName("FK_Divisions_Divisions");
        });

        modelBuilder.Entity<LikeType>(entity =>
        {
            entity.Property(e => e.LikeTypeName).HasMaxLength(24);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(e => e.ActionLink).HasMaxLength(128);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.NotificationText)
                .HasMaxLength(128)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Notifications_Users");
        });

        modelBuilder.Entity<ProfileImage>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.ProfileImages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProfileImages_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleName).HasMaxLength(40);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("date");
            entity.Property(e => e.Email).HasMaxLength(20);
            entity.Property(e => e.FirstName).HasMaxLength(20);
            entity.Property(e => e.Login).HasMaxLength(20);
            entity.Property(e => e.Password).HasMaxLength(20);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.SecondName).HasMaxLength(20);
            entity.Property(e => e.ThirdName).HasMaxLength(20);
            entity.Property(e => e.VacationCount).HasDefaultValueSql("((28))");

            entity.HasOne(d => d.Company).WithMany(p => p.Users)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Companies");

            entity.HasOne(d => d.Division).WithMany(p => p.Users)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Divisions");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<UsersLike>(entity =>
        {
            entity.HasKey(e => e.UserLikeId);

            entity.HasOne(d => d.CreatorUser).WithMany(p => p.UsersLikeCreatorUsers)
                .HasForeignKey(d => d.CreatorUserId)
                .HasConstraintName("FK_UsersLikes_Users1");

            entity.HasOne(d => d.LikeType).WithMany(p => p.UsersLikes)
                .HasForeignKey(d => d.LikeTypeId)
                .HasConstraintName("FK_UsersLikes_LikeTypes");

            entity.HasOne(d => d.LikedUser).WithMany(p => p.UsersLikeLikedUsers)
                .HasForeignKey(d => d.LikedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsersLikes_Users");
        });

        modelBuilder.Entity<UsersTimeOff>(entity =>
        {
            entity.HasKey(e => e.UserTimeOffId);

            entity.Property(e => e.EndTimeOff).HasColumnType("date");
            entity.Property(e => e.StartTimeOff).HasColumnType("date");
            entity.Property(e => e.Status).HasMaxLength(40);

            entity.HasOne(d => d.User).WithMany(p => p.UsersTimeOffs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UsersTimeOffs_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
