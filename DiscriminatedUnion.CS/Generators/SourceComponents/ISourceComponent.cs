using DiscriminatedUnion.CS.Utility;

namespace DiscriminatedUnion.CS.Generators.SourceComponents;

public interface ISourceComponent
{
    bool TryAddComponent(ISourceComponent component);
    bool IsCompatibleWith(ISourceComponent component);
    void Accept(SyntaxBuilder builder);
    void Accept(ISourceComponentVisitor visitor);
}