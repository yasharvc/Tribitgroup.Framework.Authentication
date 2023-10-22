using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tribitgroup.Framework.Shared.Enums;
using Tribitgroup.Framework.Shared.Extensions;
using Tribitgroup.Framework.Shared.Interfaces;
using Tribitgroup.Framework.Shared.Interfaces.Entity;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.EF.Repositories
{
    public abstract class GenericRepository<TEntity, TDbContext, U>
        : ICUDRepository<TEntity, U>//, IQueryRepository<T, U> 
        where TEntity : class, IEntity<U>
        where U : notnull
        where TDbContext : DbContext
    {
        TDbContext DbContext { get; set; }
        DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

        public GenericRepository(TDbContext dbContext) => DbContext = dbContext;

        public async Task DeleteManyAsync(
            IEnumerable<U> entities,
            IUnitOfWorkHostInterface? unitOfWorkHost = null,
            CancellationToken cancellationToken = default)
                => await DoItWithUOWAsync(async (dbContext) =>
                    {
                        DbSet.RemoveRange(await GetItemsByIdsAsync(entities));
                    }, unitOfWorkHost, cancellationToken);

        public async Task DeleteOneAsync(U id, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default) 
            => await DeleteManyAsync(new List<U> { id }, unitOfWorkHost, cancellationToken);

        public async Task<IEnumerable<TEntity>> InsertManyAsync(IEnumerable<TEntity> entities, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default)
        {
            await DoItWithUOWAsync(async (ctx) =>
            {
                await DbContext.AddRangeAsync(entities, cancellationToken);
            }, unitOfWorkHost, cancellationToken);
            return entities;
        }

        public async Task<TEntity> InsertOneAsync(
            TEntity entity, IUnitOfWorkHostInterface? unitOfWorkHost = null, 
            CancellationToken cancellationToken = default) 
            => (await InsertManyAsync(new List<TEntity> { entity }, unitOfWorkHost, cancellationToken)).First();

        public Task<IEnumerable<TEntity>> UpdateManyAsync(IEnumerable<TEntity> entities, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default, Expression<Func<TEntity, object>>? includes = null)
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> UpdateOneAsync(TEntity entity, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default, Expression<Func<TEntity, object>>? includes = null)
            => (await UpdateManyAsync(new List<TEntity> { entity }, unitOfWorkHost, cancellationToken)).First();

        private async Task DoItWithUOWAsync(
            Func<TDbContext, Task> action,
            IUnitOfWorkHostInterface? unitOfWorkHost, 
            CancellationToken cancellationToken = default)
        {
            await ApplyUOW(unitOfWorkHost);
            await action(DbContext);
            await CommitAsync(unitOfWorkHost, cancellationToken);
        }

        private async Task CommitAsync(IUnitOfWorkHostInterface? unitOfWorkHost, CancellationToken cancellationToken = default)
        {
            if (unitOfWorkHost == null) return;
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        private Task ApplyUOW(IUnitOfWorkHostInterface? uow)
        {
            if (uow == null) return Task.CompletedTask;
            DbContext = uow.DbContext as TDbContext ?? throw new InvalidCastException(nameof(DbContext));
            return Task.CompletedTask;
        }

        private async Task<IEnumerable<TEntity>> GetItemsByIdsAsync(IEnumerable<U> entities)
        {
            var res = DbContext.Set<TEntity>().Where(new Condition
            {
                Operator = ConditionOperatorEnum.In,
                PropertyName = nameof(Entity.Id),
                Values = entities.Select(m => m.ToString() ?? "").ToList(),
            });

            return await res.ToListAsync();
        }
    }
    //public abstract class GenericRepository<T, TDbContext, U>
    //    : ICUDRepository<T, U>, IQueryRepository<T, U> where T : class,
    //    IEntity<U>, new() where TDbContext : DbContext where U : struct
    //{
    //    private const string IsNotLogicalDeletedWhereClause = $"{nameof(ILogicalDelete.Deleted)} = false";
    //    private const string IsActiveWhereClause = $"{nameof(IHasActiveStatus.IsActive)} = true";
    //    protected DbContextOptions<TDbContext> DbOptions { get; }
    //    protected uint DefaultMaxCountForSelect { get; set; } = 200;
    //    protected bool IsLogicalDelete { get; }
    //    protected bool IsMultiTenant { get; }
    //    protected bool HasActiveFlag { get; }
    //    protected bool HasAudit { get; }
    //    protected bool IsCachable { get; }
    //    protected string CacheKey { get; } = string.Empty;
    //    protected TimeSpan? CacheTimeout { get; }


    //    IUserInfoProvider? UserInfoProvider { get; }
    //    public GenericRepository(DbContextOptions<TDbContext> options)
    //    {
    //        DbOptions = options;
    //        IsLogicalDelete = typeof(T).GetInterface($"{typeof(ILogicalDelete).FullName}", true) != null;
    //        HasActiveFlag = typeof(T).GetInterface($"{typeof(IHasActiveStatus).FullName}", true) != null;
    //        IsCachable = typeof(T).GetInterface(typeof(ICachableEntity).Name, true) != null;
    //        if (IsCachable)
    //        {
    //            CacheKey = ((ICachableEntity)new T()).GetCacheKey();
    //            CacheTimeout = ((ICachableEntity)new T()).GetExpireTime();
    //        }
    //    }

    //    public GenericRepository(DbContextOptions<TDbContext> options, IUserInfoProvider userInfoProvider) : this(options)
    //    {
    //        HasAudit = typeof(T).GetInterface($"{typeof(IHasCreatedBy).FullName}", true) != null;
    //        IsMultiTenant = typeof(T).GetInterface($"{typeof(IHasTenant).FullName}", true) != null;
    //        UserInfoProvider = userInfoProvider;
    //    }

    //    public virtual async Task<System.Linq.Dynamic.Core.PagedResult<T>> GetAllAsync(
    //         Pagination? pagination = default,
    //        List<Sort>? sorts = default,
    //        bool includeChilds = false,
    //        bool includeInActives = false,
    //        bool includeLogicalDeleted = false,
    //        CancellationToken cancellationToken = default)
    //    {
    //        if (!IsCachable)
    //        {
    //            using var ctx = GetContext();
    //            return await GetWithIncludeFlagAsync(includeChilds: includeChilds, ctx, includeInActives, includeLogicalDeleted)
    //                .Sort(sorts)
    //                .PaginateAsync(pagination, (int)DefaultMaxCountForSelect);
    //        }

    //        var query = await GetCacheableQueryAsync(includeInActives, includeLogicalDeleted, cancellationToken);
    //        return await query
    //            .SortBy(sorts)
    //            .PaginateAsync(pagination, (int)DefaultMaxCountForSelect);
    //    }

    //    public virtual async Task<System.Linq.Dynamic.Core.PagedResult<T>> WhereAsync(
    //       Expression<Func<T, bool>>? selector = null,
    //        bool includeChilds = false,
    //        Pagination? pagination = null,
    //        List<Sort>? sorts = null,
    //        bool includeInActives = false,
    //        bool includeLogicalDeleted = false,
    //        CancellationToken cancellationToken = default)
    //    {
    //        if (!IsCachable)
    //        {
    //            using var ctx = GetContext();
    //            return await GetWithIncludeFlagAsync(includeChilds, ctx, includeInActives, includeLogicalDeleted)
    //                .Where(selector)
    //                .SortBy(sorts)
    //                .PaginateAsync(pagination, (int)DefaultMaxCountForSelect);
    //        }

    //        var query = await GetCacheableQueryAsync(includeInActives, includeLogicalDeleted, cancellationToken);
    //        return await query
    //            .SortBy(sorts)
    //            .PaginateAsync(pagination, (int)DefaultMaxCountForSelect);
    //    }

    //    public virtual async Task<System.Linq.Dynamic.Core.PagedResult<T>> WhereAsync(
    //       IEnumerable<Condition> conditions,
    //       bool includeChilds = false,
    //       Pagination? pagination = null,
    //       List<Sort>? sorts = null,
    //       bool includeInActives = false,
    //       bool includeLogicalDeleted = false,
    //       CancellationToken cancellationToken = default)
    //    {
    //        if (!IsCachable)
    //        {
    //            using var ctx = GetContext();
    //            return await GetWithIncludeFlagAsync(includeChilds, ctx, includeInActives, includeLogicalDeleted)
    //                .Where(conditions)
    //                .SortBy(sorts)
    //                .PaginateAsync(pagination, (int)DefaultMaxCountForSelect);
    //        }

    //        var query = await GetCacheableQueryAsync(includeInActives, includeLogicalDeleted, cancellationToken);
    //        return await query
    //            .Where(conditions)
    //            .SortBy(sorts)
    //            .PaginateAsync(pagination, (int)DefaultMaxCountForSelect);
    //    }

    //    public virtual async Task<T?> GetByIdAsync(
    //        U id, bool includeChilds = false,
    //        bool includeInActives = false,
    //        bool includeLogicalDeleted = false,
    //        CancellationToken cancellationToken = default)
    //        => await SingleOrDefaultAsync(x => x.Id.Equals(id), includeChilds, includeInActives, includeLogicalDeleted, cancellationToken);

    //    public virtual async Task<T?> FirstOrDefaultAsync(
    //        Expression<Func<T, bool>>? selector = null,
    //        bool includeChilds = false,
    //        bool includeInActives = false,
    //        bool includeLogicalDeleted = false,
    //        CancellationToken cancellationToken = default)
    //    {
    //        if (!IsCachable)
    //        {
    //            using var ctx = GetContext();
    //            var query = GetWithIncludeFlagAsync(includeChilds, ctx, includeInActives, includeLogicalDeleted);
    //            return selector == null
    //                ? await query.FirstOrDefaultAsync(cancellationToken)
    //                : await query.FirstOrDefaultAsync(selector, cancellationToken);
    //        }
    //        else
    //        {
    //            var query = await GetCacheableQueryAsync(includeInActives, includeLogicalDeleted, cancellationToken);
    //            return selector == null
    //                ? await query.FirstOrDefaultAsync(cancellationToken)
    //                : await query.FirstOrDefaultAsync(selector, cancellationToken);
    //        }
    //    }

    //    public virtual async Task<T?> LastOrDefaultAsync(
    //        Expression<Func<T, bool>>? selector = null,
    //        bool includeChilds = false,
    //        bool includeInActives = false,
    //        bool includeLogicalDeleted = false,
    //        CancellationToken cancellationToken = default)
    //    {
    //        if (!IsCachable)
    //        {
    //            using var ctx = GetContext();
    //            var query = GetWithIncludeFlagAsync(includeChilds, ctx, includeInActives, includeLogicalDeleted)
    //                .OrderBy(x => x.Id);
    //            return selector == null
    //                ? await query.LastOrDefaultAsync(cancellationToken)
    //                : await query.LastOrDefaultAsync(selector, cancellationToken);
    //        }
    //        else
    //        {
    //            var query = await GetCacheableQueryAsync(includeInActives, includeLogicalDeleted, cancellationToken);
    //            return selector == null
    //                ? await query.LastOrDefaultAsync(cancellationToken)
    //                : await query.LastOrDefaultAsync(selector, cancellationToken);
    //        }
    //    }

    //    public virtual async Task<T?> SingleOrDefaultAsync(
    //        Expression<Func<T, bool>>? selector = null,
    //        bool includeChilds = false,
    //        bool includeInActives = false,
    //        bool includeLogicalDeleted = false,
    //        CancellationToken cancellationToken = default)
    //    {
    //        if (!IsCachable)
    //        {
    //            using var ctx = GetContext();
    //            var query = GetWithIncludeFlagAsync(includeChilds, ctx, includeInActives, includeLogicalDeleted);
    //            return selector == null
    //                ? await query.SingleOrDefaultAsync(cancellationToken)
    //                : await query.SingleOrDefaultAsync(selector, cancellationToken);
    //        }
    //        else
    //        {
    //            var query = await GetCacheableQueryAsync(includeInActives, includeLogicalDeleted, cancellationToken);
    //            return selector == null
    //                ? await query.SingleOrDefaultAsync(cancellationToken)
    //                : await query.SingleOrDefaultAsync(selector, cancellationToken);
    //        }
    //    }

    //    public virtual async Task<T> SingleAsync(
    //        Expression<Func<T, bool>>? selector = null,
    //        bool includeChilds = false,
    //        bool includeInActives = false,
    //        bool includeLogicalDeleted = false,
    //        CancellationToken cancellationToken = default)
    //    {
    //        if (!IsCachable)
    //        {
    //            using var ctx = GetContext();
    //            var query = GetWithIncludeFlagAsync(includeChilds, ctx, includeInActives, includeLogicalDeleted);
    //            return selector == null
    //                ? await query.SingleAsync(cancellationToken)
    //                : await query.SingleAsync(selector, cancellationToken);
    //        }
    //        else
    //        {
    //            var query = await GetCacheableQueryAsync(includeInActives, includeLogicalDeleted, cancellationToken);
    //            return selector == null
    //                ? await query.SingleAsync(cancellationToken)
    //                : await query.SingleAsync(selector, cancellationToken);
    //        }
    //    }

    //    public virtual async Task<int> CountAsync(
    //        Expression<Func<T, bool>>? selector = null,
    //        bool includeInActives = false,
    //        bool includeLogicalDeleted = false,
    //        CancellationToken cancellationToken = default)
    //    {
    //        if (!IsCachable)
    //        {
    //            using var ctx = GetContext();
    //            return selector == null
    //                ? await GetWithIncludeFlagAsync(includeChilds: false, ctx, includeInActives, includeLogicalDeleted)
    //                    .CountAsync(cancellationToken)
    //                : await GetWithIncludeFlagAsync(includeChilds: false, ctx, includeInActives, includeLogicalDeleted)
    //                    .CountAsync(selector, cancellationToken);
    //        }

    //        var query = await GetCacheableQueryAsync(includeInActives, includeLogicalDeleted, cancellationToken);
    //        return selector == null
    //            ? await query.CountAsync(cancellationToken)
    //            : await query.CountAsync(selector, cancellationToken);
    //    }

    //    public virtual async Task<bool> AnyAsync(
    //        Expression<Func<T, bool>>? selector = null,
    //        bool includeInActives = false,
    //        bool includeLogicalDeleted = false,
    //        CancellationToken cancellationToken = default)
    //    {
    //        if (!IsCachable)
    //        {
    //            using var ctx = GetContext();

    //            return selector == null
    //                ? await GetWithIncludeFlagAsync(includeChilds: false, ctx, includeInActives, includeLogicalDeleted)
    //                    .AnyAsync(cancellationToken)
    //                : await GetWithIncludeFlagAsync(includeChilds: false, ctx, includeInActives, includeLogicalDeleted)
    //                    .AnyAsync(selector, cancellationToken);
    //        }

    //        var query = await GetCacheableQueryAsync(includeInActives, includeLogicalDeleted, cancellationToken);
    //        return selector == null
    //            ? await query.AnyAsync(cancellationToken)
    //            : await query.AnyAsync(selector, cancellationToken);
    //    }

    //    public virtual async Task<T> InsertOneAsync(
    //        T entity,
    //        IUnitOfWorkHostInterface? unitOfWorkHost = null,
    //        CancellationToken cancellationToken = default)
    //    {
    //        var ctx = GetContextWithUOW(unitOfWorkHost);

    //        entity.Id = BasicTypesExtensions.NewId(entity.Id);
    //        SetUpdatedAndCreatedAsync(entity);
    //        await SetIsActiveFlagAsync(entity);

    //        await ctx.Set<T>().AddAsync(entity, cancellationToken);
    //        await ctx.SaveChangesAsync(cancellationToken);

    //        if (unitOfWorkHost != null)
    //        {
    //            unitOfWorkHost.Committed += async (s, e) =>
    //            {
    //                await UpdateCache();
    //            };
    //        }
    //        else
    //        {
    //            await UpdateCache(ctx, cancellationToken);
    //            await DisposeLocalContextAsync(ctx);
    //        }
    //        return entity;
    //    }

    //    public virtual async Task<IEnumerable<T>> InsertManyAsync(
    //        IEnumerable<T> entities,
    //        IUnitOfWorkHostInterface? unitOfWorkHost = null,
    //        CancellationToken cancellationToken = default)
    //    {
    //        var ctx = GetContextWithUOW(unitOfWorkHost);

    //        var createDate = DateTime.UtcNow;
    //        foreach (var entity in entities)
    //        {
    //            entity.Id = BasicTypesExtensions.NewId(entity.Id);
    //            SetUpdatedAndCreatedAsync(entity);
    //            await SetIsActiveFlagAsync(entity);
    //        }

    //        await ctx.Set<T>().AddRangeAsync(entities, cancellationToken);
    //        await ctx.SaveChangesAsync(cancellationToken);

    //        if (unitOfWorkHost != null)
    //        {
    //            unitOfWorkHost.Committed += async (s, e) =>
    //            {
    //                await UpdateCache();
    //            };
    //        }
    //        else
    //        {
    //            await UpdateCache(ctx, cancellationToken);
    //            await DisposeLocalContextAsync(ctx);
    //        }
    //        return entities;
    //    }

    //    public virtual async Task<T> UpdateOneAsync(
    //        T entity,
    //        IUnitOfWorkHostInterface? unitOfWorkHost = null,
    //        CancellationToken cancellationToken = default,
    //         params Expression<Func<T, object>>[] includes)
    //    {
    //        var ctx = GetContextWithUOW(unitOfWorkHost);
    //        var entitySet = ctx.Set<T>().Where(x => x.Id.Equals(entity.Id));
    //        foreach (var child in includes)
    //        {
    //            entitySet = entitySet.Include(child);
    //        }
    //        var existingEntity = await entitySet.SingleOrDefaultAsync();

    //        var existingEntityProperties = existingEntity.GetType().GetProperties();

    //        foreach (var property in existingEntityProperties)
    //        {
    //            property.SetValue(existingEntity, entity.GetType().GetProperty(property.Name).GetValue(entity));
    //        }

    //        SetUpdatedAsync(existingEntity);

    //        ctx.Set<T>().Attach(existingEntity);
    //        ctx.Set<T>().Update(existingEntity);

    //        await ctx.SaveChangesAsync(cancellationToken);

    //        if (unitOfWorkHost != null)
    //        {
    //            unitOfWorkHost.Committed += async (s, e) =>
    //            {
    //                await UpdateCache();
    //            };
    //        }
    //        else
    //        {
    //            await UpdateCache(ctx, cancellationToken);
    //            await DisposeLocalContextAsync(ctx);
    //        }

    //        return entity;
    //    }

    //    public virtual async Task<IEnumerable<T>> UpdateManyAsync(
    //        IEnumerable<T> entities,
    //        IUnitOfWorkHostInterface? unitOfWorkHost = null,
    //        Expression<Func<T, object>>? includes = null,
    //        CancellationToken cancellationToken = default)
    //    {
    //        var ctx = GetContextWithUOW(unitOfWorkHost);

    //        var updateDate = DateTime.UtcNow;
    //        foreach (var entity in entities)
    //        {
    //            var existingEntity = includes != null
    //                ? await ctx.Set<T>().Where(x => x.Id.Equals(entity.Id)).Include(includes).SingleOrDefaultAsync()
    //                : await ctx.Set<T>().Where(x => x.Id.Equals(entity.Id)).SingleOrDefaultAsync();

    //            var existingEntityProperties = existingEntity.GetType().GetProperties();

    //            foreach (var property in existingEntityProperties)
    //            {
    //                property.SetValue(existingEntity, entity.GetType().GetProperty(property.Name).GetValue(entity));
    //            }

    //            SetUpdatedAsync(entity);

    //            ctx.Set<T>().AttachRange(existingEntity);
    //            ctx.Set<T>().UpdateRange(existingEntity);
    //            await ctx.SaveChangesAsync(cancellationToken);
    //        }

    //        if (unitOfWorkHost != null)
    //        {
    //            unitOfWorkHost.Committed += async (s, e) =>
    //            {
    //                await UpdateCache();
    //            };
    //        }
    //        else
    //        {
    //            await UpdateCache(ctx, cancellationToken);
    //            await DisposeLocalContextAsync(ctx);
    //        }
    //        return entities;
    //    }

    //    public virtual async Task DeleteOneAsync(
    //        T item,
    //        IUnitOfWorkHostInterface? unitOfWorkHost = null,
    //        CancellationToken cancellationToken = default) 
    //        => await DeleteManyAsync(new List<T> { item }, unitOfWorkHost, cancellationToken);

    //    public virtual async Task DeleteManyAsync(
    //        IEnumerable<T> entities,
    //        IUnitOfWorkHostInterface? unitOfWorkHost = null,
    //        CancellationToken cancellationToken = default)
    //    {
    //        var ctx = GetContextWithUOW(unitOfWorkHost);

    //        if (IsLogicalDelete)
    //        {
    //            foreach (var item in entities)
    //            {
    //                var temp = (item as ILogicalDelete) ?? throw new InvalidCastException();
    //                await temp.DeleteAsync();
    //                ctx.Set<T>().Attach(item);
    //                ctx.Set<T>().Update(item);
    //            }
    //        }
    //        else
    //        {
    //            ctx.Set<T>().RemoveRange(entities);
    //        }

    //        await ctx.SaveChangesAsync(cancellationToken);

    //        if (unitOfWorkHost != null)
    //        {
    //            unitOfWorkHost.Committed += async (s, e) =>
    //            {
    //                await UpdateCache();
    //            };
    //        }
    //        else
    //        {
    //            await UpdateCache(ctx, cancellationToken);
    //            await DisposeLocalContextAsync(ctx);
    //        }
    //    }

    //    public void SetMaxSelectCount(uint count) => DefaultMaxCountForSelect = count;

    //    private async Task<Guid> GetUserId()
    //    {
    //        var temp = await UserInfoProvider?.GetCurrentUserAsync() ?? null;
    //        return temp?.UserId ?? Guid.Empty;
    //    }

    //    private async Task SetUpdatedAndCreatedAsync(T entity)
    //    {
    //        var utcNow = DateTime.UtcNow;
    //        if (entity is IHasCreatedAt hasCreated)
    //        {
    //            hasCreated.CreatedAt = utcNow;
    //        }
    //        if (entity is IHasUpdatedAt hasUpdated)
    //        {
    //            hasUpdated.UpdatedAt = utcNow;
    //        }
    //        if (entity is IHasCreatedBy hasCreatedBy)
    //        {
    //            hasCreatedBy.CreatedBy = await GetUserId();
    //        }
    //        if (entity is IHasUpdatedBy hasUpdatedBy)
    //        {
    //            hasUpdatedBy.UpdatedBy = await GetUserId();
    //        }
    //    }

    //    private async Task SetIsActiveFlagAsync(T entity)
    //    {
    //        if (entity is IHasActiveStatus isActive)
    //        {
    //            await isActive.Activate();
    //        }
    //    }

    //    private async Task SetUpdatedAsync(T entity)
    //    {
    //        var utcNow = DateTime.UtcNow;
    //        if (entity is IHasUpdatedAt hasUpdated)
    //        {
    //            hasUpdated.UpdatedAt = utcNow;
    //        }
    //        if (entity is IHasUpdatedBy hasUpdatedBy)
    //        {
    //            hasUpdatedBy.UpdatedBy = await GetUserId();
    //        }
    //    }

    //    private async Task UpdateCache(
    //        TDbContext? dbContext = null,
    //        CancellationToken cancellationToken = default)
    //        => await FillCache(dbContext, cancellationToken);

    //    private async Task<string> GetCachedData(CancellationToken cancellationToken)
    //    {
    //        //var json = await CacheDb.StringGetAsync(CacheKey);
    //        //if (string.IsNullOrEmpty(json))
    //        //{
    //        //    await FillCache(cancellationToken: cancellationToken);
    //        //    json = await CacheDb.StringGetAsync(CacheKey);
    //        //}

    //        //return json.ToString();
    //        return "";
    //    }

    //    private async Task FillCache(TDbContext? dbContext = null, CancellationToken cancellationToken = default)
    //    {
    //        //if (!IsCachable) return;
    //        //var ctx = dbContext ?? GetContext();
    //        //var serializedData = JsonSerializer.Serialize(await ctx.Set<T>().ToListAsync(cancellationToken));
    //        //await CacheDb.StringSetAsync(CacheKey, serializedData, CacheTimeout);
    //    }

    //    public async Task ResetCacheAsync()
    //    {
    //        await FillCache();
    //    }

    //    protected abstract IQueryable<T> GetDbSetWithIncludes(TDbContext ctx);

    //    protected abstract TDbContext GetContext();

    //    private static async Task DisposeLocalContextAsync(TDbContext ctx)
    //    {
    //        if (ctx != null)
    //        {
    //            try { await ctx.DisposeAsync(); } catch { }
    //        }
    //    }

    //    private TDbContext GetContextWithUOW(IUnitOfWorkHostInterface? uow)
    //        => uow == null || uow.DbContext == null ? GetContext() : uow.DbContext as TDbContext;

    //    private async Task<IQueryable<T>> ApplyTenantFilterAsync(IQueryable<T> query)
    //    {
    //        //if (!IsMultiTenant)
    //        //    return query;
    //        //if (await UserInfoProvider.GetCurrentTenant() is null)
    //        //    return query;
    //        //if (TenantService.IgnoreTenancy)
    //        //    return query;
    //        //var condition = new Condition
    //        //{
    //        //    PropertyName = nameof(IMultiTenant.TenantId),
    //        //    Operator = Shared.Enums.ConditionOperatorEnum.Equal,
    //        //    Values = new List<string> { TenantService.Tenant.ToString() }
    //        //};
    //        //return query.Where(condition);
    //        return query;
    //    }

    //    private async Task<IQueryable<T>> GetWithIncludeFlagAsync(bool includeChilds, TDbContext ctx, bool includeInActives, bool includeLogicalDeleted)
    //    {
    //        var query = includeChilds ? GetDbSetWithIncludes(ctx) : ctx.Set<T>();

    //        query = query
    //            .WhereIf(HasActiveFlag && !includeInActives, IsActiveWhereClause)
    //            .WhereIf(IsLogicalDelete && !includeLogicalDeleted, IsNotLogicalDeletedWhereClause);
    //        query = await ApplyTenantFilterAsync(query);
    //        return query;
    //    }

    //    private Task<IQueryable<T>> GetCacheableQueryAsync(bool includeInActives, bool includeLogicalDeleted, CancellationToken cancellationToken = default)
    //    {
    //        //var cachedData = await GetCachedData(cancellationToken);
    //        //var lst = JsonSerializer.Deserialize<IEnumerable<T>>(cachedData.ToString());

    //        //var query = lst.AsQueryable()
    //        //    .WhereIf(HasActiveFlag && !includeInActives, IsActiveWhereClause)
    //        //    .WhereIf(IsLogicalDelete && !includeLogicalDeleted, IsNotLogicalDeletedWhereClause);
    //        //query = ApplyTenantFilter(query);
    //        //return query;
    //        throw new NotImplementedException();
    //    }

    //}
}
