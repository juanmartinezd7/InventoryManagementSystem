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

    // Inserts a new product into the SQLite database.
    public static void InsertProduct(Product product)
    {
        // Establish a connection to the SQLite database.
        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        // SQL command to insert a new product into the Products table.
        string sql = """
        INSERT INTO Products
            (Sku, Name, Category, Price, Quantity, ReorderLevel)
        VALUES
            (@Sku, @Name, @Category, @Price, @Quantity, @ReorderLevel);
        """;

        // Create a command to execute the SQL statement.
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        // Use parameters to safely provide product values.
        command.Parameters.AddWithValue("@Sku", product.Sku);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Category", product.Category);
        command.Parameters.AddWithValue("@Price", Convert.ToDouble(product.Price));
        command.Parameters.AddWithValue("@Quantity", product.Quantity);
        command.Parameters.AddWithValue("@ReorderLevel", product.ReorderLevel);

        // Execute the command and get the number of rows affected.
        int rowsAffected = command.ExecuteNonQuery();

        // Output the result to the console.
        Console.WriteLine($"Products inserted: {rowsAffected}");
    }

    // Retrieves all products stored in SQLite.
    public static void DisplayDatabaseProducts()
    {
        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        string sql = """
        SELECT Id, Sku, Name, Category, Price, Quantity, ReorderLevel
        FROM Products
        ORDER BY Id;
        """;

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        using SqliteDataReader reader = command.ExecuteReader();

        Console.WriteLine("\n--- Products from SQLite ---");

        bool foundProducts = false;

        while (reader.Read())
        {
            foundProducts = true;

            int id = reader.GetInt32(0);
            string sku = reader.GetString(1);
            string name = reader.GetString(2);
            string category = reader.GetString(3);
            double price = reader.GetDouble(4);
            int quantity = reader.GetInt32(5);
            int reorderLevel = reader.GetInt32(6);

            Console.WriteLine(
                $"ID: {id} | SKU: {sku} | Name: {name} | " +
                $"Category: {category} | Price: ${price:F2} | " +
                $"Quantity: {quantity} | Reorder Level: {reorderLevel}"
            );
        }

        if (!foundProducts)
        {
            Console.WriteLine("No products found in the database.");
        }
    }
}