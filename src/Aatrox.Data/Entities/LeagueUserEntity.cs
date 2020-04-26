using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aatrox.Data.Entities
{
    [Table("league_users")]
    public sealed class LeagueUserEntity : Entity
    {
        [Column("username")]
        public string Username { get; set; }

        [Column("region")]
        public string Region { get; set; }

        public UserEntity User { get; set; }
    }
}
