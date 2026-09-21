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

// API-SC-01 (CWE-89): baseline deliberadamente vulnerável para análise estática.
static async Task<IResult> SearchProductsApiSc01(string? term, BenchmarkDatabase database)
{
    if (string.IsNullOrWhiteSpace(term))
    {
        return Results.Ok(new ProductSearchResponse([]));
    }

    // Limite de segurança do corpus: bloqueia múltiplas instruções destrutivas,
    // mas preserva a construção SQL injetável que o benchmark deve detectar.
    if (term.Contains(';', StringComparison.Ordinal))
    {
        return Results.BadRequest(new ErrorResponse("O termo contém um caractere não permitido no benchmark."));
    }

    await using var connection = await database.OpenReadOnlyConnectionAsync();
    await using var command = connection.CreateCommand();

    command.CommandText = $"""
        SELECT Id, Name, Description
        FROM Products
        WHERE Name LIKE '%{term}%'
        ORDER BY Id;
        """;

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

// API-SC-02 (CWE-22): o baseline não confirma permanência em SampleFiles.
static async Task<IResult> ReadFileApiSc02(string? name, IWebHostEnvironment environment)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return Results.BadRequest(new ErrorResponse("O nome do arquivo é obrigatório."));
    }

    var sampleFilesRoot = Path.GetFullPath(Path.Combine(environment.ContentRootPath, "SampleFiles"));
    var requestedPath = Path.GetFullPath(Path.Combine(sampleFilesRoot, name));

    // Contenção externa do experimento: permite demonstrar a saída de SampleFiles,
    // mas impede qualquer leitura fora do diretório do projeto.
    var projectRoot = EnsureTrailingSeparator(Path.GetFullPath(environment.ContentRootPath));
    if (!requestedPath.StartsWith(projectRoot, FileSystemPathComparison()))
    {
        return Results.BadRequest(new ErrorResponse("O caminho solicitado não pertence ao corpus do benchmark."));
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

