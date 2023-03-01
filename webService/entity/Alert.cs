using System;

namespace webService
{
    class Alert
    {
        public int id;
        public int deviceId;
        public int type;
        public string processingResult;
        public DateTime? processTime;
        public DateTime? createTime;
    }
}