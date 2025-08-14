using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;


namespace PAMC.DatabaseConnection
{
    public class FileLogin
    {
        private static ConcurrentDictionary<string, string> _databaseConnections = new ConcurrentDictionary<string, string>();
        //Expose Connections by name an passwordless connectionstring
        public static ConcurrentDictionary<string, string> DatabaseConnections
        {
            get
            {
                return _databaseConnections;
            }
        }

        // Overloaded method without baseFolder parameter, it gets the executable path and calls the main method.
        public static void GetApplicationConnection(ref SqlConnection _cn, ref string _cnString)
        {
            SqlConnection _cnRep = new SqlConnection();
            SqlConnection _cnPort = new SqlConnection(); ;
            SqlConnection _cnCpsRep = new SqlConnection();
            string _cnRepString = "";
            string _cnPortString = "";
            string _cnCpsRepString = "";

            // Get the directory of the currently executing assembly.
            String executablePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Call the main method with the executable path.
            GetApplicationConnections(executablePath, ref _cn, ref _cnRep, ref _cnPort,
                ref _cnCpsRep, ref _cnString, ref _cnRepString, ref _cnPortString, ref _cnCpsRepString);
        }

        // Overloaded method without baseFolder parameter, it gets the executable path and calls the main method.
        public static void GetApplicationConnections(
            ref SqlConnection _cn, ref SqlConnection _cnRep, ref SqlConnection _cnPort,
            ref SqlConnection _cnCpsRep, ref string _cnString, ref string _cnRepString,
            ref string _cnPortString, ref string _cnCpsRepString)
        {
            // Get the directory of the currently executing assembly.
            String executablePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Call the main method with the executable path.
            GetApplicationConnections(executablePath, ref _cn, ref _cnRep, ref _cnPort,
                ref _cnCpsRep, ref _cnString, ref _cnRepString, ref _cnPortString, ref _cnCpsRepString);
        }

        // Main method to get application connections using the provided base folder.
        public static void GetApplicationConnections(
            string baseFolder, ref SqlConnection _cn, ref SqlConnection _cnRep,
            ref SqlConnection _cnPort, ref SqlConnection _cnCpsRep, ref string _cnString,
            ref string _cnRepString, ref string _cnPortString, ref string _cnCpsRepString)
        {
            ListDictionary ld = ReadLoginFile(baseFolder);
            DecryptListAndLogin(ref _cn, ref _cnRep, ref _cnPort, ref _cnCpsRep,
                ref _cnString, ref _cnRepString, ref _cnPortString, ref _cnCpsRepString, ld);
        }

        private static ListDictionary ReadLoginFile(string baseFolder)
        {
            ListDictionary ld = new ListDictionary();
            string loginPath = Path.Combine(baseFolder, "login.txt");
            FileInfo logiFile = new FileInfo(loginPath);
            if (!logiFile.Exists)
            {
                Console.WriteLine("Login File Not Found!");
                throw new Exception("Login.txt file not found.");
            }
            else
            {
                using (StreamReader rd = logiFile.OpenText())
                {
                    while (!rd.EndOfStream)
                    {
                        string connection = rd.ReadLine();
                        string[] splitConnection = connection.Split(new char[] { '|' });
                        if (splitConnection.Length == 5)
                        {
                            Array.Resize(ref splitConnection, 6);
                            splitConnection[5] = "False";
                        }
                        ld.Add(splitConnection[0], splitConnection);
                    }
                    rd.Close();
                }
            }

            return ld;
        }

        private static void DecryptListAndLogin(
            ref SqlConnection _cn, ref SqlConnection _cnRep, ref SqlConnection _cnPort,
            ref SqlConnection _cnCpsRep, ref string _cnString, ref string _cnRepString,
            ref string _cnPortString, ref string _cnCpsRepString, ListDictionary ld)

