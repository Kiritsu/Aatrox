using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aatrox.Data.Entities;
using Aatrox.Data.Enums;
using Aatrox.Data.EventArgs;
using Aatrox.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aatrox.Data.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected readonly DbSet<TEntity> Entities;
        protected readonly AatroxDbContext Context;

        protected readonly string Name;

        protected Repository(DbSet<TEntity> entities, AatroxDbContext context, string name)
        {
            Entities = entities;
            Context = context;

            Name = name;
        }

        public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync()
        {
            try
            {
                var entities = await Entities.ToListAsync();

                Context.InvokeEvent(new DatabaseActionEventArgs
                {
                    Type = ActionType.GetAll,
                    Path = $"::/{Name}Repository/{ActionType.GetAll}"
                });

                return entities.AsReadOnly();
            }
            catch (Exception ex)
            {
                Context.InvokeEvent(new DatabaseActionEventArgs
                {
                    Type = ActionType.GetAll,
                    Path = $"::/{Name}Repository/{ActionType.GetAll}",
                    IsErrored = true,
                    Exception = ex
                });

                return new List<TEntity>();
            }
        }

        public virtual async Task<TEntity> GetAsync(ulong id)
        {
            try
            {
                var entity = await Entities.FindAsync(id);

                if (entity is null)
                {
                    return null;
                }

                Context.InvokeEvent(new DatabaseActionEventArgs
                {
                    Type = ActionType.Get,
                    Path = $"::/{Name}Repository/{ActionType.Get}/{id}"
                });

                return entity;
            }
            catch (Exception ex)
            {
                Context.InvokeEvent(new DatabaseActionEventArgs
                {
                    Type = ActionType.Get,
                    Path = $"::/{Name}Repository/{ActionType.Get}/{id}",
                    IsErrored = true,
                    Exception = ex
                });

                return null;
            }
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            try
            {
                var data = (await Entities.AddAsync(entity)).Entity;

                Context.InvokeEvent(new DatabaseActionEventArgs
                {
                    Type = ActionType.Add,
                    Path = $"::/{Name}Repository/{ActionType.Add}/{entity.Id}"
                });

                return data;
            }
            catch (Exception ex)
            {
                Context.InvokeEvent(new DatabaseActionEventArgs
                {
                    Type = ActionType.Add,
                    Path = $"::/{Name}Repository/{ActionType.Add}/{entity.Id}",
                    IsErrored = true,
                    Exception = ex
                });

                return null;
            }
        }

        public virtual Task DeleteAsync(TEntity entity)
        {
            try
            {
                Entities.Remove(entity);

                Context.InvokeEvent(new DatabaseActionEventArgs
                {
                    Type = ActionType.Delete,
                    Path = $"::/{Name}Repository/{ActionType.Delete}/{entity.Id}"
                });
            }
            catch (Exception ex)
            {
                Context.InvokeEvent(new DatabaseActionEventArgs
                {
                    Type = ActionType.Delete,
                    Path = $"::/{Name}Repository/{ActionType.Delete}/{entity.Id}",
                    IsErrored = true,
                    Exception = ex
                });
            }

            return Task.CompletedTask;
        }

        public virtual async Task DeleteAllAsync()
        {
            try
            {
                Entities.RemoveRange(await GetAllAsync());

                Context.InvokeEvent(new DatabaseActionEventArgs
                {
                    Type = ActionType.DeleteAll,
                    Path = $"::/{Name}Repository/{ActionType.DeleteAll}"
                });
            }
            catch (Exception ex)
            {
                Context.InvokeEvent(new DatabaseActionEventArgs
                {
                    Type = ActionType.DeleteAll,
                    Path = $"::/{Name}Repository/{ActionType.DeleteAll}",
                    IsErrored = true,
                    Exception = ex
                });
            }
        }
       
        public virtual Task UpdateAsync(TEntity entity)
        {
            try
            {
                Entities.Update(entity);

                Context.InvokeEvent(new DatabaseActionEventArgs
                {
                    Type = ActionType.Update,
                    Path = $"::/{Name}Repository/{ActionType.Update}/{entity.Id}"
                });
            }
            catch (Exception ex)
            {
                Context.InvokeEvent(new DatabaseActionEventArgs
                {
                    Type = ActionType.Update,
                    Path = $"::/{Name}Repository/{ActionType.Update}/{entity.Id}",
                    IsErrored = true,
                    Exception = ex
                });
            }

            return Task.CompletedTask;
        }
    }
}
