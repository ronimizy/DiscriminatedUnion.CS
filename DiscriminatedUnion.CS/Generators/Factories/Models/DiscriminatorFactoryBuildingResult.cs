using DiscriminatedUnion.CS.Models;

namespace DiscriminatedUnion.CS.Generators.Factories.Models;

public record struct DiscriminatorFactoryBuildingResult(
    bool HasInvalid,
    IEnumerable<Discriminator> DiscriminatorEnumerable);