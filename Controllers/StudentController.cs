using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using HostelMaintenanceSystem.Models;

namespace HostelMaintenanceSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly IConfiguration _configuration;

        public StudentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult StudentDashboard()
        {
            if (HttpContext.Session.GetString("Role") != "Student")
                return RedirectToAction("Login", "Account");

            List<MaintenanceRequest> requests = new List<MaintenanceRequest>();

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query = "SELECT * FROM MaintenanceRequests WHERE Username=@u";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@u", HttpContext.Session.GetString("Username"));

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    requests.Add(new MaintenanceRequest
                    {
                        RequestId = (int)reader["RequestId"],
                        IssueType = reader["IssueType"].ToString(),
                        RoomNo = reader["RoomNo"].ToString(),
                        Status = reader["Status"].ToString(),
                        Date = Convert.ToDateTime(reader["Date"])
                    });
                }
            }

            return View(requests);
        }

        public IActionResult SubmitRequest()
        {
            if (HttpContext.Session.GetString("Role") != "Student")
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public IActionResult SubmitRequest(MaintenanceRequest req)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query = @"INSERT INTO MaintenanceRequests 
                                (Username, IssueType, RoomNo, Description, Status)
                                VALUES (@u, @i, @r, @d, 'Pending')";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@u", HttpContext.Session.GetString("Username"));
                cmd.Parameters.AddWithValue("@i", req.IssueType);
                cmd.Parameters.AddWithValue("@r", req.RoomNo);
                cmd.Parameters.AddWithValue("@d", req.Description);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("StudentDashboard");
        }
    }
}