# GameServer limpio en .NET

Este directorio conserva el runtime nativo y agrega una solucion limpia inicial en `src/` para reemplazo progresivo.

## Alcance de esta primera fase

- skeleton de bootstrap con arquitectura limpia
- listener TCP minimo para reservar el puerto del GameServer
- registro legacy `0x65 0x00` contra AccountServer
- sincronizacion inicial con DBAgent para `0x5D`, `0x4B.0` y `0x4B.1`
- snapshot en memoria con server info, channels y level quests cargados desde DBAgent
- carga legacy de `NoticeInfo.ini`, `HackList.ini`, `MissionInfo.ini` y `BattleInfo.ini` desde `Data`
- sonda de conectividad opcional a Certify
- carga opcional de `Data/ServerInfo.ini` para heredar `ServerID`, `AccountServerIP` y `DBAgentServerIP`
- log por ejecucion en `log/`
- reportes de excepcion no controlada en `Report/`

No implementa todavia gameplay, rooms, sincronizacion de usuarios, handshakes de cliente ni cifrado del protocolo nativo.

## Estructura

- `Audition.GameServer.Domain`: modelos base del bootstrap, server info, channels y level quests
- `Audition.GameServer.Application`: orquestacion del arranque y contratos de clientes bootstrap
- `Audition.GameServer.Infrastructure`: cliente legacy para AccountServer, cliente bootstrap para DBAgent, parser ini, estado en memoria y loaders de `Data`
- `Audition.GameServer.Host`: worker service y listener minimo
- `Audition.GameServer.Tests`: pruebas del orquestador de bootstrap

## Build

```powershell
dotnet build src/Audition.GameServer.slnx
```

## Run

```powershell
dotnet run --project src/Audition.GameServer.Host
```

## Siguiente fase sugerida

1. mantener conexion persistente con AccountServer para handoff posterior
2. aceptar framing legado del cliente y responder handshake minimo
3. construir directorio de canales y sesiones sobre el snapshot bootstrap
4. incorporar flujo de login relay y entrada a canal