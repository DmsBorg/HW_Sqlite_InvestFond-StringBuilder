using Microsoft.Data.Sqlite;
using System.IO;

string DatabaseFile = "db2.sqlite";
string databaseConnectionString = $"Data Source={DatabaseFile}";

if (!File.Exists(DatabaseFile))
{
    File.Create(DatabaseFile).Close();
}

using (SqliteConnection databaseConnection = new SqliteConnection(databaseConnectionString))
{
    databaseConnection.Open();

    using (var command = databaseConnection.CreateCommand())
    {
        command.CommandType = System.Data.CommandType.Text;

        // Создание таблиц с уникальными ограничениями(можно добавть фамилии или по id потом...)
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Investors (
                ID INTEGER PRIMARY KEY, 
                Investor_Name TEXT UNIQUE, 
                Investor_Cash INT
            );

            CREATE TABLE IF NOT EXISTS Investments (
                Investment_ID INTEGER PRIMARY KEY, 
                Investor_ID INT,
                Stock_Name TEXT UNIQUE,
                Amount INT,
                Currency TEXT
            );

            CREATE TABLE IF NOT EXISTS Efficiency (
                Record_ID INTEGER PRIMARY KEY, 
                Investment_ID INT UNIQUE,
                Profit_Percentage REAL
            );
        ";
        command.ExecuteNonQuery();

        // Вставка данных с использованием INSERT OR IGNORE
        command.CommandText = @"
            INSERT OR IGNORE INTO Investors (Investor_Name, Investor_Cash) VALUES 
            ('Alice', 100000), 
            ('Bob', 150000);

            INSERT OR IGNORE INTO Investments (Investor_ID, Stock_Name, Amount, Currency) VALUES 
            (1, 'Apple', 50, 'USD'),
            (1, 'Microsoft', 30, 'USD'),
            (2, 'Google', 40, 'USD');

            INSERT OR IGNORE INTO Efficiency (Investment_ID, Profit_Percentage) VALUES 
            (1, 5.2),
            (2, 3.8),
            (3, 4.1);
        ";
        command.ExecuteNonQuery();
    }

    databaseConnection.Close();
}

// Повторное открытие соединения для чтения данных
using (SqliteConnection databaseConnection = new SqliteConnection(databaseConnectionString))
{
    databaseConnection.Open();

    using (var command = databaseConnection.CreateCommand())
    {
        command.CommandType = System.Data.CommandType.Text;

        // Чтение и вывод данных из Investors
        command.CommandText = "SELECT * FROM Investors;";
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"Investor ID: {reader["ID"]}, Name: {reader["Investor_Name"]}, Cash: {reader["Investor_Cash"]}");
            }
        }

        // Чтение и вывод данных из Investments
        command.CommandText = "SELECT * FROM Investments;";
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"Investment ID: {reader["Investment_ID"]}, Investor ID: {reader["Investor_ID"]}, Stock: {reader["Stock_Name"]}, Amount: {reader["Amount"]}, Currency: {reader["Currency"]}");
            }
        }

        // Чтение и вывод данных из Efficiency
        command.CommandText = "SELECT * FROM Efficiency;";
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"Record ID: {reader["Record_ID"]}, Investment ID: {reader["Investment_ID"]}, Profit Percentage: {reader["Profit_Percentage"]}");
            }
        }
    }

    databaseConnection.Close();
}
