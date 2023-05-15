using System.Data.Common;
using System.Data;
using Dapper;
using Bussiness.Services;
using System.Diagnostics.CodeAnalysis;

namespace Bussiness.Interface
{
    public interface IDapper
    {
        DbConnection GetDbConnection();
        T Get<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure);
        Task<T> GetAsync<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure, CancellationToken cancellationToken = default);
        List<T> GetAll<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure);
        Task<List<T>> GetAllAsync<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure, CancellationToken cancellationToken = default);
        Task<PaginationResult<TSource>> GetAllAndPaginationAsync<TSource>(string sp, [NotNull] PaginationInput input, DynamicParameters param, CommandType commandType = CommandType.Text);
    }
}
