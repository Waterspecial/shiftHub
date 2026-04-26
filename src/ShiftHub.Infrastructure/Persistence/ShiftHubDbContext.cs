using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShiftHub.Application.Interfaces;
using ShiftHub.Domain.Entities;

namespace ShiftHub.Infrastructure.Persistence;

public class ShiftHubDbContext : DbContext
{
    private readonly ICurrentTenantService _tenant;

    public ShiftHubDbContext(DbContextOptions<ShiftHubDbContext> options, ICurrentTenantService tenant)
        : base(options)
    {
        _tenant = tenant;
    }

    public DbSet<Organisation> Organisations => Set<Organisation>();
    public DbSet<User> Users => Set<User>();
    public DbSet<OrgMembership> OrgMemberships => Set<OrgMembership>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<PayRate> PayRates => Set<PayRate>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<ShiftAssignment> ShiftAssignments => Set<ShiftAssignment>();
    public DbSet<Timesheet> Timesheets => Set<Timesheet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // multi-tenant global query filters — scopes all queries to the active org
        modelBuilder.Entity<Client>().HasQueryFilter(c => c.OrgId == _tenant.OrgId);
        modelBuilder.Entity<Site>().HasQueryFilter(s => s.Client.OrgId == _tenant.OrgId);
        modelBuilder.Entity<PayRate>().HasQueryFilter(p => p.OrgId == _tenant.OrgId);
        modelBuilder.Entity<Shift>().HasQueryFilter(s => s.OrgId == _tenant.OrgId);
        modelBuilder.Entity<ShiftAssignment>().HasQueryFilter(a => a.Shift.OrgId == _tenant.OrgId);
        modelBuilder.Entity<Timesheet>().HasQueryFilter(t => t.Assignment.Shift.OrgId == _tenant.OrgId);
        modelBuilder.Entity<OrgMembership>().HasQueryFilter(m => m.OrgId == _tenant.OrgId);

        // explicit FK mappings — EF Core convention expects "OrganisationId" but we use "OrgId"
        modelBuilder.Entity<OrgMembership>()
            .HasOne(m => m.Organisation)
            .WithMany(o => o.Memberships)
            .HasForeignKey(m => m.OrgId);

        modelBuilder.Entity<Client>()
            .HasOne(c => c.Organisation)
            .WithMany(o => o.Clients)
            .HasForeignKey(c => c.OrgId);

        modelBuilder.Entity<PayRate>()
            .HasOne(p => p.Organisation)
            .WithMany(o => o.PayRates)
            .HasForeignKey(p => p.OrgId);

        modelBuilder.Entity<Shift>()
            .HasOne(s => s.Organisation)
            .WithMany(o => o.Shifts)
            .HasForeignKey(s => s.OrgId);

        modelBuilder.Entity<Site>()
            .HasOne(s => s.Client)
            .WithMany(c => c.Sites)
            .HasForeignKey(s => s.ClientId);

        modelBuilder.Entity<ShiftAssignment>()
            .HasOne(a => a.Shift)
            .WithMany(s => s.Assignments)
            .HasForeignKey(a => a.ShiftId);

        modelBuilder.Entity<ShiftAssignment>()
            .HasOne(a => a.User)
            .WithMany(u => u.Assignments)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<Timesheet>()
            .HasOne(t => t.Assignment)
            .WithOne(a => a.Timesheet)
            .HasForeignKey<Timesheet>(t => t.AssignmentId);

        // store string[] as a PostgreSQL text array
        modelBuilder.Entity<User>()
            .Property(u => u.Qualifications)
            .HasColumnType("text[]");

        // store Organisation.Settings as jsonb
        modelBuilder.Entity<Organisation>()
            .Property(o => o.Settings)
            .HasColumnType("jsonb");
    }
}
