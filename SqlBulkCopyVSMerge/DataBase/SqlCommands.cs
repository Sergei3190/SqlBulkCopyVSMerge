using NLog;
using SqlBulkCopyVSMerge.Helpers;
using SqlBulkCopyVSMerge.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SqlBulkCopyVSMerge.DataBase
{
    public class SqlCommands
    {
        private readonly Logger _log;

        public SqlCommands()
        {
            _log = LogManager.GetLogger(nameof(SqlCommands));
        }

        public int AddValuesRequests(in long requestId, string statusDefinition)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(Settings.Default.ConnectionString))
                using (SqlCommand sqlCommand = new SqlCommand("[Test].[dbo].[AddValuesRequests]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.AddWithValue("@RequestId", requestId);
                    sqlCommand.Parameters.AddWithValue("@StatusDefinition", statusDefinition);

                    sqlConnection.Open();

                    return sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Ошибка при выполнении метода AddValuesRequests\r\n{ex}");
                return -1;
            }
        }

        public int AddValuesPointsToRetry(DataTable dataTable, string tableNameDB)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(Settings.Default.ConnectionString))
                using (SqlCommand sqlCommand = new SqlCommand(GetCommand(tableNameDB), sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.AddWithValue("@PointsToRetry", dataTable);

                    sqlConnection.Open();

                    return sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Ошибка при выполнении метода AddValuesPointsToRetry\r\n{ex}");
                return -1;
            }
        }

        public int ExecuteBulkImportPointsToRetry(DataTable dataTable, string tableNameDB)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(Settings.Default.ConnectionString))
                {
                    SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlConnection.ConnectionString, SqlBulkCopyOptions.FireTriggers)
                    {
                        DestinationTableName = tableNameDB
                    };

                    sqlBulkCopy.WriteToServer(dataTable);

                    return 1;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Ошибка при выполнении метода ExecuteBulkImportPointsToRetry\r\n{ex}");
                return -1;
            }
        }

        public IEnumerable<int> GetPointsToRetry()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(Settings.Default.ConnectionString))
                using (SqlCommand sqlCommand = new SqlCommand("[Test].[dbo].[GetPointsForRetry]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlConnection.Open();

                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        var points = new List<int>();

                        while (sqlDataReader.Read())
                        {
                            points.Add(sqlDataReader.GetValueOrDefault<int>("PointId"));
                        }

                        return points;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Ошибка при выполнении метода GetPointsToRetry\r\n{ex}");
                return null;
            }
        }

        private string GetCommand(string tableNameDB)
        {
            switch (tableNameDB)
            {
                case "PointsToRetryMergePK":
                    return "PointsToRetryMergePK_proc";
                case "PointsToRetryMergeFK":
                    return "PointsToRetryMergeFK_proc";
                default:
                    return null;
            }
        }
    }
}
