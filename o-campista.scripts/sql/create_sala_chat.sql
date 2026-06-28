-- =============================================
-- Script: Salas de Chat (Camping + Grupo)
-- Banco: PostgreSQL (Supabase)
-- Data: 2026-06-28
-- =============================================

-- 1. Tabela de Salas de Chat
CREATE TABLE tb_sala_chat (
    id              BIGSERIAL PRIMARY KEY,
    nome            VARCHAR(100)    NOT NULL,
    tipo            VARCHAR(20)     NOT NULL,           -- 'camping' ou 'grupo'
    camping_id      BIGINT          NULL REFERENCES tb_camping(id),
    foto_capa       TEXT            NULL,
    codigo_convite  VARCHAR(8)      NULL,
    criado_por_id   UUID            NULL REFERENCES tb_usuario(id),
    criado_em       TIMESTAMPTZ     NOT NULL DEFAULT NOW()
);

-- 2. Tabela de Membros da Sala
CREATE TABLE tb_sala_chat_membro (
    sala_id                     BIGINT  NOT NULL REFERENCES tb_sala_chat(id) ON DELETE CASCADE,
    usuario_id                  UUID    NOT NULL REFERENCES tb_usuario(id),
    entrada_em                  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    ultima_mensagem_lida_id     BIGINT  NULL,

    PRIMARY KEY (sala_id, usuario_id)
);

-- 3. Tabela de Mensagens da Sala
CREATE TABLE tb_mensagem_sala_chat (
    id          BIGSERIAL PRIMARY KEY,
    sala_id     BIGINT          NOT NULL REFERENCES tb_sala_chat(id) ON DELETE CASCADE,
    usuario_id  UUID            NOT NULL REFERENCES tb_usuario(id),
    texto       VARCHAR(500)    NOT NULL,
    data_envio  TIMESTAMPTZ     NOT NULL DEFAULT NOW()
);

-- 4. Índices

-- Busca rápida de mensagens por sala ordenadas por data
CREATE INDEX IX_MensagensSalaChat_SalaId_DataEnvio
    ON tb_mensagem_sala_chat (sala_id, data_envio DESC);

-- Busca de sala por camping (para criar sala automática no check-in)
CREATE INDEX IX_SalaChat_CampingId
    ON tb_sala_chat (camping_id);

-- Busca de sala por código de convite (único, exclui nulos)
CREATE UNIQUE INDEX IX_SalaChat_CodigoConvite
    ON tb_sala_chat (codigo_convite)
    WHERE codigo_convite IS NOT NULL;
