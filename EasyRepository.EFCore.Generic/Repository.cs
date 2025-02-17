﻿ 
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using EasyRepository.EFCore.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace EasyRepository.EFCore.Generic
    {
        /// <summary>
        /// This class contains implementations of repository functions
        /// </summary>
        internal sealed class Repository : IRepository
        {
            private readonly DbContext _context;

        
            public Repository(DbContext context)
            {
                this._context = context;
            }

            public TEntity Add<TEntity>(TEntity entity) where TEntity : class
            {
                _context.Set<TEntity>().Add(entity);
                return entity;
            }

            public TEntity Add<TEntity, TPrimaryKey>(TEntity entity) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                entity.CreationDate = DateTime.UtcNow;
                _context.Set<TEntity>().Add(entity);
                return entity;
            }

            public async Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
            {
                await _context.Set<TEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
                return entity;
            }

            public async Task<TEntity> AddAsync<TEntity, TPrimaryKey>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                entity.CreationDate = DateTime.UtcNow;
                await _context.Set<TEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
                return entity;
            }

            public IEnumerable<TEntity> AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
            {
                _context.Set<TEntity>().AddRange(entities);
                return entities;
            }

            public IEnumerable<TEntity> AddRange<TEntity, TPrimaryKey>(IEnumerable<TEntity> entities) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                entities.ToList().ForEach(x => x.CreationDate = DateTime.UtcNow);
                _context.Set<TEntity>().AddRange(entities);
                return entities;
            }

            public async Task<IEnumerable<TEntity>> AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class
            {
                await _context.Set<TEntity>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
                return entities;
            }

            public async Task<IEnumerable<TEntity>> AddRangeAsync<TEntity, TPrimaryKey>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                entities.ToList().ForEach(x => x.CreationDate = DateTime.UtcNow);
                await _context.Set<TEntity>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
                return entities;
            }

            public bool Any<TEntity>(Expression<Func<TEntity, bool>> anyExpression) where TEntity : class
            {
                return _context.Set<TEntity>().Any(anyExpression);
            }

            public async Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> anyExpression, CancellationToken cancellationToken = default) where TEntity : class
            {
                bool result = await _context.Set<TEntity>().AnyAsync(anyExpression, cancellationToken).ConfigureAwait(false);
                return result;
            }

            public int Count<TEntity>() where TEntity : class
            {
                return _context.Set<TEntity>().Count();
            }

            public int Count<TEntity>(Expression<Func<TEntity, bool>> whereExpression) where TEntity : class
            {
                return _context.Set<TEntity>().Where(whereExpression).Count();
            }

            public async Task<int> Count<TEntity>(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default) where TEntity : class
            {
                int count = await _context.Set<TEntity>().Where(whereExpression).CountAsync(cancellationToken).ConfigureAwait(false);
                return count;
            }

            public int Count<TEntity, TFilter>(TFilter filter)
                where TEntity : class
                where TFilter : FilterBase
            {
                return _context.Set<TEntity>().ApplyFilter(filter).Count();
            }

            public async Task<int> CountAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : class
            {
                int count = await _context.Set<TEntity>().CountAsync(cancellationToken).ConfigureAwait(false);
                return count;
            }

            public async Task<int> CountAsync<TEntity, TFilter>(TFilter filter, CancellationToken cancellationToken = default)
                where TEntity : class
                where TFilter : FilterBase
            {
                int count = await _context.Set<TEntity>().ApplyFilter(filter).CountAsync(cancellationToken).ConfigureAwait(false);
                return count;
            }

            public void Complete()
            {
                _context.SaveChanges();
            }

            public async Task CompleteAsync(CancellationToken cancellationToken = default)
            {
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            private Expression<Func<TEntity, bool>> GenerateExpression<TEntity>(object id)
            {
                var type = _context.Model.FindEntityType(typeof(TEntity));
                string pk = type.FindPrimaryKey().Properties.Select(s => s.Name).FirstOrDefault();
                Type pkType = type.FindPrimaryKey().Properties.Select(p => p.ClrType).FirstOrDefault();

                object value = Convert.ChangeType(id, pkType, CultureInfo.InvariantCulture);

                ParameterExpression pe = Expression.Parameter(typeof(TEntity), "entity");
                MemberExpression me = Expression.Property(pe, pk);
                ConstantExpression constant = Expression.Constant(value, pkType);
                BinaryExpression body = Expression.Equal(me, constant);
                Expression<Func<TEntity, bool>> expression = Expression.Lambda<Func<TEntity, bool>>(body, new[] { pe });

                return expression;
            }

            public void HardDelete<TEntity>(TEntity entity) where TEntity : class
            {
                _context.Set<TEntity>().Remove(entity);
            }

            public void HardDelete<TEntity>(object id) where TEntity : class
            {
                var entity = _context.Set<TEntity>().Find(id);
                _context.Set<TEntity>().Remove(entity);
            }

            public void HardDelete<TEntity, TPrimaryKey>(TEntity entity) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                _context.Set<TEntity>().Remove(entity);
            }

            public void HardDelete<TEntity, TPrimaryKey>(TPrimaryKey id) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                var entity = _context.Set<TEntity>().FirstOrDefault(GenerateExpression<TEntity>(id));
                _context.Set<TEntity>().Remove(entity);
            }

            public Task HardDeleteAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
            {
                _context.Set<TEntity>().Remove(entity);
                return Task.CompletedTask;
            }

            public async Task HardDeleteAsync<TEntity>(object id, CancellationToken cancellationToken = default) where TEntity : class
            {
                var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(GenerateExpression<TEntity>(id), cancellationToken).ConfigureAwait(false);
                _context.Set<TEntity>().Remove(entity);
            }

            public Task HardDeleteAsync<TEntity, TPrimaryKey>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                _context.Set<TEntity>().Remove(entity);
                return Task.CompletedTask;
            }

            public async Task HardDeleteAsync<TEntity, TPrimaryKey>(TPrimaryKey id, CancellationToken cancellationToken = default) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(GenerateExpression<TEntity>(id), cancellationToken).ConfigureAwait(false);
                _context.Set<TEntity>().Remove(entity);
            }

            public TEntity Replace<TEntity>(TEntity entity) where TEntity : class
            {
                _context.Entry(entity).State = EntityState.Modified;
                return entity;
            }

            public TEntity Replace<TEntity, TPrimaryKey>(TEntity entity) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                entity.ModificationDate = DateTime.UtcNow;
                _context.Entry(entity).State = EntityState.Modified;
                return entity;
            }

            public Task<TEntity> ReplaceAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
            {
                _context.Entry(entity).State = EntityState.Modified;
                return Task.FromResult(entity);
            }

            public Task<TEntity> ReplaceAsync<TEntity, TPrimaryKey>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                entity.ModificationDate = DateTime.UtcNow;
                _context.Entry(entity).State = EntityState.Modified;
                return Task.FromResult(entity);
            }

            public void SoftDelete<TEntity, TPrimaryKey>(TEntity entity) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                entity.IsDeleted = true;
                entity.DeletionDate = DateTime.UtcNow;
                Replace<TEntity, TPrimaryKey>(entity);
            }

            public void SoftDelete<TEntity, TPrimaryKey>(TPrimaryKey id) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                var entity = _context.Set<TEntity>().FirstOrDefault(GenerateExpression<TEntity>(id));
                entity.IsDeleted = true;
                entity.DeletionDate = DateTime.UtcNow;
                Replace<TEntity, TPrimaryKey>(entity);
            }

            public async Task SoftDeleteAsync<TEntity, TPrimaryKey>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                entity.IsDeleted = true;
                entity.DeletionDate = DateTime.UtcNow;
                await ReplaceAsync<TEntity, TPrimaryKey>(entity, cancellationToken).ConfigureAwait(false);
            }

            public async Task SoftDeleteAsync<TEntity, TPrimaryKey>(TPrimaryKey id, CancellationToken cancellationToken = default) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(GenerateExpression<TEntity>(id), cancellationToken).ConfigureAwait(false); ;
                entity.IsDeleted = true;
                entity.DeletionDate = DateTime.UtcNow;
                await ReplaceAsync<TEntity, TPrimaryKey>(entity, cancellationToken).ConfigureAwait(false);
            }

            public TEntity Update<TEntity>(TEntity entity) where TEntity : class
            {
                _context.Set<TEntity>().Update(entity);
                return entity;
            }

            public TEntity Update<TEntity, TPrimaryKey>(TEntity entity) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                entity.ModificationDate = DateTime.UtcNow;
                _context.Set<TEntity>().Update(entity);
                return entity;
            }

            public Task<TEntity> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
            {
                _context.Set<TEntity>().Update(entity);
                return Task.FromResult(entity);
            }

            public Task<TEntity> UpdateAsync<TEntity, TPrimaryKey>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                entity.ModificationDate = DateTime.UtcNow;
                _context.Set<TEntity>().Update(entity);
                return Task.FromResult(entity);
            }

            public IEnumerable<TEntity> UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
            {
                _context.Set<TEntity>().UpdateRange(entities);
                return entities;
            }

            public IEnumerable<TEntity> UpdateRange<TEntity, TPrimaryKey>(IEnumerable<TEntity> entities) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                entities.ToList().ForEach(a => a.ModificationDate = DateTime.UtcNow);
                _context.Set<TEntity>().UpdateRange(entities);
                return entities;
            }

            public async Task<IEnumerable<TEntity>> UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class
            {
                _context.Set<TEntity>().UpdateRange(entities);
                return entities;
            }

            public async Task<IEnumerable<TEntity>> UpdateRangeAsync<TEntity, TPrimaryKey>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : EasyBaseEntity<TPrimaryKey>
            {
                entities.ToList().ForEach(a => a.ModificationDate = DateTime.UtcNow);
                _context.Set<TEntity>().UpdateRange(entities);
                return entities;
            }

            public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class
            {
                return _context.Set<TEntity>().AsQueryable();
            }

            public IQueryable<TEntity> GetQueryable<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
            {
                return _context.Set<TEntity>().Where(filter);
            }

            private IQueryable<TEntity> FindQueryable<TEntity>(bool asNoTracking) where TEntity : class
            {
                var queryable = GetQueryable<TEntity>();
                if (asNoTracking)
                {
                    queryable = queryable.AsNoTracking();
                }
                return queryable;
            }

            public List<TEntity> GetMultiple<TEntity>(bool asNoTracking) where TEntity : class
            {
                return FindQueryable<TEntity>(asNoTracking).ToList();
            }

            public async Task<List<TEntity>> GetMultipleAsync<TEntity>(bool asNoTracking, CancellationToken cancellationToken = default) where TEntity : class
            {
                return await FindQueryable<TEntity>(asNoTracking).ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            public List<TProjected> GetMultiple<TEntity, TProjected>(bool asNoTracking, Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class
            {
                return FindQueryable<TEntity>(asNoTracking).Select(projectExpression).ToList();
            }

            public async Task<List<TProjected>> GetMultipleAsync<TEntity, TProjected>(bool asNoTracking, Expression<Func<TEntity, TProjected>> projectExpression, CancellationToken cancellationToken = default) where TEntity : class
            {
                return await FindQueryable<TEntity>(asNoTracking).Select(projectExpression).ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            public List<TEntity> GetMultiple<TEntity>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression) where TEntity : class
            {
                return FindQueryable<TEntity>(asNoTracking).Where(whereExpression).ToList();
            }

            public async Task<List<TEntity>> GetMultipleAsync<TEntity>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default) where TEntity : class
            {
                return await FindQueryable<TEntity>(asNoTracking).Where(whereExpression).ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            public List<TProjected> GetMultiple<TEntity, TProjected>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class
            {
                return FindQueryable<TEntity>(asNoTracking).Where(whereExpression).Select(projectExpression).ToList();
            }

            public async Task<List<TProjected>> GetMultipleAsync<TEntity, TProjected>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProjected>> projectExpression, CancellationToken cancellationToken = default) where TEntity : class
            {
                return await FindQueryable<TEntity>(asNoTracking).Where(whereExpression).Select(projectExpression).ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            public List<TEntity> GetMultiple<TEntity>(bool asNoTracking, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking);
                queryable = includeExpression(queryable);
                return queryable.ToList();
            }

            public async Task<List<TEntity>> GetMultipleAsync<TEntity>(bool asNoTracking, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, CancellationToken cancellationToken = default) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking);
                queryable = includeExpression(queryable);
                return await queryable.ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            public List<TEntity> GetMultiple<TEntity>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(whereExpression);
                queryable = includeExpression(queryable);
                return queryable.ToList();
            }

            public async Task<List<TEntity>> GetMultipleAsync<TEntity>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, CancellationToken cancellationToken = default) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(whereExpression);
                queryable = includeExpression(queryable);
                return await queryable.ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            public List<TProjected> GetMultiple<TEntity, TProjected>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(whereExpression);
                queryable = includeExpression(queryable);
                return queryable.Select(projectExpression).ToList();
            }

            public async Task<List<TProjected>> GetMultipleAsync<TEntity, TProjected>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, Expression<Func<TEntity, TProjected>> projectExpression, CancellationToken cancellationToken = default) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(whereExpression);
                queryable = includeExpression(queryable);
                return await queryable.Select(projectExpression).ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            public List<TEntity> GetMultiple<TEntity, TFilter>(bool asNoTracking, TFilter filter)
                where TEntity : class
                where TFilter : FilterBase
            {
                return FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter).ToList();
            }

            public async Task<List<TEntity>> GetMultipleAsync<TEntity, TFilter>(bool asNoTracking, TFilter filter, CancellationToken cancellationToken = default)
                where TEntity : class
                where TFilter : FilterBase
            {
                return await FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter).ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            public List<TEntity> GetMultiple<TEntity, TFilter>(bool asNoTracking, TFilter filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression)
                where TEntity : class
                where TFilter : FilterBase
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
                queryable = includeExpression(queryable);
                return queryable.ToList();
            }

            public async Task<List<TEntity>> GetMultipleAsync<TEntity, TFilter>(bool asNoTracking, TFilter filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, CancellationToken cancellationToken = default)
                where TEntity : class
                where TFilter : FilterBase
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
                queryable = includeExpression(queryable);
                return await queryable.ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            public List<TProjected> GetMultiple<TEntity, TFilter, TProjected>(bool asNoTracking, TFilter filter, Expression<Func<TEntity, TProjected>> projectExpression)
                where TEntity : class
                where TFilter : FilterBase
            {
                return FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter).Select(projectExpression).ToList();
            }

            public async Task<List<TProjected>> GetMultipleAsync<TEntity, TFilter, TProjected>(bool asNoTracking, TFilter filter, Expression<Func<TEntity, TProjected>> projectExpression, CancellationToken cancellationToken = default)
                where TEntity : class
                where TFilter : FilterBase
            {
                return await FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter).Select(projectExpression).ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            public List<TProjected> GetMultiple<TEntity, TFilter, TProjected>(bool asNoTracking, TFilter filter, Expression<Func<TEntity, TProjected>> projectExpression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression)
                where TEntity : class
                where TFilter : FilterBase
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
                queryable = includeExpression(queryable);
                return queryable.Select(projectExpression).ToList();
            }

            public async Task<List<TProjected>> GetMultipleAsync<TEntity, TFilter, TProjected>(bool asNoTracking, TFilter filter, Expression<Func<TEntity, TProjected>> projectExpression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, CancellationToken cancellationToken = default)
                where TEntity : class
                where TFilter : FilterBase
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
                queryable = includeExpression(queryable);
                return await queryable.Select(projectExpression).ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            public TEntity GetSingle<TEntity>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(whereExpression);
                return queryable.FirstOrDefault();
            }

            public async Task<TEntity> GetSingleAsync<TEntity>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(whereExpression);
                return await queryable.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }

            public TEntity GetSingle<TEntity>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(whereExpression);
                queryable = includeExpression(queryable);
                return queryable.FirstOrDefault();
            }

            public async Task<TEntity> GetSingleAsync<TEntity>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, CancellationToken cancellationToken = default) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(whereExpression);
                queryable = includeExpression(queryable);
                return await queryable.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }

            public TProjected GetSingle<TEntity, TProjected>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class
            {
                return FindQueryable<TEntity>(asNoTracking).Where(whereExpression).Select(projectExpression).FirstOrDefault();
            }

            public async Task<TProjected> GetSingleAsync<TEntity, TProjected>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProjected>> projectExpression, CancellationToken cancellationToken = default) where TEntity : class
            {
                return await FindQueryable<TEntity>(asNoTracking).Where(whereExpression).Select(projectExpression).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }

            public TProjected GetSingle<TEntity, TProjected>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProjected>> projectExpression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(whereExpression);
                queryable = includeExpression(queryable);
                return queryable.Select(projectExpression).FirstOrDefault();
            }

            public async Task<TProjected> GetSingleAsync<TEntity, TProjected>(bool asNoTracking, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProjected>> projectExpression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, CancellationToken cancellationToken = default) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(whereExpression);
                queryable = includeExpression(queryable);
                return await queryable.Select(projectExpression).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }

            public TEntity GetSingle<TEntity, TFilter>(bool asNoTracking, TFilter filter)
                where TEntity : class
                where TFilter : FilterBase
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
                return queryable.FirstOrDefault();
            }

            public async Task<TEntity> GetSingleAsync<TEntity, TFilter>(bool asNoTracking, TFilter filter, CancellationToken cancellationToken = default)
                where TEntity : class
                where TFilter : FilterBase
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
                return await queryable.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }

            public TEntity GetSingle<TEntity, TFilter>(bool asNoTracking, TFilter filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression)
                where TEntity : class
                where TFilter : FilterBase
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
                queryable = includeExpression(queryable);
                return queryable.FirstOrDefault();
            }

            public async Task<TEntity> GetSingleAsync<TEntity, TFilter>(bool asNoTracking, TFilter filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, CancellationToken cancellationToken = default)
                where TEntity : class
                where TFilter : FilterBase
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
                queryable = includeExpression(queryable);
                return await queryable.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }

            public TProjected GetSingle<TEntity, TProjected, TFilter>(bool asNoTracking, TFilter filter, Expression<Func<TEntity, TProjected>> projectExpression)
                where TEntity : class
                where TFilter : FilterBase
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
                return queryable.Select(projectExpression).FirstOrDefault();
            }

            public async Task<TProjected> GetSingleAsync<TEntity, TProjected, TFilter>(bool asNoTracking, TFilter filter, Expression<Func<TEntity, TProjected>> projectExpression, CancellationToken cancellationToken = default)
                where TEntity : class
                where TFilter : FilterBase
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
                return await queryable.Select(projectExpression).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }

            public TProjected GetSingle<TEntity, TProjected, TFilter>(bool asNoTracking, TFilter filter, Expression<Func<TEntity, TProjected>> projectExpression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression)
                where TEntity : class
                where TFilter : FilterBase
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
                queryable = includeExpression(queryable);
                return queryable.Select(projectExpression).FirstOrDefault();
            }

            public async Task<TProjected> GetSingleAsync<TEntity, TProjected, TFilter>(bool asNoTracking, TFilter filter, Expression<Func<TEntity, TProjected>> projectExpression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, CancellationToken cancellationToken = default)
                where TEntity : class
                where TFilter : FilterBase
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).ApplyFilter(filter);
                queryable = includeExpression(queryable);
                return await queryable.Select(projectExpression).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }

            public TEntity GetById<TEntity>(bool asNoTracking, object id) where TEntity : class
            {
                return _context.Set<TEntity>().FirstOrDefault(GenerateExpression<TEntity>(id));
            }

            public async Task<TEntity> GetByIdAsync<TEntity>(bool asNoTracking, object id, CancellationToken cancellationToken = default) where TEntity : class
            {
                return await FindQueryable<TEntity>(asNoTracking).FirstOrDefaultAsync(GenerateExpression<TEntity>(id), cancellationToken).ConfigureAwait(false);
            }

            public TEntity GetById<TEntity>(bool asNoTracking, object id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(GenerateExpression<TEntity>(id));
                queryable = includeExpression(queryable);
                return queryable.FirstOrDefault();
            }

            public async Task<TEntity> GetByIdAsync<TEntity>(bool asNoTracking, object id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, CancellationToken cancellationToken = default) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(GenerateExpression<TEntity>(id));
                queryable = includeExpression(queryable);
                return await queryable.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }

            public TProjected GetById<TEntity, TProjected>(bool asNoTracking, object id, Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(GenerateExpression<TEntity>(id));
                return queryable.Select(projectExpression).FirstOrDefault();
            }

            public async Task<TProjected> GetByIdAsync<TEntity, TProjected>(bool asNoTracking, object id, Expression<Func<TEntity, TProjected>> projectExpression, CancellationToken cancellationToken = default) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(GenerateExpression<TEntity>(id));
                return await queryable.Select(projectExpression).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }

            public TProjected GetById<TEntity, TProjected>(bool asNoTracking, object id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, Expression<Func<TEntity, TProjected>> projectExpression) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(GenerateExpression<TEntity>(id));
                queryable = includeExpression(queryable);
                return queryable.Select(projectExpression).FirstOrDefault();
            }

            public async Task<TProjected> GetByIdAsync<TEntity, TProjected>(bool asNoTracking, object id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression, Expression<Func<TEntity, TProjected>> projectExpression, CancellationToken cancellationToken = default) where TEntity : class
            {
                var queryable = FindQueryable<TEntity>(asNoTracking).Where(GenerateExpression<TEntity>(id));
                queryable = includeExpression(queryable);
                return await queryable.Select(projectExpression).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
