-- =============================================================
-- Migration: IndependentTrails
-- Banco: PostgreSQL (Supabase)
-- Descrição: Adiciona suporte a trilhas independentes criadas
--            por usuários, check-in em trilhas e avaliações
--            de trilhas.
-- =============================================================

BEGIN;

-- =============================================================
-- 1. tb_trilha
--    - camping_id passa a ser opcional (trilhas independentes)
--    - Novos campos: criador, avaliação média e posição do marcador
-- =============================================================

-- Tornar camping_id opcional
ALTER TABLE tb_trilha
  ALTER COLUMN camping_id DROP NOT NULL;

-- Campos de criador (para trilhas criadas por usuários)
ALTER TABLE tb_trilha
  ADD COLUMN IF NOT EXISTS criador_id   uuid            DEFAULT NULL,
  ADD COLUMN IF NOT EXISTS criador_nome varchar(200)    DEFAULT NULL;

-- Avaliação média da trilha (atualizada a cada nova avaliação)
ALTER TABLE tb_trilha
  ADD COLUMN IF NOT EXISTS avaliacao_media float8 NOT NULL DEFAULT 0;

-- Lat/Lng do ponto inicial (para marcador no mapa)
-- Trilhas existentes (de campings) ficam com 0/0 — não são exibidas
-- como marcadores independentes pois camping_id IS NOT NULL
ALTER TABLE tb_trilha
  ADD COLUMN IF NOT EXISTS latitude  numeric NOT NULL DEFAULT 0,
  ADD COLUMN IF NOT EXISTS longitude numeric NOT NULL DEFAULT 0;

-- =============================================================
-- 2. tb_checkin
--    - camping_id passa a ser opcional
--    - Novo campo trilha_id para check-ins em trilhas
-- =============================================================

ALTER TABLE tb_checkin
  ALTER COLUMN camping_id DROP NOT NULL;

ALTER TABLE tb_checkin
  ADD COLUMN IF NOT EXISTS trilha_id bigint DEFAULT NULL
    REFERENCES tb_trilha(id) ON DELETE SET NULL;

-- =============================================================
-- 3. tb_camping_avaliacao
--    - camping_id passa a ser opcional
--    - Novo campo trilha_id para avaliações de trilhas
-- =============================================================

ALTER TABLE tb_camping_avaliacao
  ALTER COLUMN camping_id DROP NOT NULL;

ALTER TABLE tb_camping_avaliacao
  ADD COLUMN IF NOT EXISTS trilha_id bigint DEFAULT NULL
    REFERENCES tb_trilha(id) ON DELETE CASCADE;

-- =============================================================
-- 4. Índices
-- =============================================================

CREATE INDEX IF NOT EXISTS IX_tb_trilha_criador_id
  ON tb_trilha(criador_id)
  WHERE criador_id IS NOT NULL;

CREATE INDEX IF NOT EXISTS IX_tb_trilha_camping_id_null
  ON tb_trilha(id)
  WHERE camping_id IS NULL;

CREATE INDEX IF NOT EXISTS IX_tb_checkin_trilha_id
  ON tb_checkin(trilha_id)
  WHERE trilha_id IS NOT NULL;

CREATE INDEX IF NOT EXISTS IX_tb_avaliacao_trilha_id
  ON tb_camping_avaliacao(trilha_id)
  WHERE trilha_id IS NOT NULL;

COMMIT;
