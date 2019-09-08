using Qmmands;

namespace Aatrox.Core.Entities
{
    public abstract class AatroxCheckBaseAttribute : CheckAttribute
    {
        public virtual string Name { get; set; } = "Unknown check.";
    }
}
