namespace Domain.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } 
        public string Action { get; set; }
        public string EntityType { get; set; } 
        public string? EntityId { get; set; }
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? IpAddress { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}
