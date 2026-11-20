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
                ReorderLevel INTEGER NOT NULL CHECK (ReorderLevel >= 0),
                IsActive INTEGER NOT NULL DEFAULT 1 CHECK (IsActive IN (0, 1))
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
        WHERE IsActive = 1
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
    // Updates an existing product in SQLite.
    public static bool UpdateProduct(Product product)
    {
        // Validate the product information before updating.
        if (string.IsNullOrWhiteSpace(product.Sku) ||
            string.IsNullOrWhiteSpace(product.Name) ||
            string.IsNullOrWhiteSpace(product.Category) ||
            product.Price < 0 ||
            product.Price != decimal.Round(product.Price, 2) ||
            product.Quantity < 0 ||
            product.ReorderLevel < 0)
        {
            // Throw an exception if the product information is invalid.
            throw new ArgumentException("Invalid product information.");
        }
        // Convert the product price to cents for storage in the database.
        long priceCents = checked((long)(product.Price * 100m));
        // Establish a connection to the SQLite database.
        using SqliteConnection connection =
            new SqliteConnection(ConnectionString);

        connection.Open();
        // SQL command to update the product information in the Products table based on the product ID.
        string sql = """
        UPDATE Products
        SET
            Sku = @Sku,
            Name = @Name,
            Category = @Category,
            PriceCents = @PriceCents,
            Quantity = @Quantity,
            ReorderLevel = @ReorderLevel
        WHERE Id = @Id AND IsActive = 1;
        """;
        // Create a command to execute the SQL statement.
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        command.Parameters.AddWithValue("@Id", product.Id);
        command.Parameters.AddWithValue("@Sku", product.Sku);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Category", product.Category);
        command.Parameters.AddWithValue("@PriceCents", priceCents);
        command.Parameters.AddWithValue("@Quantity", product.Quantity);
        command.Parameters.AddWithValue("@ReorderLevel", product.ReorderLevel);

        // Execute the command and get the number of rows affected.
        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }
    // Soft deletes a product by setting its IsActive flag to 0.
    public static bool SoftDeleteProduct(int id)
    {
        // Establish a connection to the SQLite database.
        using SqliteConnection connection =
            new SqliteConnection(ConnectionString);

        connection.Open();
        // SQL command to soft delete a product by updating its IsActive flag to 0.
        string sql = """
        UPDATE Products
        SET IsActive = 0
        WHERE Id = @Id AND IsActive = 1;
        """;
        // Create a command to execute the SQL statement.
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        // Use a parameter to safely provide the product ID value.
        command.Parameters.AddWithValue("@Id", id);
        // Execute the command and get the number of rows affected.
        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    // Searches for products in SQLite based on a search term.
    public static List<Product> SearchProducts(string searchTerm)
    {
        // Create a list to hold the search results.
        List<Product> results = new List<Product>();
        // Establish a connection to the SQLite database.
        using SqliteConnection connection =
            new SqliteConnection(ConnectionString);

        connection.Open();

        // SQL command to search for products based on the search term in SKU, Name, or Category.
        string sql = """
        SELECT Id, Sku, Name, Category, PriceCents, Quantity, ReorderLevel
        FROM Products
        WHERE IsActive = 1
          AND (
              INSTR(LOWER(Sku), LOWER(@SearchTerm)) > 0
              OR INSTR(LOWER(Name), LOWER(@SearchTerm)) > 0
              OR INSTR(LOWER(Category), LOWER(@SearchTerm)) > 0
          )
        ORDER BY Name;
        """;

        // Create a command to execute the SQL statement.
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        // Use a parameter to safely provide the search term value.
        command.Parameters.AddWithValue("@SearchTerm", searchTerm);
        // Execute the command and obtain a reader to read the result set.
        using SqliteDataReader reader = command.ExecuteReader();

        // Read each product record from the result set and create Product objects for the search results.
        while (reader.Read())
        {
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

            // Add the newly created Product object to the search results list.
            results.Add(product);
        }

        return results;
    }

    // Adds stock to an existing product in SQLite and records the transaction.
    public static bool AddStock(int productId, int amount)
    {
        // Validate that the stock amount to be added is positive.
        if (amount <= 0)
        {
            throw new ArgumentException("Stock amount must be positive.");
        }

        // Establish a connection to the SQLite database.
        using SqliteConnection connection =
            new SqliteConnection(ConnectionString);

        connection.Open();

        // Both database changes belong to one transaction.
        using SqliteTransaction transaction = connection.BeginTransaction();

        // Increase stock only for active products.
        // The condition also prevents integer overflow.
        string updateSql = """
        UPDATE Products
        SET Quantity = Quantity + @Amount
        WHERE Id = @ProductId
          AND IsActive = 1
          AND Quantity <= @MaximumQuantity;
        """;

        // Create a command to execute the SQL statement for updating the product quantity.
        using SqliteCommand updateCommand = connection.CreateCommand();
        updateCommand.Transaction = transaction;
        updateCommand.CommandText = updateSql;

        // Use parameters to safely provide the amount, product ID, and maximum quantity values.
        updateCommand.Parameters.AddWithValue("@Amount", amount);
        updateCommand.Parameters.AddWithValue("@ProductId", productId);
        updateCommand.Parameters.AddWithValue(
            "@MaximumQuantity",
            int.MaxValue - amount
        );

        // Execute the command and get the number of rows affected.
        int rowsAffected = updateCommand.ExecuteNonQuery();

        if (rowsAffected == 0)
        {
            return false;
        }

        // Record the stock addition in the same transaction.
        string insertSql = """
        INSERT INTO InventoryTransactions
            (ProductId, TransactionType, QuantityChanged, TransactionDate)
        VALUES
            (@ProductId, @TransactionType, @QuantityChanged, @TransactionDate);
        """;

        // Create a command to execute the SQL statement for inserting the inventory transaction.
        using SqliteCommand insertCommand = connection.CreateCommand();
        insertCommand.Transaction = transaction;
        insertCommand.CommandText = insertSql;

        // Use parameters to safely provide the product ID, transaction type, quantity changed, and transaction date values.
        insertCommand.Parameters.AddWithValue("@ProductId", productId);
        insertCommand.Parameters.AddWithValue("@TransactionType", "ADD");
        insertCommand.Parameters.AddWithValue("@QuantityChanged", amount);
        insertCommand.Parameters.AddWithValue(
            "@TransactionDate",
            DateTime.UtcNow.ToString("O")
        );

        // Execute the command to insert the inventory transaction.
        insertCommand.ExecuteNonQuery();

        // Save both changes together.
        transaction.Commit();

        return true;
    }

    // Removes stock from an existing product in SQLite and records the transaction.
    public static bool RemoveStock(int productId, int amount)
    {
        // Validate that the stock amount to be removed is positive.
        if (amount <= 0)
        {
            throw new ArgumentException("Stock amount must be positive.");
        }
        // Establish a connection to the SQLite database.
        using SqliteConnection connection =
            new SqliteConnection(ConnectionString);

        connection.Open();

        // Begin an atomic database transaction.
        using SqliteTransaction transaction = connection.BeginTransaction();

        // Decrease stock only when enough inventory is available.
        string updateSql = """
        UPDATE Products
        SET Quantity = Quantity - @Amount
        WHERE Id = @ProductId
          AND IsActive = 1
          AND Quantity >= @Amount;
        """;
        // Create a command to execute the SQL statement for updating the product quantity.
        using SqliteCommand updateCommand = connection.CreateCommand();
        updateCommand.Transaction = transaction;
        updateCommand.CommandText = updateSql;

        // Use parameters to safely provide the amount and product ID values.
        updateCommand.Parameters.AddWithValue("@Amount", amount);
        updateCommand.Parameters.AddWithValue("@ProductId", productId);

        // Execute the command and get the number of rows affected.
        int rowsAffected = updateCommand.ExecuteNonQuery();

        if (rowsAffected == 0)
        {
            return false;
        }

        // Record the stock removal in the same transaction.
        string insertSql = """
        INSERT INTO InventoryTransactions
            (ProductId, TransactionType, QuantityChanged, TransactionDate)
        VALUES
            (@ProductId, @TransactionType, @QuantityChanged, @TransactionDate);
        """;

        // Create a command to execute the SQL statement for inserting the inventory transaction.
        using SqliteCommand insertCommand = connection.CreateCommand();
        insertCommand.Transaction = transaction;
        insertCommand.CommandText = insertSql;

        // Use parameters to safely provide the product ID, transaction type, quantity changed, and transaction date values.
        insertCommand.Parameters.AddWithValue("@ProductId", productId);
        insertCommand.Parameters.AddWithValue("@TransactionType", "REMOVE");
        insertCommand.Parameters.AddWithValue("@QuantityChanged", -amount);
        insertCommand.Parameters.AddWithValue(
            "@TransactionDate",
            DateTime.UtcNow.ToString("O")
        );

        // Execute the command to insert the inventory transaction.
        insertCommand.ExecuteNonQuery();

        // Commit only after both operations succeed.
        transaction.Commit();

        return true;
    }
    // Retrieves products that are low in stock based on their reorder level.
    public static List<Product> GetLowStockProducts()
    {
        // Create a list to hold the low stock products retrieved from the database.
        List<Product> lowStockProducts = new List<Product>();

        // Establish a connection to the SQLite database.
        using SqliteConnection connection =
            new SqliteConnection(ConnectionString);

        connection.Open();

        // SQL command to select products that are low in stock based on their reorder level.
        string sql = """
        SELECT Id, Sku, Name, Category, PriceCents, Quantity, ReorderLevel
        FROM Products
        WHERE IsActive = 1
          AND Quantity <= ReorderLevel
        ORDER BY Quantity ASC;
        """;
        // Create a command to execute the SQL statement.
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        // Execute the command and obtain a reader to read the result set.
        using SqliteDataReader reader = command.ExecuteReader();
        // Read each product record from the result set and create Product objects for the low stock products.
        while (reader.Read())
        {
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
            // Add the newly created Product object to the low stock products list.
            lowStockProducts.Add(product);
        }

        return lowStockProducts;
    }
    // Calculates the total value of the inventory in dollars.
    public static decimal GetTotalInventoryValue()
    {
        // Establish a connection to the SQLite database.
        using SqliteConnection connection =
            new SqliteConnection(ConnectionString);

        connection.Open();
        // SQL command to calculate the total value of the inventory by summing the product of price and quantity for all active products.
        string sql = """
        SELECT PriceCents, Quantity
        FROM Products
        WHERE IsActive = 1;
        """;
        // Create a command to execute the SQL statement.
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        // Execute the command and obtain a reader to read the result set.
        using SqliteDataReader reader = command.ExecuteReader();
        // Initialize a variable to hold the total value in cents.
        decimal totalCents = 0m;
        // Read each product record from the result set and calculate the total value.
        while (reader.Read())
        {
            long priceCents = reader.GetInt64(0);
            int quantity = reader.GetInt32(1);

            totalCents += (decimal)priceCents * quantity;
        }
        // Convert the total value from cents to dollars and return it.
        return totalCents / 100m;
    }

    // Displays the transaction history with product names and details.
    public static void DisplayTransactionHistory()
    {
        // Establish a connection to the SQLite database.
        using SqliteConnection connection =
            new SqliteConnection(ConnectionString);

        connection.Open();
        // SQL command to retrieve transaction history with product names using an INNER JOIN.
        string sql = """
        SELECT
            t.Id,
            t.ProductId,
            p.Name,
            t.TransactionType,
            t.QuantityChanged,
            t.TransactionDate
        FROM InventoryTransactions t
        INNER JOIN Products p ON t.ProductId = p.Id
        ORDER BY t.TransactionDate DESC, t.Id DESC;
        """;
        // Create a command to execute the SQL statement.
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        // Execute the command and obtain a reader to read the result set.
        using SqliteDataReader reader = command.ExecuteReader();
        // Output a header for the transaction history display.
        Console.WriteLine("TRANSACTION HISTORY");
        Console.WriteLine("------------------------------------");
        // Flag to track if any transactions were found in the database.
        bool hasTransactions = false;
        // Read and display each transaction record from the result set.
        while (reader.Read())
        {
            hasTransactions = true;

            int transactionId = reader.GetInt32(0);
            int productId = reader.GetInt32(1);
            string productName = reader.GetString(2);
            string transactionType = reader.GetString(3);
            int quantityChanged = reader.GetInt32(4);
            string transactionDateText = reader.GetString(5);

            // Parse the transaction date string into a DateTime object using invariant culture and round-trip kind.
            DateTime transactionDate = DateTime.Parse(
                transactionDateText,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.RoundtripKind
            );

            // Output the transaction details to the console in a formatted manner.
            Console.WriteLine($"Transaction ID: {transactionId}");
            Console.WriteLine($"Product ID: {productId}");
            Console.WriteLine($"Product: {productName}");
            Console.WriteLine($"Type: {transactionType}");
            Console.WriteLine(
                $"Quantity Changed: {quantityChanged:+#;-#;0}"
            );
            // Display the transaction date in UTC format for consistency.
            Console.WriteLine($"Date (UTC): {transactionDate.ToUniversalTime():g}");
            Console.WriteLine("------------------------------------");
        }
        // If no transactions were found, output a message indicating that.
        if (!hasTransactions)
        {
            Console.WriteLine("No transactions recorded.");
        }
    }
}

