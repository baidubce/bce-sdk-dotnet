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
    internal class ListObjectsSample
    {
        private static void Main(string[] args)
        {
            BosClient client = GenerateBosClient();
            const string bucketName = "sample-bucket-listobjects"; //示例Bucket名称

            // 新建一个Bucket
            client.CreateBucket(bucketName); //指定Bucket名称

            // 创建bos.jpg,fun/,fun/test.jpg,fun/movie/001.avi,fun/movie/007.avi五个文件
            client.PutObject(bucketName, "bos.jpg", "sampledata");
            client.PutObject(bucketName, "fun/", "sampledata");
            client.PutObject(bucketName, "fun/test.jpg", "sampledata");
            client.PutObject(bucketName, "fun/movie/001.avi", "sampledata");
            client.PutObject(bucketName, "fun/movie/007.avi", "sampledata");

            // 构造ListObjectsRequest请求
            ListObjectsRequest listObjectsRequest = new ListObjectsRequest() {BucketName = bucketName};

            // 1. 列出所有文件
            ListObjectsResponse listObjectsResponse = client.ListObjects(listObjectsRequest);

            // 输出：    
            // Objects:
            // bos.jpg
            // fun/
            // fun/movie/001.avi
            // fun/movie/007.avi
            // fun/test.jpg
            Console.WriteLine("Objects:");
            foreach (BosObjectSummary objectSummary in listObjectsResponse.Contents)
            {
                Console.WriteLine("ObjectKey: " + objectSummary.Key);
            }

            // 2. 使用NextMarker分次列出所有文件
            listObjectsRequest.MaxKeys = 2;
            listObjectsResponse = client.ListObjects(listObjectsRequest);

            // 输出：    
            // Objects:
            // bos.jpg
            // fun/
            // fun/movie/001.avi
            // fun/movie/007.avi
            // fun/test.jpg
            Console.WriteLine("Objects:");
            while (listObjectsResponse.IsTruncated)
            {
                foreach (BosObjectSummary objectSummary in listObjectsResponse.Contents)
                {
                    Console.WriteLine("ObjectKey: " + objectSummary.Key);
                }
                listObjectsResponse = client.ListNextBatchOfObjects(listObjectsResponse);
            }
            foreach (BosObjectSummary objectSummary in listObjectsResponse.Contents)
            {
                Console.WriteLine("ObjectKey: " + objectSummary.Key);
            }

            // 3. 递归列出fun/下所有目录和文件
            listObjectsRequest.MaxKeys = 1000;
            listObjectsRequest.Prefix = "fun/";
            listObjectsResponse = client.ListObjects(listObjectsRequest);

            // 输出：    
            // Objects:
            // fun/
            // fun/movie/001.avi
            // fun/movie/007.avi
            // fun/test.jpg
            Console.WriteLine("Objects:");
            foreach (BosObjectSummary objectSummary in listObjectsResponse.Contents)
            {
                Console.WriteLine("ObjectKey: " + objectSummary.Key);
            }

            // 4. 列出fun目录下的所有文件和文件夹
            listObjectsRequest.Delimiter = "/";
            listObjectsResponse = client.ListObjects(listObjectsRequest);

            // 输出：
            // Objects:
            // fun/
            // fun/test.jpg
            Console.WriteLine("Objects:");
            foreach (BosObjectSummary objectSummary in listObjectsResponse.Contents)
            {
                Console.WriteLine("ObjectKey: " + objectSummary.Key);
            }

            // 遍历所有CommonPrefix---相当于获取fun目录下的所有子文件夹
            // 输出：    
            // CommonPrefixs:
            // fun/movie
            Console.WriteLine("\nCommonPrefixs:");
            foreach (ObjectPrefix objectPrefix in listObjectsResponse.CommonPrefixes)
            {
                Console.WriteLine(objectPrefix.Prefix);
            }
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
