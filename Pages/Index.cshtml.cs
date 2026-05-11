using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Error200Dealership.Pages
{
    public class IndexModel : PageModel
    {
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=error200_dealership;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public string LoginError     { get; set; } = "";
        public string LoginErrorType { get; set; } = ""; // "staff" or "customer"
        public string SignupError    { get; set; } = "";

        public void OnGet() { }

        // ── LOGIN ─────────────────────────────────────────────────────────────
        public IActionResult OnPostLogin(string AccountType, string Email, string Password)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                LoginError     = "Email and password are required.";
                LoginErrorType = (AccountType == "customer") ? "customer" : "staff";
                return Page();
            }

            // Quick admin shortcut
            if (Email == "admin" && Password == "admin")
                return RedirectToPage("/Manager");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "";

                if (AccountType == "customer")
                    query = "SELECT CustomerID FROM Customer WHERE Email = @Email AND Password = @Password";
                else if (AccountType == "employee")
                    query = "SELECT EmployeeID FROM SalesEmployee WHERE Email = @Email AND Password = @Password";
                else if (AccountType == "manager")
                    query = "SELECT ManagerID FROM Manager WHERE Email = @Email AND Password = @Password";
                else
                {
                    LoginError     = "Please select a role.";
                    LoginErrorType = "staff";
                    return Page();
                }

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email",    Email);
                    cmd.Parameters.AddWithValue("@Password", Password);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        TempData["UserEmail"]   = Email;
                        TempData["AccountType"] = AccountType;
                        TempData["UserID"]      = result.ToString();

                        if (AccountType == "customer") return RedirectToPage("/Customer");
                        if (AccountType == "employee") return RedirectToPage("/Employee");
                        if (AccountType == "manager")  return RedirectToPage("/Manager");
                    }
                }
            }

            LoginError     = "Invalid credentials. Please try again.";
            LoginErrorType = (AccountType == "customer") ? "customer" : "staff";
            return Page();
        }

        // ── SIGNUP ────────────────────────────────────────────────────────────
        public IActionResult OnPostSignup(string FirstName, string LastName,
            string SignupEmail, string? PhoneNumber,
            string SignupPassword, string ConfirmPassword)
        {
            if (SignupPassword != ConfirmPassword)
            {
                SignupError = "Passwords do not match.";
                return Page();
            }
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(SignupEmail))
            {
                SignupError = "Name and email are required.";
                return Page();
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand check = new SqlCommand(
                    "SELECT COUNT(*) FROM Customer WHERE Email = @Email", con))
                {
                    check.Parameters.AddWithValue("@Email", SignupEmail);
                    if ((int)check.ExecuteScalar() > 0)
                    {
                        SignupError = "An account with that email already exists.";
                        return Page();
                    }
                }
                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Customer (FirstName, LastName, Email, PhoneNumber, Password)
                    VALUES (@FirstName, @LastName, @Email, @Phone, @Password)", con))
                {
                    cmd.Parameters.AddWithValue("@FirstName", FirstName);
                    cmd.Parameters.AddWithValue("@LastName",  LastName);
                    cmd.Parameters.AddWithValue("@Email",     SignupEmail);
                    cmd.Parameters.AddWithValue("@Phone",     (object?)PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Password",  SignupPassword);
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["UserEmail"]   = SignupEmail;
            TempData["AccountType"] = "customer";
            return RedirectToPage("/Customer");
        }
    }
}
