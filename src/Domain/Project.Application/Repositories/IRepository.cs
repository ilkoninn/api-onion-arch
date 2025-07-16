using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Repositories
{
    public interface IRepository<T> where T : BaseEntity, IAuditedEntity
    {
        DbSet<T> Table { get; }
    }
}
