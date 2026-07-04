-- =============================================
-- Script: Comentários em Posts de Viagem
-- Banco: PostgreSQL (Supabase)
-- Data: 2026-07-03
-- =============================================

-- 1. Tabela de Comentários
CREATE TABLE tb_comentario_post (
    id          BIGSERIAL       PRIMARY KEY,
    post_id     BIGINT          NOT NULL REFERENCES tb_post_viagem(id) ON DELETE CASCADE,
    usuario_id  UUID            NOT NULL REFERENCES tb_usuario(id) ON DELETE CASCADE,
    texto       VARCHAR(500)    NOT NULL,
    criado_em   TIMESTAMPTZ     NOT NULL DEFAULT NOW()
);

-- 2. Índice para busca rápida de comentários por post ordenados por data
CREATE INDEX IX_ComentariosPost_PostId_CriadoEm
    ON tb_comentario_post (post_id, criado_em DESC);
