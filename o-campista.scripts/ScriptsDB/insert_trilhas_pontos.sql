-- ============================================================
-- Migration: adiciona coluna foto_url em tb_camping_avaliacao
-- ============================================================
ALTER TABLE tb_camping_avaliacao
ADD COLUMN IF NOT EXISTS foto_url TEXT;


-- ============================================================
-- Trilhas existentes já inseridas (ids 1 e 2).
-- Adicionamos os pontos de rota para cada uma.
-- ============================================================

-- ----------------------------------------------------------------
-- TRILHA 1 — Serra do Monjolinho (camping_id = 3, São Carlos/SP)
-- Centro: -22.02796, -47.96654  |  6.2 km  |  Moderada
-- Percurso: saída do camping → subida à crista → mirante → retorno
-- ----------------------------------------------------------------
INSERT INTO tb_trilha_pontos (trilha_id, ordem, latitude, longitude) VALUES
(1,  1, -22.02796, -47.96654),
(1,  2, -22.02650, -47.96520),
(1,  3, -22.02490, -47.96380),
(1,  4, -22.02310, -47.96210),
(1,  5, -22.02120, -47.96070),
(1,  6, -22.01950, -47.95890),
(1,  7, -22.01770, -47.95740), -- ponto mais alto / mirante
(1,  8, -22.01900, -47.95550),
(1,  9, -22.02080, -47.95400),
(1, 10, -22.02300, -47.95560),
(1, 11, -22.02510, -47.95720),
(1, 12, -22.02680, -47.95910),
(1, 13, -22.02796, -47.96654); -- retorna ao ponto de início (loop)


-- ----------------------------------------------------------------
-- TRILHA 2 — Pedra Grande (camping_id = 7, São Pedro/SP)
-- Centro: -22.54870, -47.91120  |  4.8 km  |  Moderada
-- Percurso: ida e volta à formação rochosa
-- ----------------------------------------------------------------
INSERT INTO tb_trilha_pontos (trilha_id, ordem, latitude, longitude) VALUES
(2,  1, -22.54870, -47.91120),
(2,  2, -22.54650, -47.91050),
(2,  3, -22.54420, -47.90910),
(2,  4, -22.54190, -47.90760),
(2,  5, -22.53950, -47.90620),
(2,  6, -22.53710, -47.90490), -- cume / Pedra Grande
(2,  7, -22.53950, -47.90620),
(2,  8, -22.54190, -47.90760),
(2,  9, -22.54420, -47.90910),
(2, 10, -22.54650, -47.91050),
(2, 11, -22.54870, -47.91120); -- retorna ao início (ida e volta)


-- ============================================================
-- Novas trilhas
-- ============================================================

-- ----------------------------------------------------------------
-- TRILHA 3 — Circuito das Nascentes  (camping_id = 1, São Carlos/SP)
-- Camping Recanto Verde  |  3.4 km  |  Fácil
-- Percurso ao longo do rio, fácil para famílias
-- ----------------------------------------------------------------
INSERT INTO tb_trilha (camping_id, nome, descricao, distancia_km, dificuldade)
VALUES (
    1,
    'Circuito das Nascentes',
    'Trilha tranquila ao longo do rio com pontes de madeira e área de piquenique. Ideal para famílias com crianças.',
    3.4,
    'Fácil'
);

INSERT INTO tb_trilha_pontos (trilha_id, ordem, latitude, longitude) VALUES
(3,  1, -22.01740, -47.89030),
(3,  2, -22.01870, -47.88820),
(3,  3, -22.01980, -47.88600),
(3,  4, -22.02100, -47.88390),
(3,  5, -22.02200, -47.88170), -- nascente principal
(3,  6, -22.02080, -47.87990),
(3,  7, -22.01920, -47.88190),
(3,  8, -22.01800, -47.88450),
(3,  9, -22.01740, -47.89030); -- retorno (loop curto)


-- ----------------------------------------------------------------
-- TRILHA 4 — Trilha do Mirante  (camping_id = 5, Analândia/SP)
-- Camping Serra Verde  |  5.1 km  |  Difícil
-- Ascensão ao mirante com vista para o vale
-- ----------------------------------------------------------------
INSERT INTO tb_trilha (camping_id, nome, descricao, distancia_km, dificuldade)
VALUES (
    5,
    'Trilha do Mirante',
    'Ascensão exigente com 280 m de desnível positivo. No topo, vista de 360° para o vale de Analândia e a Serra do Cuscuzeiro.',
    5.1,
    'Difícil'
);

INSERT INTO tb_trilha_pontos (trilha_id, ordem, latitude, longitude) VALUES
(4,  1, -22.13340, -47.66320),
(4,  2, -22.13150, -47.66140),
(4,  3, -22.12930, -47.65950),
(4,  4, -22.12700, -47.65740),
(4,  5, -22.12480, -47.65530),
(4,  6, -22.12250, -47.65350),
(4,  7, -22.12050, -47.65150), -- mirante
(4,  8, -22.12250, -47.65350),
(4,  9, -22.12480, -47.65530),
(4, 10, -22.12700, -47.65740),
(4, 11, -22.12930, -47.65950),
(4, 12, -22.13150, -47.66140),
(4, 13, -22.13340, -47.66320); -- retorno (ida e volta)
