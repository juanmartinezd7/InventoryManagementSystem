// Load saved inventory when the application starts.
List<Product> products = InventoryStorage.LoadProducts();

// Stores stock movements during the current application session.
List<InventoryTransaction> transactions = new List<InventoryTransaction>();

// Create sample products only when no saved inventory file exists.
if (!File.Exists("inventory.json"))
{
// Add sample products for testing the inventory system.
products.Add(new Product
{
    Id = 1,
    Sku = "LAP-001",
    Name = "Dell Laptop",
    Category = "Computers",
    Price = 799.99m,
    Quantity = 5,
    ReorderLevel = 2
});

products.Add(new Product
{
    Id = 2,
    Sku = "MON-001",
    Name = "24-inch Monitor",
    Category = "Monitors",
    Price = 149.99m,
    Quantity = 8,
    ReorderLevel = 3
});

products.Add(new Product
{
    Id = 3,
    Sku = "KEY-001",
    Name = "USB Keyboard",
    Category = "Accessories",
    Price = 29.99m,
    Quantity = 12,
    ReorderLevel = 5
});

 InventoryStorage.SaveProducts(products);
}

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

    // Perform an action based on the user's selection.
    switch (selection)
    {
        case "1":
            DisplayProducts(products);
            break;

        case "2":
            AddProduct(products);
            // Save the updated inventory after adding a new product.
            InventoryStorage.SaveProducts(products);
            break;

        case "3":
            UpdateProduct(products);
            // Save the updated inventory after modifying an existing product.
            InventoryStorage.SaveProducts(products);
            break;

        case "4":
            DeleteProduct(products);
            // Save the updated inventory after deleting a product.
            InventoryStorage.SaveProducts(products);
            break;

        case "5":
            SearchProducts(products);
            break;

        case "6":
            // Add stock to an existing product and record the transaction.
            AddStock(products, transactions);
            // Save the updated inventory after adding stock to a product.
            InventoryStorage.SaveProducts(products);
            break;

        case "7":
            // Remove stock from an existing product and record the transaction.
            RemoveStock(products, transactions);
            // Save the updated inventory after removing stock from a product.
            InventoryStorage.SaveProducts(products);
            break;

        case "8":
            DisplayLowStockProducts(products);
            break;

        case "9":
            DisplayInventoryValue(products);
            break;

            case "10":
            DisplayTransactions(transactions);
            break;

        // Save the inventory and exit the application.
        case "0":
            InventoryStorage.SaveProducts(products);
            Console.WriteLine("Inventory saved successfully.");
            Console.WriteLine("Thank you for using the Inventory Management System.");
            running = false;
            break;

        default:
            Console.WriteLine("Invalid selection. Please enter a number from 0 to 9.");
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


// This function adds a new product to the inventory.
static void AddProduct(List<Product> products)
{
    // Display the title for the Add Product section.
    Console.WriteLine("ADD PRODUCT");
    Console.WriteLine("------------------------------------");

    // Ask the user to enter the product's SKU.
    Console.Write("Enter SKU: ");
    string? sku = Console.ReadLine();

    // Ask the user to enter the product's name.
    Console.Write("Enter product name: ");
    string? name = Console.ReadLine();

    // Ask the user to enter the product's category.
    Console.Write("Enter category: ");
    string? category = Console.ReadLine();

    // Declare a variable to store the product's price.
    decimal price;

    // Keep asking for a price until the user enters a valid value.
    while (true)
    {
        Console.Write("Enter price: $");
        string? input = Console.ReadLine();

        // Check that the price is a valid number and is not negative.
        if (decimal.TryParse(input, out price) && price >= 0)
        {
            // Exit the loop when a valid price is entered.
            break;
        }

        // Display an error message if the price is invalid.
        Console.WriteLine("Invalid price. Please enter a valid number.");
    }

    // Declare a variable to store the product's quantity.
    int quantity;

    // Keep asking for a quantity until the user enters a valid value.
    while (true)
    {
        Console.Write("Enter quantity: ");
        string? input = Console.ReadLine();

        // Check that the quantity is a whole number and is not negative.
        if (int.TryParse(input, out quantity) && quantity >= 0)
        {
            // Exit the loop when a valid quantity is entered.
            break;
        }

        // Display an error message if the quantity is invalid.
        Console.WriteLine("Invalid quantity. Please enter a whole number.");
    }

    // Declare a variable to store the product's reorder level.
    int reorderLevel;

    // Keep asking until the user enters a valid reorder level.
    while (true)
    {
        Console.Write("Enter reorder level: ");
        string? input = Console.ReadLine();

        // Check that the reorder level is a whole number and is not negative.
        if (int.TryParse(input, out reorderLevel) && reorderLevel >= 0)
        {
            // Exit the loop when a valid reorder level is entered.
            break;
        }

        // Display an error message if the reorder level is invalid.
        Console.WriteLine("Invalid reorder level. Please enter a whole number.");
    }

    // Set the starting product ID to 1.
    int newId = 1;

    // Check if there are already products in the inventory.
    if (products.Count > 0)
    {
        // Find the highest existing product ID and add 1 to create a new ID.
        newId = products.Max(product => product.Id) + 1;
    }

    // Create a new product using the information entered by the user.
    Product newProduct = new Product
    {
        Id = newId,
        Sku = sku ?? "",
        Name = name ?? "",
        Category = category ?? "",
        Price = price,
        Quantity = quantity,
        ReorderLevel = reorderLevel
    };

    // Add the new product to the inventory list.
    products.Add(newProduct);

    // Display a message confirming that the product was added successfully.
    Console.WriteLine();
    Console.WriteLine($"Product '{newProduct.Name}' added successfully.");
}



// This function updates an existing product in the inventory.
static void UpdateProduct(List<Product> products)
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
    Product? productToUpdate = products.Find(product => product.Id == id);

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
        if (decimal.TryParse(priceInput, out decimal newPrice) && newPrice >= 0)
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

    // Display the current quantity and ask the user to enter a new one.
    Console.Write($"Quantity ({productToUpdate.Quantity}): ");
    string? quantityInput = Console.ReadLine();

    // Check if the user entered a new quantity.
    if (!string.IsNullOrWhiteSpace(quantityInput))
    {
        // Check that the new quantity is a whole number and is not negative.
        if (int.TryParse(quantityInput, out int newQuantity) && newQuantity >= 0)
        {
            // Update the product's quantity with the new value.
            productToUpdate.Quantity = newQuantity;
        }
        else
        {
            // Display an error message and keep the current quantity if invalid.
            Console.WriteLine("Invalid quantity. The current quantity was kept.");
        }
    }

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

    // Display a message confirming that the product update is complete.
    Console.WriteLine();
    Console.WriteLine($"Product '{productToUpdate.Name}' updated successfully.");
}



