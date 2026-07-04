-- Migration: Favoritos de Campings
-- Criado em: 2026-07-03

CREATE TABLE IF NOT EXISTS tb_usuario_camping_favorito (
    usuario_id  UUID    NOT NULL REFERENCES tb_usuario(id) ON DELETE CASCADE,
    camping_id  BIGINT  NOT NULL REFERENCES tb_camping(id) ON DELETE CASCADE,
    criado_em   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT pk_usuario_camping_favorito PRIMARY KEY (usuario_id, camping_id)
);

CREATE INDEX IF NOT EXISTS ix_usuario_camping_favorito_usuario_id
    ON tb_usuario_camping_favorito (usuario_id, criado_em DESC);
