# SGISI - Sistema de Gestión de Incidentes de Seguridad Informática

Proyecto del curso **Base de Datos I (CBS04M)** - Universidad Nacional de Ingeniería,
Facultad de Ingeniería Eléctrica y Electrónica, Escuela de Ingeniería en Ciberseguridad.

Una empresa peruana de consultoría en ciberseguridad centraliza en SGISI la gestión de
organizaciones, analistas, activos, vulnerabilidades, alertas, incidentes, reportes y
una bitácora de auditoría, reemplazando el registro previo en hojas de cálculo
compartidas.

## Estructura del repositorio

```
database/
  01_schema.sql       -> Creacion de las 11 tablas (PK, FK, CHECK, DEFAULT, UNIQUE)
  02_programas.sql    -> Funciones, procedimientos, triggers y el cursor
  03_hardening.sql     -> Menor privilegio (logins), DDM y auditoria de accesos

SGISI_App/             -> Aplicacion de escritorio en C# (Windows Forms)
SGISI_Web/              -> Complemento web en C# (ASP.NET Core Razor Pages)

docs/
  latex/informe_sgisi.tex -> Informe tecnico completo (LaTeX)
  Sistema_de_Gestion_Incidentes.docx -> Informe del proyecto (Word)
  Guion_Exposicion_SGISI.docx        -> Guion para la exposicion oral
  SGISI_Presentacion.pptx            -> Diapositivas de la exposicion
```

## Aplicaciones cliente

Ambas aplicaciones comparten el mismo mecanismo de seguridad: cada usuario inicia
sesión con un **login real de SQL Server**; la propia base de datos determina si es
administrador (`db_owner` o permiso `UNMASK`) o cliente de solo consulta, y el
enmascaramiento dinámico de datos (DDM) se aplica automáticamente según ese rol.

- **SGISI_App**: Windows Forms, cubre las 10 entidades del modelo, permite
  registrar/cerrar incidentes (solo administrador).
- **SGISI_Web**: ASP.NET Core Razor Pages, alcance de solo lectura, sesión por
  navegador (no una variable compartida del servidor).

## Hardening aplicado

1. **Menor privilegio**: 4 logins nominales (1 administrador, 3 clientes de solo
   consulta) con permisos diferenciados.
2. **Enmascaramiento Dinámico de Datos (DDM)**: protege datos de contacto del
   personal, huella técnica de infraestructura (IP/SO) y detalle de vulnerabilidades
   sin parchear.
3. **Auditoría de accesos**: `SERVER AUDIT` que registra cada inicio de sesión,
   correcto o fallido.

## Nota sobre la carátula del informe LaTeX

El PDF del informe LaTeX usa una imagen de marcador de posición para el logo de la
UNI (`docs/latex/images/logo_uni.png`); debe reemplazarse por el logo oficial antes
de la entrega final.
