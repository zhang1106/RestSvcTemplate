using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Dapper;

namespace AiDollar.Infrastructure.Database
{
    public class DbOperation : IDbOperation
    {
        private readonly SqlConnection _sqlConnection;
        private SqlTransaction _transaction;

        public DbOperation(SqlConnection sqlConnection)
        {
            if (sqlConnection == null) throw new ArgumentNullException(nameof(sqlConnection));
            _sqlConnection = sqlConnection;
        }

        public int SaveItems<T>(IEnumerable<T> items, string tableName)
        {
            //curious
            DataTable dataTable;
            try
            {
                dataTable = ToDataTable(items.ToList());
            }
            catch
            {
                return 0;
            }

            using (var bulkCopy = new SqlBulkCopy(_sqlConnection, SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers, _transaction))
            {
                bulkCopy.DestinationTableName = tableName;

                // Write from the source to the destination.
                bulkCopy.WriteToServer(dataTable);
            }

            return dataTable.Rows.Count;
        }

        public void BeginTransaction()
        {
            _sqlConnection.Open();
            _transaction = _sqlConnection.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
            _sqlConnection.Close();
        }

        public int Execute(string query)
        {
            return _sqlConnection.Execute(query, transaction: _transaction);
        }

        /// <summary>
        /// assume query returned fields are the same as properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<T> Select<T>(string query)
        {
            return _sqlConnection.Query<T>(query);
        }

        private DataTable ToDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (T item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}