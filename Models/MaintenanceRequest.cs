namespace HostelMaintenanceSystem.Models
{
    public class MaintenanceRequest
    {
        public int RequestId { get; set; }
        public string Username { get; set; }
        public string IssueType { get; set; }
        public string RoomNo { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
    }
}