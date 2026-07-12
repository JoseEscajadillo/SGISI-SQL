-- ============================================================
-- SGISI - Programas: Funciones, Procedimientos, Triggers y Cursor
-- ============================================================
USE SGISI;
GO

-- ──────────────────────── FUNCIONES ────────────────────────
CREATE FUNCTION fn_BuscarOrganizacion (@id INT = NULL, @nombre_depto VARCHAR(100) = NULL)
RETURNS TABLE AS RETURN
(
    SELECT id_organizacion, num_activos
    FROM dbo.ORGANIZACION
    WHERE (@id IS NOT NULL AND id_organizacion = @id)
)
GO

CREATE FUNCTION fn_BuscarAnalista (@id INT = NULL, @cargo VARCHAR(100) = NULL)
RETURNS TABLE AS RETURN
(
    SELECT id_analista, id_organizacion
    FROM dbo.ANALISTA
    WHERE (@id IS NOT NULL AND id_analista = @id)
)
GO

CREATE FUNCTION fn_BuscarIncidente (@id INT = NULL, @tipo_ataque VARCHAR(100) = NULL)
RETURNS TABLE AS RETURN
(
    SELECT id_incidente, tipo_ataque, fecha_hora, descripcion, estado, severidad, id_analista
    FROM dbo.INCIDENTE
    WHERE (@id IS NOT NULL AND id_incidente = @id)
       OR (@tipo_ataque IS NOT NULL AND tipo_ataque LIKE '%' + @tipo_ataque + '%')
);
GO

CREATE FUNCTION fn_BuscarAlerta (@id INT = NULL, @tipo VARCHAR(100) = NULL)
RETURNS TABLE AS RETURN
(
    SELECT id_alerta, origen, timestamp_alerta, tipo, nivel_prioridad, id_activo, id_incidente
    FROM dbo.ALERTA
    WHERE (@id IS NOT NULL AND id_alerta = @id)
       OR (@tipo IS NOT NULL AND tipo LIKE '%' + @tipo + '%')
);
GO

CREATE FUNCTION fn_BuscarReporte (@id INT = NULL, @conclusiones VARCHAR(2000) = NULL)
RETURNS TABLE AS RETURN
(
    SELECT id_reporte, fecha_generacion, conclusiones, recomendaciones, id_incidente, id_analista
    FROM dbo.REPORTE
    WHERE (@id IS NOT NULL AND id_reporte = @id)
       OR (@conclusiones IS NOT NULL AND conclusiones LIKE '%' + @conclusiones + '%')
);
GO

CREATE FUNCTION fn_BuscarVulnerabilidad (@id INT = NULL)
RETURNS TABLE AS RETURN
(
    SELECT id_vulnerabilidad, descripcion, estado
    FROM dbo.VULNERABILIDAD
    WHERE (@id IS NOT NULL AND id_vulnerabilidad = @id)
);
GO

CREATE FUNCTION fn_HistorialIncidentesPorActivo (@id_activo INT)
RETURNS TABLE
AS
RETURN
(
    SELECT
        IA.id_activo,
        I.id_incidente,
        I.tipo_ataque,
        I.estado AS estado_incidente,
        I.severidad AS severidad_incidente,
        IA.impacto,
        IA.fecha_deteccion
    FROM dbo.INCIDENTE_ACTIVO IA
    JOIN dbo.INCIDENTE I ON IA.id_incidente = I.id_incidente
    WHERE IA.id_activo = @id_activo
);
GO

CREATE FUNCTION fn_ListarActivosCriticos(@id_organizacion INT)
RETURNS TABLE
AS
RETURN
(
    SELECT id_activo, nombre, direccion_ip, sistema_operativo, criticidad
    FROM ACTIVO
    WHERE id_organizacion = @id_organizacion
      AND criticidad IN ('ALTA','CRITICA')
);
GO

CREATE FUNCTION fn_ContarIncidentesPorAnalista(@id_analista INT)
RETURNS INT
AS
BEGIN
    DECLARE @total INT;
    SELECT @total = COUNT(*)
    FROM INCIDENTE
    WHERE id_analista = @id_analista;
    RETURN @total;
END;
GO

CREATE FUNCTION fn_SeveridadPromedioOrganizacion(@id_organizacion INT)
RETURNS DECIMAL(5,2)
AS
BEGIN
    DECLARE @promedio DECIMAL(5,2);
    SELECT @promedio = AVG(I.severidad)
    FROM INCIDENTE I
    JOIN ANALISTA A ON I.id_analista = A.id_analista
    WHERE A.id_organizacion = @id_organizacion;
    RETURN ISNULL(@promedio, 0);
END;
GO

