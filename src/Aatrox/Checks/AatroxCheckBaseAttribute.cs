using Qmmands;

namespace Aatrox.Checks
{
    public abstract class AatroxCheckBaseAttribute : CheckAttribute
    {
        public virtual string Name { get; set; } = "Unknown check.";
    }
}
