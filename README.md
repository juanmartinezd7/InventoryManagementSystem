# Overview

The Inventory Management System is a console-based application developed in C# that allows users to manage product information and track inventory levels. The software provides features for adding, updating, deleting, and searching products, as well as adjusting stock quantities, identifying low-stock products, calculating total inventory value, and viewing stock transaction history.

I developed this software to strengthen my skills as a software engineer by gaining hands-on experience with C# syntax, object-oriented programming, relational databases, SQL queries, input validation, and database transactions. My goal was to build a practical application that demonstrates how programming concepts can work together to solve real-world inventory management problems.

The application uses a SQLite relational database to preserve product information and stock transaction history between sessions. Product prices are stored as integer cents to maintain monetary accuracy. Stock additions and removals are recorded using database transactions to ensure that inventory quantities and transaction history remain consistent. The application also uses soft deletion to preserve historical records when products are removed from the active inventory.

[Software Demo Video](https://youtu.be/GBwl0mCfac4)

# Development Environment

I developed this software using Visual Studio Code as my code editor, the .NET 10 SDK to build and run the application, and Git and GitHub for version control and source code management.

The programming language used is **C#**, a modern, object-oriented programming language developed by Microsoft. The application demonstrates variables, data types, arithmetic expressions, conditional statements, loops, methods, classes, and collections.

I used **SQLite** as the relational database management system and the **Microsoft.Data.Sqlite** library to connect the C# application to the database. SQL commands are used to create tables, insert products, update product information, search inventory, and generate reports.

The application stores product information in a `Products` table and stock movement records in an `InventoryTransactions` table. It uses parameterized SQL queries, database constraints, and transactions to help maintain data integrity.

# Useful Websites

- [Microsoft Learn — C# Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [Microsoft Learn — .NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Microsoft Learn — Microsoft.Data.Sqlite](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/)
- [SQLite Documentation](https://www.sqlite.org/docs.html)
- [Microsoft Learn — C# Classes and Structs](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/)
- [Git Documentation](https://git-scm.com/doc)

# Future Work

- **Develop a web-based interface:** Convert the console application into an ASP.NET Core web application to provide a more user-friendly interface and allow inventory management through a web browser.
- **Improve reporting and analytics:** Add additional reports, such as inventory movement summaries, stock alerts, and product category analysis.
- **Enhance error handling and validation:** Improve how the application handles unexpected database errors, invalid product information, and unusual inventory operations.
- **Add user authentication and authorization:** Implement user accounts and permissions to control who can view or modify inventory information.
- **Expand transaction tracking:** Record additional inventory events, such as opening stock quantities and product adjustments, to provide a more complete audit history.
