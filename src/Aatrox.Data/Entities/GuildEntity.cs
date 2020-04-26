using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aatrox.Data.Entities
{
    [Table("guilds")]
    public sealed class GuildEntity : Entity
    {
        [Column("prefixes")]
        public List<string> Prefixes { get; set; }
        
        [Column("resolve_osu_urls")]
        public bool ResolveOsuUrls { get; set; }
    }
}
