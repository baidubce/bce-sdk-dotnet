using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaiduBce;
using BaiduBce.Auth;
using BaiduBce.Services.Bos;

namespace DotnetSample
{
    internal class BosClientSample
    {
        private static void Main(string[] args)
        {
            const string accessKeyId = "your-access-key-id"; // 用户的Access Key ID
            const string secretAccessKey = "your-secret-access-key"; // 用户的Secret Access Key
            const string endpoint = "domain-name";

            // 初始化一个BosClient
            BceClientConfiguration config = new BceClientConfiguration();
            config.Credentials = new DefaultBceCredentials(accessKeyId, secretAccessKey);
            config.Endpoint = endpoint;

            // 设置HTTP最大连接数为10
            config.ConnectionLimit = 10;

            // 设置TCP连接超时为5000毫秒
            config.TimeoutInMillis = 5000;

            // 设置读写数据超时的时间为50000毫秒
            config.ReadWriteTimeoutInMillis = 50000;

            BosClient client = new BosClient(config);
            
        }
    }
}
