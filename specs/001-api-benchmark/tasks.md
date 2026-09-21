# Tarefas incrementais — SPEC-001

## Incremento 1 — contexto e contrato

- [x] Importar requisitos dos documentos de origem.
- [x] Definir critérios de aceite e matriz de rastreabilidade.
- [x] Definir arquitetura, modelo de dados e contrato OpenAPI.
- [x] Definir protocolo de execução experimental.

## Incremento 2 — baseline funcional

- [ ] Criar solução, API e projeto de testes.
- [ ] Implementar health check e catálogo em memória.
- [ ] Implementar leitura das fixtures sintéticas.
- [ ] Fixar dependências com lockfiles.
- [ ] Validar build e testes.

## Incremento 3 — baseline vulnerável controlado

- [ ] Identificar API-SC-01 e API-SC-02 no código.
- [ ] Demonstrar travessia somente até a fixture sintética.
- [ ] Registrar origem, destino, CWE e gabarito.
- [ ] Criar branch `benchmark-vulnerable`.

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

