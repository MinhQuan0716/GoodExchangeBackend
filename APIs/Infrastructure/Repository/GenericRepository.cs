using Application.Common;
using Application.InterfaceRepository;
using Application.InterfaceService;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        protected DbSet<TEntity> _dbSet;
        private AppDbContext appDbContext;
        private readonly IClaimService _claimService;
        private readonly ICurrentTime _currentTime;
        public GenericRepository(AppDbContext appDbContext, IClaimService claimService, ICurrentTime currentTime)
        {
            _dbSet = appDbContext.Set<TEntity>();
            _claimService = claimService;
            _currentTime = currentTime;
        }

        public GenericRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task AddAsync(TEntity entity)
        {
            entity.CreatedBy = _claimService.GetCurrentUserId;
            entity.CreationDate =_currentTime.GetCurrentTime();
            entity.IsDelete = false;
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedBy = _claimService.GetCurrentUserId;
                entity.CreationDate = _currentTime.GetCurrentTime();
                entity.IsDelete = false;
            }
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
        {
            return await includes
         .Aggregate(_dbSet.AsQueryable(),
             (entity, property) => entity.Include(property))
         .Where(expression).Where(x => x.IsDelete == false).ToListAsync();
        }

        public async Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            return await includes
           .Aggregate(_dbSet.AsQueryable(),
               (entity, property) => entity.Include(property))
           .Where(x => x.IsDelete == false)
           .ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await this.GetByIdAsync(id, Array.Empty<Expression<Func<TEntity, object>>>());
        }

        public async Task<TEntity?> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes)
        {
            return await includes
            .Aggregate(_dbSet.AsQueryable(),
                (entity, property) => entity.Include(property))
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(id) && x.IsDelete == false);
        }


        public void SoftRemove(TEntity entity)
        {
            entity.IsDelete = true;
            entity.DeletedBy = _claimService.GetCurrentUserId;
            entity.DeletetionDate = DateTime.Now;
            _dbSet.Update(entity);
        }

        public void SoftRemoveRange(List<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                entity.IsDelete = false;
                entity.DeletedBy = _claimService.GetCurrentUserId;
                entity.DeletetionDate = _currentTime.GetCurrentTime();
            }
            _dbSet.UpdateRange(entities);
        }

        public void Update(TEntity entity)
        {
            entity.ModificationBy = _claimService.GetCurrentUserId;
            entity.ModificationDate = _currentTime.GetCurrentTime();
            _dbSet.Update(entity);
        }

        public void UpdateRange(List<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {

                entity.ModificationBy = _claimService.GetCurrentUserId;
                entity.ModificationDate = _currentTime.GetCurrentTime();
            }

            _dbSet.UpdateRange(entities);
        }
        public Task<Pagination<TEntity>> ToPagination(int pageIndex = 0, int pageSize = 10)
            => ToPagination(x => true, pageIndex, pageSize);
        public Task<Pagination<TEntity>> ToPagination(Expression<Func<TEntity, bool>> expression, int pageIndex = 0, int pageSize = 10)
          => ToPagination(_dbSet, expression, pageIndex, pageSize);

        public async Task<Pagination<TEntity>> ToPagination(IQueryable<TEntity> value, Expression<Func<TEntity, bool>> expression, int pageIndex, int pageSize)
        {
            var itemCount = await value.Where(expression).CountAsync();
            var items = await value.Where(expression)
                                    .OrderBy(x => x.CreationDate)
                                    .Skip(pageIndex * pageSize)
                                    .Take(pageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<TEntity>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = itemCount,
                Items = items,
            };
            return result;
        }
    }
}

