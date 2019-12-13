using System;

namespace Aatrox.Checks
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class HiddenAttribute : Attribute
    {
    }
}
