using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Error200Dealership.Pages
{
    public class AppointmentInfo
    {
        public int    AppointmentID   { get; set; }
        public string AppointmentType { get; set; } = "";
        public string AppointmentDate { get; set; } = "";
        public string AppointmentTime { get; set; } = "";
        public string Status          { get; set; } = "";
        public string CustomerName    { get; set; } = "";
        public string City            { get; set; } = "";
    }

    public class AppointmentsModel : PageModel
    {
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=error200_dealership;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public string SuccessMessage { get; set; } = "";
        public string ErrorMessage   { get; set; } = "";
        public string LastFullName   { get; set; } = "";
        public List<AppointmentInfo> MyAppointments { get; set; } = new();

        public void OnGet()
        {
            LoadMyAppointments();
        }

        public IActionResult OnPostBook(
            string FullName, string? Phone, string? Email,
            string? AppointmentType, string AppointmentDate, string AppointmentTime,
            string? Area, string? City, string? State)
        {
            LastFullName = FullName;

            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(AppointmentDate) || string.IsNullOrWhiteSpace(AppointmentTime))
            {
                ErrorMessage = "Full name, date, and time are required.";
                LoadMyAppointments();
                return Page();
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Find or create customer from email
                    int? customerId = null;
                    if (!string.IsNullOrWhiteSpace(Email))
                    {
                        using (SqlCommand find = new SqlCommand(
                            "SELECT CustomerID FROM Customer WHERE Email = @Email", con))
                        {
                            find.Parameters.AddWithValue("@Email", Email);
                            var result = find.ExecuteScalar();
                            if (result != null)
                            {
                                customerId = Convert.ToInt32(result);
                            }
                            else
                            {
                                // Auto-register lightweight customer record
                                var parts = FullName.Trim().Split(' ', 2);
                                using (SqlCommand ins = new SqlCommand(
                                    @"INSERT INTO Customer (FirstName, LastName, Email, PhoneNumber, Password)
                                      VALUES (@F, @L, @E, @P, '');
                                      SELECT SCOPE_IDENTITY();", con))
                                {
                                    ins.Parameters.AddWithValue("@F", parts[0]);
                                    ins.Parameters.AddWithValue("@L", parts.Length > 1 ? parts[1] : "");
                                    ins.Parameters.AddWithValue("@E", Email);
                                    ins.Parameters.AddWithValue("@P", (object?)Phone ?? DBNull.Value);
                                    customerId = Convert.ToInt32(ins.ExecuteScalar());
                                }
                            }
                        }
                    }

                    // Insert appointment
                    string insertQuery = @"
                        INSERT INTO Appointment
                            (CustomerID, AppointmentType, AppointmentDate, AppointmentTime, Area, City, [State])
                        VALUES
                            (@CID, @Type, @Date, @Time, @Area, @City, @State)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@CID",  (object?)customerId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Type", string.IsNullOrWhiteSpace(AppointmentType) ? "General Inquiry" : AppointmentType);
                        cmd.Parameters.AddWithValue("@Date", AppointmentDate);
                        cmd.Parameters.AddWithValue("@Time", AppointmentTime);
                        cmd.Parameters.AddWithValue("@Area", (object?)Area  ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@City", (object?)City  ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@State",(object?)State ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }

                SuccessMessage = "Appointment booked successfully! We will contact you to confirm.";
                LastFullName = "";
                LoadMyAppointments();
            }
            catch (SqlException ex)
            {
                ErrorMessage = "Database error: " + ex.Message;
                Console.WriteLine(ex.ToString());
            }

            return Page();
        }

        public IActionResult OnPostGoBack()
        {
            return RedirectToPage("/Customer");
        }

        private void LoadMyAppointments()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT TOP 10 AppointmentID, AppointmentType,
                               CONVERT(NVARCHAR,AppointmentDate,103) AS AppointmentDate,
                               CONVERT(NVARCHAR,AppointmentTime,108) AS AppointmentTime,
                               [Status]
                        FROM Appointment
                        ORDER BY CreatedAt DESC";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MyAppointments.Add(new AppointmentInfo
                            {
                                AppointmentID   = Convert.ToInt32(reader["AppointmentID"]),
                                AppointmentType = reader["AppointmentType"].ToString()!,
                                AppointmentDate = reader["AppointmentDate"].ToString()!,
                                AppointmentTime = reader["AppointmentTime"].ToString()!,
                                Status          = reader["Status"].ToString()!
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("LoadMyAppointments error: " + ex.Message);
            }
        }
    }
}
