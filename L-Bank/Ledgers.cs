using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace L_Bank
{
    internal class Ledgers
    {
        public static void Book(Ledger from, Ledger to)
        {
            using (SqlConnection conn = new SqlConnection(Database.CONNECTION_STRING))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        int amount = 10;
                        from.LoadBalance(conn, transaction);
                        from.Balance -= amount;
                        from.Save(conn, transaction);
                        // Complicate calculations
                        Thread.Sleep(250);
                        to.LoadBalance(conn, transaction);
                        to.Balance += amount;
                        to.Save(conn, transaction);
                        transaction.Commit();
                        Console.Write(".");
                    }
                    catch (Exception ex)
                    {
                        // Hat nicht geklappt, wir versuchen alles rückgängig zu machen
                        try
                        {
                            Console.WriteLine(ex.Message);
                            Console.Write("R");
                            transaction.Rollback();
                        }
                        // Ui, nicht mal Rollback klappt
                        catch (Exception ex2)
                        {
                            Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                            Console.WriteLine("  Message: {0}", ex2.Message);
                        }
                    }
                }
            } 
        }

        public static ImmutableHashSet<Ledger> GetAllLedgers()
        {
            var AllLedgers = new HashSet<Ledger>();

            const string query = @"SELECT id, name, balance FROM ledgers";

            using (SqlConnection conn = new SqlConnection(Database.CONNECTION_STRING))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("id"));
                            string name = reader.GetString(reader.GetOrdinal("name"));
                            decimal balance = reader.GetDecimal(reader.GetOrdinal("balance"));

                            AllLedgers.Add(new Ledger(id, name, balance));

                        }
                    }
                }
            }
            return AllLedgers.ToImmutableHashSet<Ledger>();
        }
        public static decimal GetTotalMoney()
        {
            const string query = @"SELECT SUM(balance) AS TotalBalance FROM ledgers";
            decimal totalBalance = 0;

            using (SqlConnection conn = new SqlConnection(Database.CONNECTION_STRING))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        totalBalance = Convert.ToDecimal(result);
                    }
                }
            }

            return totalBalance;
        }
    }
}
