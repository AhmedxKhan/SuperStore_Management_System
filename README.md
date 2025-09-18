🛒 SuperStore Management System

A C# Windows Forms Application connected to SQL Server 2019, built for managing products, employees, and user accounts in a retail store.
This project demonstrates CRUD operations (Create, Read, Update, Delete), authentication, and database connectivity in a simple desktop app.

✨ Features

• User Authentication

• Sign Up / Sign In functionality with SQL database.

• Product Management

• Add new products with details (name, price, quantity, Mfg/Expiry dates, packing).

• Update existing products.

• Delete products.

• Search products using product name.

• Data Display

• Products are shown in a DataGridView for easy management.

• Employee Management (via SQL Queries)

• Promote/demote employees with update queries.

• Delete employee records using delete queries.

• Database Integration

• Secure connection with SQL Server 2019 database (SuperStoreDB).

🖥️ Forms Overview
🔑 Sign Up Form

• Fields: Username, Password.

• Stores new user credentials in the database.

• Button: Register → inserts a new record into Users table.

🔐 Sign In Form

• Fields: Username, Password.

• Validates credentials against the database.

• Button: Login → grants access to the system.

📦 Product Management Form (Main Form)

• TextBoxes:

• txtproductname, txtprice, txtMfgdate, txtexpirydate, txtquantity, txtpacking.

• Buttons:

• btnAdd → Inserts a new product into database.

• btnUpdate → Updates selected product.

• btnDelete → Deletes selected product.

• btnSearch → Finds a product by name.

• DataGridView (dataGridView1):

• Displays all products in the Products table.

⚙️ Technologies Used

• C# (Windows Forms, .NET Framework)

• SQL Server 2019 (Database)

• Visual Studio 2022 (IDE)

📂 Database Design

• Database: SuperStoreDB
• Table: Products

• ProductID (Primary Key, Auto Increment)

• ProductName

• Price

• MfgDate

• ExpiryDate

• Quantity

• Packing

• Table: Users

• UserID (Primary Key)

• Username

•Password

🚀 How to Run the Project

1. Clone this repository:
   git clone https://github.com/AhmedxKhan/SuperStore_Management_System.git
   
2. Open SuperStore.sln in Visual Studio 2022.
3. Update the connection string in App.config with your SQL Server details. Example:
   <connectionStrings>
    <add name="SuperStoreDB"
         connectionString="Data Source=.;Initial Catalog=SuperStoreDB;Integrated Security=True"
         providerName="System.Data.SqlClient" />
</connectionStrings>

4. Restore the database:

• Create a new DB named SuperStoreDB.

• Run SQL scripts to create Products and Users tables.

5. Build and run the project.

📸 Screenshots (Add Later)

• Sign Up Form

• Sign In Form

• Product Management Dashboard

• SQL Queries (Select, Update, Delete)

📜 License

This project is licensed under the MIT License – free to use and modify.
