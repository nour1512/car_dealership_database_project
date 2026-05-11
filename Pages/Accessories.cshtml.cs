using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace Error200Dealership.Pages
{
    public class AccessoryInfo
    {
        public int     AccessoryID { get; set; }
        public string  Name        { get; set; } = "";
        public string  Category    { get; set; } = "";
        public decimal Price       { get; set; }
        public int     Stock       { get; set; }
        public string? ImageURL    { get; set; }
    }

    public class CartItemJson
    {
        public int     AccessoryID { get; set; }
        public int     Quantity    { get; set; }
        public decimal UnitPrice   { get; set; }
    }

    public class AccessoriesModel : PageModel
    {
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=error200_dealership;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public List<AccessoryInfo> Accessories  { get; set; } = new();
        public string              OrderSuccess { get; set; } = "";

        public void OnGet()
        {
            LoadAccessories();
        }

        public IActionResult OnPostPlaceOrder(
            string? CustomerName, string? CustomerEmail, string ItemsJson)
        {
            if (string.IsNullOrWhiteSpace(ItemsJson))
            {
                LoadAccessories();
                return Page();
            }

            List<CartItemJson>? items;
            try
            {
                items = JsonSerializer.Deserialize<List<CartItemJson>>(ItemsJson);
            }
            catch
            {
                LoadAccessories();
                return Page();
            }

            if (items == null || items.Count == 0)
            {
                LoadAccessories();
                return Page();
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction tx = con.BeginTransaction();
                    try
                    {
                        // Find or create customer
                        int? customerId = null;
                        if (!string.IsNullOrWhiteSpace(CustomerEmail))
                        {
                            using (SqlCommand find = new SqlCommand(
                                "SELECT CustomerID FROM Customer WHERE Email = @Email", con, tx))
                            {
                                find.Parameters.AddWithValue("@Email", CustomerEmail);
                                var res = find.ExecuteScalar();
                                if (res != null)
                                {
                                    customerId = Convert.ToInt32(res);
                                }
                                else if (!string.IsNullOrWhiteSpace(CustomerName))
                                {
                                    var parts = CustomerName.Trim().Split(' ', 2);
                                    using (SqlCommand ins = new SqlCommand(
                                        @"INSERT INTO Customer (FirstName,LastName,Email,Password) VALUES (@F,@L,@E,'');
                                          SELECT SCOPE_IDENTITY();", con, tx))
                                    {
                                        ins.Parameters.AddWithValue("@F", parts[0]);
                                        ins.Parameters.AddWithValue("@L", parts.Length > 1 ? parts[1] : "");
                                        ins.Parameters.AddWithValue("@E", CustomerEmail);
                                        customerId = Convert.ToInt32(ins.ExecuteScalar());
                                    }
                                }
                            }
                        }

                        // Calculate total
                        decimal total = items.Sum(i => i.UnitPrice * i.Quantity);

                        // Create order
                        int orderId;
                        using (SqlCommand cmd = new SqlCommand(
                            @"INSERT INTO [Order] (CustomerID, TotalAmount) VALUES (@CID, @Total);
                              SELECT SCOPE_IDENTITY();", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@CID",   (object?)customerId ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Total", total);
                            orderId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // Insert order items + decrement stock
                        foreach (var item in items)
                        {
                            using (SqlCommand ins = new SqlCommand(
                                @"INSERT INTO OrderItem (OrderID, AccessoryID, Quantity, UnitPrice)
                                  VALUES (@OID, @AID, @Qty, @Price);", con, tx))
                            {
                                ins.Parameters.AddWithValue("@OID",   orderId);
                                ins.Parameters.AddWithValue("@AID",   item.AccessoryID);
                                ins.Parameters.AddWithValue("@Qty",   item.Quantity);
                                ins.Parameters.AddWithValue("@Price", item.UnitPrice);
                                ins.ExecuteNonQuery();
                            }

                            using (SqlCommand upd = new SqlCommand(
                                @"UPDATE Accessory SET Stock = CASE WHEN Stock - @Qty < 0 THEN 0 ELSE Stock - @Qty END
                                  WHERE AccessoryID = @AID;", con, tx))
                            {
                                upd.Parameters.AddWithValue("@Qty", item.Quantity);
                                upd.Parameters.AddWithValue("@AID", item.AccessoryID);
                                upd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                        OrderSuccess = $"Order #{orderId} placed successfully! Thank you for your purchase.";
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("PlaceOrder error: " + ex.Message);
            }

            LoadAccessories();
            return Page();
        }

        private void LoadAccessories()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT AccessoryID, [Name], Category, Price, Stock, ImageURL FROM Accessory ORDER BY Category, [Name]";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Accessories.Add(new AccessoryInfo
                            {
                                AccessoryID = Convert.ToInt32(reader["AccessoryID"]),
                                Name        = reader["Name"].ToString()!,
                                Category    = reader["Category"].ToString()!,
                                Price       = Convert.ToDecimal(reader["Price"]),
                                Stock       = Convert.ToInt32(reader["Stock"]),
                                ImageURL    = reader["ImageURL"] == DBNull.Value ? null : reader["ImageURL"].ToString()
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("LoadAccessories error: " + ex.Message);
            }
        }
    }
}
