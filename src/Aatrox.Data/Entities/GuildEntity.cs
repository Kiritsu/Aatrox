using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aatrox.Data.Entities
{
    [Table("guild_entity")]
    public sealed class GuildEntity : Entity
    {
        [Column("prefixes")]
        public List<string> Prefixes { get; set; }
        
        [Column("auto_resolve_osu_url")]
        public bool AutoResolveOsuUrl { get; set; }
    }
}
