using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Error200Dealership.Pages
{
    public class CarInfo
    {
        public int    CarID        { get; set; }
        public string Make         { get; set; } = "";
        public string Model        { get; set; } = "";
        public int    CarYear      { get; set; }
        public string Color        { get; set; } = "";
        public int    Mileage      { get; set; }
        public decimal Price       { get; set; }
        public string Status       { get; set; } = "";
        public string FuelType     { get; set; } = "";
        public string Transmission { get; set; } = "";
        public string Description  { get; set; } = "";
    }

    public class CustomerModel : PageModel
    {
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=error200_dealership;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public List<CarInfo> Cars         { get; set; } = new();
        public string        CustomerName { get; set; } = "Customer";

        public void OnGet()
        {
            CustomerName = TempData.Peek("UserEmail")?.ToString() ?? "Customer";
            LoadCars();
        }

        public IActionResult OnPostGoToAppointment()
        {
            return RedirectToPage("/Appointments");
        }

        private void LoadCars()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT CarID, Make, Model, CarYear, Color, Mileage, Price, [Status], FuelType, Transmission FROM Car WHERE [Status] = 'Available' ORDER BY AddedAt DESC";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Cars.Add(new CarInfo
                            {
                                CarID        = Convert.ToInt32(reader["CarID"]),
                                Make         = reader["Make"].ToString()!,
                                Model        = reader["Model"].ToString()!,
                                CarYear      = Convert.ToInt32(reader["CarYear"]),
                                Color        = reader["Color"].ToString()!,
                                Mileage      = Convert.ToInt32(reader["Mileage"]),
                                Price        = Convert.ToDecimal(reader["Price"]),
                                Status       = reader["Status"].ToString()!,
                                FuelType     = reader["FuelType"].ToString()!,
                                Transmission = reader["Transmission"].ToString()!
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("LoadCars error: " + ex.Message);
            }
        }
    }
}
