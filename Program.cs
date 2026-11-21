// Create the SQLite database and tables when the application starts.
DatabaseManager.InitializeDatabase();

// Controls whether the main menu continues running.
bool running = true;

while (running)
{
    // Display the main inventory menu.
    Console.WriteLine();
    Console.WriteLine("====================================");
    Console.WriteLine("     INVENTORY MANAGEMENT SYSTEM");
    Console.WriteLine("====================================");
    Console.WriteLine();
    Console.WriteLine("1. View All Products");
    Console.WriteLine("2. Add Product");
    Console.WriteLine("3. Update Product");
    Console.WriteLine("4. Delete Product");
    Console.WriteLine("5. Search Products");
    Console.WriteLine("6. Add Stock");
    Console.WriteLine("7. Remove Stock");
    Console.WriteLine("8. View Low-Stock Products");
    Console.WriteLine("9. View Inventory Value");
    Console.WriteLine("10. View Transaction History");
    Console.WriteLine("0. Exit");
    Console.WriteLine();

    // Get the user's menu selection.
    Console.Write("Enter selection: ");
    string? selection = Console.ReadLine();

    Console.WriteLine();


    switch (selection)
    {
        case "1":
            // Display all active products from SQLite.
            List<Product> databaseProducts = DatabaseManager.LoadProducts();
            DisplayProducts(databaseProducts);
            break;

        case "2":
            // Add a new product to SQLite.
            AddProductToDatabase();
            break;

        case "3":
            // Update an existing product in SQLite.
            UpdateProductInDatabase();
            break;

        case "4":
            // Soft-delete a product in SQLite.
            DeleteProductFromDatabase();
            break;

        case "5":
            // Search active products in SQLite.
            SearchProductsFromDatabase();
            break;

        case "6":
            // Add stock and record the transaction in SQLite.
            AddStockToDatabase();
            break;

        case "7":
            // Remove stock and record the transaction in SQLite.
            RemoveStockFromDatabase();
            break;

        case "8":
            // Display low-stock products from SQLite.
            DisplayLowStockProductsFromDatabase();
            break;

        case "9":
            // Display the inventory value report.
            DisplayInventoryValueFromDatabase();
            break;

        case "10":
            // Display persistent transaction history.
            DisplayTransactionsFromDatabase();
            break;

        case "0":
            // Exit the application.
            Console.WriteLine("Thank you for using the Inventory Management System.");
            running = false;
            break;

        default:
            Console.WriteLine("Invalid selection. Please enter a number from 0 to 10.");
            break;
    }
}

// Displays all products currently stored in the inventory.
static void DisplayProducts(List<Product> products)
{
    // Display the title for the All Products section.
    Console.WriteLine("ALL PRODUCTS");
    Console.WriteLine("------------------------------------");

    // Check if there are any products in the inventory.
    foreach (Product product in products)
    {
        DisplayProduct(product);
    }
}


