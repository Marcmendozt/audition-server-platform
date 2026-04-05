# AccountServer reconstruido en .NET

Esta solucion reorganiza el dominio inferido desde el archivo decompilado en una arquitectura limpia:

- `AccountServer.Domain`: entidades y modelos del dominio.
- `AccountServer.Application`: casos de uso, directorio, board items y reglas de sesion.
- `AccountServer.Infrastructure`: repositorios en memoria para servidores, jugadores, board items y community server.
- `AccountServer.Host`: listeners TCP separados por rol, protocolo JSON y protocolo binario con cabecera de paquete.

## Build

```powershell
dotnet build src/AccountServer.slnx
```

## Run

```powershell
dotnet run --project src/AccountServer.Host
```

Al arrancar, el host emite estos logs legacy:

- `Account Server Build Version[...]...`
- `DBAgentClientManager Ready!`
- `[Board Item Mode] On|Off`
- `Account Server Ver 1.25 Launching.`
- `Gateway server connected...` al registrar un gateway

## Topologia de puertos

- `Account`: `127.0.0.1:4500`
- `Gateway`: `127.0.0.1:4501`
- `Game`: `127.0.0.1:4502`
- `Admin`: `127.0.0.1:4503`

Tambien existe una sonda de conectividad para DBAgent configurable en AccountServer:DbAgent. Para integracion de login, el host ya puede invocar al LoginDBAgent externo por TCP.

## Subsistemas agregados

- Packet manager con buffers reutilizables.
- Session runtime manager con pool de sesiones.
- Soporte binario heredado con longitud ushort y payload secuencial.
- Directorio de servidores con community server.
- Board item repository y mantenimiento periodico.
- Estado administrativo para DBAgent, sesiones y packet manager.
- Builders exactos de paquetes DBAgent recuperados desde el decompilado.

## Prueba rapida del log de gateway

1. Inicia el host:

```powershell
dotnet run --project src/AccountServer.Host
```

2. Desde otra terminal, envia un registro legado de gateway al puerto `4501`:

```powershell
$client = [System.Net.Sockets.TcpClient]::new("127.0.0.1", 4501)
$stream = $client.GetStream()
$packet = New-Object byte[] 116
[System.BitConverter]::GetBytes([UInt16]116).CopyTo($packet, 0)
$packet[2] = 0x65
$packet[3] = 0x00
[System.BitConverter]::GetBytes([UInt16]100).CopyTo($packet, 4)
[System.BitConverter]::GetBytes([UInt32]1).CopyTo($packet, 8)
[System.Text.Encoding]::ASCII.GetBytes("Gateway-01").CopyTo($packet, 12)
$packet[60] = 1
[System.BitConverter]::GetBytes([UInt32]5000).CopyTo($packet, 68)
[System.Text.Encoding]::ASCII.GetBytes("10.0.0.10").CopyTo($packet, 72)
[System.BitConverter]::GetBytes([UInt16]7000).CopyTo($packet, 104)
[System.BitConverter]::GetBytes([UInt32]1).CopyTo($packet, 108)
$packet[112] = 1
$stream.Write($packet, 0, $packet.Length)
$stream.Flush()
$client.Dispose()
```

3. En la consola del host deberias ver una linea similar a esta:

```text
[2026-04-02 22:30:32]Gateway server connected. ServerId=100, Name=Gateway-01, Ip=10.0.0.10, Port=7000
```

## Protocolo TCP

Cada listener procesa JSON por linea. Para los flujos binarios heredados, el framing real recuperado del decompilado es:

- ushort Length
- byte Opcode
- payload secuencial little-endian a partir del byte 2 del paquete

No hay JSON embebido en el paquete legado. El payload usa primitivas byte, word, dword, bloques fijos y strings ASCII con prefijo de longitud de 1 byte.

### Paquetes verificados del cliente

- `0x65 0x00` en Gateway y Game: registro de servidor con `ServerInfo` de `0x70` bytes.
- `ServerInfo` recuperado del decompilado: `hServer`, `hCluster`, `strSvrName[48]`, `svrLevel`, `curUserCount`, `maxUserCount`, `strSvrIP[32]`, `nSvrPort`, `svrVersion`, `svrStatus`.

### Paquetes verificados de DBAgent

