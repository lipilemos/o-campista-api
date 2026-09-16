-- =============================================================
-- Schema completo do o-campista, portado de PostgreSQL/Supabase
-- (o-campista.scripts/ScriptsDB/tables.sql + migrations/*.sql +
--  o-campista.scripts/sql/*.sql) para T-SQL / SQL Server.
--
-- Desvios em relação ao schema original do Supabase, e por quê:
--
-- 1) A tabela era criada como "tb_campings" (plural) em tables.sql,
--    mas todo o resto do banco (FKs) e as entidades C# (Camping.cs
--    -> [Table("tb_camping")]) usam "tb_camping" (singular). Aqui
--    ela é criada já como "tb_camping", corrigindo essa inconsistência.
--
-- 2) SQL Server não permite múltiplos caminhos de CASCADE levando à
--    mesma tabela a partir da mesma origem (erro "may cause cycles or
--    multiple cascade paths"), algo que o Postgres permite. Isso afeta:
--      - tb_notificacao (destinatario_id e remetente_id -> tb_usuario)
--      - tb_usuario_seguidor (seguidor_id e seguido_id -> tb_usuario)
--      - tb_curtida_post / tb_comentario_post (usuario_id -> tb_usuario
--        E post_id -> tb_post_viagem -> usuario_id -> tb_usuario)
--    Em cada caso mantivemos CASCADE no caminho "principal" (o dono do
--    registro) e definimos NO ACTION no outro caminho. Como tb_usuario
--    tem coluna "ativo" (soft delete), a exclusão física de usuário
--    não parece ser o fluxo principal do app; ainda assim, se houver
--    uma feature de "excluir conta" com DELETE físico, ela pode
--    precisar limpar manualmente essas linhas antes de deletar o
--    usuário nesses casos com NO ACTION.
--
-- 3) tb_camping_avaliacao.foto_url: existe na entidade CampingAvaliacao
--    (FotoUrl) e é referenciada em fix_storage_public_urls.sql, mas
--    não existe nenhum CREATE/ALTER TABLE para ela em nenhum script
--    do repositório (foi adicionada direto no Supabase). Adicionada
--    aqui como NVARCHAR(MAX) NULL para bater com a entidade.
--
-- 4) Tipos: uuid -> UNIQUEIDENTIFIER, boolean -> BIT, text/varchar ->
--    NVARCHAR (Unicode, por causa de acentuação em PT-BR),
--    numeric -> DECIMAL, timestamp(tz) -> DATETIME2,
--    geography(Point,4326) -> GEOGRAPHY (nativo no SQL Server),
--    bigint identity -> BIGINT IDENTITY(1,1).
--
-- 5) A extensão PostGIS e a função buscar_presentes_proximos (SQL)
--    foram reescritas usando o tipo GEOGRAPHY nativo do SQL Server
--    (STDistance) e um índice espacial (GEOGRAPHY_AUTO_GRID).
-- =============================================================

USE ocampista_dev;
GO

-- Necessário para índices filtrados e índices espaciais
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================================
-- Tabelas de referência / catálogo
-- =============================================================

CREATE TABLE tb_recurso (
    id      BIGINT IDENTITY(1,1) PRIMARY KEY,
    nome    NVARCHAR(100) NOT NULL UNIQUE
);
GO

CREATE TABLE tb_conquista (
    id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    nome        NVARCHAR(150)   NOT NULL,
    descricao   NVARCHAR(MAX)   NOT NULL DEFAULT (''),
    xp_bonus    INT             NOT NULL DEFAULT (0),
    icone       NVARCHAR(300)   NULL
);
GO

-- =============================================================
-- Usuário
-- =============================================================

CREATE TABLE tb_usuario (
    id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    nome            NVARCHAR(150)   NOT NULL,
    email           NVARCHAR(200)   NOT NULL UNIQUE,
    senha_hash      NVARCHAR(MAX)   NOT NULL,
    data_criacao    DATETIME2(3)    NULL DEFAULT SYSUTCDATETIME(),
    ativo           BIT             NOT NULL DEFAULT (1),
    nivel           INT             NOT NULL DEFAULT (1),
    xp              INT             NOT NULL DEFAULT (0),
    foto_perfil     NVARCHAR(MAX)   NULL
);
GO

-- =============================================================
-- Camping
-- =============================================================

CREATE TABLE tb_camping (
    id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    nome            NVARCHAR(200)   NOT NULL,
    descricao       NVARCHAR(MAX)   NULL,
    endereco        NVARCHAR(500)   NULL,
    cidade          NVARCHAR(100)   NULL,
    estado          NVARCHAR(2)     NULL,
    telefone        NVARCHAR(30)    NULL,
    latitude        DECIMAL(10,8)   NOT NULL,
    longitude       DECIMAL(11,8)   NOT NULL,
    tipo            NVARCHAR(50)    NOT NULL,
    avaliacao_media DECIMAL(3,2)    NOT NULL DEFAULT (0),
    ativo           BIT             NOT NULL DEFAULT (1),
    criado_em       DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    atualizado_em   DATETIME2(3)    NULL,
    -- Dono do camping (parceiros): NULL = sem dono; status 'pendente' | 'aprovado'
    dono_usuario_id UNIQUEIDENTIFIER NULL,
    dono_status     NVARCHAR(20)    NULL,
    CONSTRAINT fk_camping_dono_usuario FOREIGN KEY (dono_usuario_id) REFERENCES tb_usuario(id),
    CONSTRAINT ck_camping_dono_status CHECK (dono_status IN ('pendente', 'aprovado'))
);
GO

CREATE INDEX IX_tb_camping_dono_usuario_id ON tb_camping(dono_usuario_id) WHERE dono_usuario_id IS NOT NULL;
GO

CREATE TABLE tb_camping_recurso (
    camping_id  BIGINT  NOT NULL,
    recurso_id  BIGINT  NOT NULL,
    disponivel  BIT     NOT NULL DEFAULT (1),
    CONSTRAINT pk_camping_recurso PRIMARY KEY (camping_id, recurso_id),
    CONSTRAINT fk_camping_recursos_camping
        FOREIGN KEY (camping_id) REFERENCES tb_camping(id),
    CONSTRAINT fk_camping_recursos_recurso
        FOREIGN KEY (recurso_id) REFERENCES tb_recurso(id)
);
GO

CREATE TABLE tb_camping_fotos (
    id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    camping_id  BIGINT          NOT NULL,
    url         NVARCHAR(MAX)   NOT NULL,
    principal   BIT             NOT NULL DEFAULT (0),
    ordem       INT             NULL DEFAULT (0),
    criado_em   DATETIME2(3)    NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT fk_camping_fotos_camping
        FOREIGN KEY (camping_id) REFERENCES tb_camping(id) ON DELETE CASCADE
);
GO

-- =============================================================
-- Trilha (já com colunas de "trilhas independentes" da migration
-- IndependentTrails.sql incorporadas na criação da tabela)
-- =============================================================

CREATE TABLE tb_trilha (
    id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    camping_id      BIGINT          NULL,
    criador_id      UNIQUEIDENTIFIER NULL,
    criador_nome    NVARCHAR(200)   NULL,
    nome            NVARCHAR(200)   NOT NULL,
    descricao       NVARCHAR(MAX)   NULL,
    distancia_km    DECIMAL(6,2)    NULL,
    dificuldade     NVARCHAR(50)    NULL,
    avaliacao_media FLOAT           NOT NULL DEFAULT (0),
    latitude        DECIMAL(18,8)   NOT NULL DEFAULT (0),
    longitude       DECIMAL(18,8)   NOT NULL DEFAULT (0),
    criado_em       DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT fk_trilhas_camping
        FOREIGN KEY (camping_id) REFERENCES tb_camping(id)
);
GO

CREATE INDEX IX_tb_trilha_criador_id ON tb_trilha(criador_id) WHERE criador_id IS NOT NULL;
CREATE INDEX IX_tb_trilha_camping_id_null ON tb_trilha(id) WHERE camping_id IS NULL;
GO

CREATE TABLE tb_trilha_pontos (
    id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    trilha_id   BIGINT          NOT NULL,
    ordem       INT             NOT NULL,
    latitude    DECIMAL(10,8)   NOT NULL,
    longitude   DECIMAL(11,8)   NOT NULL,
    CONSTRAINT fk_trilha_pontos_trilha
        FOREIGN KEY (trilha_id) REFERENCES tb_trilha(id) ON DELETE CASCADE
);
GO

-- =============================================================
-- Checkin (já com camping_id opcional, trilha_id e ocupacao)
-- =============================================================

CREATE TABLE tb_checkin (
    id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    usuario_id  UNIQUEIDENTIFIER NOT NULL,
    camping_id  BIGINT          NULL,
    trilha_id   BIGINT          NULL,
    latitude    DECIMAL(10,8)   NULL,
    longitude   DECIMAL(11,8)   NULL,
    xp_ganho    INT             NOT NULL DEFAULT (100),
    ocupacao    NVARCHAR(20)    NULL,
    criado_em   DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT fk_checkins_usuario
        FOREIGN KEY (usuario_id) REFERENCES tb_usuario(id),
    CONSTRAINT fk_checkins_camping
        FOREIGN KEY (camping_id) REFERENCES tb_camping(id),
    CONSTRAINT fk_checkins_trilha
        FOREIGN KEY (trilha_id) REFERENCES tb_trilha(id) ON DELETE SET NULL
);
GO

CREATE INDEX IX_tb_checkin_trilha_id ON tb_checkin(trilha_id) WHERE trilha_id IS NOT NULL;
CREATE INDEX IX_Checkin_CampingId_CriadoEm_Ocupacao
    ON tb_checkin (camping_id, criado_em, ocupacao)
    WHERE camping_id IS NOT NULL AND ocupacao IS NOT NULL;
GO

-- =============================================================
-- Avaliações de camping / trilha (com trilha_id e foto_url)
-- =============================================================

CREATE TABLE tb_camping_avaliacao (
    id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    camping_id  BIGINT          NULL,
    trilha_id   BIGINT          NULL,
    usuario_id  UNIQUEIDENTIFIER NOT NULL,
    checkin_id  BIGINT          NULL UNIQUE,
    nota        INT             NOT NULL CHECK (nota >= 1 AND nota <= 5),
    comentario  NVARCHAR(MAX)   NOT NULL,
    foto_url    NVARCHAR(MAX)   NULL,
    criado_em   DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    atualizado_em DATETIME2(3)  NULL,
    CONSTRAINT fk_avaliacao_camping
        FOREIGN KEY (camping_id) REFERENCES tb_camping(id),
    CONSTRAINT fk_avaliacao_usuario
        FOREIGN KEY (usuario_id) REFERENCES tb_usuario(id),
    CONSTRAINT fk_avaliacao_checkin
        FOREIGN KEY (checkin_id) REFERENCES tb_checkin(id),
    CONSTRAINT fk_avaliacao_trilha
        FOREIGN KEY (trilha_id) REFERENCES tb_trilha(id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_tb_avaliacao_trilha_id ON tb_camping_avaliacao(trilha_id) WHERE trilha_id IS NOT NULL;
GO

-- =============================================================
-- Conquistas de usuário
-- =============================================================

CREATE TABLE tb_usuario_conquista (
    usuario_id      UNIQUEIDENTIFIER NOT NULL,
    conquista_id    BIGINT          NOT NULL,
    criado_em       DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT pk_usuario_conquista PRIMARY KEY (usuario_id, conquista_id),
    CONSTRAINT fk_usuario_conquista_usuario
        FOREIGN KEY (usuario_id) REFERENCES tb_usuario(id),
    CONSTRAINT fk_usuario_conquista_conquista
        FOREIGN KEY (conquista_id) REFERENCES tb_conquista(id)
);
GO

-- =============================================================
-- Presentes (geolocalização via GEOGRAPHY nativo do SQL Server)
-- =============================================================

CREATE TABLE tb_presente (
    id                  BIGINT IDENTITY(1,1) PRIMARY KEY,
    nome                NVARCHAR(200)   NOT NULL,
    descricao           NVARCHAR(MAX)   NULL,
    foto_url            NVARCHAR(MAX)   NOT NULL,
    location            GEOGRAPHY       NOT NULL,
    codigo_resgate      NVARCHAR(100)   NULL,
    usuario_criador_id  UNIQUEIDENTIFIER NOT NULL,
    esta_disponivel     BIT             NOT NULL DEFAULT (1),
    criado_em           DATETIME2(3)    NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT fk_presente_criador
        FOREIGN KEY (usuario_criador_id) REFERENCES tb_usuario(id)
);
GO

CREATE SPATIAL INDEX idx_tb_presente_location
    ON tb_presente(location)
    USING GEOGRAPHY_AUTO_GRID;
GO

CREATE TABLE tb_usuario_presente (
    usuario_id  UNIQUEIDENTIFIER NOT NULL,
    presente_id BIGINT          NOT NULL,
    utilizado   BIT             NOT NULL DEFAULT (0),
    criado_em   DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT pk_usuario_presente PRIMARY KEY (usuario_id, presente_id),
    CONSTRAINT fk_usuario_presente_usuario
        FOREIGN KEY (usuario_id) REFERENCES tb_usuario(id),
    CONSTRAINT fk_usuario_presente_presente
        FOREIGN KEY (presente_id) REFERENCES tb_presente(id)
);
GO

-- Função equivalente a buscar_presentes_proximos (Postgres/PostGIS ST_DWithin)
CREATE OR ALTER FUNCTION dbo.buscar_presentes_proximos
(
    @user_lat       FLOAT,
    @user_lon       FLOAT,
    @raio_metros    INT
)
RETURNS TABLE
AS
RETURN
(
    SELECT *
    FROM tb_presente
    WHERE location.STDistance(geography::Point(@user_lat, @user_lon, 4326)) <= @raio_metros
);
GO

-- =============================================================
-- Trilhas concluídas / em progresso por usuário
-- =============================================================

CREATE TABLE tb_usuario_trilha (
    id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    usuario_id      UNIQUEIDENTIFIER NOT NULL,
    trilha_id       BIGINT          NOT NULL,
    concluida       BIT             NOT NULL DEFAULT (0),
    criado_em       DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    concluida_em    DATETIME2(3)    NULL,
    CONSTRAINT uq_usuario_trilha UNIQUE (usuario_id, trilha_id),
    CONSTRAINT fk_usuario_trilha_usuario
        FOREIGN KEY (usuario_id) REFERENCES tb_usuario(id),
    CONSTRAINT fk_usuario_trilha_trilha
        FOREIGN KEY (trilha_id) REFERENCES tb_trilha(id)
);
GO

-- =============================================================
-- Chat de camping
-- =============================================================

CREATE TABLE tb_mensagem_chat (
    id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    camping_id  BIGINT          NOT NULL,
    usuario_id  UNIQUEIDENTIFIER NOT NULL,
    texto       NVARCHAR(500)   NOT NULL,
    data_envio  DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT fk_mensagem_chat_camping
        FOREIGN KEY (camping_id) REFERENCES tb_camping(id),
    CONSTRAINT fk_mensagem_chat_usuario
        FOREIGN KEY (usuario_id) REFERENCES tb_usuario(id)
);
GO

CREATE INDEX IX_MensagensChat_CampingId_DataEnvio
    ON tb_mensagem_chat (camping_id, data_envio DESC);
GO

-- =============================================================
-- Salas de chat (camping ou grupo)
-- =============================================================

CREATE TABLE tb_sala_chat (
    id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    nome            NVARCHAR(100)   NOT NULL,
    tipo            NVARCHAR(20)    NOT NULL,
    camping_id      BIGINT          NULL,
    foto_capa       NVARCHAR(MAX)   NULL,
    codigo_convite  NVARCHAR(8)     NULL,
    criado_por_id   UNIQUEIDENTIFIER NULL,
    criado_em       DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT fk_sala_chat_camping
        FOREIGN KEY (camping_id) REFERENCES tb_camping(id),
    CONSTRAINT fk_sala_chat_criador
        FOREIGN KEY (criado_por_id) REFERENCES tb_usuario(id)
);
GO

CREATE INDEX IX_SalaChat_CampingId ON tb_sala_chat (camping_id);
CREATE UNIQUE INDEX IX_SalaChat_CodigoConvite
    ON tb_sala_chat (codigo_convite)
    WHERE codigo_convite IS NOT NULL;
GO

CREATE TABLE tb_sala_chat_membro (
    sala_id                     BIGINT  NOT NULL,
    usuario_id                  UNIQUEIDENTIFIER NOT NULL,
    entrada_em                  DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    ultima_mensagem_lida_id     BIGINT  NULL,
    CONSTRAINT pk_sala_chat_membro PRIMARY KEY (sala_id, usuario_id),
    CONSTRAINT fk_sala_chat_membro_sala
        FOREIGN KEY (sala_id) REFERENCES tb_sala_chat(id) ON DELETE CASCADE,
    CONSTRAINT fk_sala_chat_membro_usuario
        FOREIGN KEY (usuario_id) REFERENCES tb_usuario(id)
);
GO

CREATE TABLE tb_mensagem_sala_chat (
    id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    sala_id     BIGINT          NOT NULL,
    usuario_id  UNIQUEIDENTIFIER NOT NULL,
    texto       NVARCHAR(500)   NOT NULL,
    data_envio  DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT fk_mensagem_sala_chat_sala
        FOREIGN KEY (sala_id) REFERENCES tb_sala_chat(id) ON DELETE CASCADE,
    CONSTRAINT fk_mensagem_sala_chat_usuario
        FOREIGN KEY (usuario_id) REFERENCES tb_usuario(id)
);
GO

CREATE INDEX IX_MensagensSalaChat_SalaId_DataEnvio
    ON tb_mensagem_sala_chat (sala_id, data_envio DESC);
GO

-- =============================================================
-- Social: seguidores e privacidade
-- =============================================================

CREATE TABLE tb_usuario_seguidor (
    seguidor_id UNIQUEIDENTIFIER NOT NULL,
    seguido_id  UNIQUEIDENTIFIER NOT NULL,
    criado_em   DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT pk_usuario_seguidor PRIMARY KEY (seguidor_id, seguido_id),
    CONSTRAINT chk_no_self_follow CHECK (seguidor_id <> seguido_id),
    -- Somente um dos dois lados pode ser CASCADE no SQL Server (ver nota
    -- no topo do arquivo). seguido_id fica NO ACTION.
    CONSTRAINT fk_usuario_seguidor_seguidor
        FOREIGN KEY (seguidor_id) REFERENCES tb_usuario(id) ON DELETE CASCADE,
    CONSTRAINT fk_usuario_seguidor_seguido
        FOREIGN KEY (seguido_id) REFERENCES tb_usuario(id) ON DELETE NO ACTION
);
GO

CREATE INDEX ix_usuario_seguidor_seguido_id ON tb_usuario_seguidor (seguido_id);
CREATE INDEX ix_usuario_seguidor_criado_em ON tb_usuario_seguidor (criado_em DESC);
GO

CREATE TABLE tb_configuracao_privacidade (
    usuario_id          UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    perfil_publico       BIT NOT NULL DEFAULT (1),
    checkins_publicos    BIT NOT NULL DEFAULT (1),
    conquistas_publicas  BIT NOT NULL DEFAULT (1),
    nivel_publico        BIT NOT NULL DEFAULT (1),
    visivel_no_mapa       BIT NOT NULL DEFAULT (0),
    CONSTRAINT fk_configuracao_privacidade_usuario
        FOREIGN KEY (usuario_id) REFERENCES tb_usuario(id) ON DELETE CASCADE
);
GO

-- Trigger: cria configuração de privacidade padrão ao registrar usuário
CREATE OR ALTER TRIGGER trg_criar_privacidade_padrao
ON tb_usuario
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO tb_configuracao_privacidade (usuario_id)
    SELECT i.id
    FROM inserted i
    WHERE NOT EXISTS (
        SELECT 1 FROM tb_configuracao_privacidade cp WHERE cp.usuario_id = i.id
    );
END
GO

-- =============================================================
-- Social: posts de viagem, curtidas, comentários, feed
-- =============================================================

CREATE TABLE tb_post_viagem (
    id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    usuario_id  UNIQUEIDENTIFIER NOT NULL,
    texto       NVARCHAR(1000)  NOT NULL,
    foto_url    NVARCHAR(MAX)   NULL,
    camping_id  BIGINT          NULL,
    trilha_id   BIGINT          NULL,
    latitude    DECIMAL(10,7)   NULL,
    longitude   DECIMAL(10,7)   NULL,
    criado_em   DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT fk_post_viagem_usuario
        FOREIGN KEY (usuario_id) REFERENCES tb_usuario(id) ON DELETE CASCADE,
    CONSTRAINT fk_post_viagem_camping
        FOREIGN KEY (camping_id) REFERENCES tb_camping(id) ON DELETE SET NULL,
    CONSTRAINT fk_post_viagem_trilha
        FOREIGN KEY (trilha_id) REFERENCES tb_trilha(id) ON DELETE SET NULL
);
GO

CREATE INDEX ix_post_viagem_usuario_id ON tb_post_viagem (usuario_id);
CREATE INDEX ix_post_viagem_criado_em ON tb_post_viagem (criado_em DESC);
GO

CREATE TABLE tb_curtida_post (
    post_id     BIGINT          NOT NULL,
    usuario_id  UNIQUEIDENTIFIER NOT NULL,
    criado_em   DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT pk_curtida_post PRIMARY KEY (post_id, usuario_id),
    CONSTRAINT fk_curtida_post_post
        FOREIGN KEY (post_id) REFERENCES tb_post_viagem(id) ON DELETE CASCADE,
    -- NO ACTION: tb_post_viagem já cascateia de tb_usuario (ver nota no topo)
    CONSTRAINT fk_curtida_post_usuario
        FOREIGN KEY (usuario_id) REFERENCES tb_usuario(id) ON DELETE NO ACTION
);
GO

CREATE INDEX ix_curtida_post_usuario_id ON tb_curtida_post (usuario_id);
GO

CREATE TABLE tb_comentario_post (
    id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    post_id     BIGINT          NOT NULL,
    usuario_id  UNIQUEIDENTIFIER NOT NULL,
    texto       NVARCHAR(500)   NOT NULL,
    criado_em   DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT fk_comentario_post_post
        FOREIGN KEY (post_id) REFERENCES tb_post_viagem(id) ON DELETE CASCADE,
    -- NO ACTION: tb_post_viagem já cascateia de tb_usuario (ver nota no topo)
    CONSTRAINT fk_comentario_post_usuario
        FOREIGN KEY (usuario_id) REFERENCES tb_usuario(id) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_ComentariosPost_PostId_CriadoEm ON tb_comentario_post (post_id, criado_em DESC);
GO

CREATE TABLE tb_atividade_feed (
    id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    usuario_id      UNIQUEIDENTIFIER NOT NULL,
    tipo            NVARCHAR(30)    NOT NULL,
    referencia_id   BIGINT          NOT NULL,
    criado_em       DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    visivel         BIT             NOT NULL DEFAULT (1),
    CONSTRAINT fk_atividade_feed_usuario
        FOREIGN KEY (usuario_id) REFERENCES tb_usuario(id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_AtividadeFeed_UsuarioId_CriadoEm ON tb_atividade_feed (usuario_id, criado_em DESC);
CREATE INDEX IX_AtividadeFeed_CriadoEm ON tb_atividade_feed (criado_em DESC);
CREATE INDEX ix_atividade_feed_visivel ON tb_atividade_feed (criado_em DESC) WHERE visivel = 1;
GO

-- =============================================================
-- Notificações
-- =============================================================

CREATE TABLE tb_notificacao (
    id                  BIGINT IDENTITY(1,1) PRIMARY KEY,
    destinatario_id     UNIQUEIDENTIFIER NOT NULL,
    remetente_id        UNIQUEIDENTIFIER NOT NULL,
    tipo                NVARCHAR(30)    NOT NULL,
    lida                BIT             NOT NULL DEFAULT (0),
    post_id             BIGINT          NULL,
    post_texto          NVARCHAR(103)   NULL,
    comentario_texto    NVARCHAR(500)   NULL,
    criado_em           DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT fk_notificacao_destinatario
        FOREIGN KEY (destinatario_id) REFERENCES tb_usuario(id) ON DELETE CASCADE,
    -- NO ACTION: já existe CASCADE via destinatario_id para a mesma
    -- tabela pai (ver nota no topo do arquivo)
    CONSTRAINT fk_notificacao_remetente
        FOREIGN KEY (remetente_id) REFERENCES tb_usuario(id) ON DELETE NO ACTION
);
GO

CREATE INDEX ix_notificacao_destinatario_criado ON tb_notificacao (destinatario_id, criado_em DESC);
CREATE INDEX ix_notificacao_destinatario_lida ON tb_notificacao (destinatario_id, lida) WHERE lida = 0;
GO

-- =============================================================
-- Favoritos de camping
-- =============================================================

CREATE TABLE tb_usuario_camping_favorito (
    usuario_id  UNIQUEIDENTIFIER NOT NULL,
    camping_id  BIGINT          NOT NULL,
    criado_em   DATETIME2(3)    NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT pk_usuario_camping_favorito PRIMARY KEY (usuario_id, camping_id),
    CONSTRAINT fk_usuario_camping_favorito_usuario
        FOREIGN KEY (usuario_id) REFERENCES tb_usuario(id) ON DELETE CASCADE,
    CONSTRAINT fk_usuario_camping_favorito_camping
        FOREIGN KEY (camping_id) REFERENCES tb_camping(id) ON DELETE CASCADE
);
GO

CREATE INDEX ix_usuario_camping_favorito_usuario_id
    ON tb_usuario_camping_favorito (usuario_id, criado_em DESC);
GO

CREATE TABLE tb_achado_perdido (
    id            BIGINT IDENTITY(1,1) PRIMARY KEY,
    camping_id    BIGINT           NOT NULL,
    usuario_id    UNIQUEIDENTIFIER NOT NULL,
    tipo          NVARCHAR(10)     NOT NULL,
    titulo        NVARCHAR(120)    NOT NULL,
    descricao     NVARCHAR(500)    NULL,
    foto_url      NVARCHAR(MAX)    NULL,
    local_guarda  NVARCHAR(120)    NULL,
    resolvido     BIT              NOT NULL DEFAULT (0),
    resolvido_em  DATETIME2(3)     NULL,
    criado_em     DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT ck_achado_perdido_tipo CHECK (tipo IN ('achado', 'perdido')),
    CONSTRAINT fk_achado_perdido_camping
        FOREIGN KEY (camping_id) REFERENCES tb_camping(id) ON DELETE CASCADE,
    CONSTRAINT fk_achado_perdido_usuario
        FOREIGN KEY (usuario_id) REFERENCES tb_usuario(id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_AchadoPerdido_CampingId_CriadoEm
    ON tb_achado_perdido (camping_id, criado_em DESC);
GO

CREATE INDEX IX_AchadoPerdido_UsuarioId ON tb_achado_perdido (usuario_id);
GO
