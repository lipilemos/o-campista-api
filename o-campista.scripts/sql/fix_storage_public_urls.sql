-- =============================================
-- Fix: URLs de storage privadas → públicas
-- Troca /object/{bucket}/ por /object/public/{bucket}/
-- Banco: PostgreSQL (Supabase)
-- Data: 2026-07-03
-- =============================================

-- Posts
UPDATE tb_post_viagem
SET foto_url = REPLACE(foto_url, '/storage/v1/object/bucketPost/', '/storage/v1/object/public/bucketPost/')
WHERE foto_url LIKE '%/storage/v1/object/bucketPost/%';

-- Presentes
UPDATE tb_presente
SET foto_url = REPLACE(foto_url, '/storage/v1/object/bucketGift/', '/storage/v1/object/public/bucketGift/')
WHERE foto_url LIKE '%/storage/v1/object/bucketGift/%';

-- Fotos de perfil
UPDATE tb_usuario
SET foto_perfil = REPLACE(foto_perfil, '/storage/v1/object/bucketUsers/', '/storage/v1/object/public/bucketUsers/')
WHERE foto_perfil LIKE '%/storage/v1/object/bucketUsers/%';

-- Avaliações
UPDATE tb_camping_avaliacao
SET foto_url = REPLACE(foto_url, '/storage/v1/object/bucketAvaliacao/', '/storage/v1/object/public/bucketAvaliacao/')
WHERE foto_url LIKE '%/storage/v1/object/bucketAvaliacao/%';
