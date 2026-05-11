using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Error200Dealership.Pages
{
    // ── View models ──────────────────────────────────────────────────────────
    public class DashboardStats
    {
        public int     TotalCars         { get; set; }
        public int     AvailableCars     { get; set; }
        public int     TotalCustomers    { get; set; }
        public int     TotalEmployees    { get; set; }
        public int     TotalAppointments { get; set; }
        public int     TotalOrders       { get; set; }
        public decimal TotalRevenue      { get; set; }
        public int     TotalFeedback     { get; set; }
    }

    public class EmployeeInfo
    {
        public int    EmployeeID  { get; set; }
        public string FirstName   { get; set; } = "";
        public string LastName    { get; set; } = "";
        public string Email       { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string HireDate    { get; set; } = "";
    }

    public class OrderSummary
    {
        public int     OrderID      { get; set; }
        public string  CustomerName { get; set; } = "";
        public string  OrderDate    { get; set; } = "";
        public decimal TotalAmount  { get; set; }
        public string  Status       { get; set; } = "";
    }

    public class FeedbackRecord
    {
        public int    FeedbackID   { get; set; }
        public string CustomerName { get; set; } = "";
        public int    Rating       { get; set; }
        public string Comments     { get; set; } = "";
        public string SubmittedAt  { get; set; } = "";
    }

    public class SupplierInfo
    {
        public int    SupplierID   { get; set; }
        public string SupplierName { get; set; } = "";
        public string ContactEmail { get; set; } = "";
        public string PhoneNumber  { get; set; } = "";
        public string Address      { get; set; } = "";
    }

    public class ManagerApptInfo : AppointmentInfo
    {
        // All fields (including CustomerName and City) inherited from AppointmentInfo
    }

    public class ManagerCustomer : CustomerRecord
    {
        public string City { get; set; } = "";
    }

    // ── Page Model ───────────────────────────────────────────────────────────
    public class ManagerModel : PageModel
    {
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=error200_dealership;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        // Data collections
        public DashboardStats         Stats        { get; set; } = new();
        public List<CarInfo>          Cars         { get; set; } = new();
        public List<ManagerCustomer>  Customers    { get; set; } = new();
        public List<EmployeeInfo>     Employees    { get; set; } = new();
        public List<ManagerApptInfo>  Appointments { get; set; } = new();
        public List<AccessoryInfo>    Accessories  { get; set; } = new();
        public List<OrderSummary>     Orders       { get; set; } = new();
        public List<FeedbackRecord>   FeedbackList { get; set; } = new();
        public List<SupplierInfo>     Suppliers    { get; set; } = new();

        // Status messages
        public string CarMsg  { get; set; } = "";
        public string EmpMsg  { get; set; } = "";
        public string ApptMsg { get; set; } = "";
        public string AccMsg  { get; set; } = "";
        public string SupMsg  { get; set; } = "";

        // ── GET ──────────────────────────────────────────────────────────────
        public void OnGet() => LoadAll();

        // ── CARS ─────────────────────────────────────────────────────────────
        public IActionResult OnPostAddCar(
            string Make, string Model, int CarYear, string? Color,
            int Mileage, decimal Price, string CarStatus,
            string? FuelType, string? Transmission, string? Description)
        {
            Exec(@"INSERT INTO Car (InventoryID,Make,Model,CarYear,Color,Mileage,Price,[Status],FuelType,Transmission,Description)
                   VALUES (1,@Make,@Model,@Year,@Color,@Mileage,@Price,@Status,@Fuel,@Trans,@Desc)",
                P("@Make", Make), P("@Model", Model), P("@Year", CarYear),
                P("@Color",   (object?)Color        ?? DBNull.Value),
                P("@Mileage", Mileage),
                P("@Price",   Price),
                P("@Status",  CarStatus),
                P("@Fuel",    (object?)FuelType     ?? DBNull.Value),
                P("@Trans",   (object?)Transmission ?? DBNull.Value),
                P("@Desc",    (object?)Description  ?? DBNull.Value));
            CarMsg = $"Car '{Make} {Model}' added successfully.";
            LoadAll(); return Page();
        }

        public IActionResult OnPostDeleteCar(int CarID)
        {
            Exec("DELETE FROM Car WHERE CarID = @ID", P("@ID", CarID));
            CarMsg = $"Car #{CarID} deleted.";
            LoadAll(); return Page();
        }

        public IActionResult OnPostMarkSold(int CarID)
        {
            Exec("UPDATE Car SET [Status]='Sold' WHERE CarID=@ID", P("@ID", CarID));
            CarMsg = $"Car #{CarID} marked as Sold.";
            LoadAll(); return Page();
        }

        // ── EMPLOYEES ────────────────────────────────────────────────────────
        public IActionResult OnPostAddEmployee(
            string EmpFirstName, string EmpLastName, string EmpEmail,
            string EmpPassword, string? EmpPhone, string? EmpHireDate)
        {
            Exec(@"INSERT INTO SalesEmployee (ManagerID,FirstName,LastName,Email,Password,PhoneNumber,HireDate)
                   VALUES (1,@F,@L,@E,@P,@Phone,@Hire)",
                P("@F",    EmpFirstName),
                P("@L",    EmpLastName),
                P("@E",    EmpEmail),
                P("@P",    EmpPassword),
                P("@Phone",(object?)EmpPhone    ?? DBNull.Value),
                P("@Hire", string.IsNullOrWhiteSpace(EmpHireDate) ? DBNull.Value : (object)EmpHireDate));
            EmpMsg = $"Employee {EmpFirstName} {EmpLastName} added.";
            LoadAll(); return Page();
        }

        public IActionResult OnPostDeleteEmployee(int EmployeeID)
        {
            Exec("DELETE FROM SalesEmployee WHERE EmployeeID=@ID", P("@ID", EmployeeID));
            EmpMsg = $"Employee #{EmployeeID} deleted.";
            LoadAll(); return Page();
        }

        // ── APPOINTMENTS ─────────────────────────────────────────────────────
        public IActionResult OnPostUpdateApptStatus(int AppointmentID, string NewStatus)
        {
            Exec("UPDATE Appointment SET [Status]=@S WHERE AppointmentID=@ID",
                P("@S",  NewStatus),
                P("@ID", AppointmentID));
            ApptMsg = $"Appointment #{AppointmentID} status updated to {NewStatus}.";
            LoadAll(); return Page();
        }

        // ── ACCESSORIES ──────────────────────────────────────────────────────
        public IActionResult OnPostAddAccessory(
            string AccName, string? AccCategory, decimal AccPrice,
            int AccStock, string? AccImageURL, int? AccSupplierID)
        {
            Exec(@"INSERT INTO Accessory (SupplierID,[Name],Category,Price,Stock,ImageURL)
                   VALUES (@Sup,@Name,@Cat,@Price,@Stock,@Img)",
                P("@Sup",   AccSupplierID.HasValue ? (object)AccSupplierID.Value : DBNull.Value),
                P("@Name",  AccName),
                P("@Cat",   (object?)AccCategory ?? DBNull.Value),
                P("@Price", AccPrice),
                P("@Stock", AccStock),
                P("@Img",   (object?)AccImageURL ?? DBNull.Value));
            AccMsg = $"Accessory '{AccName}' added.";
            LoadAll(); return Page();
        }

        public IActionResult OnPostDeleteAccessory(int AccessoryID)
        {
            Exec("DELETE FROM Accessory WHERE AccessoryID=@ID", P("@ID", AccessoryID));
            AccMsg = $"Accessory #{AccessoryID} deleted.";
            LoadAll(); return Page();
        }

        // ── SUPPLIERS ────────────────────────────────────────────────────────
        public IActionResult OnPostAddSupplier(
            string SupName, string? SupEmail, string? SupPhone, string? SupAddress)
        {
            Exec(@"INSERT INTO Supplier (SupplierName,ContactEmail,PhoneNumber,Address)
                   VALUES (@Name,@Email,@Phone,@Addr)",
                P("@Name",  SupName),
                P("@Email", (object?)SupEmail   ?? DBNull.Value),
                P("@Phone", (object?)SupPhone   ?? DBNull.Value),
                P("@Addr",  (object?)SupAddress ?? DBNull.Value));
            SupMsg = $"Supplier '{SupName}' added.";
            LoadAll(); return Page();
        }

        // ── DATA LOADING ─────────────────────────────────────────────────────
        private void LoadAll()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                Stats.TotalCars         = Count("SELECT COUNT(*) FROM Car",          con);
                Stats.AvailableCars     = Count("SELECT COUNT(*) FROM Car WHERE [Status]='Available'", con);
                Stats.TotalCustomers    = Count("SELECT COUNT(*) FROM Customer",      con);
                Stats.TotalEmployees    = Count("SELECT COUNT(*) FROM SalesEmployee", con);
                Stats.TotalAppointments = Count("SELECT COUNT(*) FROM Appointment",   con);
                Stats.TotalOrders       = Count("SELECT COUNT(*) FROM [Order]",       con);
                Stats.TotalFeedback     = Count("SELECT COUNT(*) FROM Feedback",      con);
                Stats.TotalRevenue      = Revenue(con);

                LoadCars(con);
                LoadCustomers(con);
                LoadEmployees(con);
                LoadAppointments(con);
                LoadAccessories(con);
                LoadOrders(con);
                LoadFeedback(con);
                LoadSuppliers(con);
            }
        }

        private void LoadCars(SqlConnection con)
        {
            using var cmd = new SqlCommand("SELECT CarID,Make,Model,CarYear,Color,Price,[Status],FuelType,Transmission FROM Car ORDER BY AddedAt DESC", con);
            using var r   = cmd.ExecuteReader();
            while (r.Read())
                Cars.Add(new CarInfo { CarID=r.GetInt32(0), Make=r.GetString(1), Model=r.GetString(2), CarYear=r.GetInt32(3), Color=r["Color"].ToString()!, Price=r.GetDecimal(5), Status=r.GetString(6), FuelType=r["FuelType"].ToString()!, Transmission=r["Transmission"].ToString()! });
        }

        private void LoadCustomers(SqlConnection con)
        {
            using var cmd = new SqlCommand(@"SELECT CustomerID,FirstName,LastName,Email,ISNULL(PhoneNumber,'') AS PhoneNumber,ISNULL(City,'') AS City,CONVERT(NVARCHAR,CreatedAt,106) AS CreatedAt FROM Customer ORDER BY CreatedAt DESC", con);
            using var r   = cmd.ExecuteReader();
            while (r.Read())
                Customers.Add(new ManagerCustomer { CustomerID=r.GetInt32(0), FirstName=r.GetString(1), LastName=r.GetString(2), Email=r.GetString(3), PhoneNumber=r["PhoneNumber"].ToString()!, City=r["City"].ToString()!, CreatedAt=r["CreatedAt"].ToString()! });
        }

        private void LoadEmployees(SqlConnection con)
        {
            using var cmd = new SqlCommand(@"SELECT EmployeeID,FirstName,LastName,Email,ISNULL(PhoneNumber,'') AS PhoneNumber,ISNULL(CONVERT(NVARCHAR,HireDate,106),'') AS HireDate FROM SalesEmployee ORDER BY CreatedAt DESC", con);
            using var r   = cmd.ExecuteReader();
            while (r.Read())
                Employees.Add(new EmployeeInfo { EmployeeID=r.GetInt32(0), FirstName=r.GetString(1), LastName=r.GetString(2), Email=r.GetString(3), PhoneNumber=r["PhoneNumber"].ToString()!, HireDate=r["HireDate"].ToString()! });
        }

        private void LoadAppointments(SqlConnection con)
        {
            using var cmd = new SqlCommand(@"
                SELECT a.AppointmentID,
                       ISNULL(CONCAT(c.FirstName,' ',c.LastName),'Unknown') AS CustomerName,
                       a.AppointmentType,
                       CONVERT(NVARCHAR,a.AppointmentDate,103) AS AppointmentDate,
                       CONVERT(NVARCHAR,a.AppointmentTime,108) AS AppointmentTime,
                       ISNULL(a.City,'') AS City, a.[Status]
                FROM Appointment a
                LEFT JOIN Customer c ON a.CustomerID=c.CustomerID
                ORDER BY a.CreatedAt DESC", con);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                Appointments.Add(new ManagerApptInfo
                {
                    AppointmentID=r.GetInt32(0), CustomerName=r["CustomerName"].ToString()!,
                    AppointmentType=r["AppointmentType"].ToString()!,
                    AppointmentDate=r["AppointmentDate"].ToString()!, AppointmentTime=r["AppointmentTime"].ToString()!,
                    City=r["City"].ToString()!, Status=r["Status"].ToString()!
                });
        }

        private void LoadAccessories(SqlConnection con)
        {
            using var cmd = new SqlCommand("SELECT AccessoryID,[Name],Category,Price,Stock,ISNULL(ImageURL,'') FROM Accessory ORDER BY Category,[Name]", con);
            using var r   = cmd.ExecuteReader();
            while (r.Read())
                Accessories.Add(new AccessoryInfo { AccessoryID=r.GetInt32(0), Name=r.GetString(1), Category=r["Category"].ToString()!, Price=r.GetDecimal(3), Stock=r.GetInt32(4), ImageURL=r[5]==DBNull.Value?null:r.GetString(5) });
        }

        private void LoadOrders(SqlConnection con)
        {
            using var cmd = new SqlCommand(@"
                SELECT o.OrderID,
                       ISNULL(CONCAT(c.FirstName,' ',c.LastName),'Guest') AS CustomerName,
                       CONVERT(NVARCHAR,o.OrderDate,106) AS OrderDate,
                       ISNULL(o.TotalAmount,0) AS TotalAmount, o.[Status]
                FROM [Order] o
                LEFT JOIN Customer c ON o.CustomerID=c.CustomerID
                ORDER BY o.OrderDate DESC", con);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                Orders.Add(new OrderSummary { OrderID=r.GetInt32(0), CustomerName=r["CustomerName"].ToString()!, OrderDate=r["OrderDate"].ToString()!, TotalAmount=Convert.ToDecimal(r["TotalAmount"]), Status=r["Status"].ToString()! });
        }

        private void LoadFeedback(SqlConnection con)
        {
            using var cmd = new SqlCommand(@"
                SELECT f.FeedbackID,
                       ISNULL(CONCAT(c.FirstName,' ',c.LastName),'Anonymous') AS CustomerName,
                       f.Rating, ISNULL(f.Comments,'') AS Comments,
                       CONVERT(NVARCHAR,f.SubmittedAt,106) AS SubmittedAt
                FROM Feedback f
                LEFT JOIN Customer c ON f.CustomerID=c.CustomerID
                ORDER BY f.SubmittedAt DESC", con);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                FeedbackList.Add(new FeedbackRecord { FeedbackID=r.GetInt32(0), CustomerName=r["CustomerName"].ToString()!, Rating=r.GetByte(2), Comments=r["Comments"].ToString()!, SubmittedAt=r["SubmittedAt"].ToString()! });
        }

        private void LoadSuppliers(SqlConnection con)
        {
            using var cmd = new SqlCommand("SELECT SupplierID,SupplierName,ISNULL(ContactEmail,'') AS ContactEmail,ISNULL(PhoneNumber,'') AS PhoneNumber,ISNULL(Address,'') AS Address FROM Supplier ORDER BY SupplierName", con);
            using var r   = cmd.ExecuteReader();
            while (r.Read())
                Suppliers.Add(new SupplierInfo { SupplierID=r.GetInt32(0), SupplierName=r.GetString(1), ContactEmail=r["ContactEmail"].ToString()!, PhoneNumber=r["PhoneNumber"].ToString()!, Address=r["Address"].ToString()! });
        }

        // ── Helpers ──────────────────────────────────────────────────────────
        private int Count(string query, SqlConnection con)
        {
            using var cmd = new SqlCommand(query, con);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private decimal Revenue(SqlConnection con)
        {
            using var cmd = new SqlCommand("SELECT ISNULL(SUM(TotalAmount),0) FROM [Order]", con);
            return Convert.ToDecimal(cmd.ExecuteScalar());
        }

        private void Exec(string query, params SqlParameter[] parms)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddRange(parms);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private SqlParameter P(string name, object value) => new SqlParameter(name, value);
    }
}
