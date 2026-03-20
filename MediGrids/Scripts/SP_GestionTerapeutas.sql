/*
=============================================
Procedimientos Almacenados - Gestion de Terapeutas
RF-09: Registro, Edicion, Activar/Desactivar, Visualizacion por especialidad
Ejecutar en SQL Server Management Studio contra la base de datos del proyecto
=============================================
*/

SET NOCOUNT ON;

-- Eliminar SPs si existen (para re-ejecutar el script)
IF OBJECT_ID('sp_Terapeuta_Listar', 'P') IS NOT NULL DROP PROCEDURE sp_Terapeuta_Listar;
IF OBJECT_ID('sp_Terapeuta_Registrar', 'P') IS NOT NULL DROP PROCEDURE sp_Terapeuta_Registrar;
IF OBJECT_ID('sp_Terapeuta_Actualizar', 'P') IS NOT NULL DROP PROCEDURE sp_Terapeuta_Actualizar;
IF OBJECT_ID('sp_Terapeuta_CambiarEstado', 'P') IS NOT NULL DROP PROCEDURE sp_Terapeuta_CambiarEstado;
IF OBJECT_ID('sp_Terapeuta_ObtenerPorId', 'P') IS NOT NULL DROP PROCEDURE sp_Terapeuta_ObtenerPorId;
GO

-- =============================================
-- 1. Listar terapeutas (con filtro por id_categoria - RF-09.5)
-- =============================================
CREATE PROCEDURE sp_Terapeuta_Listar
    @id_categoria INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.id_terapeuta AS IdTerapeuta,
        t.id_usuario AS IdUsuario,
        t.nombre AS Nombre,
        t.apellidos AS Apellidos,
        t.telefono AS Telefono,
        t.email AS Email,
        ISNULL(t.activo, 1) AS Activo,
        t.id_categoria AS IdCategoria,
        c.nombre AS NombreCategoria,
        tt.nombre AS NombreTipoTerapia
    FROM dbo.Terapeuta t
    LEFT JOIN dbo.CategoriaClinica c ON t.id_categoria = c.id_categoria
    LEFT JOIN dbo.TipoTerapia tt ON c.id_terapia = tt.id_terapia
    WHERE (@id_categoria IS NULL OR t.id_categoria = @id_categoria)
    ORDER BY t.apellidos, t.nombre;
END
GO

-- =============================================
-- 2. Registrar terapeuta - RF-09.1 (crea Usuario + Terapeuta)
-- =============================================
CREATE PROCEDURE sp_Terapeuta_Registrar
    @username NVARCHAR(100),
    @password_hash NVARCHAR(255),
    @nombre NVARCHAR(100),
    @apellidos NVARCHAR(100),
    @telefono NVARCHAR(50) = NULL,
    @email NVARCHAR(100) = NULL,
    @id_categoria INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @id_usuario INT;
    
    -- 1. Crear usuario (id_rol = 2 = Terapeuta)
    INSERT INTO dbo.Usuario (username, password_hash, id_rol, activo)
    VALUES (@username, @password_hash, 2, 1);
    
    SET @id_usuario = SCOPE_IDENTITY();
    
    -- 2. Crear terapeuta
    INSERT INTO dbo.Terapeuta (id_usuario, nombre, apellidos, telefono, email, activo, id_categoria)
    VALUES (@id_usuario, @nombre, @apellidos, @telefono, @email, 1, @id_categoria);
    
    SELECT SCOPE_IDENTITY() AS IdTerapeuta;
END
GO

-- =============================================
-- 3. Actualizar terapeuta - RF-09.3
-- =============================================
CREATE PROCEDURE sp_Terapeuta_Actualizar
    @id_terapeuta INT,
    @nombre NVARCHAR(100),
    @apellidos NVARCHAR(100),
    @telefono NVARCHAR(50) = NULL,
    @email NVARCHAR(100) = NULL,
    @id_categoria INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE dbo.Terapeuta
    SET nombre = @nombre,
        apellidos = @apellidos,
        telefono = @telefono,
        email = @email,
        id_categoria = @id_categoria
    WHERE id_terapeuta = @id_terapeuta;
END
GO

-- =============================================
-- 4. Activar/Desactivar terapeuta - RF-09.4
-- =============================================
CREATE PROCEDURE sp_Terapeuta_CambiarEstado
    @id_terapeuta INT,
    @activo BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE dbo.Terapeuta
    SET activo = @activo
    WHERE id_terapeuta = @id_terapeuta;
END
GO

-- =============================================
-- 5. Obtener terapeuta por ID (para formulario de edicion)
-- =============================================
CREATE PROCEDURE sp_Terapeuta_ObtenerPorId
    @id_terapeuta INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.id_terapeuta AS IdTerapeuta,
        t.id_usuario AS IdUsuario,
        t.nombre AS Nombre,
        t.apellidos AS Apellidos,
        t.telefono AS Telefono,
        t.email AS Email,
        ISNULL(t.activo, 1) AS Activo,
        t.id_categoria AS IdCategoria,
        c.nombre AS NombreCategoria,
        tt.nombre AS NombreTipoTerapia,
        u.username AS Username
    FROM dbo.Terapeuta t
    LEFT JOIN dbo.CategoriaClinica c ON t.id_categoria = c.id_categoria
    LEFT JOIN dbo.TipoTerapia tt ON c.id_terapia = tt.id_terapia
    LEFT JOIN dbo.Usuario u ON t.id_usuario = u.id_usuario
    WHERE t.id_terapeuta = @id_terapeuta;
END
GO

PRINT 'Procedimientos almacenados creados correctamente.';
