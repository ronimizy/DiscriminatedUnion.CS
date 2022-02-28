using System;

namespace DiscriminatedUnion.CS.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Discriminator : Attribute
    {
        public Discriminator(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}