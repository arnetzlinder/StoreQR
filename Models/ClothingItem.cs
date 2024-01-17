﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StoreQR.Models
{
    public class ClothingItem
    {
        [Key]
        public int ClothingId { get; set; }
        public string UserId { get; set; } = string.Empty;
        [DisplayName("Bild")]
        public byte[]? ClothingImage { get; set; }
        [DisplayName("*Beskrivning:")]
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

        //public int StorageId { get; set; }
    }
}