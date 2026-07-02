-- Feature: Status de Ocupação do Camping
-- Adiciona campo opcional de ocupação ao check-in (tranquilo | movimentado | lotado)
ALTER TABLE tb_checkin ADD COLUMN ocupacao VARCHAR(20) NULL;

-- Índice para agilizar a query de agregação das últimas 6h por camping
CREATE INDEX IF NOT EXISTS IX_Checkin_CampingId_CriadoEm_Ocupacao
    ON tb_checkin (camping_id, criado_em, ocupacao)
    WHERE camping_id IS NOT NULL AND ocupacao IS NOT NULL;
