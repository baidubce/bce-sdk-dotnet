# 安装SDK工具包

**运行环境**

C# SDK工具包可在Microsoft .NET Framework v3.5环境下运行。

**安装步骤**

1. 在[官方网站](http://bce.baidu.com/doc/SDKTool/index.html)下载C# SDK压缩工具包。

2. 将下载的`bce-dotnet-sdk-version.zip`解压后，复制到工程文件夹中。

3. 在Visual Studio项目中“添加引用 -> 浏览”。

4. 添加SDK工具包`BceSdkDotNet.dll`和第三方依赖工具包`log4net.dll`和`Newtonsoft.Json.dll`。

**SDK目录结构**

    BaiduBce
           ├── Auth                                        //BCE签名相关类
           ├── Http                                        //BCE的Http通信相关类
           ├── Internal                                    //SDK内部类
           ├── Model                                       //BCE公用model类
           ├── Services
           │       └── Bos                                 //BOS服务相关类
           │           ├── Model                           //BOS内部model，如Request或Response
           │           ├── BosClient.cs                 //BOS客户端入口类
           │           └── BosConstants.cs              //BOS特有的常量定义，如权限常量等
           ├── Util                                        //BCE公用工具类
           ├── BceClientConfiguration.cs                  //对BCE的HttpClient的配置
           ├── BceClientException.cs                      //BCE客户端的异常类
           ├── BceServiceException.cs                     //与BCE服务端交互后的异常类
           ├── BceConstants.cs                            //BCE的通用常量(区域，请求头，错误码等)
           
           
# 快速入门

请参考[快速入门](http://bce.baidu.com/doc/BOS/Cs-SDK.html#快速入门)。

# SDK使用帮助

请参考[BOS C# SDK文档](http://bce.baidu.com/doc/BOS/Cs-SDK.html)。
  