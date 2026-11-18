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
                PriceCents INTEGER NOT NULL CHECK (PriceCents >= 0),
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
    public static int InsertProduct(Product product)
    {
        if (product.Price < 0 ||
            product.Price != decimal.Round(product.Price, 2) ||
            product.Quantity < 0 ||
            product.ReorderLevel < 0)
        {
            throw new ArgumentException("Invalid product price or quantity.");
        }

        long priceCents = checked((long)(product.Price * 100m));

        using SqliteConnection connection =
            new SqliteConnection(ConnectionString);

        connection.Open();

        string sql = """
            INSERT INTO Products
                (Sku, Name, Category, PriceCents, Quantity, ReorderLevel)
            VALUES
                (@Sku, @Name, @Category, @PriceCents, @Quantity, @ReorderLevel);

            SELECT last_insert_rowid();
            """;

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        command.Parameters.AddWithValue("@Sku", product.Sku);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Category", product.Category);
        command.Parameters.AddWithValue("@PriceCents", priceCents);
        command.Parameters.AddWithValue("@Quantity", product.Quantity);
        command.Parameters.AddWithValue("@ReorderLevel", product.ReorderLevel);

        long newId = (long)command.ExecuteScalar()!;

        return checked((int)newId);
    }

    // Retrieves all products stored in SQLite.
    // This method fetches all products from the database and displays them in the console.
    public static void DisplayDatabaseProducts()
    {
        // Establish a connection to the SQLite database.
        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        // SQL command to select all products from the Products table, ordered by their ID.
        string sql = """
        SELECT Id, Sku, Name, Category, PriceCents, Quantity, ReorderLevel
        FROM Products
        ORDER BY Id;
        """;

        // Create a command to execute the SQL statement.
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        // Execute the command and obtain a reader to read the result set.
        using SqliteDataReader reader = command.ExecuteReader();

        // Output a header for the product display.
        Console.WriteLine("\n--- Products from SQLite ---");

        // Flag to track if any products were found in the database.
        bool foundProducts = false;

        // Read and display each product record from the result set.
        while (reader.Read())
        {
            // Set the flag to true since at least one product was found.
            foundProducts = true;

            // Retrieve product details from the reader.
            int id = reader.GetInt32(0);
            string sku = reader.GetString(1);
            string name = reader.GetString(2);
            string category = reader.GetString(3);
            decimal price = reader.GetInt64(4) / 100m;
            int quantity = reader.GetInt32(5);
            int reorderLevel = reader.GetInt32(6);

            // Output the product details to the console in a formatted manner.
            Console.WriteLine(
                $"ID: {id} | SKU: {sku} | Name: {name} | " +
                $"Category: {category} | Price: ${price:F2} | " +
                $"Quantity: {quantity} | Reorder Level: {reorderLevel}"
            );
        }
        // If no products were found, output a message indicating that.
        if (!foundProducts)
        {
            // Output a message indicating that no products were found in the database.
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
    // This method retrieves and displays the transaction history, including product details.
    public static void DisplayTransactionHistory()
    {
        // Establish a connection to the SQLite database.
        using SqliteConnection connection =
            new SqliteConnection(ConnectionString);

        connection.Open();
        // SQL command to retrieve transaction history with product names using an INNER JOIN.
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

        // Create a command to execute the SQL statement.
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        // Execute the command and obtain a reader to read the result set.
        using SqliteDataReader reader = command.ExecuteReader();

        // Output a header for the transaction history display.
        Console.WriteLine("\n--- SQLite Transaction History ---");

        // Flag to track if any transactions were found in the database.
        bool foundTransactions = false;

        // Read and display each transaction record from the result set.
        while (reader.Read())
        {
            // Set the flag to true since at least one transaction was found.
            foundTransactions = true;

            // Retrieve transaction details from the reader.
            string productName = reader.GetString(0);
            string sku = reader.GetString(1);
            string transactionType = reader.GetString(2);
            int quantityChanged = reader.GetInt32(3);
            string transactionDate = reader.GetString(4);

            // Output the transaction details to the console in a formatted manner.
            Console.WriteLine(
                $"Product: {productName} | " +
                $"SKU: {sku} | " +
                $"Type: {transactionType} | " +
                $"Quantity Changed: {quantityChanged} | " +
                $"Date: {transactionDate}"
            );
        }
        // If no transactions were found, output a message indicating that.
        if (!foundTransactions)
        {
            Console.WriteLine("No transactions found in the database.");
        }
    }

    // Loads products from SQLite into a C# list.
    // This method retrieves all products from the database and returns them as a list of Product objects.
    public static List<Product> LoadProducts()
    {
        // Create a list to hold the products retrieved from the database.
        List<Product> products = new List<Product>();
        // Establish a connection to the SQLite database.
        using SqliteConnection connection =
            new SqliteConnection(ConnectionString);

        connection.Open();

        // SQL command to select all products from the Products table, ordered by their ID.
        string sql = """
        SELECT Id, Sku, Name, Category, PriceCents, Quantity, ReorderLevel
        FROM Products
        ORDER BY Id;
        """;

        // Create a command to execute the SQL statement.
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        // Execute the command and obtain a reader to read the result set.
        using SqliteDataReader reader = command.ExecuteReader();

        // Read each product record from the result set and create Product objects.
        while (reader.Read())
        {
            // Create a new Product object and populate its properties with data from the reader.
            Product product = new Product
            {
                Id = reader.GetInt32(0),
                Sku = reader.GetString(1),
                Name = reader.GetString(2),
                Category = reader.GetString(3),
                Price = reader.GetInt64(4) / 100m,
                Quantity = reader.GetInt32(5),
                ReorderLevel = reader.GetInt32(6)
            };

            // Add the newly created Product object to the products list.
            products.Add(product);
        }
        // Return the list of products retrieved from the database.
        return products;
    }
}

