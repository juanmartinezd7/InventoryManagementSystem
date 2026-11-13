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

    // Updates the quantity of an existing product in SQLite.
    public static void UpdateProductQuantity(string sku, int newQuantity)
    {
        // Validate that the new quantity is not negative.
        if (newQuantity < 0)
        {
            Console.WriteLine("Quantity cannot be negative.");
            return;
        }

        // Establish a connection to the SQLite database.
        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        // SQL command to update the quantity of a product based on its SKU.
        string sql = """
        UPDATE Products
        SET Quantity = @Quantity
        WHERE Sku = @Sku;
        """;

        // Create a command to execute the SQL statement.
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        // Use parameters to safely provide the new quantity and SKU values.
        command.Parameters.AddWithValue("@Quantity", newQuantity);
        command.Parameters.AddWithValue("@Sku", sku);

        // Execute the command and get the number of rows affected.
        int rowsAffected = command.ExecuteNonQuery();

        // Output the result to the console based on whether the product was found and updated.
        if (rowsAffected > 0)
        {
            Console.WriteLine("Product quantity updated successfully.");
        }
        else
        {
            Console.WriteLine("Product not found.");
        }
    }

    // Deletes a product from the SQLite database using its SKU.
    public static void DeleteProduct(string sku)
    {
        // Establish a connection to the SQLite database.
        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        // SQL command to delete a product based on its SKU.
        string sql = """
        DELETE FROM Products
        WHERE Sku = @Sku;
        """;

        // Create a command to execute the SQL statement.
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        // Use a parameter to safely provide the SKU value.
        command.Parameters.AddWithValue("@Sku", sku);

        // Execute the command and get the number of rows affected.
        int rowsAffected = command.ExecuteNonQuery();

        // Output the result to the console based on whether the product was found and deleted.
        if (rowsAffected > 0)
        {
            Console.WriteLine("Product deleted successfully.");
        }
        else
        {
            Console.WriteLine("Product not found.");
        }
    }

    // Inserts an inventory transaction into SQLite.
    // This method records changes in product quantity, such as sales or restocking.
    public static void InsertTransaction(
        int productId,
        int quantityChanged,
        string transactionType)
    {
        // Establish a connection to the SQLite database.
        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        // Enforce foreign key constraints for this connection.
        using (SqliteCommand foreignKeysCommand = connection.CreateCommand())
        {
            foreignKeysCommand.CommandText = "PRAGMA foreign_keys = ON;";
            foreignKeysCommand.ExecuteNonQuery();
        }

        // SQL command to insert a new inventory transaction into the InventoryTransactions table.
        string sql = """
        INSERT INTO InventoryTransactions
            (ProductId, QuantityChanged, TransactionType, TransactionDate)
        VALUES
            (@ProductId, @QuantityChanged, @TransactionType, @TransactionDate);
        """;

        // Create a command to execute the SQL statement.
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        // Use parameters to safely provide transaction values.
        command.Parameters.AddWithValue("@ProductId", productId);
        command.Parameters.AddWithValue("@QuantityChanged", quantityChanged);
        command.Parameters.AddWithValue("@TransactionType", transactionType);
        command.Parameters.AddWithValue(
            // Use the current UTC date and time for the transaction date.
            "@TransactionDate",
            DateTime.UtcNow.ToString("O")
        );

        // Execute the command and get the number of rows affected.
        int rowsAffected = command.ExecuteNonQuery();

        // Output the result to the console.
        Console.WriteLine($"Transactions inserted: {rowsAffected}");
    }

    // Displays inventory transactions with product names using SQL JOIN.
    public static void DisplayTransactionHistory()
    {
        using SqliteConnection connection =
            new SqliteConnection(ConnectionString);

        connection.Open();

        string sql = """
        SELECT
            p.Name,
            p.Sku,
            t.TransactionType,
            t.QuantityChanged,
            t.TransactionDate
        FROM InventoryTransactions AS t
        INNER JOIN Products AS p
            ON t.ProductId = p.Id
        ORDER BY t.TransactionDate DESC, t.Id DESC;
        """;

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        using SqliteDataReader reader = command.ExecuteReader();

        Console.WriteLine("\n--- SQLite Transaction History ---");

        bool foundTransactions = false;

        while (reader.Read())
        {
            foundTransactions = true;

            string productName = reader.GetString(0);
            string sku = reader.GetString(1);
            string transactionType = reader.GetString(2);
            int quantityChanged = reader.GetInt32(3);
            string transactionDate = reader.GetString(4);

            Console.WriteLine(
                $"Product: {productName} | " +
                $"SKU: {sku} | " +
                $"Type: {transactionType} | " +
                $"Quantity Changed: {quantityChanged} | " +
                $"Date: {transactionDate}"
            );
        }

        if (!foundTransactions)
        {
            Console.WriteLine("No transactions found in the database.");
        }
    }
}

