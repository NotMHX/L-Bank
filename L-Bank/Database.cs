using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections;

namespace L_Bank
{
    internal class Database
    {
        public static readonly string DATABASE_NAME = @"l_bank";
        public static readonly string MASTER_CONNECTION_STRING = @"Server=MHX-SCHULE\SQLEXPRESS22; Integrated Security=True;";
        public static readonly string CONNECTION_STRING = $@"Server=MHX-SCHULE\SQLEXPRESS22; Database={DATABASE_NAME}; Integrated Security = True;";


        private static void CreateDatabaseIfNotExists()
        {
            string createDbQuery = $@"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{DATABASE_NAME}') CREATE DATABASE {DATABASE_NAME}";
            using (var masterConnection = new SqlConnection(MASTER_CONNECTION_STRING))
            {
                var command = new SqlCommand(createDbQuery, masterConnection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static void CreateTableIfNotExists()
        {
            const string createDbQuery = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ledgers' and xtype='U')
                BEGIN
                    CREATE TABLE ledgers (
                        id int IDENTITY(1,1) PRIMARY KEY,
                        name nvarchar(50) NOT NULL,
                        balance money NOT NULL
                    )
                END";
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                var command = new SqlCommand(createDbQuery, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void Initialize()
        {
            CreateDatabaseIfNotExists();
            CreateTableIfNotExists();
        }

        private static bool IsEmpty(string tableName)
        {
            string query = "SELECT COUNT(*) FROM ledgers";

            int count = 0;
            using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());

                }
            }
            return count == 0;
        }

        public static void Seed()
        {
            if(!IsEmpty("ledgers"))
            {
                return;
            }

            Random moneyProvider = new Random();

            var seedLedgers = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                    {
                        { "name", "Manitu AG" },
                        { "balance", moneyProvider.Next(100, 10001) },
                },
                new Dictionary<string, object>
                    {
                        { "name", "Chrysalkis GmbH" },
                        { "balance", moneyProvider.Next(100, 10001) },
                },
                new Dictionary<string, object>
                    {
                        { "name", "Smith & Co KG" },
                        { "balance", moneyProvider.Next(100, 10001) },
                },
            };

            using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                foreach (var data in seedLedgers)
                {
                    const string query = "INSERT INTO ledgers (name, balance) VALUES (@Name, @Balance)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", data["name"]);
                        cmd.Parameters.AddWithValue("@Balance", data["balance"]);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }       
    }


}
