using System.Data.Common;
using System.Data;
using Dapper;
using System.Diagnostics.CodeAnalysis;
using Bussiness.Services.Core;

namespace Bussiness.Interface.Core
{
    public interface IDapper
    {
        // Return a connection
        DbConnection GetDbConnection();
        // Execute stored procedure and return a DTO
        T Get<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure);
        // Execute asynchronous stored procedure and return a DTO
        Task<T> GetAsync<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure, CancellationToken cancellationToken = default);
        // Execute stored procedure and return a list have type of DTO
        List<T> GetAll<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure);
        // Execute asynchronous stored procedure and return a list have type of DTO
        Task<List<T>> GetAllAsync<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure, CancellationToken cancellationToken = default);
        // Paging sql command
        Task<PaginationResult<TSource>> GetAllAndPaginationAsync<TSource>(string sp, [NotNull] PaginationInput input, DynamicParameters param, CommandType commandType = CommandType.Text);
        Task<T> ExecuteScalarAsync<T>(string query, DynamicParameters? param = null, CommandType commandType = CommandType.Text);
    }
}
