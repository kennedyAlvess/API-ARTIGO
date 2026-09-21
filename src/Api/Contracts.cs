namespace ApiBenchmark;

public sealed record HealthResponse(string Status);

public sealed record ProductResponse(int Id, string Name, string Description);

public sealed record ProductSearchResponse(IReadOnlyCollection<ProductResponse> Items);

public sealed record ErrorResponse(string Error);

