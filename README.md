# Audition Server Platform

Plataforma monorepo para reconstruccion, operacion y pruebas de servicios backend de Audition, incluyendo servidores principales, agentes de base de datos, herramientas de soporte y scripts de diagnostico.

## Descripcion corta para GitHub

Monorepo con servicios backend, agentes, herramientas y base de datos para una plataforma Audition reconstruida en .NET.

## Alcance del repositorio

Este repositorio agrupa componentes usados para levantar y evolucionar una plataforma privada de Audition con enfoque en compatibilidad progresiva con el protocolo legacy y reorganizacion de servicios hacia soluciones mantenibles en .NET.

Incluye:

- GameDBAgent
- LoginDBAgent
- GatewayServer
- AccountServer
- GameServer
- herramientas operativas y de diagnostico
- scripts auxiliares
- esquemas SQL base

No incluye el cliente del juego en el control de versiones.

## Estructura

```text
01-GameDBAgent/
02-LoginDBAgent/
03-GatewayServer/
04-AccountServer/
05-GameServer/
07-Tools/
08-Database/
```

## Componentes principales

### 01-GameDBAgent

Servicio orientado a operaciones de datos del juego y flujos heredados relacionados con gameplay.

### 02-LoginDBAgent

Servicio de autenticacion y operaciones de login heredadas.

### 03-GatewayServer

Punto de entrada de red para directorio, handoff y coordinacion de acceso hacia otros servicios.

### 04-AccountServer

Servidor reconstruido en .NET para sesiones, directorio de servidores, board items, integracion con LoginDBAgent y operaciones administrativas.

### 05-GameServer

Servidor de juego en evolucion, con trabajo activo sobre compatibilidad de sala, arranque de partida, flujo legacy y soporte de datos runtime.

### 07-Tools

Herramientas de soporte para operacion, pruebas y administracion. Incluye el gestor de partners sinteticos y utilidades de trazabilidad.

### 08-Database

Scripts SQL base para las bases de datos del ecosistema.

## Stack tecnico

- .NET 10 en los servicios reconstruidos principales
- Worker Services para hosts backend
- WinForms para herramientas administrativas
- PowerShell para automatizacion y diagnostico
- SQL scripts para bootstrap de datos

## Inicio rapido

### Requisitos

- .NET SDK 10
- Windows
- SQL Server u otro entorno compatible con los scripts incluidos

### Build de ejemplos

AccountServer:

```powershell
dotnet build 04-AccountServer/src/AccountServer.slnx
```

GameServer:

```powershell
dotnet build 05-GameServer/src/Audition.GameServer.slnx
```

### Ejecucion de ejemplos

AccountServer:

```powershell
dotnet run --project 04-AccountServer/src/AccountServer.Host
```

GameServer:

```powershell
dotnet run --project 05-GameServer/src/Audition.GameServer.Host
```

## Estado actual

- AccountServer y GameServer ya tienen soluciones .NET operativas dentro de `src/`
- parte del protocolo legacy ya fue recuperada y documentada
- el GameServer sigue en trabajo activo de compatibilidad con cliente legacy
- el repositorio prioriza codigo fuente, configuracion y scripts por encima de assets runtime pesados

## Notas de versionado

El repositorio ignora por defecto:

- binarios de compilacion
- logs y reportes
- artefactos temporales
- contenido decompilado
- cliente del juego
- binarios nativos auxiliares
- assets runtime pesados como sonido y scripts legacy del GameServer

Los assets runtime excluidos deben provisionarse por separado en entornos locales o privados cuando sean necesarios para pruebas completas.

## Documentacion por servicio

- Ver [04-AccountServer/README.md](04-AccountServer/README.md)
- Ver [05-GameServer/README.md](05-GameServer/README.md)

## Objetivo del proyecto

Centralizar la reconstruccion y operacion de la plataforma Audition en un monorepo mantenible, con trazabilidad sobre protocolo heredado, herramientas de soporte y una base mas clara para evolucion futura.