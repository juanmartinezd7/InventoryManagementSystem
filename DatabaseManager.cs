using Microsoft.Data.Sqlite;

public static class DatabaseManager
{
    // Defines the SQLite database file.
    private const string ConnectionString = "Data Source=inventory.db";

    // Creates the database tables if they do not already exist.
    public static void InitializeDatabase()
    {
        // Establish a connection to the SQLite database.
        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        // Enable foreign key constraints for this connection.
        using (SqliteCommand foreignKeysCommand = connection.CreateCommand())
        {
            foreignKeysCommand.CommandText = "PRAGMA foreign_keys = ON;";
            foreignKeysCommand.ExecuteNonQuery();
        }

        // SQL command to create the Products table.
        string createProductsTable = """
            CREATE TABLE IF NOT EXISTS Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Sku TEXT NOT NULL UNIQUE,
                Name TEXT NOT NULL,
                Category TEXT NOT NULL,
                Price REAL NOT NULL CHECK (Price >= 0),
                Quantity INTEGER NOT NULL CHECK (Quantity >= 0),
                ReorderLevel INTEGER NOT NULL CHECK (ReorderLevel >= 0)
            );
            """;

        // SQL command to create the InventoryTransactions table.
        string createTransactionsTable = """
            CREATE TABLE IF NOT EXISTS InventoryTransactions (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ProductId INTEGER NOT NULL,
                TransactionType TEXT NOT NULL,
                QuantityChanged INTEGER NOT NULL,
                TransactionDate TEXT NOT NULL,
                FOREIGN KEY (ProductId) REFERENCES Products(Id)
            );
            """;

        // Create the Products table.
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = createProductsTable;
            command.ExecuteNonQuery();
        }

        // Create the InventoryTransactions table.
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = createTransactionsTable;
            command.ExecuteNonQuery();
        }

        Console.WriteLine("Database initialized successfully.");
    }
}