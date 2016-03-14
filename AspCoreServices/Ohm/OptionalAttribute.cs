using System;

namespace Ohm
{
    [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
    public class OptionalAttribute : Attribute
    {
    }
}
