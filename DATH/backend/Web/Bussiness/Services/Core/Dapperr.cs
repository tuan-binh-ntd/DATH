using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Bussiness.Interface.Core;

namespace Bussiness.Services.Core
{
    public class Dapperr : IDapper
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString = "DefaultConnection";

        public Dapperr(IConfiguration config)
        {
            _config = config;
        }
        public static void Dispose()
        {

        }

        public async Task<T> ExecuteScalarAsync<T>(string query, DynamicParameters? param = null, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(_connectionString));
            T res = await db.ExecuteScalarAsync<T>(query, param);
            return res;
        }

        public T Get<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(_connectionString));
            return db.Query<T>(sp, param, commandType: commandType).FirstOrDefault()!;
        }

        public List<T> GetAll<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(_connectionString));
            return db.Query<T>(sp, param, commandType: commandType).ToList();
        }

        public async Task<PaginationResult<TSource>> GetAllAndPaginationAsync<TSource>(string sql, [NotNull] PaginationInput input, DynamicParameters param, CommandType commandType = CommandType.Text)
        {
            if (commandType == CommandType.StoredProcedure)
            {
                throw new ArgumentOutOfRangeException("Not support for stored procedures");
            }
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(_connectionString));

            // count total records with dynamic sql
            int totalRecodes = await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM (" + sql + ") AS CountTable", param);

            // paging with dynamic sql
            var pagedSql = sql + " ORDER BY Id OFFSET (@PageNum - 1)*@PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";

            param.Add("PageNum", input.PageNum);
            param.Add("PageSize", input.PageSize);

            IEnumerable<TSource> result = await db.QueryAsync<TSource>(pagedSql, param);

            PaginationResult<TSource> data = new()
            {
                TotalCount = totalRecodes,
                TotalPage = (int)Math.Ceiling((decimal)totalRecodes / (int)input.PageSize!),
                Content = result.ToList()
            };

            return data;
        }

        public async Task<List<T>> GetAllAsync<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure, CancellationToken cancellationToken = default)
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(_connectionString));
            var query = await db.QueryAsync<T>(sp, param, commandType: commandType);
            return query.ToList();
        }

        public async Task<T> GetAsync<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure, CancellationToken cancellationToken = default)
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(_connectionString));
            var query = await db.QueryAsync<T>(sp, param, commandType: commandType);
            return query.FirstOrDefault()!;
        }

        public DbConnection GetDbConnection()
        {
            return new SqlConnection(_config.GetConnectionString(_connectionString));
        }
    }
}
