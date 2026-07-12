-- ============================================================
-- SGISI - Esquema fisico (11 tablas)
-- ============================================================
CREATE DATABASE SGISI;
GO
USE SGISI;
GO

CREATE TABLE ORGANIZACION (
    id_organizacion     INT IDENTITY(1,1) PRIMARY KEY,
    nombre_departamento VARCHAR(100) NOT NULL,
    responsable         VARCHAR(100) NOT NULL,
    num_activos         INT NULL DEFAULT (0)
                        CHECK (num_activos >= 0)
);
GO

CREATE TABLE ANALISTA (
    id_analista      INT IDENTITY(1,1) PRIMARY KEY,
    nombre           VARCHAR(80)  NOT NULL,
    apellido         VARCHAR(80)  NOT NULL,
    cargo            VARCHAR(100) NOT NULL,
    especializacion  VARCHAR(100) NULL,
    nivel_acceso     TINYINT NOT NULL
                     CHECK (nivel_acceso >= 1 AND nivel_acceso <= 5),
    email            VARCHAR(150) NOT NULL UNIQUE,
    telefono         VARCHAR(20)  NULL,
    id_organizacion  INT NOT NULL,
    CONSTRAINT FK_ANALISTA_ORG FOREIGN KEY (id_organizacion)
        REFERENCES ORGANIZACION(id_organizacion) ON DELETE CASCADE
);
GO

CREATE TABLE SUPERVISOR (
    id_analista          INT PRIMARY KEY,
    num_analistas_cargo  INT NULL DEFAULT (0)
                         CHECK (num_analistas_cargo >= 0),
    CONSTRAINT FK_SUPERVISOR_ANALISTA FOREIGN KEY (id_analista)
        REFERENCES ANALISTA(id_analista) ON DELETE CASCADE
);
GO

CREATE TABLE SUPERVISADO (
    id_analista       INT PRIMARY KEY,
    nivel_experiencia VARCHAR(20)  NOT NULL
                      CHECK (nivel_experiencia IN ('Senior','Mid','Junior')),
    area_operacion    VARCHAR(100) NOT NULL,
    CONSTRAINT FK_SUPERVISADO_ANALISTA FOREIGN KEY (id_analista)
        REFERENCES ANALISTA(id_analista) ON DELETE CASCADE
);
GO

CREATE TABLE ACTIVO (
    id_activo          INT IDENTITY(1,1) PRIMARY KEY,
    nombre             VARCHAR(100) NOT NULL,
    direccion_ip       VARCHAR(45) NULL,
    sistema_operativo  VARCHAR(80) NULL,
    criticidad         VARCHAR(10) NOT NULL
                       CHECK (criticidad IN ('CRITICA','ALTA','MEDIA','BAJA')),
    id_organizacion    INT NOT NULL,
    CONSTRAINT FK_ACTIVO_ORG FOREIGN KEY (id_organizacion)
        REFERENCES ORGANIZACION(id_organizacion)
);
GO

CREATE TABLE VULNERABILIDAD (
    id_vulnerabilidad     INT IDENTITY(1,1) PRIMARY KEY,
    cve_id                VARCHAR(30) NULL UNIQUE,
    descripcion           VARCHAR(500) NOT NULL,
    severidad_cvss        DECIMAL(3,1) NULL
                          CHECK (severidad_cvss >= 0.0 AND severidad_cvss <= 10.0),
    fecha_descubrimiento  DATE NOT NULL DEFAULT (GETDATE()),
    estado                VARCHAR(20) NOT NULL
                          CHECK (estado IN ('PARCHEADA','EN_PROCESO','PENDIENTE')),
    id_activo             INT NOT NULL,
    CONSTRAINT FK_VULN_ACTIVO FOREIGN KEY (id_activo)
        REFERENCES ACTIVO(id_activo) ON DELETE CASCADE
);
GO

