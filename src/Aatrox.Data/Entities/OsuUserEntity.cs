using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aatrox.Data.Entities
{
    [Table("osu_users")]
    public sealed class OsuUserEntity : Entity
    {
        [Column("username")]
        public string Username { get; set; }

        public UserEntity User { get; set; }
    }
}
