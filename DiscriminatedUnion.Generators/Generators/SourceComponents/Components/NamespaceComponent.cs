namespace DiscriminatedUnion.Generators.Generators.SourceComponents.Components
{
    public class NamespaceComponent : SourceComponentWithBody
    {
        private readonly string _name;

        public NamespaceComponent(string name)
        {
            _name = name;
        }

        public override bool IsCompatibleWith(ISourceComponent component) => false;
        protected override string GetTitle() => $"namespace {_name}";
    }
}