using System.ComponentModel.DataAnnotations;

namespace NurediniCargoAPI.Entities
{
    public class Warehouse : EntityBase
    {
        [Required]
        public required string Name { get; set; }
        public string? Address {  get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }


    }
}
