using Microsoft.AspNetCore.SignalR;

namespace StoreQR.Models
{
    public class FamilyMember
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;

    }
}
