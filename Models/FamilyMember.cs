using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreQR.Models
{
    public class FamilyMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        [DisplayName("Namn")]
        public string Name { get; set; } = string.Empty;

    }
}
