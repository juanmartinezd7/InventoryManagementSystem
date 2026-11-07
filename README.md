# Overview

The Inventory Management System is a console-based application developed in C# that allows users to manage product information and track inventory levels. The software provides features for adding, updating, deleting, and searching products, as well as adjusting stock quantities, identifying low-stock products, calculating total inventory value, and viewing stock transaction history.

I developed this software to strengthen my skills as a software engineer by gaining hands-on experience with C# syntax, object-oriented programming, data structures, input validation, and file management. My goal was to build a practical application that demonstrates how programming concepts can work together to solve real-world inventory management problems.

The application uses JSON file storage to preserve product information between sessions. Stock transaction history is recorded during the current session.

[Software Demo Video](https://youtu.be/Z_lHu_Z4DhM)

# Development Environment

I developed this software using Visual Studio Code as my code editor, the .NET 10 SDK to build and run the application, and Git and GitHub for version control and source code management.

The programming language used is **C#**, a modern, object-oriented programming language developed by Microsoft. The application demonstrates variables, data types, arithmetic expressions, conditional statements, loops, methods, classes, structures, and collections.

I used the built-in `System.Text.Json` library to serialize and deserialize product information, allowing the application to read and write inventory data in JSON format. I also used the `System.IO` file operations and the `List<T>` collection to manage inventory data.

# Useful Websites

- [Microsoft Learn — C# Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [Microsoft Learn — .NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Microsoft Learn — System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/overview)
- [Microsoft Learn — C# Classes and Structs](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/)
- [Git Documentation](https://git-scm.com/doc)

# Future Work

- **Implement SQL database integration:** Replace JSON file storage with a relational database to improve data management, support more complex queries, and prepare the application for future expansion.
- **Save transaction history permanently:** Store stock transactions in a database or JSON file so users can review previous inventory movements after restarting the application.
- **Develop a web-based interface:** Convert the console application into an ASP.NET Core web application to provide a more user-friendly interface and allow inventory management through a web browser.
- **Improve reporting and analytics:** Add additional reports, such as inventory movement summaries, stock alerts, and product category analysis.
- **Enhance error handling and validation:** Improve how the application handles unexpected errors, invalid product information, and file storage issues.