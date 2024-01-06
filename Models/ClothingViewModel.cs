using System;
using System.ComponentModel;

namespace StoreQR.Models
{
    public class ClothingViewModel
    {
        [DisplayName("Namn")]
        public string Name { get; set; } = string.Empty;
        [DisplayName("Användare")]
        public string ClothingUser { get; set; } =string.Empty;
        [DisplayName("QR-kod")]
        public string QRCode { get; set; } = string.Empty;
        [DisplayName("Märke")]
        public string Brand { get; set; } = string.Empty;
        [DisplayName("Märke")]
        public string Size { get; set; } = string.Empty;
        [DisplayName("Färg")]
        public string Color {  get; set; } = string.Empty;
        [DisplayName("Säsong")]
        public string Season {  get; set; } = string.Empty;
        [DisplayName("Material")]
        public string Material {  get; set; } = string.Empty;

        [DisplayName("Typ av plagg")]
        public string TypeOfClothing {  get; set; } = string.Empty;
        public int StorageId { get; set; }

        public string Image { get; set; } = string.Empty;
       
    }
}