// This function deletes an existing product from the inventory.
static void DeleteProduct(List<Product> products)
{
    // Display the title for the Delete Product section.
    Console.WriteLine("DELETE PRODUCT");
    Console.WriteLine("------------------------------------");

    // Ask the user to enter the ID of the product they want to delete.
    Console.Write("Enter product ID to delete: ");
    string? input = Console.ReadLine();

    // Check if the entered ID is a valid whole number.
    if (!int.TryParse(input, out int id))
    {
        // Display an error message and exit the function if the ID is invalid.
        Console.WriteLine("Invalid ID. Please enter a whole number.");
        return;
    }

    // Search the inventory list for a product that matches the entered ID.
    Product? productToDelete = products.Find(product => product.Id == id);

    // Check if the product was found in the inventory.
    if (productToDelete == null)
    {
        // Display an error message and exit if the product does not exist.
        Console.WriteLine($"Product with ID {id} was not found.");
        return;
    }

    // Display the selected product's information before deleting it.
    Console.WriteLine();
    Console.WriteLine($"Product: {productToDelete.Name}");
    Console.WriteLine($"SKU: {productToDelete.Sku}");
    Console.WriteLine($"Price: ${productToDelete.Price:F2}");
    Console.WriteLine($"Quantity: {productToDelete.Quantity}");
    Console.WriteLine();

    // Ask the user to confirm if they want to delete the selected product.
    Console.Write($"Are you sure you want to delete '{productToDelete.Name}'? (y/n): ");
    string? confirmation = Console.ReadLine();

    // Check if the user entered "y" to confirm the deletion.
    if (confirmation?.ToLower() == "y")
    {
        // Remove the selected product from the inventory list.
        products.Remove(productToDelete);

        // Display a message confirming that the product was deleted.
        Console.WriteLine($"Product '{productToDelete.Name}' deleted successfully.");
    }
    else
    {
        // Cancel the deletion if the user does not confirm.
        Console.WriteLine("Delete canceled.");
    }
}


