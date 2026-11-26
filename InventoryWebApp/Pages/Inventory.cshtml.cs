// Licensed to the .NET Foundation under one or more agreements.
// Licensed under the MIT license.
using Microsoft.AspNetCore.Mvc.RazorPages;
// Represents the model for the Inventory page.
namespace InventoryWebApp.Pages;
// The InventoryModel class inherits from PageModel and is responsible for handling the logic of the Inventory page.
public class InventoryModel : PageModel
{
    // Stores the active products retrieved from SQLite.
    public List<Product> Products { get; private set; } = new();
    // Handles GET requests to the Inventory page.
    public void OnGet()
    {
        // Retrieve all active products from the database.
        Products = DatabaseManager.LoadProducts();
    }
}
