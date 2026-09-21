# Tarefas incrementais — SPEC-001

## Incremento 1 — contexto e contrato

- [x] Importar requisitos dos documentos de origem.
- [x] Definir critérios de aceite e matriz de rastreabilidade.
- [x] Definir arquitetura, modelo de dados e contrato OpenAPI.
- [x] Definir protocolo de execução experimental.

## Incremento 2 — baseline funcional

- [x] Criar solução, API e projeto de testes.
- [x] Implementar health check e catálogo em memória.
- [x] Implementar leitura das fixtures sintéticas.
- [x] Fixar dependências com lockfiles.
- [x] Validar build e testes.

## Incremento 3 — baseline vulnerável controlado

- [x] Identificar API-SC-01 e API-SC-02 no código.
- [x] Demonstrar travessia somente até a fixture sintética.
- [x] Registrar origem, destino, CWE e gabarito.
- [x] Criar branch `benchmark-vulnerable`.

## Incremento 4 — referência segura

- [ ] Derivar branch `benchmark-secure` do baseline.
- [ ] Parametrizar a consulta SQL.
- [ ] Restringir caminhos à pasta `SampleFiles`.
- [ ] Atualizar teste para rejeitar travessia.
- [ ] Validar build e testes.

## Incremento 5 — verificação final

- [ ] Executar restore em modo locked nas duas branches.
- [ ] Executar build e testes nas duas branches.
- [ ] Executar smoke test HTTP local.
- [ ] Registrar versões, commits e resultados em `docs/experimental-protocol.md`.
- [ ] Manter a tag `experiment-001` pendente até aprovação explícita do baseline.
