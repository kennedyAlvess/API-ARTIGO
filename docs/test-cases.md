# Casos de teste e gabarito estático

> Este documento descreve vulnerabilidades intencionais do corpus educacional. Não use a branch vulnerável em produção nem a exponha em rede pública.

## API-SC-01 — SQL injection (CWE-89)

- **Branch de benchmark:** `benchmark-vulnerable`
- **Origem:** parâmetro HTTP `term` de `GET /api/products`.
- **Propagação:** binding da query string para a variável `term`.
- **Destino:** `SqliteCommand.CommandText`, executado por `ExecuteReaderAsync`.
- **Construção vulnerável esperada:** interpolação de `term` dentro do texto SQL.
- **Dados alcançáveis:** somente a tabela `Products` do SQLite efêmero com três registros sintéticos.
- **Justificativa:** o conteúdo controlado pelo solicitante altera a estrutura da instrução SQL em vez de ser enviado como valor parametrizado.
- **Comportamento benigno de aceite:** `term=cad` retorna `Caderno`.
- **Correção de referência:** `@term` em `CommandText` e `AddWithValue` para o valor de busca.
- **Critério do scanner:** localizar o fluxo da origem HTTP até o sink SQL e associá-lo a CWE-89.

## API-SC-02 — Path traversal (CWE-22)

- **Branch de benchmark:** `benchmark-vulnerable`
- **Origem:** parâmetro HTTP `name` de `GET /api/files`.
- **Propagação:** combinação de `name` com o caminho de `SampleFiles`.
- **Destino:** `File.ReadAllTextAsync`.
- **Construção vulnerável esperada:** o caminho é normalizado sem verificar permanência dentro de `SampleFiles`.
- **Limite de segurança do benchmark:** antes da leitura, o caminho precisa continuar dentro do content root do projeto; assim, o cenário não acessa sistema, perfil de usuário ou arquivos externos ao corpus.
- **Fixture controlada:** `name=../TestFixtures/marker.txt` pode retornar apenas o marcador sintético na branch vulnerável.
- **Justificativa:** o solicitante atravessa o limite lógico da pasta autorizada `SampleFiles`.
- **Comportamento benigno de aceite:** `name=public.txt` retorna a fixture pública.
- **Correção de referência:** canonicalizar e exigir prefixo de `SampleFiles` seguido do separador de diretório.
- **Critério do scanner:** localizar o fluxo da origem HTTP até o sink de leitura e associá-lo a CWE-22.

## Entradas proibidas no protocolo

- payloads destrutivos de SQL;
- caminhos para arquivos reais do sistema, perfil do usuário ou configuração;
- credenciais, tokens, dados pessoais ou serviços externos;
- qualquer execução da branch vulnerável fora do loopback local.

