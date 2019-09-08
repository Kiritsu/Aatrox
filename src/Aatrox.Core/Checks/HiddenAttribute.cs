using System;

namespace Aatrox.Checks
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class HiddenAttribute : Attribute
    {
    }
}
