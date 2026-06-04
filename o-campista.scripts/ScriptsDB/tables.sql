create table tb_recurso (
    id bigint generated always as identity primary key,
    nome varchar(100) not null unique
);

create table tb_campings (
    id bigint generated always as identity primary key,
    nome varchar(200) not null,
    descricao text,
    endereco varchar(500),
    cidade varchar(100),
    estado varchar(2),
    telefone varchar(30),
    latitude numeric(10,8) not null,
    longitude numeric(11,8) not null,
    tipo varchar(50) not null,
    avaliacao_media numeric(3,2) default 0,
    ativo boolean not null default true,
    criado_em timestamp not null default current_timestamp,
    atualizado_em timestamp
);

create table tb_usuario (
    id uuid primary key default gen_random_uuid(),
    nome varchar(150) not null,
    email varchar(200) not null unique,
    senha_hash text not null,
    data_criacao timestamp default now(),
    ativo boolean default true,
    nivel integer default 1,
    xp integer default 0
);

create table tb_camping_recurso (
    camping_id bigint not null,
    recurso_id bigint not null,
    disponivel boolean not null default true,
    primary key (
        camping_id,
        recurso_id
    ),
    constraint fk_camping_recursos_camping
        foreign key (camping_id)
        references tb_camping(id),
    constraint fk_camping_recursos_recurso
        foreign key (recurso_id)
        references tb_recurso(id)
);

create table tb_camping_fotos (
    id bigint generated always as identity primary key,
    camping_id bigint not null,
    url text not null,
    principal boolean not null default false,
    ordem integer default 0,
    criado_em timestamp default current_timestamp,
    constraint fk_camping_fotos_camping
        foreign key (camping_id)
        references tb_camping(id)
        on delete cascade
);

create table tb_checkin (
    id bigint generated always as identity primary key,
    usuario_id uuid not null,
    camping_id bigint not null,
    latitude numeric(10,8),
    longitude numeric(11,8),
    xp_ganho integer not null default 100,
    criado_em timestamp not null default current_timestamp,
    constraint fk_checkins_usuario
        foreign key (usuario_id)
        references tb_usuario(id),
    constraint fk_checkins_camping
        foreign key (camping_id)
        references tb_camping(id)
);

create table tb_camping_avaliacao (
    id bigint generated always as identity primary key,
    camping_id bigint not null,
    usuario_id uuid not null,
    nota integer not null,
    comentario text,
    criado_em timestamp not null default current_timestamp,
    constraint fk_avaliacao_camping
        foreign key (camping_id)
        references tb_camping(id),
    constraint fk_avaliacao_usuario
        foreign key (usuario_id)
        references tb_usuario(id)
);

create table tb_trilha (
    id bigint generated always as identity primary key,
    camping_id bigint not null,
    nome varchar(200) not null,
    descricao text,
    distancia_km numeric(6,2),
    dificuldade varchar(50),
    criado_em timestamp not null default current_timestamp,
    constraint fk_trilhas_camping
        foreign key (camping_id)
        references tb_camping(id)
);

create table tb_trilha_pontos (
    id bigint generated always as identity primary key,
    trilha_id bigint not null,
    ordem integer not null,
    latitude numeric(10,8) not null,
    longitude numeric(11,8) not null,
    constraint fk_trilha_pontos_trilha
        foreign key (trilha_id)
        references tb_trilha(id)
        on delete cascade
);

create table tb_conquista (
    id bigint generated always as identity primary key,
    nome varchar(150) not null,
    descricao text,
    xp_bonus integer default 0,
    icone varchar(300)
);

create table tb_usuario_conquista (
    usuario_id uuid not null,
    conquista_id bigint not null,
    criado_em timestamp not null default current_timestamp,
    primary key (
        usuario_id,
        conquista_id
    ),
    constraint fk_usuario_conquista_usuario
        foreign key (usuario_id)
        references tb_usuario(id),
    constraint fk_usuario_conquista_conquista
        foreign key (conquista_id)
        references tb_conquista(id)
);