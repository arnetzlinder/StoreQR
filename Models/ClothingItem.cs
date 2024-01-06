using System.ComponentModel.DataAnnotations;

namespace StoreQR.Models
{
    public class ClothingItem
    {
        public int ClothingId { get; set; }
        public int UserId { get; set; }
        
        public byte[] ClothingImage { get; set; }
        public string ClothingName { get; set; } = string.Empty;

        
        public string ClothingUser { get; set; } = string.Empty;

        
        //public string QRCode { get; set; } = string.Empty;

        
        public string ClothingBrand { get; set; } = string.Empty;

      
        public string ClothingSize { get; set; } = string.Empty;

      
        public string ClothingColor { get; set; } = string.Empty;

     
        public string Season { get; set; } = string.Empty;

    
        public string ClothingMaterial { get; set; } = string.Empty;

        
        public string TypeOfClothing { get; set; } = string.Empty;

        //public int StorageId { get; set; }
    }
}