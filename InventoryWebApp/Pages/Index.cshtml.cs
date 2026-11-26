// Licensed to the .NET Foundation under one or more agreements.
using Microsoft.AspNetCore.Mvc.RazorPages;
// Licensed under the MIT license.
namespace InventoryWebApp.Pages;
// The IndexModel class represents the model for the Index page of the InventoryWebApp.
public class IndexModel : PageModel
{
    // Properties to hold application name, welcome message, current date and time, and product count.
    public string ApplicationName { get; private set; } = string.Empty;
    public string WelcomeMessage { get; private set; } = string.Empty;
    public DateTime CurrentDateTime { get; private set; } = DateTime.Now;

    // Stores the number of active products in the database.
    public int ProductCount { get; private set; }
    // The OnGet method is called when the Index page is accessed via a GET request.
    public void OnGet()
    {
        ApplicationName = "Inventory Management System";
        WelcomeMessage = "Welcome to your web-based inventory management system.";
        CurrentDateTime = DateTime.Now;

        // Retrieve active products from SQLite.
        List<Product> products = DatabaseManager.LoadProducts();

        // Count the number of products retrieved.
        ProductCount = products.Count;
    }
}