// This function updates an existing product in the inventory.
static void UpdateProductInDatabase()
{
    // Display the title for the Update Product section.
    Console.WriteLine("UPDATE PRODUCT");
    Console.WriteLine("------------------------------------");

    // Ask the user to enter the ID of the product they want to update.
    Console.Write("Enter product ID to update: ");
    string? input = Console.ReadLine();

    // Check if the entered ID is a valid whole number.
    if (!int.TryParse(input, out int id))
    {
        // Display an error message and exit the function if the ID is invalid.
        Console.WriteLine("Invalid ID. Please enter a whole number.");
        return;
    }

    // Search the inventory list for a product that matches the entered ID.
    List<Product> databaseProducts = DatabaseManager.LoadProducts();

    Product? productToUpdate =
        databaseProducts.Find(product => product.Id == id);

    // Check if the product was found in the inventory.
    if (productToUpdate == null)
    {
        // Display an error message and exit if the product does not exist.
        Console.WriteLine($"Product with ID {id} was not found.");
        return;
    }

    // Display the selected product and instructions for updating its information.
    Console.WriteLine();
    Console.WriteLine($"Updating: {productToUpdate.Name}");
    Console.WriteLine("Press Enter without typing anything to keep the current value.");
    Console.WriteLine();

    // Display the current SKU and ask the user to enter a new one.
    Console.Write($"SKU ({productToUpdate.Sku}): ");
    string? sku = Console.ReadLine();

    // Update the SKU only if the user enters a new value.
    if (!string.IsNullOrWhiteSpace(sku))
    {
        productToUpdate.Sku = sku;
    }

    // Display the current product name and ask the user to enter a new one.
    Console.Write($"Name ({productToUpdate.Name}): ");
    string? name = Console.ReadLine();

    // Update the product name only if the user enters a new value.
    if (!string.IsNullOrWhiteSpace(name))
    {
        productToUpdate.Name = name;
    }

    // Display the current category and ask the user to enter a new one.
    Console.Write($"Category ({productToUpdate.Category}): ");
    string? category = Console.ReadLine();

    // Update the category only if the user enters a new value.
    if (!string.IsNullOrWhiteSpace(category))
    {
        productToUpdate.Category = category;
    }

    // Display the current price and ask the user to enter a new one.
    Console.Write($"Price (${productToUpdate.Price:F2}): ");
    string? priceInput = Console.ReadLine();

    // Check if the user entered a new price.
    if (!string.IsNullOrWhiteSpace(priceInput))
    {
        // Check that the new price is a valid number and is not negative.
        if (decimal.TryParse(priceInput, out decimal newPrice) &&
            newPrice >= 0 &&
            newPrice == decimal.Round(newPrice, 2))
        {
            // Update the product's price with the new value.
            productToUpdate.Price = newPrice;
        }
        else
        {
            // Display an error message and keep the current price if invalid.
            Console.WriteLine("Invalid price. The current price was kept.");
        }
    }

    // Display the current quantity without allowing direct changes.
    Console.WriteLine();
    Console.WriteLine($"Current Quantity: {productToUpdate.Quantity}");
    Console.WriteLine(
        "To change quantity, use Option 6 (Add Stock) or Option 7 (Remove Stock)."
    );
    Console.WriteLine();


    // Display the current reorder level and ask the user to enter a new one.
    Console.Write($"Reorder Level ({productToUpdate.ReorderLevel}): ");
    string? reorderInput = Console.ReadLine();

    // Check if the user entered a new reorder level.
    if (!string.IsNullOrWhiteSpace(reorderInput))
    {
        // Check that the reorder level is a whole number and is not negative.
        if (int.TryParse(reorderInput, out int newReorderLevel) && newReorderLevel >= 0)
        {
            // Update the product's reorder level with the new value.
            productToUpdate.ReorderLevel = newReorderLevel;
        }
        else
        {
            // Display an error message and keep the current reorder level if invalid.
            Console.WriteLine("Invalid reorder level. The current reorder level was kept.");
        }
    }

    // Attempt to update the product in the SQLite database and handle potential exceptions.
    // Display a message indicating whether the update was successful or if there were any issues.
    try
    {
        // Attempt to update the product in the SQLite database.
        bool updated = DatabaseManager.UpdateProduct(productToUpdate);

        // Add a blank line for better readability in the console output.
        Console.WriteLine();

        // Check if the update was successful and display an appropriate message.
        if (updated)
        {
            Console.WriteLine(
                $"Product '{productToUpdate.Name}' updated successfully in SQLite."
            );
        }
        else
        {
            Console.WriteLine("Product was not found in the database.");
        }
    }
    // Handle specific exceptions that may occur during the update process.
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Invalid product: {ex.Message}");
    }
    // Handle SQLite constraint violations, such as duplicate SKUs.
    catch (Microsoft.Data.Sqlite.SqliteException ex)
        when (ex.SqliteErrorCode == 19)
    {
        Console.WriteLine(
            "Update failed. The SKU may already exist or a database constraint was violated."
        );
    }
    // Handle cases where the product price is too large to be stored in the database.
    catch (OverflowException)
    {
        Console.WriteLine("The product price is too large.");
    }
}


