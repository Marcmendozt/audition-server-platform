namespace AccountServer.Host.Services;

public static class EndpointCommandPolicy
{
    private static readonly Dictionary<string, HashSet<string>> Policies = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Account"] = new(StringComparer.OrdinalIgnoreCase)
        {
            "health",
            "open_session",
            "close_session",
            "list_servers",
            "list_players",
            "request_login_china"
        },
        ["Gateway"] = new(StringComparer.OrdinalIgnoreCase)
        {
            "health",
            "register_gateway",
            "open_session",
            "close_session",
            "list_servers",
            "list_directory",
            "request_login_china"
        },
        ["Game"] = new(StringComparer.OrdinalIgnoreCase)
        {
            "health",
            "register_game_server",
            "list_servers",
            "open_session",
            "close_session",
            "dbagent_pay_status",
            "dbagent_pay_probe",
            "dbagent_pay_heartbeat",
            "dbagent_pay_account_info",
            "dbagent_pay_purchase",
            "dbagent_pay_game_results",
            "dbagent_pay_level_quest_log"
        },
        ["Admin"] = new(StringComparer.OrdinalIgnoreCase)
        {
            "health",
            "register_gateway",
            "register_game_server",
            "register_community_server",
            "open_session",
            "close_session",
            "list_servers",
            "list_players",
            "list_directory",
            "upsert_board_item",
            "list_board_items",
            "session_pool_status",
            "packet_manager_status",
            "dbagent_status",
            "request_login_china",
            "dbagent_pay_status",
            "dbagent_pay_probe",
            "dbagent_pay_heartbeat",
            "dbagent_pay_account_info",
            "dbagent_pay_purchase",
            "dbagent_pay_game_results",
            "dbagent_pay_level_quest_log"
        }
    };

    public static bool IsAllowed(string endpointName, string command)
    {
        if (!Policies.TryGetValue(endpointName, out var allowedCommands))
        {
            return false;
        }

        return allowedCommands.Contains(command);
    }
}