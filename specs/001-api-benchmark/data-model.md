# Modelo de dados — SPEC-001

## Product

| Campo | Tipo | Regra |
|---|---|---|
| `Id` | INTEGER | chave primária |
| `Name` | TEXT | obrigatório e distinto na carga inicial |
| `Description` | TEXT | obrigatório, conteúdo sintético |

## Carga inicial determinística

| Id | Name | Description |
|---:|---|---|
| 1 | Caderno | Produto demonstrativo para anotações |
| 2 | Caneta | Produto demonstrativo para escrita |
| 3 | Mochila | Produto demonstrativo para transporte |

O banco existe somente em memória e é recriado a cada inicialização do processo.

## DTOs HTTP

- `HealthResponse(status)`
- `ProductResponse(id, name, description)`
- `ProductSearchResponse(items)`
- `ErrorResponse(error)`

