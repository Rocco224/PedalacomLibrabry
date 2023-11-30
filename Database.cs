using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedalacomLibrary
{
    public class Database
    {
        public static SqlConnection sqlConnection = new();
        public static bool isDbOpen = false;

        public Database(string _databaseConnection)
        {
            try
            {
                sqlConnection.ConnectionString = _databaseConnection;
                sqlConnection.Open();
                isDbOpen = true;
                sqlConnection.Close();
            }
            catch (Exception _error)
            {
                Console.WriteLine($"Errore: {_error.Message}");
            }
        }

        public static void DBcheck()
        {
            isDbOpen = false;
            try
            {
                if (sqlConnection.State == System.Data.ConnectionState.Closed)
                {
                    sqlConnection.Open();
                    isDbOpen = true;
                }
                else if (sqlConnection.State == System.Data.ConnectionState.Open)
                    isDbOpen = true;
            }
            catch (Exception _error)
            {
                Console.WriteLine($"Errore: {_error.Message}");
            }
        }

        public static List<List<string>> GetDatabaseData(string _query)
        {
            DBcheck();

            using (SqlCommand sqlCommand = new())
            {
                sqlCommand.CommandText = _query;
                sqlCommand.Connection = sqlConnection;

                SqlDataReader _reader = sqlCommand.ExecuteReader();

                List<List<string>> _table = new();

                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        List<string> _row = new();

                        for (int i = 0; i < _reader.FieldCount; i++)
                            _row.Add(_reader[i].ToString());

                        _table.Add(_row);
                    }
                }
                _reader.Close();
                return _table;
            }
        }

        public static List<List<string>> GetDatabaseData(string _query, Dictionary<string, string> _params)
        {
            try
            {
                DBcheck();

                using (SqlCommand sqlCommand = new())
                {
                    sqlCommand.CommandText = _query;
                    sqlCommand.Connection = sqlConnection;


                    foreach (var _param in _params)
                    {
                        SqlParameter sqlParameter = new()
                        {
                            ParameterName = _param.Key,
                            Value = _param.Value
                        };
                        sqlCommand.Parameters.Add(sqlParameter);
                    }

                    SqlDataReader _reader = sqlCommand.ExecuteReader();

                    List<List<string>> _table = new();

                    if (_reader.HasRows)
                    {
                        while (_reader.Read())
                        {
                            List<string> _row = new();

                            for (int i = 0; i < _reader.FieldCount; i++)
                                _row.Add(_reader[i].ToString());

                            _table.Add(_row);
                        }
                    }
                    _reader.Close();
                    return _table;
                }
            }
            catch (Exception _error)
            {
                Console.WriteLine($"Errore: {_error.Message}");
                throw;
            }
        }

        public static void SetDatabaseData(string _query, Dictionary<string, string> _params)
        {
            SqlTransaction _sqlTransaction = null;
            try
            {
                DBcheck();

                _sqlTransaction = sqlConnection.BeginTransaction();

                using (SqlCommand sqlCommand = new())
                {
                    sqlCommand.CommandText = _query;
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.Transaction = _sqlTransaction;

                    foreach (var _param in _params)
                    {
                        SqlParameter sqlParameter = new()
                        {
                            ParameterName = _param.Key,
                            Value = _param.Value
                        };
                        sqlCommand.Parameters.Add(sqlParameter);
                    }

                    int _rowNumber = sqlCommand.ExecuteNonQuery();
                    _sqlTransaction.Commit();
                    Console.WriteLine($"Righe aggiornate: {_rowNumber}");
                }
                sqlConnection.Close();
            }
            catch (Exception _error)
            {
                _sqlTransaction.Rollback();
                Console.WriteLine($"Errore: {_error.Message}");
                throw;
            }
        }
    }
}
