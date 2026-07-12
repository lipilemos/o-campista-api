-- Backfill tb_usuario_trilha para criadores de trilhas existentes
-- Insere registro com Concluida=true para quem criou a trilha mas ainda não tem entrada na tabela
INSERT INTO tb_usuario_trilha (usuario_id, trilha_id, concluida, criado_em, concluida_em)
SELECT
    t.criador_id,
    t.id,
    TRUE,
    t.criado_em,
    t.criado_em
FROM tb_trilha t
WHERE t.criador_id IS NOT NULL
  AND NOT EXISTS (
      SELECT 1 FROM tb_usuario_trilha ut
      WHERE ut.usuario_id = t.criador_id
        AND ut.trilha_id = t.id
  );

-- Backfill tb_usuario_trilha para check-ins de trilha existentes
-- Insere registro com Concluida=false para quem já fez check-in mas ainda não tem entrada na tabela
INSERT INTO tb_usuario_trilha (usuario_id, trilha_id, concluida, criado_em)
SELECT DISTINCT ON (c.usuario_id, c.trilha_id)
    c.usuario_id,
    c.trilha_id,
    FALSE,
    MIN(c.criado_em) OVER (PARTITION BY c.usuario_id, c.trilha_id)
FROM tb_checkin c
WHERE c.trilha_id IS NOT NULL
  AND NOT EXISTS (
      SELECT 1 FROM tb_usuario_trilha ut
      WHERE ut.usuario_id = c.usuario_id
        AND ut.trilha_id = c.trilha_id
  );
