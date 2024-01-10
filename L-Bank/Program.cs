using System;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

namespace L_Bank
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The L-Bank");
            Console.WriteLine();
            try
            {
                Console.WriteLine("Initializing database.");
                Database.Initialize();
                Console.WriteLine("Seeding data.");
                Database.Seed();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in initializing database.");
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("All Ledgers:");
            var allLedgers = Ledgers.GetAllLedgers();
            foreach (var ledger in allLedgers)
            {
                Console.WriteLine($"ID: {ledger.Id} Name: {ledger.Name} Balance: {ledger.Balance}.");
            }


            Console.WriteLine();
            Console.WriteLine("Getting total money in system at the start.");
            try
            {
                decimal startMoney = Ledgers.GetTotalMoney();
                Console.WriteLine($"Total start money: {startMoney}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in getting total money.");
                Console.WriteLine(ex.Message);
            }

            ////////////////////
            // Your Code Here

            Console.WriteLine("Booking... press ESC to stop.");
            Ledger[] allLedgersAsArray = allLedgers.ToArray();
            Random random = new();

            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                var from = allLedgersAsArray[random.Next(allLedgersAsArray.Length)];
                var to = allLedgersAsArray[random.Next(allLedgersAsArray.Length)];
                // decimal amount = random.Next(1, 100);
                Ledgers.Book(from, to);
            }
            Console.WriteLine("\nBooking stopped.");
            ////////////////////

            Console.WriteLine();
            Console.WriteLine("Getting total money in system at the end.");
            try
            {
                decimal startMoney = Ledgers.GetTotalMoney();
                Console.WriteLine($"Total end money: {startMoney}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in getting total money.");
                Console.WriteLine(ex.Message);
            }

        }
    }
}