- `0x60 0x00`: `RequestLoginChina` con `relayContext[12]` y cuatro strings ASCII con longitud prefijada.
- `0x5F 0x00`: `SaveUserLoginOut` con `userSerial`, `word reservado`, `flag` y `userId`.
- `0x5B 0x20`: `UsedBoardItem` con dos `dword`.

Las utilidades de construccion de estos paquetes estan en `AccountServer.Host/Services/DbAgentProtocol.cs`.

La topologia recomendada es:

- `4500` para account y health.
- `4501` para gateway registration.
- `4502` para game server registration.
- `4503` para operaciones administrativas.

### Health check

```json
{"command":"health","payload":{}}
```

### Registrar gateway

```json
{"command":"register_gateway","payload":{"serverId":100,"clusterId":1,"name":"Gateway-01","level":1,"ipAddress":"10.0.0.10","port":7000,"maxUserCount":5000,"version":1,"status":"Online"}}
```

El camino binario legado para este mismo registro ya no usa JSON embebido: usa `0x65 0x00` y el bloque `ServerInfo` descrito arriba.

### Registrar game server

```json
{"command":"register_game_server","payload":{"serverId":200,"name":"Channel-01","ipAddress":"10.0.0.20","grade":1,"maxChannelCount":20,"maxUserCountPerChannel":250,"maxUserCount":5000,"status":"Online"}}
```

### Registrar community server

```json
{"command":"register_community_server","payload":{"serverId":300,"clusterId":1,"name":"Community-01","level":1,"ipAddress":"10.0.0.30","port":7200,"maxUserCount":5000,"version":1,"status":"Online"}}
```

### Abrir sesion

```json
{"command":"open_session","payload":{"userSerial":1,"userExperience":0,"userId":"player01","clusterId":1,"serverId":200,"gatewayServerId":100,"ipAddress":"192.168.1.5","socketHandle":77}}
```

### Cerrar sesion

```json
{"command":"close_session","payload":{"sessionId":"00000000-0000-0000-0000-000000000000"}}
```

### Obtener snapshot

```json
{"command":"list_servers","payload":{}}
```

### Obtener directorio

```json
{"command":"list_directory","payload":{}}
```

### Board items

```json
{"command":"upsert_board_item","payload":{"boardSerial":1,"userSerial":10,"userNickname":"player01","destinationServer":200,"title":"Trade","payload":"Selling avatar set"}}
```

```json
{"command":"list_board_items","payload":{}}
```

### Estado administrativo

```json
{"command":"session_pool_status","payload":{}}
```

```json
{"command":"packet_manager_status","payload":{}}
```

```json
{"command":"dbagent_status","payload":{}}
```

### Login contra LoginDBAgent

La configuracion actual del host apunta al LoginDBAgent externo en `127.0.0.1:25527`. El monitoreo usa una sonda breve de TCP para no bloquear ese servicio, que procesa una conexion a la vez.

```json
{"command":"request_login_china","payload":{"userId":"test","password":"test","extraField3":"","extraField4":"","socketHandle":77}}
```

Si el LoginDBAgent responde correctamente, AccountServer devuelve un JSON con el paquete legado interpretado, por ejemplo `packetLength`, `opCode`, `region`, `internalResult`, `userSerial`, `loginFlag` y `userId`.

### Integracion con DBAgent.Pay

El host tambien puede enviar operaciones al servicio `Audition.DBAgent.Pay` en `127.0.0.1:25525`. Este servicio no cubre login; se usa para operaciones de juego, economia y trazas.

Estado actual soportado desde AccountServer:

```json
{"command":"dbagent_pay_status","payload":{}}
```

```json
{"command":"dbagent_pay_probe","payload":{}}
```

```json
{"command":"dbagent_pay_heartbeat","payload":{}}
```

```json
{"command":"dbagent_pay_account_info","payload":{"userSerial":1}}
```

```json
{"command":"dbagent_pay_purchase","payload":{"userSerial":1,"itemId":1001,"days":30}}
```

```json
{"command":"dbagent_pay_game_results","payload":{"userSerial":1,"experienceGain":100,"denGain":250}}
```

```json
{"command":"dbagent_pay_level_quest_log","payload":{"userSerial":1,"procLevel":10,"beforeDen":1000,"afterDen":1200,"beforeExp":5000,"afterExp":5300,"pass":1}}
```

`Audition.DBAgent.Pay` se hizo tolerante a esquemas donde falten columnas como `Den`, para convivir mejor con la misma base usada por `LoginDBAgent`.