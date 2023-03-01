using System;
using System.Data;

namespace webService
{
    class SqlMapperCollection
    {
        public static Func<DataRow, Device> device = row =>
            new Device
            {
                id = row.Field<int>("id"),
                name = row.Field<string>("name"),
                type = row.Field<string>("type"),
                brand = row.Field<string>("brand"),
                location = row.Field<string>("location"),
                channelNumber = row.Field<int>("channel_number"),
                controlNumber = row.Field<int>("control_number"),
                ip = row.Field<string>("ip"),
                port = row.Field<int>("port"),
                manager = row.Field<string>("manager"),
            };

        public static Func<DataRow, User> user = row =>
            new User
            {
                id = row.Field<int>("id"),
                name = row.Field<string>("name"),
                password = row.Field<string>("password"),
                token = row.Field<string>("token"),
                sex = row.Field<int>("sex"),
                role = row.Field<int>("role") == 0 ? Role.Admin : Role.NormalUser,
                avatar = row.Field<string>("avatar"),
            };

        public static Func<DataRow, Alert> alert = row =>
        new Alert
        {
            id = row.Field<int>("id"),
            deviceId = row.Field<int>("device_id"),
            type = row.Field<int>("type"),
            processingResult = row.Field<string?>("processing_result"),
            processTime = row.Field<DateTime?>("processing_time"),
            createTime = row.Field<DateTime>("create_time"),
        };

    }
}