using System.Linq.Expressions;
using event_horizon_backend.Common.Models;
using event_horizon_backend.Modules.Category.Models;
using event_horizon_backend.Modules.Events.Models;
using event_horizon_backend.Modules.Users.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace event_horizon_backend.Core.Context;

// ORM para la base de datos, hereda de IdentityDbContext para manejar la autenticación y autorización.
public class AppDbContext: IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<EventModel> Events { get; set; } = null!;
    public DbSet<CategoryModel> Categories { get; set; } = null!;
    public DbSet<AssistanceModel> Assistance { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "p");
                var deletedAtProperty = Expression.Property(parameter, nameof(BaseModel.DeletedAt));
                var nullCheck = Expression.Equal(deletedAtProperty, Expression.Constant(null));
                var lambda = Expression.Lambda(nullCheck, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        modelBuilder.Entity<EventModel>().HasQueryFilter(e => e.Active);
        modelBuilder.Entity<CategoryModel>().HasQueryFilter(e => e.Active);
        modelBuilder.Entity<AssistanceModel>().HasQueryFilter(e => e.Active);
    }

    public override int SaveChanges()
    {
        UpdateSoftDeleteStatuses();
        return base.SaveChanges();
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateSoftDeleteStatuses();
        return base.SaveChangesAsync(cancellationToken);
    }
    
    private void UpdateSoftDeleteStatuses()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is BaseModel baseEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        baseEntity.CreatedAt = DateTime.UtcNow;
                        baseEntity.UpdatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        baseEntity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
        }
    }
}