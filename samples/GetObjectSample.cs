using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BaiduBce;
using BaiduBce.Auth;
using BaiduBce.Services.Bos;
using BaiduBce.Services.Bos.Model;

namespace DotnetSample
{
    internal class GetObjectSample
    {
        private static void Main(string[] args)
        {
            BosClient client = GenerateBosClient();
            const string bucketName = "sample-bucket-getobject"; //示例Bucket名称

            // 初始化：创建示例Bucket和Object
            client.CreateBucket(bucketName); //指定Bucket名称
            string objectName = "sample";
            client.PutObject(bucketName, objectName, "sampledata");

            // 1. 获取BosObject对象并通过BosObject的输入流获取内容
            BosObject bosObject = client.GetObject(bucketName, objectName);
            Stream objectContent = bosObject.ObjectContent;
            string content = new StreamReader(objectContent).ReadToEnd();
            Console.WriteLine(content); // "sampledata"

            // 2. 通过GetObjectRequest只获取部分数据
            GetObjectRequest getObjectRequest = new GetObjectRequest() {BucketName = bucketName, Key = objectName};
            getObjectRequest.SetRange(0, 5);
            bosObject = client.GetObject(getObjectRequest);
            objectContent = bosObject.ObjectContent;
            content = new StreamReader(objectContent).ReadToEnd();
            Console.WriteLine(content); // "sample"

            // 3. 直接通过GetObjectContent获取byte[]内容
            byte[] bytes = client.GetObjectContent(bucketName, objectName);
            content = Encoding.Default.GetString(bytes);
            Console.WriteLine(content); // "sampledata"

            // 4. 将object内容下载到文件
            FileInfo fileInfo = new FileInfo("d:\\lzb\\sample.txt");
            client.GetObject(bucketName, objectName,fileInfo );
            content = File.ReadAllText(fileInfo.FullName);
            Console.WriteLine(content); // "sampledata"

            // 5. 只获取object的meta，不获取内容
            ObjectMetadata objectMetadata = client.GetObjectMetadata(bucketName, objectName);
            Console.WriteLine(objectMetadata.ContentLength); // "10"

            Console.ReadKey();
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
