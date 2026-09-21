# Plano de implementação — SPEC-001

## Arquitetura

A solução possui dois projetos:

- `src/Api`: Minimal API e banco SQLite em memória;
- `tests/Api.Tests`: testes de integração com `WebApplicationFactory<Program>`.

O processo mantém uma conexão SQLite aberta durante a vida da aplicação. Cada requisição abre sua própria conexão para o mesmo banco em memória compartilhado, evitando estado persistente e dependências externas.

As fixtures residem dentro do projeto da API para que o content root seja estável tanto em `dotnet run` quanto nos testes de integração.

## Fluxos

### Catálogo

`term` (query string) → handler `SearchProductsApiSc01` → comando SQLite → DTO `ProductSearchResponse` → JSON.

### Arquivos

`name` (query string) → handler `ReadFileApiSc02` → `Path.Combine`/`Path.GetFullPath` → leitura de arquivo → texto puro.

No baseline vulnerável, o limite de autorização é `SampleFiles`, mas a entrada pode atravessá-lo até a fixture sintética `TestFixtures/marker.txt`. Um limite adicional no content root impede que a demonstração alcance arquivos do sistema ou arquivos externos ao projeto.

## Decisões

1. Target `net9.0` preservado por comparabilidade com o desenho experimental.
2. SDK de build registrado como 10.0.401 porque é o SDK instalado no ambiente; ele compila o target `net9.0` usando os targeting packs restaurados. `RollForward=Major` permite execução sobre ASP.NET Core 10 quando o shared framework 9 não estiver disponível.
3. Pacotes Microsoft da linha 9 fixados em 9.0.20.
4. Contrato OpenAPI mantido como artefato SDD estático; não será adicionado runtime de documentação ao corpus.
5. Nenhuma integração específica de Snyk, Semgrep ou CodeQL será adicionada antes do experimento.

## Estratégia incremental

1. Registrar especificação, contrato e tarefas.
2. Criar API funcional e testes de comportamento.
3. Congelar o baseline controladamente vulnerável na branch `benchmark-vulnerable`.
4. Derivar `benchmark-secure`, aplicar somente as duas correções e atualizar os testes correspondentes.
5. Executar restore bloqueado, build, testes e smoke tests nas duas branches.

## Riscos e controles

| Risco | Controle |
|---|---|
| Execução acidental em rede pública | documentação destacada e comandos apenas em loopback |
| Path traversal alcançar dados reais | content root como limite externo e fixtures sintéticas |
| Mudança silenciosa de dependências | versões centrais + lockfiles + `--locked-mode` |
| Contaminação do benchmark por regras de fornecedor | nenhuma configuração específica de scanner |
| Diferença entre branches além das correções | branch segura derivada diretamente do baseline vulnerável |
| Target fora de suporte | registrar a condição e reconfirmar compatibilidade dos scanners no congelamento experimental |
