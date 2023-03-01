using System;

namespace webService
{
    class SqlQueryCollection
    {
        public static string GET_DEVICES()
        {
            return """
                SELECT
                    device.id AS id,
                    device.brand AS brand,
                    device.name AS name,
                    device.type AS type,
                    device.location AS location,
                    device.channel_number AS channel_number,
                    device.control_number AS control_number,
                    device.ip AS ip,
                    device.port AS port,
                    [user].name AS manager 
                FROM
                    device
                    INNER JOIN [user] ON device.manager_id = [user].id
                """;
        }

        public static string GET_DEVICES_BY_MANAGER(int managerId)
        {
            return $"""
                SELECT
                    device.id AS id,
                    device.brand AS brand,
                    device.name AS name,
                    device.type AS type,
                    device.location AS location,
                    device.channel_number AS channel_number,
                    device.control_number AS control_number,
                    device.ip AS ip,
                    device.port AS port,
                    [user].name AS manager 
                FROM
                    device
                    INNER JOIN [user] ON device.manager_id = [user].id
                WHERE
                    device.manager_id = {managerId}
                """;
        }

        public static string GET_DEVICE_BY_ID(int deviceId)
        {
            return $"""
                SELECT
                    device.id AS id,
                    device.brand AS brand,
                    device.name AS name,
                    device.type AS type,
                    device.location AS location,
                    device.channel_number AS channel_number,
                    device.control_number AS control_number,
                    device.ip AS ip,
                    device.port AS port,
                    [user].name AS manager 
                FROM
                    device
                    INNER JOIN [user] ON device.manager_id = [user].id
                WHERE
                    device.id = {deviceId}
                """;
        }

        public static string GET_ALL_ALERTING_DEVICES()
        {
            return $"""
                SELECT
                    device.id AS id,
                    device.brand AS brand,
                    device.name AS name,
                    device.type AS type,
                    device.location AS location,
                    device.channel_number AS channel_number,
                    device.control_number AS control_number,
                    device.ip AS ip,
                    device.port AS port,
                    [user].name AS manager 
                FROM
                    device
                    INNER JOIN [user] ON device.manager_id = [user].id 
                WHERE
                    ( SELECT COUNT(*) FROM alert WHERE alert.device_id = device.id AND alert.processing_result IS NULL ) > 0
                """;
        }

        public static string GET_ALL_NOT_ALERTING_DEVICES()
        {
            return $"""
                SELECT
                    device.id AS id,
                    device.brand AS brand,
                    device.name AS name,
                    device.type AS type,
                    device.location AS location,
                    device.channel_number AS channel_number,
                    device.control_number AS control_number,
                    device.ip AS ip,
                    device.port AS port,
                    [user].name AS manager 
                FROM
                    device
                    INNER JOIN [user] ON device.manager_id = [user].id 
                WHERE
                    ( SELECT COUNT(*) FROM alert WHERE alert.device_id = device.id AND alert.processing_result IS NULL ) = 0
                """;
        }

        public static string GET_ALERTING_DEVICES(int managerId)
        {
            return $"""
                SELECT
                    device.id AS id,
                    device.brand AS brand,
                    device.name AS name,
                    device.type AS type,
                    device.location AS location,
                    device.channel_number AS channel_number,
                    device.control_number AS control_number,
                    device.ip AS ip,
                    device.port AS port,
                    [user].name AS manager 
                FROM
                    device
                    INNER JOIN [user] ON device.manager_id = [user].id 
                WHERE
                   	device.manager_id = {managerId}
                    AND ( SELECT COUNT(*) FROM alert WHERE alert.device_id = device.id AND alert.processing_result IS NULL ) > 0
                """;
        }

        public static string GET_NOT_ALERTING_DEVICES(int managerId)
        {
            return $"""
                SELECT
                    device.id AS id,
                    device.brand AS brand,
                    device.name AS name,
                    device.type AS type,
                    device.location AS location,
                    device.channel_number AS channel_number,
                    device.control_number AS control_number,
                    device.ip AS ip,
                    device.port AS port,
                    [user].name AS manager 
                FROM
                    device
                    INNER JOIN [user] ON device.manager_id = [user].id 
                WHERE
                   	device.manager_id = {managerId}
                    AND ( SELECT COUNT(*) FROM alert WHERE alert.device_id = device.id AND alert.processing_result IS NULL ) = 0
                """;
        }

        public static string DELETE_DEVICE(int id)
        {
            return $"DELETE FROM device WHERE id = {id}";
        }

        public static string GET_USERS()
        {
            return $"SELECT * FROM [user]";
        }

        public static string GET_USERS_BY_NAME(string name)
        {
            return $"SELECT * FROM [user] WHERE name = '{name}'";
        }

        public static string GET_USERS_BY_ID(int id)
        {
            return $"SELECT * FROM [user] WHERE id = {id}";
        }

        public static string DELETE_USER(int id)
        {
            return $"DELETE FROM user WHERE id = {id}";
        }

        public static string INSERT_USER(string name, string password, string token, int sex, int role, string avatar)
        {
            return $"""
                INSERT INTO [user]
                VALUES
                    ('{name}', '{password}', '{token}', {sex}, {role}, '{avatar}' )
                Select @@IDENTITY AS 'Identity'
                """;
        }

        public static string UPDATE_USER(int id, string password, string token, int sex, string avatar)
        {
            return $"""
                UPDATE [user] 
                SET 
                    password = '{password}',
                    token = '{token}',
                    sex = {sex},
                    avatar = '{avatar}'
                WHERE
                    id = {id}
                """;
        }

        public static string GET_ALERTS()
        {
            return $"""
                SELECT
                    * 
                FROM
                    alert 
                ORDER BY
                    create_time DESC
                """;
        }

        public static string GET_ALERTS_BY_MANAGER(int managerId)
        {
            return $"""
                SELECT
                    alert.* 
                FROM
                    alert
                    INNER JOIN device ON alert.device_id = device.id
                    INNER JOIN [user] ON device.manager_id = [user].id 
                WHERE
                    [user].id = {managerId} 
                ORDER BY
                    create_time DESC
                """;
        }

        public static string GET_ALERTS_BY_DEVICE(int deviceId)
        {
            return $"""
                SELECT
                    alert.*
                FROM
                    alert
                    INNER JOIN device ON alert.device_id = device.id
                    INNER JOIN [user] ON device.manager_id = [user].id 
                WHERE
                    device.id = {deviceId}
                ORDER BY
                    create_time DESC 
                """;
        }

        public static string GET_ALERT_BY_ID(int alertId)
        {
            return $"""
                SELECT
                    * 
                FROM
                    alert 
                WHERE
                    id = {alertId}
                """;
        }

        public static string PUT_ALERT_PROCESSING_RESULT(int alertId, string processingResult)
        {
            return $"""
                UPDATE alert 
                SET processing_result = '{processingResult}', processing_time = GETDATE() 
                WHERE
                    id = {alertId}
                """;
        }

        public static string POST_ALERT(int deviceId, int alertType)
        {
            return $"""
                INSERT INTO alert
                VALUES
                    ( {deviceId}, {alertType}, NULL, NULL, GETDATE() )
                Select @@IDENTITY AS 'Identity'
                """;
        }

        public static string GET_USER_BY_NAME_AND_PWD(string name, string password)
        {
            return $"""
                SELECT
                    * 
                FROM
                    [user] 
                WHERE
                    name = '{name}' 
                    AND password = '{password}'
                """;
        }

    }
}