// Store all products currently available in the inventory.
List<Product> products = new List<Product>();

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
    break;

        case "3":
    UpdateProduct(products);
    break;

        case "4":
            Console.WriteLine("Delete Product selected.");
            break;

        case "5":
            Console.WriteLine("Search Products selected.");
            break;

        case "6":
            Console.WriteLine("Add Stock selected.");
            break;

        case "7":
            Console.WriteLine("Remove Stock selected.");
            break;

        case "8":
            Console.WriteLine("View Low-Stock Products selected.");
            break;

        case "9":
            Console.WriteLine("View Inventory Value selected.");
            break;

        case "0":
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
    Console.WriteLine("ALL PRODUCTS");
    Console.WriteLine("------------------------------------");

    foreach (Product product in products)
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
}

// Adds a new product to the inventory using information entered by the user.
static void AddProduct(List<Product> products)
{
    Console.WriteLine("ADD PRODUCT");
    Console.WriteLine("------------------------------------");

    Console.Write("Enter SKU: ");
    string? sku = Console.ReadLine();

    Console.Write("Enter product name: ");
    string? name = Console.ReadLine();

    Console.Write("Enter category: ");
    string? category = Console.ReadLine();

    decimal price;

    while (true)
    {
        Console.Write("Enter price: $");
        string? input = Console.ReadLine();

        if (decimal.TryParse(input, out price) && price >= 0)
        {
            break;
        }

        Console.WriteLine("Invalid price. Please enter a valid number.");
    }

    int quantity;

    while (true)
    {
        Console.Write("Enter quantity: ");
        string? input = Console.ReadLine();

        if (int.TryParse(input, out quantity) && quantity >= 0)
        {
            break;
        }

        Console.WriteLine("Invalid quantity. Please enter a whole number.");
    }

    int reorderLevel;

    while (true)
    {
        Console.Write("Enter reorder level: ");
        string? input = Console.ReadLine();

        if (int.TryParse(input, out reorderLevel) && reorderLevel >= 0)
        {
            break;
        }

        Console.WriteLine("Invalid reorder level. Please enter a whole number.");
    }

    int newId = products.Count + 1;

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

    products.Add(newProduct);

    Console.WriteLine();
    Console.WriteLine($"Product '{newProduct.Name}' added successfully.");
}

// Updates an existing product selected by its ID.
static void UpdateProduct(List<Product> products)
{
    Console.WriteLine("UPDATE PRODUCT");
    Console.WriteLine("------------------------------------");

    Console.Write("Enter product ID to update: ");
    string? input = Console.ReadLine();

    if (!int.TryParse(input, out int id))
    {
        Console.WriteLine("Invalid ID. Please enter a whole number.");
        return;
    }

    Product? productToUpdate = products.Find(product => product.Id == id);

    if (productToUpdate == null)
    {
        Console.WriteLine($"Product with ID {id} was not found.");
        return;
    }

    Console.WriteLine();
    Console.WriteLine($"Updating: {productToUpdate.Name}");
    Console.WriteLine("Press Enter without typing anything to keep the current value.");
    Console.WriteLine();

    Console.Write($"SKU ({productToUpdate.Sku}): ");
    string? sku = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(sku))
    {
        productToUpdate.Sku = sku;
    }

    Console.Write($"Name ({productToUpdate.Name}): ");
    string? name = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(name))
    {
        productToUpdate.Name = name;
    }

    Console.Write($"Category ({productToUpdate.Category}): ");
    string? category = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(category))
    {
        productToUpdate.Category = category;
    }

    Console.Write($"Price (${productToUpdate.Price:F2}): ");
    string? priceInput = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(priceInput))
    {
        if (decimal.TryParse(priceInput, out decimal newPrice) && newPrice >= 0)
        {
            productToUpdate.Price = newPrice;
        }
        else
        {
            Console.WriteLine("Invalid price. The current price was kept.");
        }
    }

    Console.Write($"Quantity ({productToUpdate.Quantity}): ");
    string? quantityInput = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(quantityInput))
    {
        if (int.TryParse(quantityInput, out int newQuantity) && newQuantity >= 0)
        {
            productToUpdate.Quantity = newQuantity;
        }
        else
        {
            Console.WriteLine("Invalid quantity. The current quantity was kept.");
        }
    }

    Console.Write($"Reorder Level ({productToUpdate.ReorderLevel}): ");
    string? reorderInput = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(reorderInput))
    {
        if (int.TryParse(reorderInput, out int newReorderLevel) && newReorderLevel >= 0)
        {
            productToUpdate.ReorderLevel = newReorderLevel;
        }
        else
        {
            Console.WriteLine("Invalid reorder level. The current reorder level was kept.");
        }
    }

    Console.WriteLine();
    Console.WriteLine($"Product '{productToUpdate.Name}' updated successfully.");
}