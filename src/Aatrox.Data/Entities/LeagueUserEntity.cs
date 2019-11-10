using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aatrox.Data.Entities
{
    [Table("league_user_entity")]
    public sealed class LeagueUserEntity : Entity
    {
        [Column("username")]
        public string Username { get; set; }

        [Column("region")]
        public string Region { get; set; }

        [Column("channels")]
        public List<ulong> Channels { get; set; }

        [Column("send_current_game_info")]
        public bool CurrentGameInfo { get; set; }

        public UserEntity User { get; set; }
    }
}
