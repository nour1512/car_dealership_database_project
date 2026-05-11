using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Error200Dealership.Pages
{
    public class OrderHistoryModel : PageModel
    {
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=error200_dealership;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public List<OrderSummary> Orders { get; set; } = new();

        public void OnGet()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT TOP 20 OrderID,
                               CONVERT(NVARCHAR,OrderDate,106) AS OrderDate,
                               ISNULL(TotalAmount,0) AS TotalAmount,
                               [Status]
                        FROM [Order]
                        ORDER BY OrderDate DESC", con))
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            Orders.Add(new OrderSummary
                            {
                                OrderID      = r.GetInt32(0),
                                OrderDate    = r["OrderDate"].ToString()!,
                                TotalAmount  = Convert.ToDecimal(r["TotalAmount"]),
                                Status       = r["Status"].ToString()!,
                                CustomerName = ""
                            });
                    }
                }
            }
            catch (SqlException ex) { Console.WriteLine(ex.Message); }
        }

        public IActionResult OnPostGoBack() => RedirectToPage("/Customer");
    }
}
