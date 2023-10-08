using Dapper;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq.Expressions;
using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Dapper
{
    public class DapperCUDRepository<TEntity> : ICUDRepository<TEntity, Guid> where TEntity : class, IAggregateRoot, IEntity<Guid>, new()
    {
        protected bool IsLogicalDelete { get; }
        DapperCUDConnectionProvider<TEntity> ConnectionProvider { get; }
        TEntity Sample { get; } = new TEntity();

        public DapperCUDRepository(DapperCUDConnectionProvider<TEntity> connectionProvider)
        {
            ConnectionProvider = connectionProvider;
            IsLogicalDelete = typeof(TEntity)!.GetInterface(typeof(ILogicalDelete)!.FullName ?? "", ignoreCase: true) != null;
        }


        public async Task DeleteManyAsync(IEnumerable<TEntity> entities, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default)
        {
            var query = "";
            var tableName = Sample.GetTableName(unitOfWorkHost?.DbContext as DbContext);

            if (!IsLogicalDelete)
                query = $"delete from {tableName} where Id in (@Id)";
            else
                query = $"update {tableName} set ${nameof(ILogicalDelete.Deleted)} = @value where Id in (@Id)";
            await ExecuteAsync(unitOfWorkHost, query, new { id = entities.Select(m => m.Id) , value = true}, cancellationToken);
            
        }

        public async Task DeleteOneAsync(Guid id, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default)
        {
            var query = "";
            var tableName = Sample.GetTableName(unitOfWorkHost?.DbContext as DbContext);

            if (!IsLogicalDelete)
                query = $"delete from {tableName} where Id in (@Id)";
            else
                query = $"update {tableName} set ${nameof(ILogicalDelete.Deleted)} = @value where Id in (@Id)";
            await ExecuteAsync(unitOfWorkHost, $"delete from {tableName} where Id in (@Id)", new { id, value = true }, cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> InsertManyAsync(IEnumerable<TEntity> entities, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default)
        {
            //await ExecuteAsync(unitOfWorkHost, queries.GetSql(), queries.Param, cancellationToken);
            return entities;
        }

        public async Task<TEntity> InsertOneAsync(TEntity entity, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default)
        {
            await InsertManyAsync(new List<TEntity> { entity }, unitOfWorkHost, cancellationToken);
            return entity;
        }

        public async Task<IEnumerable<TEntity>> UpdateManyAsync(IEnumerable<TEntity> entities, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default, Expression<Func<TEntity, object>>? includes = null)
        {
            //await ExecuteAsync(unitOfWorkHost, queries.GetSql(), queries.Param, cancellationToken);
            return entities;
        }

        public async Task<TEntity> UpdateOneAsync(TEntity entity, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default, Expression<Func<TEntity, object>>? includes = null)
        {
            await UpdateManyAsync(new List<TEntity> { entity }, unitOfWorkHost, cancellationToken);
            return entity;
        }

        private Tuple<IDbConnection, IDbTransaction?> GetConnection(IUnitOfWorkHostInterface? unitOfWorkHost)
        {
            var conn = ConnectionProvider.GetConnection();
            IDbTransaction? tran = null;
            if (unitOfWorkHost != null)
            {
                var ctx = (DbContext)unitOfWorkHost.DbContext;
                tran = ctx.Database.CurrentTransaction?.GetDbTransaction();
            }
            return new Tuple<IDbConnection, IDbTransaction?>(conn, tran);
        }

        private async Task ExecuteAsync(IUnitOfWorkHostInterface? unitOfWorkHost, string query, object? @params, CancellationToken cancellationToken)
        {
            IDbTransaction? tran;
            IDbConnection conn;
            (conn, tran) = GetConnection(unitOfWorkHost);
            var cmd = new CommandDefinition(query, @params, tran, null, null, CommandFlags.Buffered, cancellationToken);

            await conn.ExecuteAsync(cmd);
        }
    }
}
