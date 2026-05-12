using Microsoft.EntityFrameworkCore;
using UrbanGrid.Core.Entities;

namespace UrbanGrid.Infrastructure.Data;

public class UrbanGridDbContext : DbContext
{
    public UrbanGridDbContext(DbContextOptions<UrbanGridDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<FaultReport> FaultReports => Set<FaultReport>();
    public DbSet<ValidationNote> ValidationNotes => Set<ValidationNote>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<Crew> Crews => Set<Crew>();
    public DbSet<WorkLog> WorkLogs => Set<WorkLog>();
    public DbSet<MaterialUsage> MaterialUsages => Set<MaterialUsage>();
    public DbSet<Inspection> Inspections => Set<Inspection>();
    public DbSet<InspectionPlan> InspectionPlans => Set<InspectionPlan>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(e => {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Role).HasConversion<string>();
        });

        // Asset
        modelBuilder.Entity<Asset>(e => {
            e.HasIndex(a => a.AssetTag).IsUnique();
            e.Property(a => a.Type).HasConversion<string>();
            e.Property(a => a.Status).HasConversion<string>();
        });

        // FaultReport
        modelBuilder.Entity<FaultReport>(e => {
            e.Property(f => f.Status).HasConversion<string>();
            e.HasOne(f => f.Reporter).WithMany(u => u.FaultReports)
                .HasForeignKey(f => f.ReportedBy).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(f => f.Asset).WithMany(a => a.FaultReports)
                .HasForeignKey(f => f.AssetId).OnDelete(DeleteBehavior.Restrict);
        });

        // ValidationNote
        modelBuilder.Entity<ValidationNote>(e => {
            e.HasOne(n => n.FaultReport).WithMany(f => f.ValidationNotes)
                .HasForeignKey(n => n.FaultId).OnDelete(DeleteBehavior.Cascade);
        });

        // WorkOrder
        modelBuilder.Entity<WorkOrder>(e => {
            e.Property(w => w.SourceType).HasConversion<string>();
            e.Property(w => w.Priority).HasConversion<string>();
            e.Property(w => w.Status).HasConversion<string>();
            e.HasOne(w => w.Asset).WithMany(a => a.WorkOrders)
                .HasForeignKey(w => w.AssetId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(w => w.Creator).WithMany()
                .HasForeignKey(w => w.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(w => w.Crew).WithMany(c => c.WorkOrders)
                .HasForeignKey(w => w.AssignedCrewId).OnDelete(DeleteBehavior.SetNull);
        });

        // WorkLog
        modelBuilder.Entity<WorkLog>(e => {
            e.HasOne(l => l.WorkOrder).WithMany(w => w.WorkLogs)
                .HasForeignKey(l => l.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
        });

        // MaterialUsage
        modelBuilder.Entity<MaterialUsage>(e => {
            e.HasOne(m => m.WorkOrder).WithMany(w => w.MaterialUsages)
                .HasForeignKey(m => m.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(m => m.Part).WithMany(p => p.MaterialUsages)
                .HasForeignKey(m => m.PartId).OnDelete(DeleteBehavior.Restrict);
        });

        // Inspection
        modelBuilder.Entity<Inspection>(e => {
            e.HasOne(i => i.Asset).WithMany(a => a.Inspections)
                .HasForeignKey(i => i.AssetId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(i => i.Inspector).WithMany()
                .HasForeignKey(i => i.InspectorId).OnDelete(DeleteBehavior.Restrict);
        });

        // Notification
        modelBuilder.Entity<Notification>(e => {
            e.HasOne(n => n.User).WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // AuditLog (append-only, no cascade)
        modelBuilder.Entity<AuditLog>(e => {
            e.HasKey(a => a.Id);
            e.HasOne(a => a.User).WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.SetNull);
        });

        // ✅ REMOVED: HasData seed block
        // Admin is seeded via DbSeeder.cs instead
        // Reason: HasData requires static data — BCrypt hash is dynamic
    }
}
