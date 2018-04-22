using System.Collections.Generic;

namespace AiDollar.Infrastructure.Database
{
    public interface IDbOperation
    {
        int SaveItems<T>(IEnumerable<T> items, string tableName);
        IEnumerable<T> Select<T>(string query);
        void BeginTransaction();
        void Commit();
        int Execute(string query);
    }
}