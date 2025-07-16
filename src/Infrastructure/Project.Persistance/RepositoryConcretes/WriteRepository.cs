using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Project.Application.Repositories;
using Project.Core.Entities.Commons;
using Project.Persistance.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Persistance.RepositoryConcretes
{
    public class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity, IAuditedEntity
    {
        private readonly AppDbContext _context;

        public WriteRepository(AppDbContext context)
        {
            _context = context;
        }

        public DbSet<T> Table => _context.Set<T>();

        public async Task<bool> AddAsync(T model)
        {
            EntityEntry<T> entityEntry = await Table.AddAsync(model);
            return entityEntry.State == EntityState.Added;
        }

        public async Task<bool> AddRangeAsync(List<T> datas)
        {
            await Table.AddRangeAsync(datas);
            return true;
        }

        public bool Update(T model)
        {
            EntityEntry entityEntry = Table.Update(model);

            return entityEntry.State == EntityState.Modified;
        }

        public bool Delete(T entity)
        {
            entity.IsDeleted = true;            

            return Update(entity);
        }

        public bool RecoverAsync(T entity)
        {
            entity.IsDeleted = false;

            return Update(entity);
        }

        public bool Remove(T model)
        {
            EntityEntry<T> entityEntry = Table.Remove(model);

            return entityEntry.State == EntityState.Deleted;
        }

        public bool RemoveRange(List<T> datas)
        {
            Table.RemoveRange(datas);

            return true;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            T model = await Table.FirstOrDefaultAsync(data => data.Id == id);

            return Remove(model);
        }
       
        public async Task<int> SaveDatabaseAsync()
            => await _context.SaveChangesAsync();
    }
}
