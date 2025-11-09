using BTManagement.Core;
using BTManagement.Core.Entities;
using BTManagement.Core.Entities.Guide;
using BTManagement.Core.Entities.Inventory;
using BTManagement.Core.Entities.Purchase;
using BTManagement.Core.Entities.User;
using BTManagement.Core.Entities.WorkDone;
using BTManagement.Core.Logs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace BTManagement.Data.DataContext
{
    public class DatabaseContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        public DatabaseContext(DbContextOptions<DatabaseContext> options, ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        // Inventory
        public DbSet<Products> Products { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Departments> Departments { get; set; }

        // Logs
        public DbSet<AuditLog> AuditLogs { get; set; }

        // Guide
        public DbSet<MyGuide> MyGuide { get; set; }

        // Purchases
        public DbSet<Purchases> Purchases { get; set; }
        public DbSet<PurchaseType> PurchaseTypes { get; set; }
        public DbSet<PurchaseKind> PurchaseKinds { get; set; }
        public DbSet<PurchaseForm> PurchaseForms { get; set; }
        public DbSet<FirmKind> FirmKinds { get; set; }
        public DbSet<Firm> Firms { get; set; }

        // WorksDone
        public DbSet<WorksDone> WorksDone { get; set; }
        public DbSet<Images> Images { get; set; }

        // Users
        public DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); // çalışan dll içinden configuration class ları bul
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = OnBeforeSaveChanges();
            AddAuditInfo();
            var result = await base.SaveChangesAsync(cancellationToken);

            if (auditEntries.Any())
            {
                await AuditLogs.AddRangeAsync(auditEntries);
                await base.SaveChangesAsync(cancellationToken);
            }

            return result;
        }

        private List<AuditLog> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditLog>();
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is not AuditLog &&
                            (e.State == EntityState.Added ||
                             e.State == EntityState.Modified ||
                             e.State == EntityState.Deleted));

            foreach (var entry in entries)
            {
                var audit = new AuditLog
                {
                    TableName = entry.Metadata.GetTableName(),
                    ChangedAt = DateTime.UtcNow,
                    UserName = "system", // burada o anki kullanıcıyı inject edebilirsin
                    Action = entry.State.ToString()
                };

                // Primary key
                var keyValues = new Dictionary<string, object>();
                foreach (var property in entry.Properties.Where(p => p.Metadata.IsPrimaryKey()))
                {
                    keyValues[property.Metadata.Name] = property.CurrentValue;
                }
                audit.KeyValues = System.Text.Json.JsonSerializer.Serialize(keyValues);

                // Added
                if (entry.State == EntityState.Added)
                {
                    var newValues = new Dictionary<string, object>();
                    foreach (var property in entry.Properties)
                    {
                        newValues[property.Metadata.Name] = property.CurrentValue;
                    }
                    audit.NewValues = System.Text.Json.JsonSerializer.Serialize(newValues);
                }
                // Deleted
                else if (entry.State == EntityState.Deleted)
                {
                    var oldValues = new Dictionary<string, object>();
                    foreach (var property in entry.Properties)
                    {
                        oldValues[property.Metadata.Name] = property.OriginalValue;
                    }
                    audit.OldValues = System.Text.Json.JsonSerializer.Serialize(oldValues);
                }
                // Modified
                else if (entry.State == EntityState.Modified)
                {
                    var oldValues = new Dictionary<string, object>();
                    var newValues = new Dictionary<string, object>();
                    foreach (var property in entry.Properties)
                    {
                        if (property.IsModified)
                        {
                            oldValues[property.Metadata.Name] = property.OriginalValue;
                            newValues[property.Metadata.Name] = property.CurrentValue;
                        }
                    }
                    audit.OldValues = System.Text.Json.JsonSerializer.Serialize(oldValues);
                    audit.NewValues = System.Text.Json.JsonSerializer.Serialize(newValues);
                }

                auditEntries.Add(audit);
            }

            return auditEntries;
        }

        private void AddAuditInfo()
        {
            var username = _currentUserService.Username ?? "system";

            foreach (var entry in ChangeTracker.Entries<CommonEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Created = username;
                    entry.Entity.CreatedDate = DateTime.Now;
                }
            }
        }
    }
}
