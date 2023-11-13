using System;
using System.Collections.Generic;
using BaiduBce;
using BaiduBce.Auth;
using BaiduBce.Services.Sms;
using BaiduBce.Services.Sms.Model;

namespace DotnetSample
{
    internal class BaseSample
    {
        private static void Main(string[] args)
        {
            string accessKeyId = "xxx";
            string secretAccessKey = "xxx";

            BceClientConfiguration config = new BceClientConfiguration();
            config.Credentials = new DefaultBceCredentials(accessKeyId, secretAccessKey);
            config.Endpoint = "http://smsv3.bj.baidubce.com";
            // 设置HTTP最大连接数为10
            config.ConnectionLimit = 10;
            // 设置TCP连接超时为5000毫秒
            config.TimeoutInMillis = 5000;
            // 设置读写数据超时的时间为50000毫秒
            config.ReadWriteTimeoutInMillis = 50000;

            SmsClient client = new SmsClient(config);

            SendMessageRequest request = new SendMessageRequest();
            request.Mobile = "18800000000";
            request.SignatureId = "sms-sign-xxx";
            request.Template = "sms-tmpl-xxx";
            request.ContentVar = new Dictionary<string, string>
            {
                {
                    "content", "测试"
                }

            }
            SendMessageResponse response = client.SendMessage(request);
            Console.Write(response.BceRequestId);
        }
    }
}
