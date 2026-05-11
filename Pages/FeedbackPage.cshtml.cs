using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Error200Dealership.Pages
{
    public class ReviewInfo
    {
        public int    Rating       { get; set; }
        public string Comments     { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string SubmittedAt  { get; set; } = "";
    }

    public class FeedbackPageModel : PageModel
    {
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=error200_dealership;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public string             SuccessMessage { get; set; } = "";
        public string             ErrorMessage   { get; set; } = "";
        public List<ReviewInfo>   Reviews        { get; set; } = new();

        public void OnGet() => LoadReviews();

        public IActionResult OnPostSubmit(string? FullName, string? Email, int Rating, string? Comments)
        {
            if (Rating < 1 || Rating > 5)
            {
                ErrorMessage = "Please select a star rating (1–5).";
                LoadReviews();
                return Page();
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    int? customerId = null;
                    if (!string.IsNullOrWhiteSpace(Email))
                    {
                        using (SqlCommand find = new SqlCommand(
                            "SELECT CustomerID FROM Customer WHERE Email = @Email", con))
                        {
                            find.Parameters.AddWithValue("@Email", Email);
                            var res = find.ExecuteScalar();
                            if (res != null)
                            {
                                customerId = Convert.ToInt32(res);
                            }
                            else if (!string.IsNullOrWhiteSpace(FullName))
                            {
                                var parts = FullName.Trim().Split(' ', 2);
                                using (SqlCommand ins = new SqlCommand(
                                    @"INSERT INTO Customer (FirstName,LastName,Email,Password) VALUES (@F,@L,@E,'');
                                      SELECT SCOPE_IDENTITY();", con))
                                {
                                    ins.Parameters.AddWithValue("@F", parts[0]);
                                    ins.Parameters.AddWithValue("@L", parts.Length > 1 ? parts[1] : "");
                                    ins.Parameters.AddWithValue("@E", Email);
                                    customerId = Convert.ToInt32(ins.ExecuteScalar());
                                }
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Feedback (CustomerID, Rating, Comments) VALUES (@CID, @Rating, @Comments)", con))
                    {
                        cmd.Parameters.AddWithValue("@CID",      (object?)customerId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Rating",   Rating);
                        cmd.Parameters.AddWithValue("@Comments", (object?)Comments   ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }

                SuccessMessage = "Thank you for your feedback!";
            }
            catch (SqlException ex)
            {
                ErrorMessage = "Error saving feedback. Please try again.";
                Console.WriteLine(ex.Message);
            }

            LoadReviews();
            return Page();
        }

        private void LoadReviews()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT TOP 10
                               f.Rating, f.Comments,
                               ISNULL(CONCAT(c.FirstName,' ',c.LastName),'Anonymous') AS CustomerName,
                               CONVERT(NVARCHAR,f.SubmittedAt,106)                   AS SubmittedAt
                        FROM Feedback f
                        LEFT JOIN Customer c ON f.CustomerID = c.CustomerID
                        ORDER BY f.SubmittedAt DESC";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Reviews.Add(new ReviewInfo
                            {
                                Rating       = Convert.ToInt32(reader["Rating"]),
                                Comments     = reader["Comments"].ToString()!,
                                CustomerName = reader["CustomerName"].ToString()!,
                                SubmittedAt  = reader["SubmittedAt"].ToString()!
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("LoadReviews error: " + ex.Message);
            }
        }

        public IActionResult OnPostGoBack() => RedirectToPage("/Customer");
    }
}
