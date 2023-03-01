using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Newtonsoft.Json;

namespace webService
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    [ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        static string ToJson(Object o)
        {
            return JsonConvert.SerializeObject(o);
        }


        static string TokenGenerator(string name, string password)
        {
            return new PasswordHelper().MD5Encoding(name + password);
        }

        [WebMethod]
        public string GetDevices(string token)
        {
            User user = AuthenticationHelper.GetUser(token);
            if (user == null)
            {
                return ToJson(new FailResult { errMsg = "403" });
            }

            switch (user.role)
            {
                case Role.Admin:
                    {
                        DataTable deviceTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.GET_DEVICES());
                        Device[] devices = deviceTable.AsEnumerable().Select(SqlMapperCollection.device).ToArray();
                        return ToJson(new SuccessResult { data = devices });
                    }
                case Role.NormalUser:
                    {
                        DataTable deviceTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.GET_DEVICES_BY_MANAGER(user.id));
                        Device[] devices = deviceTable.AsEnumerable().Select(SqlMapperCollection.device).ToArray();
                        return ToJson(new SuccessResult { data = devices });
                    }
                default:
                    return ToJson(new FailResult { errMsg = "500" });
            }
        }

        [WebMethod]
        public string GetDeviceById(string token, int id)
        {
            User user = AuthenticationHelper.GetUser(token);
            if (user == null)
            {
                return ToJson(new FailResult { errMsg = "403" });
            }

            DataTable deviceTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.GET_DEVICE_BY_ID(id));
            Device[] devices = deviceTable.AsEnumerable().Select(SqlMapperCollection.device).ToArray();
            if (devices.Length == 0)
            {
                return ToJson(new FailResult { errMsg = "设备不存在" });
            }

            if (!devices[0].manager.Equals(user.name) && user.role != Role.Admin)
            {
                return ToJson(new FailResult { errMsg = "403" });
            }
            return ToJson(new SuccessResult { data = devices[0] });
        }

        [WebMethod]
        public string GetDevicesByStatus(string token, string status)
        {
            User user = AuthenticationHelper.GetUser(token);
            if (user == null)
            {
                return ToJson(new FailResult { errMsg = "403" });
            }

            string sqlQueryString;
            if (user.role == Role.Admin)
            {
                if (status.Equals("alerting"))
                {
                    sqlQueryString = SqlQueryCollection.GET_ALL_ALERTING_DEVICES();
                }
                else
                {
                    sqlQueryString = SqlQueryCollection.GET_ALL_NOT_ALERTING_DEVICES();
                }
            }
            else
            {
                if (status.Equals("alerting"))
                {
                    sqlQueryString = SqlQueryCollection.GET_ALERTING_DEVICES(user.id);
                }
                else
                {
                    sqlQueryString = SqlQueryCollection.GET_NOT_ALERTING_DEVICES(user.id);
                }
            }

            DataTable deviceTable = SqlHelper.ExecuteDataTable(sqlQueryString);
            Device[] devices = deviceTable.AsEnumerable().Select(SqlMapperCollection.device).ToArray();
            return ToJson(new SuccessResult { data = devices });
        }

        [WebMethod]
        public string PostDevice(string token)
        {
            User user = AuthenticationHelper.GetUser(token);
            if (user == null || user.role != Role.Admin)
            {
                return ToJson(new FailResult { errMsg = "403" });
            }

            var name = HttpContext.Current.Request.Form["name"];
            var type = HttpContext.Current.Request.Form["type"];
            var brand = HttpContext.Current.Request.Form["brand"];
            var location = HttpContext.Current.Request.Form["location"];
            var channelNumber = Convert.ToInt32(HttpContext.Current.Request.Form["channelNumber"]);
            var controlNumber = Convert.ToInt32(HttpContext.Current.Request.Form["controlNumber"]);
            var ip = HttpContext.Current.Request.Form["ip"];
            var port = Convert.ToInt32(HttpContext.Current.Request.Form["port"]);
            var manager = Convert.ToInt32(HttpContext.Current.Request.Form["manager"]);

            var resultTable = SqlHelper.ExecuteDataTable($"""
                INSERT INTO device
                VALUES
                    ( '{name}', '{type}', '{brand}', '{location}', {channelNumber}, {controlNumber}, '{ip}', {port}, {manager} )
                Select @@IDENTITY AS 'Identity'
                """);

            var newDeviceId = Convert.ToInt32(resultTable.Rows[0]["Identity"].ToString());
            return GetDeviceById(token, newDeviceId);
        }

        [WebMethod]
        public string DeleteDevice(string token, int id)
        {
            User user = AuthenticationHelper.GetUser(token);
            if (user == null || user.role != Role.Admin)
            {
                return ToJson(new FailResult { errMsg = "403" });
            }

            var affectRowCount = SqlHelper.ExecuteNonQuery(SqlQueryCollection.DELETE_DEVICE(id));
            if (affectRowCount > 0)
            {
                return ToJson(new SuccessResult { data = affectRowCount });
            }
            else
            {
                return ToJson(new FailResult { errMsg = "删除失败" });
            }

        }

        [WebMethod]
        public string GetUsers(string token)
        {
            User user = AuthenticationHelper.GetUser(token);
            if (user == null || user.role != Role.Admin)
            {
                return ToJson(new FailResult { errMsg = "403" });
            }

            DataTable userTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.GET_USERS());
            User[] users = userTable.AsEnumerable().Select(SqlMapperCollection.user).ToArray();
            return ToJson(new SuccessResult { data = users });
        }

        [WebMethod]
        public string DeleteUser(string token, int id)
        {
            User user = AuthenticationHelper.GetUser(token);
            if (user == null || user.role != Role.Admin)
            {
                return ToJson(new FailResult { errMsg = "403" });
            }

            var affectRowCount = SqlHelper.ExecuteNonQuery(SqlQueryCollection.DELETE_USER(id));
            if (affectRowCount > 0)
            {
                return ToJson(new SuccessResult { data = affectRowCount });
            }
            else
            {
                return ToJson(new FailResult { errMsg = "删除失败" });
            }
        }

        [WebMethod]
        public string PostUser(string token)
        {
            User user = AuthenticationHelper.GetUser(token);
            if (user == null || user.role != Role.Admin)
            {
                return ToJson(new FailResult { errMsg = "403" });
            }

            var name = HttpContext.Current.Request.Form["name"];
            DataTable userTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.GET_USERS_BY_NAME(name));
            if (userTable.Rows.Count > 0)
            {
                return ToJson(new FailResult { errMsg = "用户名已存在" });
            }

            // TODO 各类参数值的校验
            // TODO password要加密储存
            var password = HttpContext.Current.Request.Form["password"];
            var sex = Convert.ToInt32(HttpContext.Current.Request.Form["sex"]);
            var role = Convert.ToInt32(HttpContext.Current.Request.Form["role"]);
            var avatar = HttpContext.Current.Request.Form["avatar"];
            var newUserToken = TokenGenerator(name, password);

            var resultTable = SqlHelper.ExecuteDataTable(
                SqlQueryCollection.INSERT_USER(name, password, newUserToken, sex, role, avatar)
            );
            var newUserId = Convert.ToInt32(resultTable.Rows[0]["Identity"].ToString());

            return ToJson(new SuccessResult { data = AuthenticationHelper.GetUserById(newUserId) });
        }

        [WebMethod]
        public string PutUser(string token)
        {
            User sessionUser = AuthenticationHelper.GetUser(token);
            if (sessionUser == null)
            {
                return ToJson(new FailResult { errMsg = "403" });
            }

            var id = Convert.ToInt32(HttpContext.Current.Request.Form["id"]);
            var password = HttpContext.Current.Request.Form["password"];
            var sex = Convert.ToInt32(HttpContext.Current.Request.Form["sex"]);
            var avatar = HttpContext.Current.Request.Form["avatar"];

            var oldUser = AuthenticationHelper.GetUserById(id);
            if (oldUser == null)
            {
                return ToJson(new FailResult { errMsg = "要更新的用户不存在" });
            }

            if (sessionUser.role == Role.NormalUser && sessionUser.id != id)
            {
                return ToJson(new FailResult { errMsg = "403,普通用户只能修改自己的信息" });
            }

            var affectRowCount = SqlHelper.ExecuteNonQuery(
                SqlQueryCollection.UPDATE_USER(id, password, TokenGenerator(oldUser.name, password), sex, avatar)
            );
            if (affectRowCount == 0)
            {
                return ToJson(new FailResult { errMsg = "更新失败" });
            }

            return ToJson(new SuccessResult { data = AuthenticationHelper.GetUserById(id) });
        }

        [WebMethod]
        public string GetAlerts(string token)
        {
            User user = AuthenticationHelper.GetUser(token);
            if (user == null)
            {
                return ToJson(new FailResult { errMsg = "403" });
            }

            if (user.role == Role.Admin)
            {
                DataTable alertTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.GET_ALERTS());
                Alert[] alerts = alertTable.AsEnumerable().Select(SqlMapperCollection.alert).ToArray();
                return ToJson(new SuccessResult { data = alerts });
            }
            else
            {
                DataTable alertTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.GET_ALERTS_BY_MANAGER(user.id));
                Alert[] alerts = alertTable.AsEnumerable().Select(SqlMapperCollection.alert).ToArray();
                return ToJson(new SuccessResult { data = alerts });
            }
        }

        [WebMethod]
        public string GetAlertsByDevice(string token, int deviceId)
        {
            User user = AuthenticationHelper.GetUser(token);
            if (user == null)
            {
                return ToJson(new FailResult { errMsg = "403" });
            }

            var deviceTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.GET_DEVICE_BY_ID(deviceId));
            var devices = deviceTable.AsEnumerable().Select(SqlMapperCollection.device).ToArray();
            if (devices.Length == 0)
            {
                return ToJson(new FailResult { errMsg = "设备不存在" });
            }

            if (!user.name.Equals(devices[0].manager) && user.role != Role.Admin)
            {
                return ToJson(new FailResult { errMsg = "403" });
            }

            DataTable alertTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.GET_ALERTS_BY_DEVICE(deviceId));
            Alert[] alerts = alertTable.AsEnumerable().Select(SqlMapperCollection.alert).ToArray();
            return ToJson(new SuccessResult { data = alerts });
        }

        [WebMethod]
        public string PutAlertProcessingResult(string token, int alertId, string processingResult)
        {
            User user = AuthenticationHelper.GetUser(token);
            if (user == null)
            {
                return ToJson(new FailResult { errMsg = "403" });
            }

            DataTable alertTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.GET_ALERT_BY_ID(alertId));
            Alert[] alerts = alertTable.AsEnumerable().Select(SqlMapperCollection.alert).ToArray();
            if (alerts.Length == 0)
            {
                return ToJson(new FailResult { errMsg = "alert不存在" });
            }

            DataTable deviceTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.GET_DEVICE_BY_ID(alerts[0].deviceId));
            Device[] devices = deviceTable.AsEnumerable().Select(SqlMapperCollection.device).ToArray();
            if (!devices[0].manager.Equals(user.name))
            {
                return ToJson(new FailResult { errMsg = "403" });
            }

            int affectRowCount = SqlHelper.ExecuteNonQuery(SqlQueryCollection.PUT_ALERT_PROCESSING_RESULT(alertId, processingResult));
            if (affectRowCount == 0)
            {
                return ToJson(new FailResult { errMsg = "更新失败" });
            }

            DataTable updatedAlertTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.GET_ALERT_BY_ID(alertId));
            Alert[] updatedAlerts = updatedAlertTable.AsEnumerable().Select(SqlMapperCollection.alert).ToArray();
            return ToJson(new SuccessResult { data = updatedAlerts[0] });
        }

        [WebMethod]
        public string PostAlert(string token)
        {
            User user = AuthenticationHelper.GetUser(token);
            if (user == null || user.role != Role.Admin)
            {
                return ToJson(new FailResult { errMsg = "403" });
            }

            var deviceId = Convert.ToInt32(HttpContext.Current.Request.Form["deviceId"]);
            var alertType = Convert.ToInt32(HttpContext.Current.Request.Form["alertType"]);

            DataTable resultTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.POST_ALERT(deviceId, alertType));
            var newAlertId = Convert.ToInt32(resultTable.Rows[0]["Identity"].ToString());

            DataTable alertTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.GET_ALERT_BY_ID(newAlertId));
            Alert[] alerts = alertTable.AsEnumerable().Select(SqlMapperCollection.alert).ToArray();
            return ToJson(new SuccessResult { data = alerts[0] });
        }

        [WebMethod]
        public string Login(string username, string password)
        {
            DataTable userTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.GET_USER_BY_NAME_AND_PWD(username, password));
            User[] users = userTable.AsEnumerable().Select(SqlMapperCollection.user).ToArray();
            if (users.Length == 0)
            {
                return ToJson(new FailResult { errMsg = "登陆失败" });
            }
            return ToJson(new SuccessResult { data = users[0] });
        }
    }
}
