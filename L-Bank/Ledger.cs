using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace L_Bank
{
    internal class Ledger
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public decimal Balance { get; set; }

        public Ledger(int id, string name, decimal balance)
        {
            this.Id = id;
            this.Name = name;
            this.Balance = balance;
        }

        public void Save(SqlConnection conn, SqlTransaction transaction)
        {
            const string updateQuery = "UPDATE ledgers SET balance=@Balance WHERE id=@Id";
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                {
                    // Define the values to be inserted
                    cmd.Parameters.AddWithValue("@Balance", Balance);
                    cmd.Parameters.AddWithValue("@Id", Id);

                    // Execute the command
                    cmd.ExecuteNonQuery();
                }

        }

        public decimal LoadBalance(SqlConnection conn, SqlTransaction transaction)
        {
            const string getQuery = "SELECT balance FROM ledgers WHERE id = @Id";
            decimal balance = 0;
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(getQuery, conn, transaction))
                {
                       cmd.Parameters.AddWithValue("@Id", Id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            balance = reader.GetDecimal(reader.GetOrdinal("balance"));
                        }
                            return balance;
                    }
                }

        }
    }
}
