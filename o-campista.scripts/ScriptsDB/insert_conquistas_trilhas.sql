-- Conquistas para trilhas: check-ins e criação
-- Execute após insert.sql (os IDs 1-23 já devem existir)
-- PostgreSQL: usa OVERRIDING SYSTEM VALUE para inserir IDs explícitos em colunas GENERATED ALWAYS AS IDENTITY

INSERT INTO tb_conquista (id, nome, descricao, xp_bonus, icone)
OVERRIDING SYSTEM VALUE
VALUES
-- Check-ins em trilhas
(24, 'Primeiro Passo na Trilha', 'Realize seu primeiro check-in em uma trilha.',    75,  '🥾'),
(25, 'Trilheiro Assíduo',        'Realize check-in em 5 trilhas.',                 150,  '🏕️'),
(26, 'Explorador de Trilhas',    'Realize check-in em 20 trilhas.',                300,  '🧭'),
(27, 'Mestre das Trilhas',       'Realize check-in em 50 trilhas.',                500,  '🏔️'),

-- Criação de trilhas
(28, 'Criador de Trilhas',       'Crie sua primeira trilha.',                      200,  '🗺️'),
(29, 'Arquiteto da Trilha',      'Crie 5 trilhas.',                                400,  '⛰️'),
(30, 'Lenda das Trilhas',        'Crie 20 trilhas.',                               750,  '🌟');

-- Corrige a sequence para continuar a partir do maior ID inserido
SELECT setval(
    pg_get_serial_sequence('tb_conquista', 'id'),
    (SELECT MAX(id) FROM tb_conquista)
);
