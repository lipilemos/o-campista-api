-- Associação de recursos aos campings existentes
-- Usa subquery por nome para evitar dependência de IDs fixos
INSERT INTO tb_camping_recurso (camping_id, recurso_id, disponivel)
SELECT c.id, r.id, true
FROM (VALUES
    -- Camping Recanto Verde (camping familiar, perto do rio)
    (1, 'Churrasqueira'),
    (1, 'Chuveiro quente'),
    (1, 'Água potável'),
    (1, 'Área para barracas'),
    (1, 'Área para fogueira'),
    (1, 'Quiosques'),
    (1, 'Aceita pets'),

    -- Camping Cachoeira Azul (mata nativa)
    (2, 'Chuveiro quente'),
    (2, 'Água potável'),
    (2, 'Área para barracas'),
    (2, 'Cachoeira'),
    (2, 'Trilhas'),
    (2, 'Área para fogueira'),
    (2, 'Aceita pets'),

    -- Camping Serra do Monjolinho (trilhas e mirantes)
    (3, 'Chuveiro quente'),
    (3, 'Água potável'),
    (3, 'Trilhas'),
    (3, 'Área para fogueira'),
    (3, 'Locação de equipamentos'),
    (3, 'Lanchonete'),

    -- Camping Vale das Águas (cachoeira e piscinas naturais)
    (4, 'Cachoeira'),
    (4, 'Chuveiro quente'),
    (4, 'Lanchonete'),
    (4, 'Segurança 24h'),
    (4, 'Quiosques'),
    (4, 'Área para fogueira'),
    (4, 'Aceita pets'),

    -- Camping Serra Verde (famílias e motorhomes)
    (5, 'Área para motorhome'),
    (5, 'Chuveiro quente'),
    (5, 'Cozinha compartilhada'),
    (5, 'Água potável'),
    (5, 'Playground'),
    (5, 'Lavanderia'),
    (5, 'Carregador para veículo elétrico'),

    -- Camping Recanto do Sol (vista para o pôr do sol)
    (6, 'Chuveiro quente'),
    (6, 'Churrasqueira'),
    (6, 'Área para barracas'),
    (6, 'Área para fogueira'),
    (6, 'Água potável'),
    (6, 'Redário'),

    -- Camping Pedra Grande (trilhas e observação da natureza)
    (7, 'Trilhas'),
    (7, 'Chuveiro quente'),
    (7, 'Água potável'),
    (7, 'Área para barracas'),
    (7, 'Locação de equipamentos'),
    (7, 'Área para fogueira'),
    (7, 'Cachoeira')
) AS dados(camping_id, recurso_nome)
JOIN tb_camping c ON c.id = dados.camping_id
JOIN tb_recurso r ON r.nome = dados.recurso_nome
ON CONFLICT (camping_id, recurso_id) DO NOTHING;
