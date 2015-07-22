using System;
using System.Collections.Generic;
using System.Configuration;

namespace BaiduBce.UnitTest.Services.Bos
{
    public class BceClientUnitTestBase
    {
        protected string endpoint;
        protected string ak;
        protected string sk;

        public BceClientUnitTestBase()
        {
            this.endpoint = ConfigurationManager.AppSettings["endpoint"];
            this.ak = ConfigurationManager.AppSettings["ak"];
            this.sk = ConfigurationManager.AppSettings["sk"];
        }
    }
}
