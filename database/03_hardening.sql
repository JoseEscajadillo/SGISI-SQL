-- ============================================================
-- SGISI - Hardening: menor privilegio, DDM y auditoria de accesos
-- ============================================================

-- ──────────────────────── MENOR PRIVILEGIO: LOGINS NOMINALES ────────────────────────
-- NOTA: CHECK_POLICY=OFF porque las contrasenas de ejemplo son simples
-- (solo para fines academicos; en un entorno real usar contrasenas robustas).
CREATE LOGIN [Jose Escajadillo]  WITH PASSWORD = '7ycabhcynhej', CHECK_POLICY = OFF;
CREATE LOGIN [Fabrizio Valencia] WITH PASSWORD = 'fabrizio123',  CHECK_POLICY = OFF;
CREATE LOGIN [Diego Borja]       WITH PASSWORD = 'diegoxyz',     CHECK_POLICY = OFF;
CREATE LOGIN [Pedro Carpio]      WITH PASSWORD = 'basededatos',  CHECK_POLICY = OFF;
GO

USE SGISI;
GO
CREATE USER [Jose Escajadillo]  FOR LOGIN [Jose Escajadillo];
CREATE USER [Fabrizio Valencia] FOR LOGIN [Fabrizio Valencia];
CREATE USER [Diego Borja]       FOR LOGIN [Diego Borja];
CREATE USER [Pedro Carpio]      FOR LOGIN [Pedro Carpio];
GO

-- Los 4 pueden leer y ejecutar procedimientos/funciones
ALTER ROLE db_datareader ADD MEMBER [Jose Escajadillo];
ALTER ROLE db_datareader ADD MEMBER [Fabrizio Valencia];
ALTER ROLE db_datareader ADD MEMBER [Diego Borja];
ALTER ROLE db_datareader ADD MEMBER [Pedro Carpio];
GO
GRANT EXECUTE TO [Jose Escajadillo], [Fabrizio Valencia], [Diego Borja], [Pedro Carpio];
GO

-- Solo el administrador puede escribir (registrar/cerrar incidentes)
ALTER ROLE db_datawriter ADD MEMBER [Jose Escajadillo];
GO

-- A los clientes se les niega explicitamente la escritura, aunque tengan EXECUTE general
DENY EXECUTE ON dbo.sp_RegistrarIncidente TO [Fabrizio Valencia], [Diego Borja], [Pedro Carpio];
DENY EXECUTE ON dbo.sp_CerrarIncidente    TO [Fabrizio Valencia], [Diego Borja], [Pedro Carpio];
GO

-- ──────────────────────── ENMASCARAMIENTO DINAMICO DE DATOS (DDM) ────────────────────────
USE SGISI;
GO

-- ANALISTA: datos de contacto del personal interno de la consultora
ALTER TABLE dbo.ANALISTA
    ALTER COLUMN email VARCHAR(150) MASKED WITH (FUNCTION = 'email()') NOT NULL;
ALTER TABLE dbo.ANALISTA
    ALTER COLUMN telefono VARCHAR(20) MASKED WITH (FUNCTION = 'partial(0,"XXXXXX",4)') NULL;
ALTER TABLE dbo.ANALISTA
    ALTER COLUMN nivel_acceso TINYINT MASKED WITH (FUNCTION = 'default()') NOT NULL;
GO

-- ACTIVO: huella tecnica de infraestructura (IP y SO exactos)
ALTER TABLE dbo.ACTIVO
    ALTER COLUMN direccion_ip VARCHAR(45) MASKED WITH (FUNCTION = 'default()') NULL;
ALTER TABLE dbo.ACTIVO
    ALTER COLUMN sistema_operativo VARCHAR(80) MASKED WITH (FUNCTION = 'default()') NULL;
GO

-- VULNERABILIDAD: detalle exacto de como es vulnerable un activo
ALTER TABLE dbo.VULNERABILIDAD
    ALTER COLUMN cve_id VARCHAR(30) MASKED WITH (FUNCTION = 'default()') NULL;
ALTER TABLE dbo.VULNERABILIDAD
    ALTER COLUMN descripcion VARCHAR(500) MASKED WITH (FUNCTION = 'default()') NOT NULL;
GO

-- SUPERVISOR / SUPERVISADO: jerarquia interna del personal
ALTER TABLE dbo.SUPERVISOR
    ALTER COLUMN num_analistas_cargo INT MASKED WITH (FUNCTION = 'default()') NULL;
ALTER TABLE dbo.SUPERVISADO
    ALTER COLUMN nivel_experiencia VARCHAR(20) MASKED WITH (FUNCTION = 'default()') NOT NULL;
ALTER TABLE dbo.SUPERVISADO
    ALTER COLUMN area_operacion VARCHAR(100) MASKED WITH (FUNCTION = 'default()') NOT NULL;
GO

-- Solo el administrador (Jose Escajadillo) ve los datos reales
GRANT UNMASK ON dbo.ANALISTA(email)                 TO [Jose Escajadillo];
GRANT UNMASK ON dbo.ANALISTA(telefono)              TO [Jose Escajadillo];
GRANT UNMASK ON dbo.ANALISTA(nivel_acceso)          TO [Jose Escajadillo];
GRANT UNMASK ON dbo.ACTIVO(direccion_ip)            TO [Jose Escajadillo];
GRANT UNMASK ON dbo.ACTIVO(sistema_operativo)       TO [Jose Escajadillo];
GRANT UNMASK ON dbo.VULNERABILIDAD(cve_id)          TO [Jose Escajadillo];
GRANT UNMASK ON dbo.VULNERABILIDAD(descripcion)     TO [Jose Escajadillo];
GRANT UNMASK ON dbo.SUPERVISOR(num_analistas_cargo) TO [Jose Escajadillo];
GRANT UNMASK ON dbo.SUPERVISADO(nivel_experiencia)  TO [Jose Escajadillo];
GRANT UNMASK ON dbo.SUPERVISADO(area_operacion)     TO [Jose Escajadillo];
GO

-- ──────────────────────── AUDITORIA DE ACCESOS ────────────────────────
USE master;
GO

CREATE SERVER AUDIT AUDIT_SGISI
TO FILE (FILEPATH = 'D:\SQL_AUDILOG_SGISI', MAXSIZE = 10 MB, MAX_ROLLOVER_FILES = 10)
WHERE server_principal_name = 'Jose Escajadillo'
   OR server_principal_name = 'Fabrizio Valencia'
   OR server_principal_name = 'Diego Borja'
   OR server_principal_name = 'Pedro Carpio';
GO
ALTER SERVER AUDIT AUDIT_SGISI WITH (STATE = ON);
GO

CREATE SERVER AUDIT SPECIFICATION Audit_SGISI_Logins
FOR SERVER AUDIT AUDIT_SGISI
    ADD (FAILED_LOGIN_GROUP),
    ADD (SUCCESSFUL_LOGIN_GROUP);
GO
ALTER SERVER AUDIT SPECIFICATION Audit_SGISI_Logins WITH (STATE = ON);
GO

-- Permiso puntual para que la app pueda leer su propio log sin ser sysadmin
GRANT VIEW SERVER SECURITY AUDIT TO [Jose Escajadillo];
GO

-- Lectura del log de auditoria
SELECT event_time, action_id, succeeded, server_principal_name, host_name
FROM sys.fn_get_audit_file('D:\SQL_AUDILOG_SGISI\*', DEFAULT, DEFAULT)
ORDER BY event_time DESC;
