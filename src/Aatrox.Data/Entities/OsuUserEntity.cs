using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aatrox.Data.Entities
{
    [Table("osu_user_entity")]
    public sealed class OsuUserEntity : Entity
    {
        [Column("username")]
        public string Username { get; set; }

        [Column("channels")]
        public List<ulong> Channels { get; set; }

        [Column("send_recent_score")]
        public bool SendRecentScore { get; set; }

        [Column("send_new_best_score")]
        public bool SendNewBestScore { get; set; }

        [Column("pp_min")]
        public int PpMin { get; set; }

        [Column("country_rank_min")]
        public int CountryRankMin { get; set; }

        public UserEntity User { get; set; }
    }
}
