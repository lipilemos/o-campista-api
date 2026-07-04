-- Migration: Social Phase 2 — Feed Híbrido + Posts de Viagem + Curtidas
-- Criado em: 2026-07-02

-- ============================================================
-- Tabela de posts de viagem
-- ============================================================
CREATE TABLE IF NOT EXISTS tb_post_viagem (
    id          BIGSERIAL PRIMARY KEY,
    usuario_id  UUID        NOT NULL REFERENCES tb_usuario(id) ON DELETE CASCADE,
    texto       VARCHAR(1000) NOT NULL,
    foto_url    TEXT,
    camping_id  BIGINT      REFERENCES tb_camping(id) ON DELETE SET NULL,
    trilha_id   BIGINT      REFERENCES tb_trilha(id) ON DELETE SET NULL,
    latitude    NUMERIC(10, 7),
    longitude   NUMERIC(10, 7),
    criado_em   TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS ix_post_viagem_usuario_id
    ON tb_post_viagem (usuario_id);

CREATE INDEX IF NOT EXISTS ix_post_viagem_criado_em
    ON tb_post_viagem (criado_em DESC);

-- ============================================================
-- Tabela de curtidas de posts
-- ============================================================
CREATE TABLE IF NOT EXISTS tb_curtida_post (
    post_id     BIGINT NOT NULL REFERENCES tb_post_viagem(id) ON DELETE CASCADE,
    usuario_id  UUID   NOT NULL REFERENCES tb_usuario(id) ON DELETE CASCADE,
    criado_em   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT pk_curtida_post PRIMARY KEY (post_id, usuario_id)
);

CREATE INDEX IF NOT EXISTS ix_curtida_post_usuario_id
    ON tb_curtida_post (usuario_id);

-- ============================================================
-- Tabela de atividades do feed
-- tipos: checkin | conquista | avaliacao | post | trilha_concluida
-- ============================================================
CREATE TABLE IF NOT EXISTS tb_atividade_feed (
    id              BIGSERIAL PRIMARY KEY,
    usuario_id      UUID        NOT NULL REFERENCES tb_usuario(id) ON DELETE CASCADE,
    tipo            VARCHAR(30) NOT NULL,
    referencia_id   BIGINT      NOT NULL,
    criado_em       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    visivel         BOOLEAN     NOT NULL DEFAULT TRUE
);

CREATE INDEX IF NOT EXISTS ix_atividade_feed_usuario_criado
    ON tb_atividade_feed (usuario_id, criado_em DESC);

CREATE INDEX IF NOT EXISTS ix_atividade_feed_criado_em
    ON tb_atividade_feed (criado_em DESC);

-- Índice parcial para feed público (descobrir)
CREATE INDEX IF NOT EXISTS ix_atividade_feed_visivel
    ON tb_atividade_feed (criado_em DESC)
    WHERE visivel = TRUE;
