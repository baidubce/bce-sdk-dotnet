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
    internal class BaseSample
    {
        private static void Main(string[] args)
        {
            BosClient client = GenerateBosClient();
            const string bucketName = "sample-base"; //示例Bucket名称
            const string objectKey = "sample-object"; //示例object名称

            //创建Bucket
            client.CreateBucket(bucketName);

            //上传Object
            FileInfo file = new FileInfo("d:\\lzb\\sample.txt"); //上传文件的目录
            PutObjectResponse putObjectFromFileResponse = client.PutObject(bucketName, objectKey, file);
            Console.WriteLine(putObjectFromFileResponse.ETAG);

            //查看Object
            ListObjectsResponse listObjectsResponse = client.ListObjects(bucketName);
            foreach (BosObjectSummary objectSummary in listObjectsResponse.Contents)
            {
                Console.WriteLine("ObjectKey: " + objectSummary.Key);
            }

            // 获取Object
            BosObject bosObject = client.GetObject(bucketName, objectKey);
            // 获取ObjectMeta
            ObjectMetadata meta = bosObject.ObjectMetadata;
            // 获取Object的输入流
            Stream objectContent = bosObject.ObjectContent;
            // 处理Object
            FileStream fileStream = new FileInfo("d:\\lzb\\sampleout.txt").OpenWrite(); //下载文件的目录/文件名
            byte[] buffer = new byte[2048];
            int count = 0;
            while ((count = objectContent.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileStream.Write(buffer, 0, count);
            }

            // 关闭流
            objectContent.Close();
            fileStream.Close();
            Console.WriteLine(meta.ETag);
            Console.WriteLine(meta.ContentLength);
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