-- ──────────────────────── PROCEDIMIENTOS ALMACENADOS ────────────────────────
CREATE PROCEDURE sp_RegistrarIncidente
    @tipo_ataque VARCHAR(100),
    @descripcion VARCHAR(1000),
    @severidad   DECIMAL(3,1),
    @id_analista INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO INCIDENTE
            (tipo_ataque, fecha_hora, descripcion, estado, severidad, id_analista)
        VALUES
            (@tipo_ataque, GETDATE(), @descripcion, 'ABIERTO', @severidad, @id_analista);
        COMMIT TRANSACTION;
        SELECT SCOPE_IDENTITY() AS id_incidente_creado;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE sp_RegistrarAlerta
    @origen         VARCHAR(50),
    @tipo           VARCHAR(100),
    @nivel_prioridad VARCHAR(10),
    @id_activo      INT,
    @id_incidente   INT = NULL
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO ALERTA
            (origen, timestamp_alerta, tipo, nivel_prioridad, id_activo, id_incidente)
        VALUES
            (@origen, GETDATE(), @tipo, @nivel_prioridad, @id_activo, @id_incidente);
        COMMIT TRANSACTION;
        SELECT SCOPE_IDENTITY() AS id_alerta_creada;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE sp_AsignarActivoAIncidente
    @id_incidente INT,
    @id_activo    INT,
    @impacto      VARCHAR(20)
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM INCIDENTE WHERE id_incidente = @id_incidente)
            THROW 50001, 'El incidente especificado no existe.', 1;
        IF NOT EXISTS (SELECT 1 FROM ACTIVO WHERE id_activo = @id_activo)
            THROW 50002, 'El activo especificado no existe.', 1;
        INSERT INTO INCIDENTE_ACTIVO (id_incidente, id_activo, impacto, fecha_deteccion)
        VALUES (@id_incidente, @id_activo, @impacto, GETDATE());
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE sp_CerrarIncidente
    @id_incidente INT,
    @conclusiones VARCHAR(2000),
    @recomendaciones VARCHAR(2000),
    @id_analista  INT
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE INCIDENTE
        SET estado = 'CERRADO'
        WHERE id_incidente = @id_incidente;

        INSERT INTO REPORTE
            (fecha_generacion, conclusiones, recomendaciones, id_analista, id_incidente)
        VALUES
            (GETDATE(), @conclusiones, @recomendaciones, @id_analista, @id_incidente);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE sp_GenerarReporte
    @id_incidente    INT,
    @conclusiones    VARCHAR(2000),
    @recomendaciones VARCHAR(2000),
    @id_analista     INT
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM INCIDENTE WHERE id_incidente = @id_incidente)
            THROW 50003, 'El incidente especificado no existe.', 1;
        INSERT INTO REPORTE
            (fecha_generacion, conclusiones, recomendaciones, id_analista, id_incidente)
        VALUES
            (GETDATE(), @conclusiones, @recomendaciones, @id_analista, @id_incidente);
        COMMIT TRANSACTION;
        SELECT SCOPE_IDENTITY() AS id_reporte_creado;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- ──────────────────────── TRIGGERS ────────────────────────
CREATE TRIGGER trg_ActualizarNumActivos_Insert
ON ACTIVO
AFTER INSERT
AS
BEGIN
    UPDATE ORGANIZACION
    SET num_activos = num_activos + 1
    WHERE id_organizacion IN (SELECT id_organizacion FROM inserted);
END;
GO

CREATE TRIGGER trg_ActualizarNumActivos_Delete
ON ACTIVO
AFTER DELETE
AS
BEGIN
    UPDATE ORGANIZACION
    SET num_activos = num_activos - 1
    WHERE id_organizacion IN (SELECT id_organizacion FROM deleted);
END;
GO

CREATE TRIGGER trg_ActualizarNumActivos_Update
ON dbo.ACTIVO
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(id_organizacion)
    BEGIN
        UPDATE o
        SET o.num_activos = o.num_activos - x.cnt
        FROM dbo.ORGANIZACION o
        JOIN (SELECT id_organizacion, COUNT(*) AS cnt
              FROM deleted GROUP BY id_organizacion) x
          ON o.id_organizacion = x.id_organizacion;

        UPDATE o
        SET o.num_activos = o.num_activos + x.cnt
        FROM dbo.ORGANIZACION o
        JOIN (SELECT id_organizacion, COUNT(*) AS cnt
              FROM inserted GROUP BY id_organizacion) x
          ON o.id_organizacion = x.id_organizacion;
    END
END;
GO

CREATE TRIGGER trg_ValidarEspecializacionDisjunta
ON SUPERVISADO
AFTER INSERT
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM inserted I
        JOIN SUPERVISOR S ON I.id_analista = S.id_analista
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50010, 'Un analista no puede ser Supervisor y Supervisado al mismo tiempo.', 1;
    END
END;
GO

