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
    internal class PutObjectSample
    {
        private static void Main(string[] args)
        {
            BosClient client = GenerateBosClient();
            const string bucketName = "sample-bucket-putobject"; //示例Bucket名称
            const string objectNameFile = "sample-bucket-putobject-file"; //file上传的object名称
            const string objectNameStream = "sample-bucket-putobject-stream"; //stream上传的object名称
            const string objectNameString = "sample-bucket-putobject-string"; //string上传的object名称
            const string objectNameByte = "sample-bucket-putobject-byte"; //byte上传的object名称

            // 新建一个Bucket
            client.CreateBucket(bucketName); //指定Bucket名称

            // 设置待上传的文件名，例如d:\\sample.txt
            const string fileName = "my file path and name";

            // 以文件形式上传Object
            PutObjectResponse putObjectFromFileResponse = client.PutObject(bucketName, objectNameFile,
                new FileInfo(fileName));
            // 以数据流形式上传Object
            PutObjectResponse putObjectResponseFromInputStream = client.PutObject(bucketName, objectNameStream,
                new FileInfo(fileName).OpenRead());
            // 以二进制串上传Object
            PutObjectResponse putObjectResponseFromByte = client.PutObject(bucketName, objectNameByte,
                Encoding.Default.GetBytes("sampledata"));
            // 以字符串上传Object
            PutObjectResponse putObjectResponseFromString = client.PutObject(bucketName, objectNameString,
                "sampledata");

            // 打印四种方式的ETag---演示例子中，文件方式和stream方式的ETag相等，string方式和byte方式的ETag相等
            Console.WriteLine(putObjectFromFileResponse.ETAG);
            Console.WriteLine(putObjectResponseFromInputStream.ETAG);
            Console.WriteLine(putObjectResponseFromByte.ETAG);
            Console.WriteLine(putObjectResponseFromString.ETAG);
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
