-- Vincula um usuário dono ao camping (cadastro/reivindicação por parceiros).
-- dono_status: 'pendente' (aguardando aprovação manual) | 'aprovado'
ALTER TABLE tb_camping
  ADD COLUMN IF NOT EXISTS dono_usuario_id uuid NULL REFERENCES tb_usuario(id),
  ADD COLUMN IF NOT EXISTS dono_status varchar(20) NULL
    CHECK (dono_status IN ('pendente', 'aprovado'));

CREATE INDEX IF NOT EXISTS ix_camping_dono_usuario_id ON tb_camping (dono_usuario_id);
