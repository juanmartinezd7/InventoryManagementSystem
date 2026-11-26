var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();


// Locate the existing SQLite database in the parent directory.
string databasePath = Path.GetFullPath(
    Path.Combine(
        app.Environment.ContentRootPath,
        "..",
        "inventory.db"
    )
);

// Verify that the existing database file is available.
if (!File.Exists(databasePath))
{
    throw new FileNotFoundException(
        "Inventory database was not found.",
        databasePath
    );
}

// Configure SQLite in read-only mode for initial testing.
DatabaseManager.ConfigureDatabase(databasePath, readOnly: true);

// Display the database location in the terminal.
app.Logger.LogInformation("Inventory database: {DatabasePath}", databasePath);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
