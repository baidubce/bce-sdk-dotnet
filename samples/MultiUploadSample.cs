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
    internal class MultiUploadSample
    {
        private static void Main(string[] args)
        {
            BosClient client = SampleUtil.GetSampleBosClient();
            const string bucketName = "sample-bucket-multi-upload1"; //示例Bucket名称

            // 初始化：创建示例Bucket
            client.CreateBucket(bucketName); //指定Bucket名称
            string objectKey = "sample";

            // 1.开始Multipart Upload
            InitiateMultipartUploadRequest initiateMultipartUploadRequest =
                new InitiateMultipartUploadRequest() { BucketName = bucketName, Key = objectKey };
            InitiateMultipartUploadResponse initiateMultipartUploadResponse =
                client.InitiateMultipartUpload(initiateMultipartUploadRequest);

            // 2.获取Bucket内的Multipart Upload
            ListMultipartUploadsRequest listMultipartUploadsRequest =
                new ListMultipartUploadsRequest() { BucketName = bucketName };       
            ListMultipartUploadsResponse listMultipartUploadsResponse =
                client.ListMultipartUploads(listMultipartUploadsRequest);
            foreach (MultipartUploadSummary multipartUpload in listMultipartUploadsResponse.Uploads)
            {
                Console.WriteLine("Key: " + multipartUpload.Key + " UploadId: " + multipartUpload.UploadId);
            }

            // 3.分块上传，首先设置每块为 5Mb
            long partSize = 1024 * 1024 * 5L;
            FileInfo partFile = new FileInfo("d:\\lzb\\sample");
            // 计算分块数目
            int partCount = (int)(partFile.Length / partSize);
            if (partFile.Length % partSize != 0)
            {
                partCount++;
            }
            // 新建一个List保存每个分块上传后的ETag和PartNumber
            List<PartETag> partETags = new List<PartETag>();
            for (int i = 0; i < partCount; i++)
            {
                // 获取文件流
                Stream stream = partFile.OpenRead();
                // 跳到每个分块的开头
                long skipBytes = partSize * i;
                stream.Seek(skipBytes, SeekOrigin.Begin);
                // 计算每个分块的大小
                long size = Math.Min(partSize, partFile.Length - skipBytes);
                // 创建UploadPartRequest，上传分块
                UploadPartRequest uploadPartRequest = new UploadPartRequest();
                uploadPartRequest.BucketName = bucketName;
                uploadPartRequest.Key = objectKey;
                uploadPartRequest.UploadId = initiateMultipartUploadResponse.UploadId;
                uploadPartRequest.InputStream = stream;
                uploadPartRequest.PartSize = size;
                uploadPartRequest.PartNumber = i + 1;
                UploadPartResponse uploadPartResponse = client.UploadPart(uploadPartRequest);
                // 将返回的PartETag保存到List中。
                partETags.Add(new PartETag()
                {
                    ETag = uploadPartResponse.ETag,
                    PartNumber = uploadPartResponse.PartNumber
                });
                // 关闭文件
                stream.Close();
            }

            // 4. 获取UploadId的所有Upload Part
            ListPartsRequest listPartsRequest = new ListPartsRequest()
            {
                BucketName = bucketName,
                Key = objectKey,
                UploadId = initiateMultipartUploadResponse.UploadId,
            };
            // 获取上传的所有Part信息
            ListPartsResponse listPartsResponse = client.ListParts(listPartsRequest);
            // 遍历所有Part
            foreach (PartSummary part in listPartsResponse.Parts)
            {
                Console.WriteLine("PartNumber: " + part.PartNumber + " ETag: " + part.ETag);
            }

            // 5. 完成分块上传
            CompleteMultipartUploadRequest completeMultipartUploadRequest =
                new CompleteMultipartUploadRequest()
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    UploadId = initiateMultipartUploadResponse.UploadId,
                    PartETags = partETags
                };
            CompleteMultipartUploadResponse completeMultipartUploadResponse =
                client.CompleteMultipartUpload(completeMultipartUploadRequest);
            Console.WriteLine(completeMultipartUploadResponse.ETag);                  
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
