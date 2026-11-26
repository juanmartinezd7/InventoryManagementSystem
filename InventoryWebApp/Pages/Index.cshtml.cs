// Licensed to the .NET Foundation under one or more agreements.
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryWebApp.Pages;

public class IndexModel : PageModel
{
    // Properties to hold the application name, welcome message, and current date/time.
    public string ApplicationName { get; private set; } = string.Empty;
    public string WelcomeMessage { get; private set; } = string.Empty;
    public DateTime CurrentDateTime { get; private set; }
    // This method is called when the page is accessed via a GET request.
    public void OnGet()
    {
        ApplicationName = "Inventory Management System";
        WelcomeMessage = "Welcome to your web-based inventory management system.";
        CurrentDateTime = DateTime.Now;
    }
}

