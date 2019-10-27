using System;

namespace Aatrox.Core.Checks
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class HiddenAttribute : Attribute
    {
    }
}