// This function searches for products in the inventory by name, SKU, or category.
static void SearchProducts(List<Product> products)
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
    // Check the product name, SKU, and category without considering uppercase or lowercase letters.
    List<Product> results = products.FindAll(product =>
        product.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
        product.Sku.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
        product.Category.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
    );

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

// Increases the quantity of an existing product.
static void AddStock(
    List<Product> products,
    List<InventoryTransaction> transactions)
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

    // Search for the product with the specified ID.
    Product? product = products.Find(p => p.Id == id);

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

    // Increase the product's quantity by the specified amount.
    product.Quantity += amount;

    // Record the stock addition.
InventoryTransaction transaction = new InventoryTransaction
{
    ProductId = product.Id,
    QuantityChanged = amount,
    TransactionType = "ADD",
    TransactionDate = DateTime.Now
};

transactions.Add(transaction);

    // Display a message confirming that the stock was added successfully.
    Console.WriteLine($"Stock added successfully. New quantity: {product.Quantity}");
}

// Decreases the quantity of an existing product without allowing negative stock.
static void RemoveStock(
    List<Product> products,
    List<InventoryTransaction> transactions)
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

    // Search for the product with the specified ID.
    Product? product = products.Find(p => p.Id == id);

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

    // Decrease the product's quantity by the specified amount.
    product.Quantity -= amount;

    // Record the stock removal.
InventoryTransaction transaction = new InventoryTransaction
{
    ProductId = product.Id,
    QuantityChanged = -amount,
    TransactionType = "REMOVE",
    TransactionDate = DateTime.Now
};

transactions.Add(transaction);

    // Display a message confirming that the stock was removed successfully.
    Console.WriteLine($"Stock removed successfully. New quantity: {product.Quantity}");
}

// Displays products whose quantities are at or below their reorder levels.
static void DisplayLowStockProducts(List<Product> products)
{
    // Display the title for the Low-Stock Products section.
    Console.WriteLine("LOW-STOCK PRODUCTS");
    Console.WriteLine("------------------------------------");

    // Initialize a counter to keep track of the number of low-stock products.
    int lowStockCount = 0;

    // Go through each product in the inventory to check its stock level.
    foreach (Product product in products)
    {
        if (product.Quantity <= product.ReorderLevel)
        {
            DisplayProduct(product);
            lowStockCount++;
        }
    }

    // Display a message indicating whether there are any low-stock products.
    if (lowStockCount == 0)
    {
        Console.WriteLine("All products have sufficient stock.");
    }
    else
    {
        Console.WriteLine($"Total low-stock products: {lowStockCount}");
    }
}

// Calculates and displays the total monetary value of the inventory.
static void DisplayInventoryValue(List<Product> products)
{
    // Display the title for the Inventory Value Report section.
    Console.WriteLine("INVENTORY VALUE REPORT");
    Console.WriteLine("------------------------------------");

    // Initialize a variable to keep track of the total inventory value.
    decimal totalValue = 0;

    // Go through each product in the inventory to calculate its total value.
    foreach (Product product in products)
    {
        decimal productValue = product.Price * product.Quantity;

        Console.WriteLine(
            $"{product.Name}: {product.Quantity} x ${product.Price:F2} = ${productValue:F2}"
        );

        totalValue += productValue;
    }

    // Display the total inventory value at the end of the report.
    Console.WriteLine("------------------------------------");
    Console.WriteLine($"Total Inventory Value: ${totalValue:F2}");
}

// Displays stock movements recorded during the current session.
static void DisplayTransactions(List<InventoryTransaction> transactions)
{
    // Display the title for the Transaction History section.
    Console.WriteLine("TRANSACTION HISTORY");
    Console.WriteLine("------------------------------------");

    // Check if there are any transactions recorded during the session.
    if (transactions.Count == 0)
    {
        Console.WriteLine("No transactions recorded.");
        return;
    }

    // Go through each transaction in the list and display its details.
    foreach (InventoryTransaction transaction in transactions)
    {
        Console.WriteLine($"Product ID: {transaction.ProductId}");
        Console.WriteLine($"Type: {transaction.TransactionType}");
        Console.WriteLine($"Quantity Changed: {transaction.QuantityChanged:+#;-#;0}");
        Console.WriteLine($"Date: {transaction.TransactionDate:g}");
        Console.WriteLine("------------------------------------");
    }
}