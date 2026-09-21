using System.Text;
using ApiBenchmark;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BenchmarkDatabase>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new HealthResponse("ok")));
app.MapGet("/api/products", SearchProductsApiSc01);
app.MapGet("/api/files", ReadFileApiSc02);

app.Run();

// API-SC-01 (CWE-89): referência segura com consulta parametrizada.
static async Task<IResult> SearchProductsApiSc01(string? term, BenchmarkDatabase database)
{
    if (string.IsNullOrWhiteSpace(term))
    {
        return Results.Ok(new ProductSearchResponse([]));
    }

    await using var connection = await database.OpenReadOnlyConnectionAsync();
    await using var command = connection.CreateCommand();

    command.CommandText = """
        SELECT Id, Name, Description
        FROM Products
        WHERE Name LIKE @term
        ORDER BY Id;
        """;
    command.Parameters.AddWithValue("@term", $"%{term}%");

    var products = new List<ProductResponse>();
    await using var reader = await command.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        products.Add(new ProductResponse(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2)));
    }

    return Results.Ok(new ProductSearchResponse(products));
}

// API-SC-02 (CWE-22): referência segura restringe a leitura a SampleFiles.
static async Task<IResult> ReadFileApiSc02(string? name, IWebHostEnvironment environment)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return Results.BadRequest(new ErrorResponse("O nome do arquivo é obrigatório."));
    }

    var sampleFilesRoot = Path.GetFullPath(Path.Combine(environment.ContentRootPath, "SampleFiles"));
    var requestedPath = Path.GetFullPath(Path.Combine(sampleFilesRoot, name));

    var allowedRoot = EnsureTrailingSeparator(sampleFilesRoot);
    if (!requestedPath.StartsWith(allowedRoot, FileSystemPathComparison()))
    {
        return Results.BadRequest(new ErrorResponse("O caminho solicitado está fora da pasta autorizada."));
    }

    if (!File.Exists(requestedPath))
    {
        return Results.NotFound(new ErrorResponse("Fixture não encontrada."));
    }

    var content = await File.ReadAllTextAsync(requestedPath, Encoding.UTF8);
    return Results.Text(content, "text/plain", Encoding.UTF8);
}

static string EnsureTrailingSeparator(string path) =>
    path.EndsWith(Path.DirectorySeparatorChar)
        ? path
        : path + Path.DirectorySeparatorChar;

static StringComparison FileSystemPathComparison() =>
    OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

public partial class Program;
