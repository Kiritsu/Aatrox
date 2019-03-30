using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Element.Data.Entities
{
    [Table("user_entity")]
    public class UserEntity : Entity
    {
        [Column("user_id"), Key]
        public ulong UserId { get; set; }
    }
}
