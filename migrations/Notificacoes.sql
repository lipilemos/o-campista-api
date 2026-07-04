-- Migration: Notificações Sociais
-- Criado em: 2026-07-03

-- ============================================================
-- Tabela de notificações
-- tipos: nova_curtida | novo_comentario | novo_seguidor | mencao
-- ============================================================
CREATE TABLE IF NOT EXISTS tb_notificacao (
    id                  BIGSERIAL   PRIMARY KEY,
    destinatario_id     UUID        NOT NULL REFERENCES tb_usuario(id) ON DELETE CASCADE,
    remetente_id        UUID        NOT NULL REFERENCES tb_usuario(id) ON DELETE CASCADE,
    tipo                VARCHAR(30) NOT NULL,
    lida                BOOLEAN     NOT NULL DEFAULT FALSE,
    post_id             BIGINT,
    post_texto          VARCHAR(103),
    comentario_texto    VARCHAR(500),
    criado_em           TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS ix_notificacao_destinatario_criado
    ON tb_notificacao (destinatario_id, criado_em DESC);

CREATE INDEX IF NOT EXISTS ix_notificacao_destinatario_lida
    ON tb_notificacao (destinatario_id, lida)
    WHERE lida = FALSE;
