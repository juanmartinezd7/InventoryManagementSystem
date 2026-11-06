using System.Text.Json;

public static class InventoryStorage
{
    private static readonly string FilePath = "inventory.json";

    // Loads products from the JSON file.
    public static List<Product> LoadProducts()
    {
        // If the file doesn't exist, return an empty list.
        if (!File.Exists(FilePath))
        {
            return new List<Product>();
        }

        // Read the JSON file and deserialize it into a list of products.
        try
        {
            string json = File.ReadAllText(FilePath);

            List<Product>? products =
                JsonSerializer.Deserialize<List<Product>>(json);

            return products ?? new List<Product>();
        }
        // Catch any exceptions that occur during file reading or deserialization.
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading inventory: {ex.Message}");
            throw;
        }
    }

    // Saves the current inventory to the JSON file.
    public static void SaveProducts(List<Product> products)
    {
        // Serialize the list of products to JSON and write it to the file.
        try
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(products, options);

            File.WriteAllText(FilePath, json);
        }
        // Catch any exceptions that occur during serialization or file writing.
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving inventory: {ex.Message}");
            throw;
        }
    }
}