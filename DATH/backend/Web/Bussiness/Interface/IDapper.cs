using System.Data.Common;
using System.Data;
using Dapper;

namespace Bussiness.Interface
{
    public interface IDapper
    {
        DbConnection GetDbconnection();
        T Get<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure);
        List<T> GetAll<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure);
        int Execute(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure);
        T Insert<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure);
        T Update<T>(string sp, DynamicParameters param, CommandType commandType = CommandType.StoredProcedure);
    }
}
