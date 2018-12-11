using System.Configuration;
using System.Data.SqlClient;
using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;
using StackExchange.Profiling.Helpers.Dapper;
using StackExchange.Profiling.Storage;

namespace Patient.Demographics.API
{
    public class WebApiProfiler
    {
        private static string _connectionString;

        /// <summary>
        ///     Ensure MiniProfiler tables exist.
        /// </summary>
        public static void Initialise()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            if (!DoesTableExist("MiniProfilerTimings"))
            {
                var sql = SqlServerStorage.TableCreationScript;
                using (var conn =
                    new SqlConnection(_connectionString))
                {
                    conn.Open();
                    conn.Execute(sql);
                }
            }

            MiniProfilerEF6.Initialize();
        }

        public static void Start()
        {
            MiniProfiler.Settings.Storage = new SqlServerStorage(_connectionString);
            MiniProfiler.Start();
        }

        public static void Stop()
        {
            MiniProfiler.Stop();
        }

        private static bool DoesTableExist(string tableName)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var dTable = conn.GetSchema("TABLES",
                    new[] { null, null, tableName });

                return dTable.Rows.Count > 0;
            }
        }
    }
}