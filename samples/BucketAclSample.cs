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
    internal class BucketAclSample
    {
        private static void Main(string[] args)
        {
            BosClient client = GenerateBosClient();
            const string bucketName = "sample-bucket"; //示例Bucket名称

            // 新建一个Bucket
            client.CreateBucket(bucketName); //指定Bucket名称


            // 设置Bucket为Private
            client.SetBucketAcl(bucketName, BosConstants.CannedAcl.Private);

            // 设置Bucket为PublicRead
            client.SetBucketAcl(bucketName, BosConstants.CannedAcl.PublicRead);

            List<Grant> grants = new List<Grant>();
            List<Grantee> grantee = new List<Grantee>();
            List<string> permission = new List<string>();

            //授权给特定用户
            grantee.Add(new Grantee() { Id = "userid1" });
            grantee.Add(new Grantee() { Id = "userid2" });
            //授权给Everyone
            grantee.Add(new Grantee() { Id = "*" });

            //设置权限
            permission.Add(BosConstants.Permission.Read);
            permission.Add(BosConstants.Permission.Write);

            grants.Add(new Grant() { Grantee = grantee, Permission = permission });
            client.SetBucketAcl(new SetBucketAclRequest() { BucketName = bucketName, AccessControlList = grants });
            
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
