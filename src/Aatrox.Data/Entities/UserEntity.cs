using Aatrox.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aatrox.Data.Entities
{
    [Table("users")]
    public sealed class UserEntity : Entity
    {
        [Column("language")]
        public Lang Language { get; set; }

        [Column("premium")]
        public bool Premium { get; set; }

        [Column("blacklisted")]
        public bool Blacklisted { get; set; }

        public LeagueUserEntity LeagueProfile { get; set; }

        public OsuUserEntity OsuProfile { get; set; }
    }
}
