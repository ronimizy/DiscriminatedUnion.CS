namespace DiscriminatedUnion.CS.Annotations
{
    // ReSharper disable UnusedType.Global
    // ReSharper disable UnusedTypeParameter

    public interface IDiscriminator<T> { }

    public interface IDiscriminator<TWrapped, TName> { }
}