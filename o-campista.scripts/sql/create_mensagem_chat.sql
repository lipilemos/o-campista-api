CREATE TABLE tb_mensagem_chat (
    id          BIGSERIAL PRIMARY KEY,
    camping_id  BIGINT        NOT NULL REFERENCES tb_camping(id),
    usuario_id  UUID          NOT NULL REFERENCES tb_usuario(id),
    texto       VARCHAR(500)  NOT NULL,
    data_envio  TIMESTAMPTZ   NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_MensagensChat_CampingId_DataEnvio
    ON tb_mensagem_chat (camping_id, data_envio DESC);
