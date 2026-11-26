using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryWebApp.Pages;

public class AddProductModel : PageModel
{
    // Bind the NewProduct property to the form data submitted by the user.
    [BindProperty]
    public Product NewProduct { get; set; } = new();
    // Message property to display validation results or success messages.
    public string Message { get; private set; } = string.Empty;
    // Handles GET requests to display the initial form.
    public void OnGet()
    {
        // Displays the initial form.
    }
    // Handles POST requests when the form is submitted.
    public IActionResult OnPost()
    {
        // Validate required text fields.
        if (string.IsNullOrWhiteSpace(NewProduct.Sku))
        {
            ModelState.AddModelError(
                "NewProduct.Sku",
                "SKU is required.");
        }

        if (string.IsNullOrWhiteSpace(NewProduct.Name))
        {
            ModelState.AddModelError(
                "NewProduct.Name",
                "Product name is required.");
        }

        if (string.IsNullOrWhiteSpace(NewProduct.Category))
        {
            ModelState.AddModelError(
                "NewProduct.Category",
                "Category is required.");
        }

        // Validate the product price.
        if (NewProduct.Price < 0)
        {
            ModelState.AddModelError(
                "NewProduct.Price",
                "Price cannot be negative.");
        }

        if (decimal.Round(NewProduct.Price, 2) != NewProduct.Price)
        {
            ModelState.AddModelError(
                "NewProduct.Price",
                "Price cannot have more than two decimal places.");
        }

        // Validate inventory quantities.
        if (NewProduct.Quantity < 0)
        {
            ModelState.AddModelError(
                "NewProduct.Quantity",
                "Quantity cannot be negative.");
        }

        if (NewProduct.ReorderLevel < 0)
        {
            ModelState.AddModelError(
                "NewProduct.ReorderLevel",
                "Reorder level cannot be negative.");
        }

        // Return the form if validation fails.
        if (!ModelState.IsValid)
        {
            return Page();
        }


        // Save the validated product to the SQLite database.
        try
        {
            int newProductId = DatabaseManager.InsertProduct(NewProduct);

            // Redirect to the Inventory page after successful insertion.
            return RedirectToPage("/Inventory");
        }
        catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067)
        {
            // Handle a duplicate SKU.
            ModelState.AddModelError(
                "NewProduct.Sku",
                "A product with this SKU already exists.");
        }
        catch (SqliteException)
        {
            // Handle other database errors.
            ModelState.AddModelError(
                string.Empty,
                "Unable to save the product due to a database error.");
        }
        catch (ArgumentException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Invalid product information.");
        }
        catch (OverflowException)
        {
            ModelState.AddModelError(
                string.Empty,
                "A product value exceeds the supported numeric range.");
        }

        // Redisplay the form if an error occurs.
        return Page();

    }
}

