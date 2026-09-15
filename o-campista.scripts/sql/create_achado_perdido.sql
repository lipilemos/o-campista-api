-- =============================================
-- Script: Achados e Perdidos por camping
-- Banco: PostgreSQL (Supabase)
-- Data: 2026-09-14
-- =============================================

-- 1. Tabela de itens achados/perdidos
CREATE TABLE tb_achado_perdido (
    id            BIGSERIAL    PRIMARY KEY,
    camping_id    BIGINT       NOT NULL REFERENCES tb_camping(id) ON DELETE CASCADE,
    usuario_id    UUID         NOT NULL REFERENCES tb_usuario(id) ON DELETE CASCADE,
    tipo          VARCHAR(10)  NOT NULL CHECK (tipo IN ('achado', 'perdido')),
    titulo        VARCHAR(120) NOT NULL,
    descricao     VARCHAR(500) NULL,
    foto_url      TEXT         NULL,
    local_guarda  VARCHAR(120) NULL,
    resolvido     BOOLEAN      NOT NULL DEFAULT FALSE,
    resolvido_em  TIMESTAMPTZ  NULL,
    criado_em     TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

-- 2. Índice do mural: itens de um camping ordenados por data (itens com mais de 30 dias são filtrados na query)
CREATE INDEX IX_AchadoPerdido_CampingId_CriadoEm
    ON tb_achado_perdido (camping_id, criado_em DESC);

-- 3. Índice para a checagem de vínculo que libera a DM entre quem achou e quem perdeu
CREATE INDEX IX_AchadoPerdido_UsuarioId
    ON tb_achado_perdido (usuario_id);
