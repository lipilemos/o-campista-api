insert into tb_recurso (nome)
values
('Banheiro'),
('Energia'),
('WiFi'),
('Pet Friendly'),
('Piscina'),
('Restaurante'),
('Churrasqueira'),
('Estacionamento');

insert into tb_conquista
(
    nome,
    descricao,
    xp_bonus
)
values
(
    'Primeiro Acampamento',
    'Realize seu primeiro check-in.',
    100
),
(
    'Explorador Regional',
    'Visite 5 campings.',
    250
),
(
    'Aventureiro',
    'Conclua sua primeira trilha.',
    150
);

insert into tb_camping
(
    nome,
    descricao,
    endereco,
    cidade,
    estado,
    telefone,
    latitude,
    longitude,
    tipo,
    avaliacao_media
)
values

(
    'Camping Recanto Verde',
    'Camping familiar próximo ao rio.',
    'Estrada Municipal km 15',
    'São Carlos',
    'SP',
    '(16) 99999-9999',
    -22.01740000,
    -47.89030000,
    'camping',
    4.8
),

(
    'Camping Cachoeira Azul',
    'Área cercada por mata nativa.',
    'Rodovia SP-215 km 120',
    'São Carlos',
    'SP',
    '(16) 98888-8888',
    -21.99870000,
    -47.87450000,
    'cachoeira',
    4.6
),

(
    'Camping Serra do Monjolinho',
    'Camping com acesso a trilhas e mirantes naturais.',
    'Estrada Serra do Monjolinho',
    'São Carlos',
    'SP',
    '(16) 99777-1111',
    -22.02795670,
    -47.96653590,
    'trilha',
    4.9
),

(
    'Camping Vale das Águas',
    'Área de camping com cachoeira e piscinas naturais.',
    'Estrada do Vale',
    'Brotas',
    'SP',
    '(14) 99888-7777',
    -22.28460000,
    -48.12660000,
    'cachoeira',
    4.7
),

(
    'Camping Serra Verde',
    'Local tranquilo para famílias e motorhomes.',
    'Estrada Rural km 8',
    'Analândia',
    'SP',
    '(19) 99911-2233',
    -22.13340000,
    -47.66320000,
    'camping',
    4.5
),

(
    'Camping Recanto do Sol',
    'Vista privilegiada para o pôr do sol.',
    'Estrada Municipal 12',
    'Ibaté',
    'SP',
    '(16) 99666-3322',
    -21.95870000,
    -47.99610000,
    'camping',
    4.4
),

(
    'Camping Pedra Grande',
    'Ideal para trilhas e observação da natureza.',
    'Estrada Pedra Grande',
    'São Pedro',
    'SP',
    '(19) 99444-8899',
    -22.54870000,
    -47.91120000,
    'trilha',
    4.8
);

insert into tb_camping_fotos
(
    camping_id,
    url,
    principal
)
values
(1, 'https://cdn.ocampista.com/campings/1.jpg', true),
(2, 'https://cdn.ocampista.com/campings/2.jpg', true),
(3, 'https://cdn.ocampista.com/campings/3.jpg', true),
(4, 'https://cdn.ocampista.com/campings/4.jpg', true),
(5, 'https://cdn.ocampista.com/campings/5.jpg', true),
(6, 'https://cdn.ocampista.com/campings/6.jpg', true),
(7, 'https://cdn.ocampista.com/campings/7.jpg', true);


insert into tb_camping_recurso
(camping_id, recurso_id, disponivel)
values

(1,1,true),
(1,2,true),
(1,4,true),

(2,1,true),

(3,1,true),
(3,2,true),
(3,4,true),
(3,8,true),

(4,1,true),
(4,5,true),
(4,7,true),

(5,1,true),
(5,2,true),
(5,8,true),

(6,1,true),
(6,4,true),

(7,1,true),
(7,2,true),
(7,7,true),
(7,8,true);

insert into tb_trilha
(
    camping_id,
    nome,
    descricao,
    distancia_km,
    dificuldade
)
values

(
    3,
    'Trilha Serra do Monjolinho',
    'Percurso com vista panorâmica.',
    6.2,
    'Moderada'
),

(
    7,
    'Trilha Pedra Grande',
    'Subida com vista para a serra.',
    4.8,
    'Moderada'
);

ALTER TABLE tb_usuario_trilha
ADD CONSTRAINT uq_usuario_trilha
UNIQUE(usuario_id, trilha_id);

INSERT INTO tb_conquista (id, nome, descricao, xp_bonus) VALUES
(19, 'Primeira Avaliação',  'Avalie um camping pela primeira vez.', 50),
(20, 'Crítico Iniciante',   'Avalie 5 campings.',                  100),
(21, 'Crítico Experiente',  'Avalie 10 campings.',                 200),
(22, 'Avaliador',           'Avalie 25 campings.',                 350),
(23, 'Mestre das Críticas', 'Avalie 50 campings.',                 500);