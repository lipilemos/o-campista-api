-- =============================================================
-- Dados de referência (catálogo) para o banco local de dev.
-- Não inclui dados de demonstração (campings/trilhas fictícios de
-- o-campista.scripts/ScriptsDB/insert*.sql) — apenas as tabelas de
-- apoio que o app espera ter populadas: recursos e conquistas.
--
-- NOTA: os ids de conquista 4 a 18 existem no Supabase de produção
-- mas o conteúdo deles não está versionado em nenhum script deste
-- repositório (foram inseridos manualmente em algum momento). Este
-- seed cria os ids 1-3 (insert.sql), 19-23 (avaliações, insert.sql)
-- e 24-30 (trilhas, insert_conquistas_trilhas.sql); o intervalo
-- 4-18 fica em aberto no banco local.
-- =============================================================

USE ocampista_dev;
GO

-- ---------------------------------------------------------------
-- Recursos (união de ScriptsDB/insert.sql + ScriptsDB/insert_recursos.sql)
-- ---------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM tb_recurso)
BEGIN
    INSERT INTO tb_recurso (nome) VALUES
    (N'Banheiro'), (N'Energia'), (N'WiFi'), (N'Pet Friendly'), (N'Piscina'),
    (N'Restaurante'), (N'Churrasqueira'), (N'Estacionamento'),
    (N'Chuveiro quente'), (N'Cozinha compartilhada'), (N'Água potável'),
    (N'Área para barracas'), (N'Área para motorhome'), (N'Chalés'),
    (N'Playground'), (N'Quadra esportiva'), (N'Lago para pesca'),
    (N'Cachoeira'), (N'Trilhas'), (N'Área para fogueira'), (N'Lanchonete'),
    (N'Conveniência'), (N'Aceita pets'), (N'Segurança 24h'),
    (N'Acessibilidade'), (N'Lavanderia'), (N'Redário'), (N'Quiosques'),
    (N'Salão de jogos'), (N'Locação de equipamentos'),
    (N'Carregador para veículo elétrico');
END
GO

-- ---------------------------------------------------------------
-- Conquistas (ids 1-3, depois 19-30 explícitos)
-- ---------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM tb_conquista)
BEGIN
    INSERT INTO tb_conquista (nome, descricao, xp_bonus) VALUES
    (N'Primeiro Acampamento', N'Realize seu primeiro check-in.', 100),
    (N'Explorador Regional',  N'Visite 5 campings.', 250),
    (N'Aventureiro',          N'Conclua sua primeira trilha.', 150);

    SET IDENTITY_INSERT tb_conquista ON;

    INSERT INTO tb_conquista (id, nome, descricao, xp_bonus, icone) VALUES
    (19, N'Primeira Avaliação',  N'Avalie um camping pela primeira vez.', 50, NULL),
    (20, N'Crítico Iniciante',   N'Avalie 5 campings.', 100, NULL),
    (21, N'Crítico Experiente',  N'Avalie 10 campings.', 200, NULL),
    (22, N'Avaliador',           N'Avalie 25 campings.', 350, NULL),
    (23, N'Mestre das Críticas', N'Avalie 50 campings.', 500, NULL),
    (24, N'Primeiro Passo na Trilha', N'Realize seu primeiro check-in em uma trilha.', 75,  N'🥾'),
    (25, N'Trilheiro Assíduo',        N'Realize check-in em 5 trilhas.', 150, N'🏕️'),
    (26, N'Explorador de Trilhas',    N'Realize check-in em 20 trilhas.', 300, N'🧭'),
    (27, N'Mestre das Trilhas',       N'Realize check-in em 50 trilhas.', 500, N'🏔️'),
    (28, N'Criador de Trilhas',       N'Crie sua primeira trilha.', 200, N'🗺️'),
    (29, N'Arquiteto da Trilha',      N'Crie 5 trilhas.', 400, N'⛰️'),
    (30, N'Lenda das Trilhas',        N'Crie 20 trilhas.', 750, N'🌟');

    SET IDENTITY_INSERT tb_conquista OFF;
END
GO
