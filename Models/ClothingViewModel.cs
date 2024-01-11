using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace StoreQR.Models
{
    public class ClothingViewModel
    {
        public int ClothingId { get; set; }
        //public int UserId { get; set; }
        //[DisplayName("Bild")]
        //public byte[] Image { get; set; }
        [DisplayName("Beskrivning:")]
        public string ClothingName { get; set; } = string.Empty;

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
        [DisplayName("Tillhör:")]
        public int ClothingUserId { get; set; }
        [DisplayName("Tillhör:")]
        public string FamilyMemberName {  get; set; } = string.Empty;

        public string FamilyUserId { get; set; } = string.Empty;

        //public List<SelectListItem>? Brands { get; set; }
        //public List<SelectListItem>? Sizes { get; set; }
        //public List<SelectListItem>? Colors { get; set; }
        //public List<SelectListItem>? Seasons { get; set; }
        //public List<SelectListItem>? Materials { get; set; }
        //public List<SelectListItem>? TypeOfClothings { get; set; }

        //[DisplayName("Valt märke:")]
        //public string SelectedBrand {  get; set; } = string.Empty;
        //[DisplayName("Vald storlek:")]
        //public string SelectedSize { get; set; } = string.Empty;
        //[DisplayName("Vald färg:")]
        //public string SelectedColor { get; set; } = string.Empty;
        //[DisplayName("Vald säsong:")]
        //public string SelectedSeason { get; set; } = string.Empty;
        //[DisplayName("Valt material:")]
        //public string SelectedMaterial { get; set; } = string.Empty;
        //[DisplayName("Vald typ av plagg:")]
        //public string SelectedTypeOfClothing { get; set; } = string.Empty;

        //[DisplayName("Förvaras i: ")]
        //public string StorageName { get; set; }
    }
}
