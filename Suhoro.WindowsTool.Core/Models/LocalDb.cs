using Microsoft.EntityFrameworkCore;
using Suhoro.WindowsTool.Core.Interfaces;
using Suhoro.WindowsTool.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suhoro.WindowsTool.Core.Models
{
    public class LocalDb : DbContext
    {
        public LocalDb() { }
        public LocalDb(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var assemblies = CommonVariables.ProjectAssemblies;
            var types = assemblies.SelectMany(a => a.GetTypes())
                .Where(t=>t.IsAssignableTo(typeof(IDbEntity))&&!t.IsAbstract);
            foreach (var type in types)
            {
                modelBuilder.Model.AddEntityType(type);
            }
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            var list = this.ChangeTracker.Entries();
            var now=DateTime.UtcNow;
            foreach (var entry in list)
            {
                var entity = entry.Entity as IDbEntity;
                switch (entry.State)
                {
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Deleted:
                        break;
                    case EntityState.Modified:
                        entity.UpdateTime = now;
                        break;
                    case EntityState.Added:
                        entity.Id = Guid.NewGuid();
                        entity.CreateTime = now;
                        entity.IsDeleted = false;
                        break;
                    default:
                        break;
                }
            }

            return base.SaveChanges();
        }
    }
}
