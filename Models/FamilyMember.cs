using Microsoft.AspNetCore.SignalR;
using System.ComponentModel;

namespace StoreQR.Models
{
    public class FamilyMember
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        [DisplayName("Namn")]
        public string Name { get; set; } = string.Empty;

    }
}
