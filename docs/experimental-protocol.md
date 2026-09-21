# Protocolo experimental

## Identificação

- Projeto: API .NET para benchmark de análise estática
- Especificação: `SPEC-001`
- Data da implementação inicial: 2026-09-21
- Sistema observado: Windows 10.0.26200, win-x64
- SDK de build observado: .NET SDK 10.0.401
- Target framework: `net9.0`
- Runtime `Microsoft.NETCore.App` observado: 9.0.16
- Runtime `Microsoft.AspNetCore.App` observado: 9.0.16

O .NET 9 já está fora da janela regular de suporte na data desta implementação. O target foi mantido para preservar a especificação do corpus; a compatibilidade efetiva de cada scanner deve ser reconfirmada antes de congelar a rodada experimental.

## Dependências diretas fixadas

| Pacote | Versão |
|---|---:|
| Microsoft.Data.Sqlite | 9.0.20 |
| Microsoft.AspNetCore.Mvc.Testing | 9.0.20 |
| Microsoft.NET.Test.Sdk | 18.10.1 |
| xunit.v3 | 4.0.1 |

As dependências transitivas são registradas em `packages.lock.json` por projeto.

## Procedimento de preparação

```powershell
dotnet restore .\ApiBenchmark.slnx
dotnet restore .\ApiBenchmark.slnx --locked-mode
dotnet build .\ApiBenchmark.slnx --no-restore
dotnet test .\tests\Api.Tests\Api.Tests.csproj --no-restore
```

## Smoke test local

```powershell
dotnet run --project .\src\Api\Api.csproj --no-restore --urls http://127.0.0.1:5080
Invoke-RestMethod http://127.0.0.1:5080/health
Invoke-RestMethod 'http://127.0.0.1:5080/api/products?term=cad'
Invoke-WebRequest 'http://127.0.0.1:5080/api/files?name=public.txt'
```

Encerrar a aplicação com `Ctrl+C`. Não substituir o endereço de loopback por interface pública.

## Congelamento da rodada

1. Revisar se o repositório contém somente fixtures sintéticas e nenhuma credencial.
2. Confirmar build e testes nas branches `benchmark-vulnerable` e `benchmark-secure`.
3. Registrar abaixo os hashes completos dos commits verificados.
4. Somente após aprovação explícita, criar a tag imutável `experiment-001` no commit vulnerável.
5. Registrar versão/configuração de Snyk, Semgrep e CodeQL fora do código-fonte do corpus.

## Registro de verificação

| Variante | Branch | Commit | Restore locked | Build | Testes | Smoke |
|---|---|---|---|---|---|---|
| Vulnerável | `benchmark-vulnerable` | PENDENTE | PENDENTE | PENDENTE | PENDENTE | PENDENTE |
| Segura | `benchmark-secure` | PENDENTE | PENDENTE | PENDENTE | PENDENTE | PENDENTE |

Tag `experiment-001`: **não criada; aguarda aprovação do baseline**.