// This function searches for products in the inventory based on a user-provided search term.
static void SearchProductsFromDatabase()
{
    // Display the title for the Search Products section.
    Console.WriteLine("SEARCH PRODUCTS");
    Console.WriteLine("------------------------------------");

    // Ask the user to enter a product name, SKU, or category to search for.
    Console.Write("Enter product name, SKU, or category: ");
    string? searchTerm = Console.ReadLine();

    // Check if the user entered a valid search term.
    if (string.IsNullOrWhiteSpace(searchTerm))
    {
        // Display an error message and exit if the search term is empty.
        Console.WriteLine("Please enter a search term.");
        return;
    }

    // Search the inventory for products that match the entered search term.
    List<Product> results = DatabaseManager.SearchProducts(searchTerm.Trim());


    // Check if the search returned any matching products.
    if (results.Count == 0)
    {
        // Display a message and exit if no matching products were found.
        Console.WriteLine($"No products found matching '{searchTerm}'.");
        return;
    }

    // Display the title for the search results section.
    Console.WriteLine();
    Console.WriteLine("SEARCH RESULTS");
    Console.WriteLine("------------------------------------");

    // Go through each matching product in the search results.
    foreach (Product product in results)
    {
        // Display the information for each product using the DisplayProduct function.
        DisplayProduct(product);
    }
}


// Displays the details for a single product.
static void DisplayProduct(Product product)
{
    Console.WriteLine($"ID: {product.Id}");
    Console.WriteLine($"SKU: {product.Sku}");
    Console.WriteLine($"Name: {product.Name}");
    Console.WriteLine($"Category: {product.Category}");
    Console.WriteLine($"Price: ${product.Price:F2}");
    Console.WriteLine($"Quantity: {product.Quantity}");
    Console.WriteLine($"Reorder Level: {product.ReorderLevel}");
    Console.WriteLine("------------------------------------");
}

// Increases the quantity of an existing product without allowing overflow.
static void AddStockToDatabase()
{
    // Display the title for the Add Stock section.
    Console.WriteLine("ADD STOCK");
    Console.WriteLine("------------------------------------");

    // Ask the user to enter the ID of the product they want to add stock to.
    Console.Write("Enter product ID: ");
    string? input = Console.ReadLine();

    // Check if the entered ID is a valid whole number.
    if (!int.TryParse(input, out int id))
    {
        Console.WriteLine("Invalid ID. Please enter a whole number.");
        return;
    }

    // Load the list of products from the SQLite database.
    List<Product> databaseProducts = DatabaseManager.LoadProducts();

    Product? product = databaseProducts.Find(p => p.Id == id);

    // Check if the product was found in the inventory.
    if (product == null)
    {
        Console.WriteLine($"Product with ID {id} was not found.");
        return;
    }

    // Display the product information.
    Console.WriteLine($"Product: {product.Name}");
    Console.WriteLine($"Current quantity: {product.Quantity}");

    // Ask the user to enter the quantity of stock they want to add.
    Console.Write("Enter quantity to add: ");
    string? quantityInput = Console.ReadLine();

    // Check if the entered quantity is a valid whole number and is positive.
    if (!int.TryParse(quantityInput, out int amount) || amount <= 0)
    {
        Console.WriteLine("Invalid quantity. Please enter a positive whole number.");
        return;
    }

    // Prevent the quantity from exceeding the maximum integer value.
    if (amount > int.MaxValue - product.Quantity)
    {
        Console.WriteLine("Cannot add that much stock.");
        return;
    }

    // Increase the product's quantity by the specified amount and update the database.
    bool stockAdded = DatabaseManager.AddStock(product.Id, amount);
    // Check if the stock was added successfully and display the new quantity or an error message.
    if (stockAdded)
    {
        // Reload the product to get its updated quantity.
        List<Product> updatedProducts = DatabaseManager.LoadProducts();
        // Find the updated product in the list of products.
        Product? updatedProduct =
            updatedProducts.Find(p => p.Id == product.Id);
        // Display the new quantity if the product was found.
        if (updatedProduct != null)
        {
            Console.WriteLine(
                $"Stock added successfully. New quantity: {updatedProduct.Quantity}"
            );
        }
    }
    else
    {
        // Display an error message if the stock could not be added due to constraints or issues.
        Console.WriteLine(
            "Stock could not be added. The product may be inactive, missing, or exceed the quantity limit."
        );
    }
}