CREATE TABLE INCIDENTE (
    id_incidente  INT IDENTITY(1,1) PRIMARY KEY,
    tipo_ataque   VARCHAR(100) NOT NULL,
    fecha_hora    DATETIME NOT NULL DEFAULT (GETDATE()),
    descripcion   VARCHAR(1000) NULL,
    estado        VARCHAR(20) NOT NULL
                  CHECK (estado IN ('ABIERTO','EN_INVESTIGACION','CERRADO')),
    severidad     DECIMAL(3,1) NULL
                  CHECK (severidad >= 0.0 AND severidad <= 10.0),
    id_analista   INT NOT NULL,
    CONSTRAINT FK_INCIDENTE_ANALISTA FOREIGN KEY (id_analista)
        REFERENCES ANALISTA(id_analista)
);
GO

CREATE TABLE ALERTA (
    id_alerta         INT IDENTITY(1,1) PRIMARY KEY,
    origen            VARCHAR(50) NOT NULL
                      CHECK (origen IN ('MANUAL','ANTIVIRUS','FIREWALL','SIEM','IDS')),
    timestamp_alerta  DATETIME NOT NULL DEFAULT (GETDATE()),
    tipo              VARCHAR(100) NOT NULL,
    nivel_prioridad   VARCHAR(10) NOT NULL
                      CHECK (nivel_prioridad IN ('CRITICA','ALTA','MEDIA','BAJA')),
    id_activo         INT NOT NULL,
    id_incidente      INT NULL,
    CONSTRAINT FK_ALERTA_ACTIVO FOREIGN KEY (id_activo)
        REFERENCES ACTIVO(id_activo) ON DELETE CASCADE,
    CONSTRAINT FK_ALERTA_INCIDENTE FOREIGN KEY (id_incidente)
        REFERENCES INCIDENTE(id_incidente)
);
GO

CREATE TABLE INCIDENTE_ACTIVO (
    id_incidente     INT NOT NULL,
    id_activo        INT NOT NULL,
    impacto          VARCHAR(20) NULL
                     CHECK (impacto IN ('CRITICO','GRAVE','MODERADO','LEVE')),
    fecha_deteccion  DATETIME NULL DEFAULT (GETDATE()),
    CONSTRAINT PK_INCIDENTE_ACTIVO PRIMARY KEY (id_incidente, id_activo),
    CONSTRAINT FK_IA_INCIDENTE FOREIGN KEY (id_incidente)
        REFERENCES INCIDENTE(id_incidente) ON DELETE CASCADE,
    CONSTRAINT FK_IA_ACTIVO FOREIGN KEY (id_activo)
        REFERENCES ACTIVO(id_activo) ON DELETE CASCADE
);
GO

CREATE TABLE REPORTE (
    id_reporte        INT IDENTITY(1,1) PRIMARY KEY,
    fecha_generacion  DATE NOT NULL DEFAULT (GETDATE()),
    conclusiones      VARCHAR(2000) NULL,
    recomendaciones   VARCHAR(2000) NULL,
    id_analista       INT NOT NULL,
    id_incidente      INT NOT NULL,
    CONSTRAINT FK_REPORTE_ANALISTA FOREIGN KEY (id_analista)
        REFERENCES ANALISTA(id_analista),
    CONSTRAINT FK_REPORTE_INCIDENTE FOREIGN KEY (id_incidente)
        REFERENCES INCIDENTE(id_incidente) ON DELETE CASCADE
);
GO

-- Entidad de auditoria: registra los cambios sobre INCIDENTE
CREATE TABLE BITACORA (
    id_bitacora     INT IDENTITY(1,1) PRIMARY KEY,
    tabla_afectada  VARCHAR(100) NOT NULL,
    operacion       VARCHAR(10) NOT NULL
                    CHECK (operacion IN ('INSERT','UPDATE','DELETE')),
    id_registro     INT NULL,
    usuario         SYSNAME NOT NULL DEFAULT (SUSER_SNAME()),
    fecha           DATETIME NOT NULL DEFAULT (GETDATE()),
    valor_anterior  VARCHAR(MAX) NULL,
    valor_nuevo     VARCHAR(MAX) NULL,
    id_analista     INT NULL,
    CONSTRAINT FK_BITACORA_ANALISTA FOREIGN KEY (id_analista)
        REFERENCES ANALISTA(id_analista)
);
GO
