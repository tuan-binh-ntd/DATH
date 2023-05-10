using Bussiness.Interface;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Bussiness.Helper;

namespace Bussiness.Services
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

        public async Task<PaginationResult<TSource>> GetAllAndPaginationAsync<TSource>(string sp, [NotNull] PaginationInput input, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(_connectionString));
            var query = await db.QueryAsync<TSource>(sp, param, commandType: commandType);

            int totalCount = query.Count();

            query = query!.Skip((int)input.PageSize! * ((int)input.PageNum! - 1)).Take((int)input.PageSize);

            PaginationResult<TSource> data = new()
            {
                TotalCount = totalCount,
                TotalPage = (int)Math.Ceiling((decimal)totalCount / (int)input.PageSize!),
                Content = query.ToList()
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

        public T Insert<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(_connectionString));
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = db.Query<T>(sp, param, commandType: commandType, transaction: tran).FirstOrDefault()!;
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }

            return result!;
        }

        public T Update<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(_connectionString));
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = db.Query<T>(sp, param, commandType: commandType, transaction: tran).FirstOrDefault()!;
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }

            return result;
        }
    }
}
