using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DB_lab2;

public partial class DbLab2Context : DbContext
{
    public DbLab2Context()
    {
    }

    public DbLab2Context(DbContextOptions<DbLab2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Contest> Contests { get; set; }

    public virtual DbSet<ContestProblem> ContestProblems { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupContest> GroupContests { get; set; }

    public virtual DbSet<GroupUser> GroupUsers { get; set; }

    public virtual DbSet<Problem> Problems { get; set; }

    public virtual DbSet<ProblemTag> ProblemTags { get; set; }

    public virtual DbSet<Submission> Submissions { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contest>(entity =>
        {
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Contests)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Contests_Users");
        });

        modelBuilder.Entity<ContestProblem>(entity =>
        {
            entity.HasKey(cp => new { cp.ContestId, cp.ProblemId });

            entity.HasOne(d => d.Contest).WithMany()
                .HasForeignKey(d => d.ContestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContestProblems_Contests");

            entity.HasOne(d => d.Problem).WithMany()
                .HasForeignKey(d => d.ProblemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContestProblems_Problems");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Groups_Users");
        });

        modelBuilder.Entity<GroupContest>(entity =>
        {
            entity.HasKey(gc => new { gc.ContestId, gc.GroupId });

            entity.HasOne(d => d.Contest).WithMany()
                .HasForeignKey(d => d.ContestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupContests_Contests");

            entity.HasOne(d => d.Group).WithMany()
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupContests_Groups");
        });

        modelBuilder.Entity<GroupUser>(entity =>
        {
            entity.HasKey(gu => new { gu.GroupId, gu.UserId });

            entity.HasOne(d => d.Group).WithMany()
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupUsers_Groups");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupUsers_Users");
        });

        modelBuilder.Entity<Problem>(entity =>
        {
            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Problems)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Problems_Users");
        });

        modelBuilder.Entity<ProblemTag>(entity =>
        {
            entity.HasKey(pt => new { pt.ProblemId, pt.TagId});

            entity.HasOne(d => d.Problem).WithMany()
                .HasForeignKey(d => d.ProblemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProblemTags_Problems");

            entity.HasOne(d => d.Tag).WithMany()
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProblemTags_Tags");
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.SubmittedAt).HasColumnType("datetime");
            entity.Property(e => e.Verdict).HasMaxLength(50);

            entity.HasOne(d => d.Contest).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.ContestId)
                .HasConstraintName("FK_Submissions_Contests");

            entity.HasOne(d => d.Group).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_Submissions_Groups");

            entity.HasOne(d => d.Problem).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.ProblemId)
                .HasConstraintName("FK_Submissions_Problems");

            entity.HasOne(d => d.SubmittedByNavigation).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.SubmittedBy)
                .HasConstraintName("FK_Submissions_Users");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Login).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
