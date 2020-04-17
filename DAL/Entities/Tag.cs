using System.ComponentModel.DataAnnotations;
using DAL.Entities.Base;

namespace DAL.Entities
{
    public class Tag : EntityBase<int>
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
    }
}