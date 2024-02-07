using Microsoft.Data.Sqlite;
using System;
using System.IO;

string DatabaseFile = "InvFondum.sqlite";

// SQLiteConnectionStringBuilder для создания строки подключения
var sqliteConnectionStringBuilder = new SqliteConnectionStringBuilder
{
    DataSource = DatabaseFile
};
string databaseConnectionString = sqliteConnectionStringBuilder.ToString();

if (!File.Exists(DatabaseFile))
{
    File.Create(DatabaseFile).Close();
}

void InitializeDatabase()
{
    using (SqliteConnection databaseConnection = new SqliteConnection(databaseConnectionString))
    {
        databaseConnection.Open();

        using (var command = databaseConnection.CreateCommand())
        {
            command.CommandType = System.Data.CommandType.Text;
            // Создание таблиц
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Investors (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Name TEXT UNIQUE, 
                    Email TEXT UNIQUE
                );

                CREATE TABLE IF NOT EXISTS Investments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    InvestorId INT,
                    StockId INT,
                    SharesCount INT,
                    PurchaseDate TEXT,
                    FOREIGN KEY (InvestorId) REFERENCES Investors(Id),
                    FOREIGN KEY (StockId) REFERENCES Stocks(Id)
                );

                CREATE TABLE IF NOT EXISTS Stocks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Symbol TEXT UNIQUE,
                    Name TEXT UNIQUE,
                    CurrentPrice REAL
                );
            ";
            command.ExecuteNonQuery();
        }

        databaseConnection.Close();
    }
}

void AddData()
{
    using (SqliteConnection databaseConnection = new SqliteConnection(databaseConnectionString))
    {
        databaseConnection.Open();

        using (var command = databaseConnection.CreateCommand())
        {
            command.CommandType = System.Data.CommandType.Text;

            Console.WriteLine("Where do you want to add data?");
            Console.WriteLine("1: Investors");
            Console.WriteLine("2: Investments");
            Console.WriteLine("3: Stocks");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("Enter Name:");
                    string name = Console.ReadLine();
                    Console.WriteLine("Enter Email:");
                    string email = Console.ReadLine();

                    command.CommandText = $"INSERT INTO Investors (Name, Email) VALUES ('{name}', '{email}')";
                    break;
                case "2":
                    Console.WriteLine("Enter InvestorId:");
                    int investorId = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter StockId:");
                    int stockId = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter SharesCount:");
                    int sharesCount = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter PurchaseDate (YYYY-MM-DD):");
                    string purchaseDate = Console.ReadLine();

                    command.CommandText = $"INSERT INTO Investments (InvestorId, StockId, SharesCount, PurchaseDate) VALUES ({investorId}, {stockId}, {sharesCount}, '{purchaseDate}')";
                    break;
                case "3":
                    Console.WriteLine("Enter Symbol:");
                    string symbol = Console.ReadLine();
                    Console.WriteLine("Enter Name:");
                    string stockName = Console.ReadLine();
                    Console.WriteLine("Enter CurrentPrice:");
                    double currentPrice = Convert.ToDouble(Console.ReadLine());

                    command.CommandText = $"INSERT INTO Stocks (Symbol, Name, CurrentPrice) VALUES ('{symbol}', '{stockName}', {currentPrice})";
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }
            command.ExecuteNonQuery();
        }

        databaseConnection.Close();
    }
}

void ViewData()
{
    using (SqliteConnection databaseConnection = new SqliteConnection(databaseConnectionString))
    {
        databaseConnection.Open();

        Console.WriteLine("\nInvestors:");
        using (var commandInvestors = databaseConnection.CreateCommand())
        {
            commandInvestors.CommandText = "SELECT * FROM Investors";
            using (var reader = commandInvestors.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"Investor ID: {reader["Id"]}, Name: {reader["Name"]}, Email: {reader["Email"]}");
                }
            }
        }

        Console.WriteLine("\nInvestments:");
        using (var commandInvestments = databaseConnection.CreateCommand())
        {
            commandInvestments.CommandText = "SELECT * FROM Investments";
            using (var reader = commandInvestments.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"Investment ID: {reader["Id"]}, Investor ID: {reader["InvestorId"]}, Stock ID: {reader["StockId"]}, Shares Count: {reader["SharesCount"]}, Purchase Date: {reader["PurchaseDate"]}");
                }
            }
        }

        Console.WriteLine("\nStocks:");
        using (var commandStocks = databaseConnection.CreateCommand())
        {
            commandStocks.CommandText = "SELECT * FROM Stocks";
            using (var reader = commandStocks.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"Stock ID: {reader["Id"]}, Symbol: {reader["Symbol"]}, Name: {reader["Name"]}, Current Price: {reader["CurrentPrice"]}");
                }
            }
        }

        databaseConnection.Close();
    }
}


// Инициализация базы данных
InitializeDatabase();

bool running = true;
while (running)
{
    Console.WriteLine("\nSelect an action:");
    Console.WriteLine("1 - Add data");
    Console.WriteLine("2 - View data");
    Console.WriteLine("3 - Exit");
    string action = Console.ReadLine();

    switch (action)
    {
        case "1":
            AddData();
            break;
        case "2":
            ViewData();
            break;
        case "3":
            running = false;
            break;
        default:
            Console.WriteLine("Invalid action.");
            break;
    }
}
