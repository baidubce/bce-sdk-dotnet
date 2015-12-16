using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaiduBce;
using BaiduBce.Auth;
using BaiduBce.Services.Bos;
using BaiduBce.Services.Bos.Model;

namespace DotnetSample
{
    internal class BucketSample
    {
        private static void Main(string[] args)
        {
            BosClient client = GenerateBosClient();
            const string bucketName = "sample-bucket"; //示例Bucket名称

            // 新建一个Bucket
            client.CreateBucket(bucketName); //指定Bucket名称

            // 获取用户的Bucket列表
            List<BucketSummary> buckets = client.ListBuckets().Buckets;
            // 遍历Bucket
            foreach (BucketSummary bucket in buckets)
            {
                Console.WriteLine(bucket.Name);
            }

            // 获取Bucket的存在信息---应该输出Bucket exists
            bool exists = client.DoesBucketExist(bucketName); //指定Bucket名称
            // 输出结果
            if (exists)
            {
                Console.WriteLine("Bucket exists");
            }
            else
            {
                Console.WriteLine("Bucket not exists");
            }

            // 删除Bucket
            client.DeleteBucket(bucketName); //指定Bucket名称

            // 再次获取Bucket的存在信息---应该输出Bucket not exists
            exists = client.DoesBucketExist(bucketName); //指定Bucket名称
            // 输出结果
            if (exists)
            {
                Console.WriteLine("Bucket exists");
            }
            else
            {
                Console.WriteLine("Bucket not exists");
            }
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
