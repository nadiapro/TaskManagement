using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Data.Context;

public partial class TaskManagementDbContext : IdentityDbContext
{
    public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Models.Task> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.Task>(entity =>
        {
            entity.ToTable("Tasks", t => t.ExcludeFromMigrations());
            
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description);
            entity.Property(e => e.CreatedByUserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.AssignedToUserId).HasMaxLength(450);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())", "DF_Tasks_CreatedAt");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AssignedToUser)
                .WithMany()
                .HasForeignKey(e => e.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
