-- Migration: Social Phase 1 — Seguidores + Configuração de Privacidade
-- Criado em: 2026-07-02

-- ============================================================
-- Tabela de seguidores (many-to-many de usuários)
-- ============================================================
CREATE TABLE IF NOT EXISTS tb_usuario_seguidor (
    seguidor_id UUID NOT NULL REFERENCES tb_usuario(id) ON DELETE CASCADE,
    seguido_id  UUID NOT NULL REFERENCES tb_usuario(id) ON DELETE CASCADE,
    criado_em   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT pk_usuario_seguidor PRIMARY KEY (seguidor_id, seguido_id),
    CONSTRAINT chk_no_self_follow CHECK (seguidor_id <> seguido_id)
);

CREATE INDEX IF NOT EXISTS ix_usuario_seguidor_seguido_id
    ON tb_usuario_seguidor (seguido_id);

CREATE INDEX IF NOT EXISTS ix_usuario_seguidor_criado_em
    ON tb_usuario_seguidor (criado_em DESC);

-- ============================================================
-- Tabela de configuração de privacidade
-- ============================================================
CREATE TABLE IF NOT EXISTS tb_configuracao_privacidade (
    usuario_id           UUID    NOT NULL PRIMARY KEY REFERENCES tb_usuario(id) ON DELETE CASCADE,
    perfil_publico       BOOLEAN NOT NULL DEFAULT TRUE,
    checkins_publicos    BOOLEAN NOT NULL DEFAULT TRUE,
    conquistas_publicas  BOOLEAN NOT NULL DEFAULT TRUE,
    nivel_publico        BOOLEAN NOT NULL DEFAULT TRUE
);

-- ============================================================
-- Trigger: cria configuração de privacidade padrão ao registrar usuário
-- ============================================================
CREATE OR REPLACE FUNCTION fn_criar_privacidade_padrao()
RETURNS TRIGGER LANGUAGE plpgsql AS $$
BEGIN
    INSERT INTO tb_configuracao_privacidade (usuario_id)
    VALUES (NEW.id)
    ON CONFLICT (usuario_id) DO NOTHING;
    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS trg_criar_privacidade_padrao ON tb_usuario;

CREATE TRIGGER trg_criar_privacidade_padrao
    AFTER INSERT ON tb_usuario
    FOR EACH ROW
    EXECUTE FUNCTION fn_criar_privacidade_padrao();

-- ============================================================
-- Backfill: criar configuração padrão para usuários existentes
-- ============================================================
INSERT INTO tb_configuracao_privacidade (usuario_id)
SELECT id FROM tb_usuario
ON CONFLICT (usuario_id) DO NOTHING;
