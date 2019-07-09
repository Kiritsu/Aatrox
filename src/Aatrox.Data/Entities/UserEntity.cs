using Aatrox.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aatrox.Data.Entities
{
    [Table("user_entity")]
    public sealed class UserEntity : Entity
    {
        [Column("language")]
        public Lang Language { get; set; }
    }
}
