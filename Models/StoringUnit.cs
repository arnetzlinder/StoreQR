using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreQR.Models
{
    public class StoringUnit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StorageId { get; set; }
        [DisplayName("*Namn:")]
        public string StorageName { get; set;} = string.Empty;

        [DisplayName("*Beskrivning:")]
        public string StorageDescription { get; set; } = string.Empty;
        //public string QRCode { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        [DisplayName("Bild")]
        [DataType(DataType.Upload)]
        [NotMapped] //Ta bort från mappningen i databasen, test för att få till uppladdning
        public IFormFile? StorageImageFile { get; set; }
        [DisplayName("Bild")]
        [NotMapped]
        public string StorageImage { get; set; } = string.Empty;
    }
}
