using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using BaiduBce;
using BaiduBce.Auth;
using BaiduBce.Services.Bos;
using BaiduBce.Services.Bos.Model;

namespace DotnetSample
{
    internal class DeleteObjectSample
    {
        private static void Main(string[] args)
        {
            BosClient client = GenerateBosClient();
            const string bucketName = "sample-bucket-deleteobject"; //示例Bucket名称

            // 初始化：创建示例Bucket和Object
            client.CreateBucket(bucketName); //指定Bucket名称
            string objectName = "sample";
            client.PutObject(bucketName, objectName, "sampledata");

            // 删除Object
            client.DeleteObject(bucketName, objectName);
        }

        private static BosClient GenerateBosClient()
        {
            const string accessKeyId = "your-access-key-id"; // 用户的Access Key ID
            const string secretAccessKey = "your-secret-access-key"; // 用户的Secret Access Key
            const string endpoint = "domain-name"; // 指定BOS服务域名

            // 初始化一个BosClient
            BceClientConfiguration config = new BceClientConfiguration();
            config.Credentials = new DefaultBceCredentials(accessKeyId, secretAccessKey);
            config.Endpoint = endpoint;

            return new BosClient(config);
        }
    }
}
