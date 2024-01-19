using System.ComponentModel.DataAnnotations;

namespace StoreQR.Models
{
    public class StoringUnit
    {
        [Key]
        public int StorageId { get; set; }
        //public int UserId { get; set;}
        public string StorageName { get; set;} = string.Empty;
        public string StorageDescription { get; set; } = string.Empty;
        //public string QRCode { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
