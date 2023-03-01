using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webService
{
    public class SuccessResult
    {
        public bool success = true;
        public Object data;
    }
    public class FailResult
    {
        public bool success = false;
        public string errMsg;
    }
}