using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Error200Dealership.Pages
{
    public class CustomerRecord
    {
        public int    CustomerID  { get; set; }
        public string FirstName   { get; set; } = "";
        public string LastName    { get; set; } = "";
        public string Email       { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string CreatedAt   { get; set; } = "";
    }

    public class EmployeeModel : PageModel
    {
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=error200_dealership;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public string EmployeeName      { get; set; } = "Employee";
        public string ApptSuccess       { get; set; } = "";
        public int    TotalAppointments { get; set; }
        public int    TotalCustomers    { get; set; }
        public int    AvailableCars     { get; set; }
        public int    TotalOrders       { get; set; }

        public List<AppointmentInfo>  Appointments { get; set; } = new();
        public List<CarInfo>          Cars         { get; set; } = new();
        public List<CustomerRecord>   Customers    { get; set; } = new();

        public void OnGet()
        {
            EmployeeName = TempData.Peek("UserEmail")?.ToString() ?? "Employee";
            LoadAll();
        }

        public IActionResult OnPostCompleteAppt(int AppointmentID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE Appointment SET [Status]='Completed' WHERE AppointmentID=@ID", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", AppointmentID);
                        cmd.ExecuteNonQuery();
                    }
                }
                ApptSuccess = $"Appointment #{AppointmentID} marked as Completed.";
            }
            catch (SqlException ex) { Console.WriteLine(ex.Message); }

            LoadAll();
            return Page();
        }

        private void LoadAll()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Stats
                TotalAppointments = (int)(ExecuteScalar("SELECT COUNT(*) FROM Appointment", con) ?? 0);
                TotalCustomers    = (int)(ExecuteScalar("SELECT COUNT(*) FROM Customer",    con) ?? 0);
                AvailableCars     = (int)(ExecuteScalar("SELECT COUNT(*) FROM Car WHERE [Status]='Available'", con) ?? 0);
                TotalOrders       = (int)(ExecuteScalar("SELECT COUNT(*) FROM [Order]",      con) ?? 0);

                // Appointments
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP 20 a.AppointmentID, a.AppointmentType,
                           CONVERT(NVARCHAR,a.AppointmentDate,103) AS AppointmentDate,
                           CONVERT(NVARCHAR,a.AppointmentTime,108) AS AppointmentTime,
                           a.[Status],
                           ISNULL(CONCAT(c.FirstName,' ',c.LastName),'Unknown') AS CustomerName
                    FROM Appointment a
                    LEFT JOIN Customer c ON a.CustomerID=c.CustomerID
                    ORDER BY a.CreatedAt DESC", con))
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        Appointments.Add(new AppointmentInfo
                        {
                            AppointmentID   = Convert.ToInt32(r["AppointmentID"]),
                            AppointmentType = r["AppointmentType"].ToString()!,
                            AppointmentDate = r["AppointmentDate"].ToString()!,
                            AppointmentTime = r["AppointmentTime"].ToString()!,
                            Status          = r["Status"].ToString()!,
                            CustomerName    = r["CustomerName"].ToString()!
                        });
                }

                // Cars
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT CarID,Make,Model,CarYear,Color,Price,[Status],FuelType,Transmission FROM Car ORDER BY AddedAt DESC", con))
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        Cars.Add(new CarInfo
                        {
                            CarID        = Convert.ToInt32(r["CarID"]),
                            Make         = r["Make"].ToString()!,
                            Model        = r["Model"].ToString()!,
                            CarYear      = Convert.ToInt32(r["CarYear"]),
                            Color        = r["Color"].ToString()!,
                            Price        = Convert.ToDecimal(r["Price"]),
                            Status       = r["Status"].ToString()!,
                            FuelType     = r["FuelType"].ToString()!,
                            Transmission = r["Transmission"].ToString()!
                        });
                }

                // Customers
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP 20 CustomerID,FirstName,LastName,Email,
                           ISNULL(PhoneNumber,'') AS PhoneNumber,
                           CONVERT(NVARCHAR,CreatedAt,106) AS CreatedAt
                    FROM Customer ORDER BY CreatedAt DESC", con))
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        Customers.Add(new CustomerRecord
                        {
                            CustomerID  = Convert.ToInt32(r["CustomerID"]),
                            FirstName   = r["FirstName"].ToString()!,
                            LastName    = r["LastName"].ToString()!,
                            Email       = r["Email"].ToString()!,
                            PhoneNumber = r["PhoneNumber"].ToString()!,
                            CreatedAt   = r["CreatedAt"].ToString()!
                        });
                }
            }
        }

        private object? ExecuteScalar(string query, SqlConnection con)
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
                return cmd.ExecuteScalar();
        }
    }
}
