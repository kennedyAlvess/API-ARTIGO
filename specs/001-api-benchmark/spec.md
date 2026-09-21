# Especificação — API para benchmark de análise estática

**ID:** SPEC-001  
**Estado:** aprovada para implementação  
**Origem:** documentos SDD fornecidos para o artigo  
**Escopo:** somente API C#/.NET executável localmente

## Propósito

Criar um corpus pequeno, determinístico e reproduzível para comparar ferramentas de análise estática com recursos de IA. O corpus contém um catálogo sintético e dois fluxos vulneráveis conhecidos, deliberados e controlados.

O projeto é exclusivamente educacional. A versão vulnerável não pode ser implantada, publicada na internet nem usada com dados reais.

## Histórias e critérios de aceite

### US-01 — Consultar saúde da aplicação

Como pesquisador, quero confirmar que a API iniciou para separar falhas de ambiente de falhas dos cenários analisados.

- Dado que a aplicação iniciou, quando `GET /health` for chamado, então retorna `200` e `{ "status": "ok" }`.
- A operação não usa rede nem banco externo.

### US-02 — Pesquisar catálogo sintético

Como pesquisador, quero pesquisar produtos fictícios para exercitar um fluxo HTTP → SQL reconhecível.

- O catálogo contém exatamente três produtos iniciais de nomes distintos.
- `term=cad` encontra `Caderno` sem diferenciar maiúsculas de minúsculas.
- termo ausente, vazio ou sem correspondência retorna `200` com `items: []`.
- Na branch `benchmark-vulnerable`, a entrada é incorporada ao SQL por interpolação (`API-SC-01`, CWE-89).
- Na branch `benchmark-secure`, o contrato é preservado e a consulta usa parâmetro.

### US-03 — Ler fixture pública

Como pesquisador, quero ler uma fixture local para exercitar um fluxo HTTP → sistema de arquivos reconhecível.

- `name=public.txt` retorna somente o conteúdo de `SampleFiles/public.txt`.
- Arquivo inexistente retorna `404` sem revelar caminhos locais.
- Nome ausente ou vazio retorna `400`.
- Na branch `benchmark-vulnerable`, a combinação de caminho não restringe o resultado à pasta `SampleFiles` (`API-SC-02`, CWE-22), mas uma contenção externa impede acesso fora do diretório do projeto.
- Na branch `benchmark-secure`, qualquer saída de `SampleFiles` é rejeitada com `400`.

## Requisitos não funcionais

- NF-01: ASP.NET Core Minimal API em `net9.0`.
- NF-02: SQLite em memória, sem dependências de infraestrutura.
- NF-03: build e testes executáveis no Windows.
- NF-04: dependências fixadas por `packages.lock.json`.
- NF-05: fixtures e banco contêm somente dados sintéticos.
- NF-06: execução documentada apenas em loopback (`127.0.0.1`).
- NF-07: nenhum segredo, autenticação, frontend, Docker obrigatório, serviço externo ou workflow específico de fornecedor.

## Fora do escopo

Autenticação, autorização, rate limiting, deploy, observabilidade completa, microserviços, banco persistente, comandos de sistema, SSRF e exploração destrutiva.

## Matriz de rastreabilidade

| Requisito | Contrato | Implementação | Verificação |
|---|---|---|---|
| US-01 | `GET /health` | `Program.cs` | `Health_returns_ok` |
| US-02 / API-SC-01 | `GET /api/products` | `SearchProductsApiSc01` | testes de busca + `docs/test-cases.md` |
| US-03 / API-SC-02 | `GET /api/files` | `ReadFileApiSc02` | testes de fixture/travessia + `docs/test-cases.md` |
| NF-04 | arquivos de projeto | `Directory.Build.props` | restore em modo locked |
| NF-06 | comando de execução | `launchSettings.json` e README | smoke test em `127.0.0.1:5080` |

