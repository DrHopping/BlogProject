using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class Tag
    {
        public int TagId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
    }
}