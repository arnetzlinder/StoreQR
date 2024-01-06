using System.ComponentModel.DataAnnotations;

namespace StoreQR.Models
{
    public class ClothingItem
    {
        public int Id { get; set; }

        
        public string Name { get; set; } = string.Empty;

        
        public string ClothingUser { get; set; } = string.Empty;

        
        //public string QRCode { get; set; } = string.Empty;

        
        public string Brand { get; set; } = string.Empty;

      
        public string Size { get; set; } = string.Empty;

      
        public string Color { get; set; } = string.Empty;

     
        public string Season { get; set; } = string.Empty;

    
        public string Material { get; set; } = string.Empty;

        
        public string TypeOfClothing { get; set; } = string.Empty;

        //public int StorageId { get; set; }
       // public string Image {  get; set; } = string.Empty;
    }
}