CREATE TRIGGER trg_ValidarSupervisorDisjunto
ON SUPERVISOR
AFTER INSERT
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM inserted I
        JOIN SUPERVISADO S ON I.id_analista = S.id_analista
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50011, 'Un analista no puede ser Supervisado y Supervisor al mismo tiempo.', 1;
    END
END;
GO

CREATE TRIGGER trg_AuditarIncidente
ON dbo.INCIDENTE
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @hayIns BIT = CASE WHEN EXISTS(SELECT 1 FROM inserted) THEN 1 ELSE 0 END;
    DECLARE @hayDel BIT = CASE WHEN EXISTS(SELECT 1 FROM deleted)  THEN 1 ELSE 0 END;

    IF @hayIns = 1 AND @hayDel = 1
    BEGIN
        INSERT INTO dbo.BITACORA (tabla_afectada, operacion, id_registro, id_analista, valor_anterior, valor_nuevo)
        SELECT 'INCIDENTE', 'UPDATE', i.id_incidente, i.id_analista,
               'estado=' + ISNULL(d.estado COLLATE DATABASE_DEFAULT,'NULL')
                 + '; severidad=' + ISNULL(CAST(d.severidad AS VARCHAR(10)),'NULL')
                 + '; analista=' + ISNULL(CAST(d.id_analista AS VARCHAR(10)),'NULL'),
               'estado=' + ISNULL(i.estado COLLATE DATABASE_DEFAULT,'NULL')
                 + '; severidad=' + ISNULL(CAST(i.severidad AS VARCHAR(10)),'NULL')
                 + '; analista=' + ISNULL(CAST(i.id_analista AS VARCHAR(10)),'NULL')
        FROM inserted i JOIN deleted d ON i.id_incidente = d.id_incidente;
    END
    ELSE IF @hayIns = 1
    BEGIN
        INSERT INTO dbo.BITACORA (tabla_afectada, operacion, id_registro, id_analista, valor_anterior, valor_nuevo)
        SELECT 'INCIDENTE', 'INSERT', i.id_incidente, i.id_analista, NULL,
               'estado=' + ISNULL(i.estado COLLATE DATABASE_DEFAULT,'NULL')
                 + '; severidad=' + ISNULL(CAST(i.severidad AS VARCHAR(10)),'NULL')
                 + '; tipo=' + ISNULL(i.tipo_ataque COLLATE DATABASE_DEFAULT,'NULL')
        FROM inserted i;
    END
    ELSE IF @hayDel = 1
    BEGIN
        INSERT INTO dbo.BITACORA (tabla_afectada, operacion, id_registro, id_analista, valor_anterior, valor_nuevo)
        SELECT 'INCIDENTE', 'DELETE', d.id_incidente, d.id_analista,
               'estado=' + ISNULL(d.estado COLLATE DATABASE_DEFAULT,'NULL')
                 + '; severidad=' + ISNULL(CAST(d.severidad AS VARCHAR(10)),'NULL')
                 + '; tipo=' + ISNULL(d.tipo_ataque COLLATE DATABASE_DEFAULT,'NULL'),
               NULL
        FROM deleted d;
    END
END;
GO

-- ──────────────────────── CURSOR ────────────────────────
-- Recorre fila por fila cada analista y consolida su resumen de seguridad.
CREATE PROCEDURE dbo.sp_ResumenSeguridadPorAnalista
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @id INT, @org INT, @total INT, @prom DECIMAL(5,2);

    DECLARE @Resumen TABLE (
        id_analista        INT,
        id_organizacion    INT,
        total_incidentes   INT,
        severidad_prom_org DECIMAL(5,2)
    );

    DECLARE cur_analistas CURSOR LOCAL FAST_FORWARD FOR
        SELECT id_analista, id_organizacion
        FROM dbo.ANALISTA
        ORDER BY id_analista;

    OPEN cur_analistas;
    FETCH NEXT FROM cur_analistas INTO @id, @org;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @total = dbo.fn_ContarIncidentesPorAnalista(@id);
        SET @prom  = dbo.fn_SeveridadPromedioOrganizacion(@org);

        PRINT 'Analista ' + CAST(@id AS VARCHAR(10))
            + ' | Incidentes: ' + CAST(@total AS VARCHAR(10))
            + ' | Severidad prom. org: ' + CAST(@prom AS VARCHAR(10));

        INSERT INTO @Resumen (id_analista, id_organizacion, total_incidentes, severidad_prom_org)
        VALUES (@id, @org, @total, @prom);

        FETCH NEXT FROM cur_analistas INTO @id, @org;
    END

    CLOSE cur_analistas;
    DEALLOCATE cur_analistas;

    SELECT id_analista, id_organizacion, total_incidentes, severidad_prom_org
    FROM @Resumen
    ORDER BY total_incidentes DESC;
END;
GO
