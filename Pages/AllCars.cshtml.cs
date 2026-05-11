using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Error200Dealership.Pages
{
    public class AllCarsModel : PageModel
    {
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=error200_dealership;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        public List<CarInfo> Cars { get; set; } = new();

        public void OnGet()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT CarID,Make,Model,CarYear,Color,Mileage,Price,[Status],FuelType,Transmission,ISNULL(Description,'') AS Description FROM Car ORDER BY AddedAt DESC", con))
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            Cars.Add(new CarInfo
                            {
                                CarID=r.GetInt32(0), Make=r.GetString(1), Model=r.GetString(2),
                                CarYear=r.GetInt32(3), Color=r["Color"].ToString()!,
                                Mileage=r.GetInt32(5), Price=r.GetDecimal(6),
                                Status=r.GetString(7), FuelType=r["FuelType"].ToString()!,
                                Transmission=r["Transmission"].ToString()!,
                                Description=r["Description"].ToString()!
                            });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}