// Decreases the quantity of an existing product without allowing negative stock.
static void RemoveStockFromDatabase()
{
    // Display the title for the Remove Stock section.
    Console.WriteLine("REMOVE STOCK");
    Console.WriteLine("------------------------------------");

    // Ask the user to enter the ID of the product they want to remove stock from.
    Console.Write("Enter product ID: ");
    string? input = Console.ReadLine();

    // Check if the entered ID is a valid whole number.
    if (!int.TryParse(input, out int id))
    {
        Console.WriteLine("Invalid ID. Please enter a whole number.");
        return;
    }

    // Load the list of products from the SQLite database.
    List<Product> databaseProducts = DatabaseManager.LoadProducts();

    Product? product = databaseProducts.Find(p => p.Id == id);

    // Check if the product was found in the inventory.
    if (product == null)
    {
        Console.WriteLine($"Product with ID {id} was not found.");
        return;
    }

    // Display the product information.
    Console.WriteLine($"Product: {product.Name}");
    Console.WriteLine($"Current quantity: {product.Quantity}");

    // Ask the user to enter the quantity of stock they want to remove.
    Console.Write("Enter quantity to remove: ");
    string? quantityInput = Console.ReadLine();

    // Check if the entered quantity is a valid whole number and is positive.
    if (!int.TryParse(quantityInput, out int amount) || amount <= 0)
    {
        Console.WriteLine("Invalid quantity. Please enter a positive whole number.");
        return;
    }

    // Check if the requested quantity is available in stock.
    if (amount > product.Quantity)
    {
        Console.WriteLine("Not enough stock available.");
        return;
    }

    // Attempt to decrease the product's quantity by the specified amount and update the database.
    bool stockRemoved = DatabaseManager.RemoveStock(product.Id, amount);

    // Check if the stock was removed successfully and display the new quantity or an error message.
    if (stockRemoved)
    {
        List<Product> updatedProducts = DatabaseManager.LoadProducts();

        Product? updatedProduct =
            updatedProducts.Find(p => p.Id == product.Id);

        if (updatedProduct != null)
        {
            Console.WriteLine(
                $"Stock removed successfully. New quantity: {updatedProduct.Quantity}"
            );
        }
    }
    else
    {
        Console.WriteLine(
            "Stock could not be removed. The product may be inactive, missing, or have insufficient stock."
        );
    }
}

// Displays products that are low in stock based on their reorder level.
static void DisplayLowStockProductsFromDatabase()
{
    // Display the title for the Low-Stock Products section.
    Console.WriteLine("LOW-STOCK PRODUCTS");
    Console.WriteLine("------------------------------------");
    // Retrieve the list of low-stock products from the SQLite database.
    List<Product> lowStockProducts = DatabaseManager.GetLowStockProducts();
    // Check if there are any low-stock products to display.
    if (lowStockProducts.Count == 0)
    {
        Console.WriteLine("All products have sufficient stock.");
        return;
    }
    // Go through each low-stock product and display its details.
    foreach (Product product in lowStockProducts)
    {
        DisplayProduct(product);
    }
    // Display the total number of low-stock products at the end of the report.
    Console.WriteLine(
        $"Total low-stock products: {lowStockProducts.Count}"
    );
}

// Displays the total value of the inventory based on product prices and quantities.
static void DisplayInventoryValueFromDatabase()
{
    // Display the title for the Inventory Value Report section.
    Console.WriteLine("INVENTORY VALUE REPORT");
    Console.WriteLine("------------------------------------");

    // Load active products from SQLite.
    List<Product> products = DatabaseManager.LoadProducts();

    // Display each product's inventory value.
    foreach (Product product in products)
    {
        // Calculate the total value of the product based on its price and quantity.
        decimal productValue = product.Price * product.Quantity;
        // Display the product's name, quantity, price, and total value in a formatted manner.
        Console.WriteLine(
            $"{product.Name}: {product.Quantity} x ${product.Price:F2} = ${productValue:F2}"
        );
    }

    // Calculate the total using DatabaseManager.
    decimal totalValue = DatabaseManager.GetTotalInventoryValue();

    // Display the total inventory value at the end of the report.
    Console.WriteLine("------------------------------------");
    Console.WriteLine($"Total Inventory Value: ${totalValue:F2}");
}

// Displays stock movements stored permanently in SQLite.
static void DisplayTransactionsFromDatabase()
{
    DatabaseManager.DisplayTransactionHistory();
}

