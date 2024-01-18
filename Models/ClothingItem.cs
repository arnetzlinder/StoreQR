using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreQR.Models
{
    public class ClothingItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClothingId { get; set; }
        public string UserId { get; set; } = string.Empty;
        [DisplayName("Bild")]
        [DataType(DataType.Upload)]
        [NotMapped] //Ta bort från mappningen i databasen, test för att få till uppladdning
        public IFormFile? ClothingImageFile { get; set; }
        [DisplayName("Bild")]
        [NotMapped]
        public byte[]? ClothingImage {  get; set; }
        [DisplayName("*Beskrivning:")]
        [Required(ErrorMessage ="Beskrivning är obligatoriskt")]
        public string ClothingName { get; set; } = string.Empty;

        [DisplayName("Tillhör:")]
        public int ClothingUserId { get; set; }


        //public string QRCode { get; set; } = string.Empty;

        [DisplayName("Märke:")]
        public string ClothingBrand { get; set; } = string.Empty;

        [DisplayName("Storlek:")]
        public string ClothingSize { get; set; } = string.Empty;

        [DisplayName("Färg:")]
        public string ClothingColor { get; set; } = string.Empty;

        [DisplayName("Säsong:")]
        public string Season { get; set; } = string.Empty;

        [DisplayName("Material:")]
        public string ClothingMaterial { get; set; } = string.Empty;

        [DisplayName("Typ av plagg:")]
        public string TypeOfClothing { get; set; } = string.Empty;

        [DisplayName("Förvaringsutrymme:")]
        public int StorageId { get; set; }
    }
}