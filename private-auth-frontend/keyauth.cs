using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PrivateAuth
{
    public class KeyAuthAPI
    {
        private string appName;
        private string ownerId;
        private string version;
        private string apiUrl = "https://api-s736.onrender.com/api"; // Your deployed API server
        
        public KeyAuthAPI(string name, string ownerid, string version)
        {
            this.appName = name;
            this.ownerId = ownerid;
            this.version = version;
        }

        // Login with username and password
        public async Task<AuthResponse> Login(string username, string password)
        {
            try
            {
                var loginData = new
                {
                    type = "login",
                    username = username,
                    password = password,
                    app_name = appName,
                    owner_id = ownerId,
                    version = version
                };

                var response = await SendRequest(loginData);
                return JsonConvert.DeserializeObject<AuthResponse>(response);
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    success = false,
                    message = $"Login failed: {ex.Message}"
                };
            }
        }

        // Register new user
        public async Task<AuthResponse> Register(string username, string password, string licenseKey)
        {
            try
            {
                var registerData = new
                {
                    type = "register",
                    username = username,
                    password = password,
                    license_key = licenseKey,
                    app_name = appName,
                    owner_id = ownerId,
                    version = version
                };

                var response = await SendRequest(registerData);
                return JsonConvert.DeserializeObject<AuthResponse>(response);
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    success = false,
                    message = $"Registration failed: {ex.Message}"
                };
            }
        }

        // Verify license key
        public async Task<AuthResponse> VerifyLicense(string licenseKey)
        {
            try
            {
                var verifyData = new
                {
                    type = "verify_license",
                    license_key = licenseKey,
                    app_name = appName,
                    owner_id = ownerId,
                    version = version
                };

                var response = await SendRequest(verifyData);
                return JsonConvert.DeserializeObject<AuthResponse>(response);
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    success = false,
                    message = $"License verification failed: {ex.Message}"
                };
            }
        }

        // Check if user is online
        public async Task<AuthResponse> CheckOnline(string username)
        {
            try
            {
                var onlineData = new
                {
                    type = "check_online",
                    username = username,
                    app_name = appName,
                    owner_id = ownerId,
                    version = version
                };

                var response = await SendRequest(onlineData);
                return JsonConvert.DeserializeObject<AuthResponse>(response);
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    success = false,
                    message = $"Online check failed: {ex.Message}"
                };
            }
        }

        // Send HTTP request to API
        private async Task<string> SendRequest(object data)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await client.PostAsync(apiUrl, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }

    // Response model
    public class AuthResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public UserData user { get; set; }
        public LicenseData license { get; set; }
    }

    // User data model
    public class UserData
    {
        public string username { get; set; }
        public string password { get; set; }
        public DateTime expiry_date { get; set; }
        public string license_key { get; set; }
        public DateTime created_at { get; set; }
        public string status { get; set; }
    }

    // License data model
    public class LicenseData
    {
        public string key { get; set; }
        public DateTime expiry_date { get; set; }
        public string description { get; set; }
        public DateTime created_at { get; set; }
        public string status { get; set; }
    }

    // Main application class
    public class Program
    {
        // Initialize KeyAuth API
        public static KeyAuthAPI KeyAuthApp = new KeyAuthAPI(
            name: "EXM",                    // App name - USER CAN CHANGE THIS
            ownerid: "X3lapZ9vPT",         // Account ID - USER CAN CHANGE THIS  
            version: "1.5"                 // Application version - USER CAN CHANGE THIS
        );

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Private Auth - C# Application");
            Console.WriteLine("=============================");
            
            while (true)
            {
                Console.WriteLine("\nChoose an option:");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Verify License");
                Console.WriteLine("4. Check Online");
                Console.WriteLine("5. Exit");
                
                Console.Write("Enter your choice (1-5): ");
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        await HandleLogin();
                        break;
                    case "2":
                        await HandleRegister();
                        break;
                    case "3":
                        await HandleVerifyLicense();
                        break;
                    case "4":
                        await HandleCheckOnline();
                        break;
                    case "5":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static async Task HandleLogin()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            
            Console.WriteLine("Logging in...");
            var response = await KeyAuthApp.Login(username, password);
            
            if (response.success)
            {
                Console.WriteLine($"✅ Login successful!");
                Console.WriteLine($"Welcome, {response.user.username}!");
                Console.WriteLine($"License: {response.user.license_key}");
                Console.WriteLine($"Expires: {response.user.expiry_date}");
            }
            else
            {
                Console.WriteLine($"❌ Login failed: {response.message}");
            }
        }

        private static async Task HandleRegister()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            
            Console.Write("Enter license key: ");
            string licenseKey = Console.ReadLine();
            
            Console.WriteLine("Registering...");
            var response = await KeyAuthApp.Register(username, password, licenseKey);
            
            if (response.success)
            {
                Console.WriteLine($"✅ Registration successful!");
                Console.WriteLine($"User: {response.user.username}");
                Console.WriteLine($"License: {response.user.license_key}");
                Console.WriteLine($"Expires: {response.user.expiry_date}");
            }
            else
            {
                Console.WriteLine($"❌ Registration failed: {response.message}");
            }
        }

        private static async Task HandleVerifyLicense()
        {
            Console.Write("Enter license key: ");
            string licenseKey = Console.ReadLine();
            
            Console.WriteLine("Verifying license...");
            var response = await KeyAuthApp.VerifyLicense(licenseKey);
            
            if (response.success)
            {
                Console.WriteLine($"✅ License is valid!");
                Console.WriteLine($"Key: {response.license.key}");
                Console.WriteLine($"Description: {response.license.description}");
                Console.WriteLine($"Expires: {response.license.expiry_date}");
            }
            else
            {
                Console.WriteLine($"❌ License verification failed: {response.message}");
            }
        }

        private static async Task HandleCheckOnline()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            
            Console.WriteLine("Checking online status...");
            var response = await KeyAuthApp.CheckOnline(username);
            
            if (response.success)
            {
                Console.WriteLine($"✅ User is online!");
                Console.WriteLine($"Username: {response.user.username}");
                Console.WriteLine($"Status: {response.user.status}");
            }
            else
            {
                Console.WriteLine($"❌ User is offline or not found: {response.message}");
            }
        }
    }
}
