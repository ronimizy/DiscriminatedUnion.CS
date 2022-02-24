using System.Collections.Generic;
using DiscriminatedUnion.Generators.Generators.SourceComponents.Models;
using DiscriminatedUnion.Generators.Utility;

namespace DiscriminatedUnion.Generators.Generators.SourceComponents.Decorators
{
    public class UsingComponentDecorator : ComponentDecoratorBase
    {
        private readonly HashSet<string> _usedNamespaces = new HashSet<string>();
        private readonly List<TypeAlias> _aliases = new List<TypeAlias>();

        public UsingComponentDecorator(ISourceComponent component) : base(component) { }

        public void AddUsedNamespace(string value)
            => _usedNamespaces.Add(value);

        public void AddUsingAlias(string name, string type)
            => AddUsingAlias(new TypeAlias(name, type));

        public void AddUsingAlias(TypeAlias alias)
            => _aliases.Add(alias);

        public override void Accept(SyntaxBuilder builder)
        {
            foreach (var ns in _usedNamespaces)
            {
                builder.AppendLine($"using {ns};");
            }

            foreach (var (name, type) in _aliases)
            {
                builder.Append("using ");
                builder.Append(name);
                builder.Append(" = ");
                builder.Append(type);
                builder.AppendLine(";");
            }

            builder.AppendLine();

            Wrapped.Accept(builder);
        }

        public override void Accept(ISourceComponentVisitor visitor)
        {
            visitor.VisitUsingComponent(this);
        }
    }
}