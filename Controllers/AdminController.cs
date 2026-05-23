using HostelMaintenanceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace HostelMaintenanceSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult AdminDashboard()
        {
            int pendingCount = 0;
            List<MaintenanceRequest> requests = new List<MaintenanceRequest>();

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Get all requests
                string query = "SELECT * FROM MaintenanceRequests ORDER BY Date DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string status = reader["Status"].ToString();

                    if (status == "Pending")
                        pendingCount++;

                    requests.Add(new MaintenanceRequest
                    {
                        RequestId = (int)reader["RequestId"],
                        Username = reader["Username"].ToString(),
                        IssueType = reader["IssueType"].ToString(),
                        RoomNo = reader["RoomNo"].ToString(),
                        Status = status,
                        Date = Convert.ToDateTime(reader["Date"])
                    });
                }
            }

            // Send data to view
            ViewBag.PendingCount = pendingCount;

            return View(requests);
        }
        public IActionResult Approve(int id)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "UPDATE MaintenanceRequests SET Status='Approved' WHERE RequestId=@id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("AdminDashboard");
        }
        public IActionResult Reject(int id)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "UPDATE MaintenanceRequests SET Status='Rejected' WHERE RequestId=@id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("AdminDashboard");
        }
    }
}