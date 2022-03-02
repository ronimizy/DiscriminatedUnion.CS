using System;
using System.Linq;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Generators.SourceComponents;
using DiscriminatedUnion.CS.Generators.SourceComponents.Components;
using FluentScanning;
using FluentScanning.DependencyInjection;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace DiscriminatedUnion.CS.Generators.Pipeline.WrappedTypeBuilding.MemberBuilding.MemberBuilders;

public class MethodMemberBuilder : MemberBuilderBase<IMethodSymbol>
{
    private readonly IMethodBuilder _methodBuilder;

    public MethodMemberBuilder()
    {
        var collection = new ServiceCollection();

        using (var scanner = collection.UseAssemblyScanner(typeof(IAssemblyMarker)))
        {
            scanner.EnqueueAdditionOfTypesThat()
                .WouldBeRegisteredAs<IMethodBuilder>()
                .WithSingletonLifetime()
                .MustBeAssignableTo<IMethodBuilder>()
                .AreNotInterfaces()
                .AreNotAbstractClasses();
        }

        var provider = collection.BuildServiceProvider();
        _methodBuilder = provider
            .GetServices<IMethodBuilder>()
            .Aggregate((a, b) => a.AddNext(b));
    }

    public override bool TryBuildMemberSyntaxComponent(
        MemberBuildingContext<ISymbol> context, out ISourceComponent? memberSyntax)
    {
        var contextOption = context.As<IMethodSymbol>();
        memberSyntax = null;

        if (contextOption is null)
            return Next?.TryBuildMemberSyntaxComponent(context, out memberSyntax) ?? false;

        var typedContext = contextOption.Value;

        var response = _methodBuilder.TryBuildMemberSyntaxComponent(typedContext, out memberSyntax);

        return response switch
        {
            MethodMemberBuilderResponse.Built => true,
            MethodMemberBuilderResponse.NotBuilt => BuildMemberSyntaxComponent(typedContext, out memberSyntax),
            MethodMemberBuilderResponse.Invalid => false,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    protected override bool BuildMemberSyntaxComponent(
        MemberBuildingContext<IMethodSymbol> context, out ISourceComponent memberSource)
    {
        var (symbol, name) = context;
        var attributes = new ComponentModifiers(symbol.DeclaredAccessibility);
        var arguments = symbol.Parameters.ToArguments();

        var argumentNames = string.Join(", ", symbol.Parameters.Select(p => p.Name));

        var (returnTypeName, call) = symbol.ReturnsVoid switch
        {
            true => ("void", $"{name}.{symbol.Name}({argumentNames});"),
            false => (symbol.ReturnType.GetFullyQualifiedName(), $"return {name}.{symbol.Name}({argumentNames});")
        };

        memberSource = new MethodComponent(attributes, returnTypeName, symbol.Name, arguments)
        {
            new ExpressionComponent(call),
        };

        return true;
    }
}