// Adds a new product to the SQLite database.
static void AddProductToDatabase()
{
    // Display the title for the Add Product to SQLite section.
    Console.WriteLine("\n--- Add Product to SQLite ---");
    // Prompt the user for product details.
    Console.Write("SKU: ");
    string sku = Console.ReadLine()?.Trim() ?? "";
    // Validate that the SKU is not empty.
    Console.Write("Name: ");
    string name = Console.ReadLine()?.Trim() ?? "";
    // Validate that the name is not empty.
    Console.Write("Category: ");
    string category = Console.ReadLine()?.Trim() ?? "";
    // Validate that the category is not empty.
    Console.Write("Price: ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal price))
    {
        // Display an error message and exit if the price is invalid.
        Console.WriteLine("Invalid price.");
        return;
    }
    // Validate that the price is a valid decimal number.
    Console.Write("Quantity: ");
    if (!int.TryParse(Console.ReadLine(), out int quantity))
    {
        Console.WriteLine("Invalid quantity.");
        return;
    }
    // Validate that the quantity is a valid integer.
    Console.Write("Reorder Level: ");
    if (!int.TryParse(Console.ReadLine(), out int reorderLevel))
    {
        Console.WriteLine("Invalid reorder level.");
        return;
    }
    // Validate that the reorder level is a valid integer.
    if (string.IsNullOrWhiteSpace(sku) ||
        string.IsNullOrWhiteSpace(name) ||
        string.IsNullOrWhiteSpace(category))
    {
        Console.WriteLine("SKU, name, and category are required.");
        return;
    }
    // Create a new Product object with the provided details.
    Product product = new Product
    {
        Sku = sku,
        Name = name,
        Category = category,
        Price = price,
        Quantity = quantity,
        ReorderLevel = reorderLevel
    };
    // Attempt to insert the product into the SQLite database and handle potential exceptions.
    try
    {
        int newId = DatabaseManager.InsertProduct(product);
        Console.WriteLine($"Product added successfully! ID: {newId}");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Invalid product: {ex.Message}");
    }
    catch (Microsoft.Data.Sqlite.SqliteException ex)
        when (ex.SqliteErrorCode == 19)
    {
        Console.WriteLine("Product could not be added. The SKU may already exist.");
    }
    catch (OverflowException)
    {
        Console.WriteLine("The price or generated product ID is too large.");
    }
}

// Deletes an existing product from the SQLite database.
static void DeleteProductFromDatabase()
{
    // Display the title for the Delete Product from SQLite section.
    Console.WriteLine("\nDELETE PRODUCT");
    Console.WriteLine("------------------------------------");

    // Prompt the user to enter the ID of the product they want to delete.
    Console.Write("Enter product ID to delete: ");
    string? input = Console.ReadLine();

    // Check if the entered ID is a valid whole number.
    if (!int.TryParse(input, out int id))
    {
        Console.WriteLine("Invalid ID. Please enter a whole number.");
        return;
    }

    // Load the list of products from the SQLite database.
    List<Product> databaseProducts = DatabaseManager.LoadProducts();

    // Search for the product with the specified ID in the database.
    Product? productToDelete =
        databaseProducts.Find(product => product.Id == id);

    // Check if the product was found in the database.
    if (productToDelete == null)
    {
        Console.WriteLine($"Active product with ID {id} was not found.");
        return;
    }

    // Display the product's details before confirming deletion.
    Console.WriteLine();
    Console.WriteLine($"Product: {productToDelete.Name}");
    Console.WriteLine($"SKU: {productToDelete.Sku}");
    Console.WriteLine($"Quantity: {productToDelete.Quantity}");

    // Ask the user to confirm if they want to delete the selected product.
    Console.Write("\nAre you sure you want to delete this product? (y/n): ");
    string? confirmation = Console.ReadLine()?.Trim();

    // Check if the user entered "y" to confirm the deletion.
    if (!string.Equals(confirmation, "y", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Deletion canceled.");
        return;
    }

    //  Attempt to perform a soft delete of the product in the SQLite database.
    bool deleted = DatabaseManager.SoftDeleteProduct(id);

    // Display a message indicating whether the product was deactivated successfully or if there were any issues.
    if (deleted)
    {
        Console.WriteLine(
            $"Product '{productToDelete.Name}' was deactivated successfully."
        );
    }
    else
    {
        Console.WriteLine("Product could not be deactivated.");
    }
}