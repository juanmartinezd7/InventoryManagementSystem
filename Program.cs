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
            Console.WriteLine("View All Products selected.");
            break;

        case "2":
            Console.WriteLine("Add Product selected.");
            break;

        case "3":
            Console.WriteLine("Update Product selected.");
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
