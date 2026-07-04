-- Adiciona coluna visivel_no_mapa em tb_configuracao_privacidade
-- Feature: localização em tempo real de seguidores no mapa
ALTER TABLE tb_configuracao_privacidade
ADD COLUMN IF NOT EXISTS visivel_no_mapa BOOLEAN NOT NULL DEFAULT FALSE;
