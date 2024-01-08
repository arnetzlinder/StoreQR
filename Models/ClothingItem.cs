using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StoreQR.Models
{
    public class ClothingItem
    {
        [Key]
        public int ClothingId { get; set; }
        //public int UserId { get; set; }
        //[DisplayName("Bild")]
        //public byte[] Image { get; set; }
        [DisplayName("Beskrivning:")]
        public string ClothingName { get; set; } = string.Empty;

        [DisplayName("Tillhör:")]
        public string ClothingUser { get; set; } = string.Empty;


        //public string QRCode { get; set; } = string.Empty;

        [DisplayName("Märke:")]
        public string Brand { get; set; } = string.Empty;

        [DisplayName("Storlek:")]
        public string Size { get; set; } = string.Empty;

        [DisplayName("Färg:")]
        public string Color { get; set; } = string.Empty;

        [DisplayName("Säsong:")]
        public string Season { get; set; } = string.Empty;

        [DisplayName("Material:")]
        public string Material { get; set; } = string.Empty;

        [DisplayName("Typ av plagg:")]
        public string TypeOfClothing { get; set; } = string.Empty;

        //public int StorageId { get; set; }
    }
}