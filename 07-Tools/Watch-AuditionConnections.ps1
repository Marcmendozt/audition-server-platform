param(
    [int[]]$Ports = @(25510, 25511, 25527, 4500, 4501, 4502, 28888),
    [int]$IntervalMs = 250,
    [switch]$Once
)

$ErrorActionPreference = 'Stop'

function Format-ConnectionKey {
    param([object]$Connection)

    return (
        '{0}|{1}|{2}|{3}|{4}|{5}' -f
            $Connection.State,
            $Connection.LocalAddress,
            $Connection.LocalPort,
            $Connection.RemoteAddress,
            $Connection.RemotePort,
            $Connection.OwningProcess
    )
}

function Get-RelevantConnections {
    param([int[]]$TargetPorts)

    $connections = Get-NetTCPConnection -ErrorAction SilentlyContinue |
        Where-Object {
            $_.LocalPort -in $TargetPorts -or $_.RemotePort -in $TargetPorts
        } |
        Sort-Object LocalPort, RemotePort, State, OwningProcess

    return @($connections)
}

function Write-ConnectionSet {
    param(
        [string]$Label,
        [object[]]$Connections
    )

    $timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss.fff'
    Write-Host "[$timestamp] $Label"

    if (-not $Connections -or $Connections.Count -eq 0) {
        Write-Host '  (sin conexiones relevantes)'
        return
    }

    foreach ($connection in $Connections) {
        Write-Host (
            '  {0,-12} {1}:{2} -> {3}:{4} PID={5}' -f
                $connection.State,
                $connection.LocalAddress,
                $connection.LocalPort,
                $connection.RemoteAddress,
                $connection.RemotePort,
                $connection.OwningProcess
        )
    }
}

$portsLabel = ($Ports | Sort-Object | ForEach-Object { $_.ToString() }) -join ', '
Write-Host "Watching Audition TCP activity on ports: $portsLabel"

$previous = @{}

while ($true) {
    $currentConnections = Get-RelevantConnections -TargetPorts $Ports
    $current = @{}

    foreach ($connection in $currentConnections) {
        $key = Format-ConnectionKey -Connection $connection
        $current[$key] = $connection
    }

    $opened = @($current.Keys | Where-Object { -not $previous.ContainsKey($_) } | ForEach-Object { $current[$_] })
    $closed = @($previous.Keys | Where-Object { -not $current.ContainsKey($_) } | ForEach-Object { $previous[$_] })

    if ($opened.Count -gt 0) {
        Write-ConnectionSet -Label 'OPENED' -Connections $opened
    }

    if ($closed.Count -gt 0) {
        Write-ConnectionSet -Label 'CLOSED' -Connections $closed
    }

    if ($Once) {
        if ($opened.Count -eq 0 -and $closed.Count -eq 0) {
            Write-ConnectionSet -Label 'SNAPSHOT' -Connections $currentConnections
        }

        break
    }

    $previous = $current
    Start-Sleep -Milliseconds $IntervalMs
}