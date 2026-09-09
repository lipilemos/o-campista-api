-- =============================================================
-- Provisionamento do banco de dados local de desenvolvimento
-- Instância: SQL Server Express (local)
-- Executar como usuário com privilégio sysadmin (ex: Windows Auth)
-- =============================================================
USE master;
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'devocampista')
BEGIN
    CREATE LOGIN devocampista WITH PASSWORD = '094001Fe@dev', CHECK_POLICY = ON;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'ocampista_dev')
BEGIN
    CREATE DATABASE ocampista_dev;
END
GO

ALTER LOGIN devocampista WITH DEFAULT_DATABASE = ocampista_dev;
GO

USE ocampista_dev;
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'devocampista')
BEGIN
    CREATE USER devocampista FOR LOGIN devocampista;
END
GO

ALTER ROLE db_owner ADD MEMBER devocampista;
GO
