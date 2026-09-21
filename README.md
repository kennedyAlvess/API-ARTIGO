# API .NET — benchmark de análise estática

> **Atenção:** este repositório contém uma branch deliberadamente vulnerável para pesquisa. Não use em produção, não implante e não exponha a API em rede pública.

API local criada como corpus experimental do artigo sobre ferramentas de análise estática com recursos de IA. O projeto usa ASP.NET Core Minimal API, SQLite em memória e dados exclusivamente sintéticos.

## Variantes do corpus

- `benchmark-vulnerable`: baseline de análise com `API-SC-01` (CWE-89) e `API-SC-02` (CWE-22), ambos controlados pelos limites descritos em `docs/test-cases.md`;
- `benchmark-secure`: referência comparável com consulta parametrizada e contenção em `SampleFiles`;
- `main`: histórico da criação incremental; após a implementação, acompanha a referência segura.

A tag `experiment-001` não é criada automaticamente. Ela deve apontar para o commit aprovado da branch vulnerável somente depois da revisão do corpus.

## Pré-requisitos

- Windows;
- .NET SDK 10.0.401 ou patch compatível da mesma feature band;
- targeting pack capaz de compilar `net9.0`.

O target `net9.0` foi preservado conforme a especificação do artigo, embora esteja fora de suporte na data desta implementação. Reconfirme o suporte em cada ferramenta antes de congelar o experimento. A solução usa `RollForward=Major`: prefere ASP.NET Core 9 quando disponível e aceita a próxima major instalada para viabilizar a reprodução em ambientes atuais.

## Executar

```powershell
dotnet restore .\ApiBenchmark.slnx
dotnet restore .\ApiBenchmark.slnx --locked-mode
dotnet build .\ApiBenchmark.slnx --no-restore
dotnet test .\tests\Api.Tests\Api.Tests.csproj --no-restore
dotnet run --project .\src\Api\Api.csproj --no-restore --urls http://127.0.0.1:5080
```

A aplicação deve ser encerrada com `Ctrl+C`. Use somente o endereço de loopback documentado.

## Endpoints

| Método | Rota | Resultado |
|---|---|---|
| GET | `/health` | `{ "status": "ok" }` |
| GET | `/api/products?term=cad` | catálogo no formato `{ "items": [...] }` |
| GET | `/api/files?name=public.txt` | conteúdo da fixture pública |

Exemplos:

```powershell
Invoke-RestMethod http://127.0.0.1:5080/health
Invoke-RestMethod 'http://127.0.0.1:5080/api/products?term=cad'
Invoke-WebRequest 'http://127.0.0.1:5080/api/files?name=public.txt'
```

## Contexto SDD

- `specs/001-api-benchmark/spec.md`: requisitos e critérios de aceite;
- `specs/001-api-benchmark/plan.md`: desenho técnico, riscos e estratégia incremental;
- `specs/001-api-benchmark/data-model.md`: modelo e carga sintética;
- `specs/001-api-benchmark/contracts/openapi.yaml`: contrato HTTP;
- `specs/001-api-benchmark/tasks.md`: progresso rastreável por incremento;
- `docs/test-cases.md`: gabarito dos cenários estáticos;
- `docs/experimental-protocol.md`: ambiente, comandos e registro de resultados.

## Limites de segurança do benchmark

- SQLite é efêmero, contém apenas três produtos sintéticos e abre conexões de consulta em modo `query_only`.
- O baseline bloqueia `;` no termo para impedir múltiplas instruções, sem remover o fluxo vulnerável que as ferramentas devem identificar.
- A travessia controlada pode alcançar apenas arquivos dentro do content root do projeto; o teste usa exclusivamente `TestFixtures/marker.txt`.
- Nenhum teste usa rede externa, arquivo real do usuário, credencial ou payload destrutivo.

Consulte o [gabarito dos casos](docs/test-cases.md) antes de modificar IDs, fluxos ou fixtures.
