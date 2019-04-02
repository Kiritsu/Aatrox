using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aatrox.Data.Entities
{

    public abstract class Entity 
    {
        [Column("snowflake_id"), Key]
        public ulong Id { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