        {
            if (ld.Count > 0)
            {
                string connectionMsg = "";
                foreach (var item in ld.Keys)
                {
                    SqlConnection cn = null;
                    string[] details = (string[])ld[item];
                    string server = "";
                    string username = "";
                    string database = "";


                    try
                    {
                        if (details.Length>4)
                        {
                            string conName = details[0];
                            server = Sugoi.Security.Rijndael.Decrypt(details[4]);
                            bool integratedSecurity = details.Length > 5 && details[5].Equals("True", StringComparison.OrdinalIgnoreCase);
                            username = string.IsNullOrEmpty(details[1]) ? "" : Sugoi.Security.Rijndael.Decrypt(details[1]);
                            string password = string.IsNullOrEmpty(details[2]) ? "" : Sugoi.Security.Rijndael.Decrypt(details[2]);
                            database = Sugoi.Security.Rijndael.Decrypt(details[3]);

                            string cnString = integratedSecurity
                                ? $"Server = {server}; Database = {database}; Integrated Security=True; Encrypt=True;TrustServerCertificate=True;"
                                : $"Server = {server}; Database = {database}; User Id = {username}; Password = \"{password}\"; Encrypt=True;TrustServerCertificate=True;";
                            FileLogin._databaseConnections[item.ToString()] = integratedSecurity
                                ? $"Server = {server}; Database = {database}; Integrated Security=True; Encrypt=True;TrustServerCertificate=True;"
                                : $"Server = {server}; Database = {database}; User Id = {username}; Encrypt=True;TrustServerCertificate=True;";

                            cn = new SqlConnection(cnString);
                            cn.Open();

                            connectionMsg += $"CONNECTION SUCCESS : {server} - User : {username} - Database : {database}. ";
                            if (item.ToString().ToUpper() == "DEFAULT")
                            {
                                _cn = cn;
                                _cnString = cnString;
                            }
                            else if (item.ToString().ToUpper() == "REPORTING")
                            {
                                _cnRep = cn;
                                _cnRepString = cnString;
                            }
                            else if (item.ToString().ToUpper() == "PORTAL")
                            {
                                _cnPort = cn;
                                _cnPortString = cnString;
                            }
                            else if (item.ToString().ToUpper() == "CPSREPORTS")
                            {
                                _cnCpsRep = cn;
                                _cnCpsRepString = cnString;
                            } 
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Connection Failed :{server} - User : {username} - Database : {database}\r\n {ex.Message}");
                        System.Threading.Thread.Sleep(6000);
                        Environment.Exit(0);
                    }
                    finally
                    {
                    }
                }
            }
            else
            {
                Console.WriteLine("Login File is Empty!");
                throw new Exception("Login.txt file is empty!");
            }
        }

        /// <summary>
        /// This method does not log in. I merely populated a Dictionary with Connection Names (key)
        /// and Connection Strin (value) into the DatabaseConnections
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static void DecryptFilePopulateConnections()
        {
            String executablePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ListDictionary ld = ReadLoginFile(executablePath);
            if (ld.Count > 0)
            {
                foreach (var item in ld.Keys)
                {
                    SqlConnection cn = null;
                    string[] details = (string[])ld[item];
                    string server = "";
                    string username = "";
                    string database = "";

                    try
                    {
                        if (details.Length>4)
                        {

                            string conName = details[0];
                            server = Sugoi.Security.Rijndael.Decrypt(details[4]);
                            bool integratedSecurity = details.Length > 5 && details[5].Equals("True", StringComparison.OrdinalIgnoreCase);
                            username = string.IsNullOrEmpty(details[1]) ? "" : Sugoi.Security.Rijndael.Decrypt(details[1]);
                            database = Sugoi.Security.Rijndael.Decrypt(details[3]);

                            FileLogin._databaseConnections[item.ToString()] = integratedSecurity
                                ? $"Server = {server}; Database = {database}; Integrated Security=True; Encrypt=True;TrustServerCertificate=True;"
                                : $"Server = {server}; Database = {database}; User Id = {username}; Encrypt=True;TrustServerCertificate=True;"; 
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Generating Passwordless Connectionstrings Failed:{server} - User : {username} - Database : {database}\r\n {ex.Message}");
                    }
                    finally
                    {
                    }
                }
            }
            else
            {
                Console.WriteLine("Login File is Empty!");
                throw new Exception("Login.txt file is empty!");
            }
        }

        public static void UpdateLoginFile(string connectionName, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionName))
                throw new ArgumentException("Connection name cannot be null or empty.", nameof(connectionName));
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            string server = builder.DataSource;
            string database = builder.InitialCatalog;
            string username = builder.UserID;
            string password = builder.Password;
            bool integratedSecurity = builder.IntegratedSecurity;
            string encryptedUsername = integratedSecurity ? "" : Sugoi.Security.Rijndael.Encrypt(username);
            string encryptedPassword = integratedSecurity ? "" : Sugoi.Security.Rijndael.Encrypt(password);
            string encryptedDatabase = Sugoi.Security.Rijndael.Encrypt(database);
            string encryptedServer = Sugoi.Security.Rijndael.Encrypt(server);
            string newConnectionLine = $"{connectionName}|{encryptedUsername}|{encryptedPassword}|{encryptedDatabase}|{encryptedServer}|{integratedSecurity}";
            string baseFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string loginPath = Path.Combine(baseFolder, "login.txt");
            List<string> allLines = new List<string>();
            if (File.Exists(loginPath))
            {
                allLines = File.ReadAllLines(loginPath).ToList();
            }
            bool connectionFound = false;
            for (int i = 0; i < allLines.Count; i++)
            {
                string line = allLines[i];
                string[] splitLine = line.Split('|');
                if (splitLine.Length > 0)
                {
                    string existingConnectionName = splitLine[0];
                    if (existingConnectionName.Equals(connectionName, StringComparison.OrdinalIgnoreCase))
                    {
                        allLines[i] = newConnectionLine;
                        connectionFound = true;
                        break;
                    }
                }
            }
            if (!connectionFound)
            {
                allLines.Add(newConnectionLine);
            }
            File.WriteAllLines(loginPath, allLines);
        }

    }
}