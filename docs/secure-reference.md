# Referência segura

A branch `benchmark-secure` deriva diretamente de `benchmark-vulnerable` e preserva endpoints, DTOs, fixtures e dados. Somente os controles dos dois cenários e seus testes de segurança mudam.

## API-SC-01 / CWE-89

- Antes: o valor de `term` era interpolado em `CommandText`.
- Depois: a estrutura SQL contém `@term`, e o valor é enviado por `AddWithValue`.
- Verificação adicional: uma entrada inerte com aspas e operadores é tratada como valor e retorna coleção vazia, sem erro 500.

## API-SC-02 / CWE-22

- Antes: o caminho precisava permanecer somente dentro do content root, permitindo sair de `SampleFiles` e alcançar a fixture sintética `TestFixtures/marker.txt`.
- Depois: o caminho canonicalizado precisa começar pelo caminho completo de `SampleFiles` seguido do separador de diretório.
- Verificação adicional: a tentativa controlada de alcançar `TestFixtures/marker.txt` retorna `400` sem revelar caminhos locais.

## Invariantes comparáveis

- `GET /health` permanece inalterado.
- Busca normal por `cad` permanece inalterada.
- Leitura de `public.txt` permanece inalterada.
- Banco, carga inicial, DTOs, versões e target framework permanecem inalterados.

