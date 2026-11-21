# Overview

The Inventory Management System is a console-based application developed using C# and .NET 10 that allows users to manage product information, track inventory quantities, and monitor stock movements. The application integrates with a SQLite relational database to store product information and transaction history permanently.

The program provides a menu with ten features that allow users to view, add, update, delete, and search products; add and remove stock; identify low-stock products; calculate total inventory value; and view stock transaction history. Users interact with the application through the console by selecting a menu option and following the instructions. All inventory information is stored in the SQLite database, allowing the data to remain available after closing and restarting the program.

I developed this software to strengthen my skills as a software engineer by learning how to design and implement relational databases, write SQL queries, and connect a C# application to a database. My goal was to understand how relational databases can be used to organize information, maintain data integrity, and support real-world software applications. I also wanted to gain practical experience using SQL commands, primary and foreign keys, database constraints, and transactions.

[Software Demo Video](https://youtu.be/GBwl0mCfac4)

# Relational Database

The application uses **SQLite**, a lightweight relational database management system that stores information in a local database file named `inventory.db`. SQLite does not require a separate database server, making it a practical choice for a console-based application.

I used the `Microsoft.Data.Sqlite` library to connect the C# application to the SQLite database and execute SQL commands. The application uses SQL statements such as `CREATE TABLE`, `INSERT`, `SELECT`, and `UPDATE` to manage inventory information.

The relational database contains two tables:

**1. Products**

The `Products` table stores information about each inventory item. It contains the following columns:

- `Id`: A unique identifier for each product and the primary key.
- `Sku`: A unique stock-keeping unit used to identify a product.
- `Name`: The name of the product.
- `Category`: The category to which the product belongs.
- `PriceCents`: The product price stored as an integer number of cents to maintain monetary accuracy.
- `Quantity`: The number of units currently available.
- `ReorderLevel`: The minimum inventory level used to identify products that need restocking.
- `IsActive`: Indicates whether a product is active or has been soft-deleted.

**2. InventoryTransactions**

The `InventoryTransactions` table stores information about stock movements. It contains the following columns:

- `Id`: A unique identifier for each transaction and the primary key.
- `ProductId`: A foreign key that references the `Id` column in the `Products` table.
- `TransactionType`: Identifies whether stock was added (`ADD`) or removed (`REMOVE`).
- `QuantityChanged`: Records the number of units added or removed.
- `TransactionDate`: Stores the date and time of the transaction.

The relationship between these tables is **one-to-many**. One product can have multiple inventory transactions, but each transaction belongs to one product.

I implemented database constraints to prevent duplicate SKUs, negative prices, and negative inventory quantities. I also used parameterized SQL queries to safely handle user input.

When adding or removing stock, the application uses database transactions to update the product quantity and record the stock movement together. This helps maintain consistency between the two tables.

The application also uses soft deletion. Instead of permanently removing a product from the database, it changes the `IsActive` value to indicate that the product is inactive. This allows historical stock transactions to remain associated with the product.

# Development Environment

I developed this software using **Visual Studio Code** as my code editor and the **.NET 10 SDK** to build and run the application. I used the terminal to execute the program and test its features. I also used Git and GitHub for version control and source code management.

The programming language used is **C#**, an object-oriented programming language developed by Microsoft.

I used the following technologies and libraries:

- **C#:** To implement the application logic, user interface, input validation, and inventory operations.
- **.NET 10:** To build and execute the console application.
- **SQLite:** To create and manage the relational database.
- **Microsoft.Data.Sqlite:** To establish database connections and execute SQL commands from C#.
- **System.Collections.Generic:** To work with collections of product objects.
- **Git and GitHub:** To track changes and maintain the project repository.

# Useful Websites

- [Microsoft Learn — C# Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [Microsoft Learn — .NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Microsoft Learn — Microsoft.Data.Sqlite](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/)
- [SQLite Documentation](https://www.sqlite.org/docs.html)
- [SQLite — CREATE TABLE](https://www.sqlite.org/lang_createtable.html)
- [SQLite — Foreign Key Support](https://www.sqlite.org/foreignkeys.html)
- [SQLite — Transactions](https://www.sqlite.org/lang_transaction.html)
- [Git Documentation](https://git-scm.com/doc)

# Future Work

- **Develop a web-based interface:** Convert the console application into an ASP.NET Core web application to allow users to manage inventory through a browser.
- **Improve reporting and analytics:** Add reports for inventory movement summaries, product category analysis, and stock trends.
- **Enhance error handling and validation:** Improve how the application handles unexpected database errors and invalid user input.
- **Add user authentication and authorization:** Implement user accounts and permissions to control access to inventory information.
- **Expand transaction tracking:** Record additional inventory events, such as opening stock quantities and manual inventory adjustments, to provide a more complete audit history.
