# Login GUI Based on `PAMC.DatabaseConnection.FileLogin`

## TL;DR

```csharp
// Create a new instance of the LoginForm
PAMC.LoginGui.LoginForm loginForm = new PAMC.LoginGui.LoginForm();

// Display the LoginForm as a modal dialog
loginForm.ShowDialog();

// After the LoginForm is closed, check if the user chose to continue
if (loginForm.Continue)
{
    // Start the main application by running the mainForm
    Application.Run(new mainForm());
}
```

## 1. Introduction

The **Login GUI** is a Windows Forms application that provides a user interface for entering database connection details and authenticating with a SQL Server database. It leverages the `FileLogin` class from the `PAMC.DatabaseConnection` namespace to manage connection strings and interactions with the `login.txt` file.

Upon a successful login, the application updates the `login.txt` file with the new or updated connection details, ensuring that future sessions can utilize the saved settings.

**Current Version uses and Updates the "Default" Connection only**

## 2. Features

- **User-Friendly Interface:** Simple and intuitive form for entering server, database, username, and password.
- **Pre-Filled Fields:** Automatically populates fields with existing connection information if available.
- **Connection Validation:** Attempts to establish a connection to the database to verify credentials.
- **Persistent Storage:** Updates the `login.txt` file with encrypted connection details upon successful login.
- **Error Handling:** Provides user-friendly error messages without exposing sensitive information.

## 3. Usage Example

### 3.1. Running the Application

1. **Launch the Application:** Start the Login GUI executable.
2. **Enter Connection Details:**
   - **Server:** The name or IP address of the SQL Server instance.
   - **Database:** The name of the database to connect to.
   - **Username:** Your SQL Server username.
   - **Password:** Your SQL Server password.
3. **Login:**
   - Click the **Login** button to attempt a connection.
   - If the login is successful, a confirmation message will appear, and the application will update the `login.txt` file with the new connection details.
   - If the login fails, an error message will inform you of the issue.

### 3.2. Updating Connection Details

- **Current Version uses and Updates the "Default" Connection only**
- **(Unimplemented) New Connections:**
  - If you are connecting to a new server or database, simply enter the new details and click **Login**.
  - The application will add the new connection to the `login.txt` file upon successful authentication.
- **Updating Existing Connections:**
  - If you modify the connection details for an existing connection, the application will update the corresponding entry in the `login.txt` file after a successful login.

## 4. Behind the Scenes

### 4.1. `FileLogin` Class Integration

- The `FileLogin` class handles reading from and writing to the `login.txt` file.
- It encrypts sensitive information like usernames and passwords using the `Sugoi.Security.Rijndael` encryption methods.
- Provides methods to decrypt and populate connection strings for use in the application.

### 4.2. Login Process Flow

1. **Initialization:**
   - The application calls `FileLogin.DecryptFilePopulateConnections()` to populate the connection strings.
   - If a default connection is available, it pre-fills the login form fields.

2. **User Input:**
   - The user enters or modifies the connection details.

3. **Authentication:**
   - Upon clicking **Login**, the application attempts to establish a `SqlConnection` using the provided details.
   - If the connection is successful, it proceeds to update the `login.txt` file.

4. **Updating `login.txt`:**
   - The `FileLogin.UpdateLoginFile()` method is called with the connection name (e.g., "Default") and the connection string.
   - The method encrypts the connection details and writes them to `login.txt`, replacing existing entries if necessary.

## 5. Error Handling

- The application catches and handles exceptions related to database connections.
- Provides generic error messages to the user to avoid exposing sensitive information.
- Ensures that any resources, such as open connections, are properly disposed of in the event of an error.

## 6. Security Considerations

- **Encryption:** Uses strong encryption algorithms to protect sensitive data in `login.txt`.
- **Password Handling:** Passwords are not stored in plain text and are securely managed within the application.
- **Exception Management:** Avoids displaying detailed error messages that could reveal internal application logic or database structure.

## 7. Conclusion

The Login GUI provides a secure and user-friendly interface for managing database connections within applications that utilize the `PAMC.DatabaseConnection.FileLogin` class. By updating the `login.txt` file upon successful login, it ensures that connection settings are persistently and securely stored for future use.