-- Vincula um usuário dono ao camping (cadastro/reivindicação por parceiros).
-- dono_status: 'pendente' (aguardando aprovação manual) | 'aprovado'
-- Versão T-SQL (SQL Server local, Database:Provider = "SqlServer").
-- A versão PostgreSQL/Supabase está em add_camping_dono.sql.
USE ocampista_dev;
GO

IF COL_LENGTH('dbo.tb_camping', 'dono_usuario_id') IS NULL
BEGIN
    ALTER TABLE dbo.tb_camping
        ADD dono_usuario_id UNIQUEIDENTIFIER NULL
            CONSTRAINT fk_camping_dono_usuario FOREIGN KEY REFERENCES dbo.tb_usuario(id);
END
GO

IF COL_LENGTH('dbo.tb_camping', 'dono_status') IS NULL
BEGIN
    ALTER TABLE dbo.tb_camping
        ADD dono_status NVARCHAR(20) NULL
            CONSTRAINT ck_camping_dono_status CHECK (dono_status IN ('pendente', 'aprovado'));
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_tb_camping_dono_usuario_id' AND object_id = OBJECT_ID('dbo.tb_camping')
)
BEGIN
    CREATE INDEX IX_tb_camping_dono_usuario_id
        ON dbo.tb_camping(dono_usuario_id)
        WHERE dono_usuario_id IS NOT NULL;
END
GO
