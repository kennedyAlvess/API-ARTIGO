using Microsoft.Data.Sqlite;

namespace ApiBenchmark;

public sealed class BenchmarkDatabase : IDisposable
{
    private readonly string _connectionString;
    private readonly SqliteConnection _keeperConnection;

    public BenchmarkDatabase()
    {
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = $"ApiBenchmark-{Guid.NewGuid():N}",
            Mode = SqliteOpenMode.Memory,
            Cache = SqliteCacheMode.Shared
        }.ToString();

        _keeperConnection = new SqliteConnection(_connectionString);
        _keeperConnection.Open();
        InitializeDatabase();
    }

    public async Task<SqliteConnection> OpenReadOnlyConnectionAsync()
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA query_only = ON;";
        await command.ExecuteNonQueryAsync();

        return connection;
    }

    private void InitializeDatabase()
    {
        using var command = _keeperConnection.CreateCommand();
        command.CommandText = """
            CREATE TABLE Products (
                Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                Description TEXT NOT NULL
            );

            INSERT INTO Products (Id, Name, Description) VALUES
                (1, 'Caderno', 'Produto demonstrativo para anotações'),
                (2, 'Caneta', 'Produto demonstrativo para escrita'),
                (3, 'Mochila', 'Produto demonstrativo para transporte');
            """;
        command.ExecuteNonQuery();
    }

    public void Dispose() => _keeperConnection.Dispose();
}

