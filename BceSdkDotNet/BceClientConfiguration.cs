﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using BaiduBce.Auth;
using System.Net;
using System.Reflection;

namespace BaiduBce
{
    /// <summary>
    /// Basic client configurations for BCE clients.
    /// </summary>
    public class BceClientConfiguration
    {
        /// <summary>
        /// The default timeout for creating new connections.
        /// </summary>
        public const int DefaultTimeoutInMillis = 50 * 1000;

        /// <summary>
        /// The default timeout for reading from a connected socket.
        /// </summary>
        public const int DefaultReadWriteTimeoutInMillis = 50 * 1000;

        /// <summary>
        /// The default socket buffer size.
        /// </summary>
        public const int DefaultSocketBufferSizeInBytes = 8192;

        /// <summary>
        /// The default Signer.
        /// </summary>
        public static readonly ISigner DefaultSigner = new BceV1Signer();

        /// <summary>
        /// The default UserAgent.
        /// </summary>
        private static readonly string DefaultUserAgent = BceClientConfiguration.GenerateDefaultUserAgent();

        /// <summary>
        /// The BCE credentials used by the client to sign HTTP requests.
        /// </summary>
        public IBceCredentials Credentials { get; set; }

        /// <summary>
        /// The region of service. This value is used by the client to construct the endpoint URL automatically, and is
        /// ignored if endpoint is not null.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// The service endpoint URL to which the client will connect.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// The protocol (HTTP/HTTPS) to use when connecting to BCE services.
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// The connection timeout in milliseconds. A value of -1 means infinity, and is not recommended.
        /// </summary>
        public int? TimeoutInMillis { get; set; }

        /// <summary>
        /// The socket timeout (SO_TIMEOUT) in milliseconds, which is a maximum period inactivity between two consecutive
        /// data packets. A value of -1 means infinity, and is not recommended.
        /// </summary>
        public int? ReadWriteTimeoutInMillis { get; set; }

        public string ProxyHost { get; set; }

        public int? ProxyPort { get; set; }

        public ICredentials ProxyCredentials { get; set; }

        public bool? UseNagleAlgorithm { get; set; }

        public int? MaxIdleTimeInMillis { get; set; }

        /// <summary>
        /// The maximum number of open HTTP connections.
        /// </summary>
        public int? ConnectionLimit { get; set; }

        /// <summary>
        /// The optional size (in bytes) for the low level TCP socket buffer. This is an advanced option for advanced users
        /// who want to tune low level TCP parameters to try and squeeze out more performance. Ignored if not positive.
        /// </summary>
        public int? SocketBufferSizeInBytes { get; set; }

        public ISigner Signer { get; set; }

        public SignOptions SignOptions { get; set; }

        /// <summary>
        /// The retry policy for failed requests.
        /// </summary>
        public IRetryPolicy RetryPolicy { get; set; }
        
        public bool? CnameEnabled { get; set; }

        public bool? PathStyleEnabled { get; set; }

        public string UserAgent
        {
            get { return DefaultUserAgent; }
        }

        public static int DefaultMaxIdleTimeInMillis
        {
            get
            {
                // MaxServicePointIdleTime is set to 100,000 (100 seconds) by default.
                // If it is not changed, we will use 50 seconds instead, otherwise use its value directly.
                if (ServicePointManager.MaxServicePointIdleTime == 100 * 1000)
                {
                    return 50 * 1000;
                }
                else
                {
                    return ServicePointManager.MaxServicePointIdleTime;
                }
            }
        }

        public static int DefaultConnectionLimit
        {
            get
            {
                // DefaultConnectionLimit is set to 2 by default.
                // If it is not changed, we will use 50 instead, otherwise use its value directly.
                if (ServicePointManager.DefaultConnectionLimit == 2)
                {
                    return 50;
                }
                else
                {
                    return ServicePointManager.DefaultConnectionLimit;
                }
            }
        }

        public BceClientConfiguration()
        {
        }

        public BceClientConfiguration(BceClientConfiguration other)
        {
            this.Credentials = other.Credentials;
            this.Region = other.Region;
            this.Endpoint = other.Endpoint;
            this.Protocol = other.Protocol;
            this.TimeoutInMillis = other.TimeoutInMillis;
            this.ReadWriteTimeoutInMillis = other.ReadWriteTimeoutInMillis;
            this.ProxyHost = other.ProxyHost;
            this.ProxyPort = other.ProxyPort;
            this.ProxyCredentials = other.ProxyCredentials;
            this.UseNagleAlgorithm = other.UseNagleAlgorithm;
            this.MaxIdleTimeInMillis = other.MaxIdleTimeInMillis;
            this.ConnectionLimit = other.ConnectionLimit;
            this.SocketBufferSizeInBytes = other.SocketBufferSizeInBytes;
            this.Signer = other.Signer;
            this.SignOptions = other.SignOptions;
            this.RetryPolicy = other.RetryPolicy;
            this.CnameEnabled = other.CnameEnabled;
            this.PathStyleEnabled = other.PathStyleEnabled;
        }

        public BceClientConfiguration Merge(BceClientConfiguration other)
        {
            BceClientConfiguration ret = new BceClientConfiguration(this);
            if (other == null)
            {
                return ret;
            }
            if (other.Credentials != null)
            {
                ret.Credentials = other.Credentials;
            }
            if (other.Region != null)
            {
                ret.Region = other.Region;
            }
            if (other.Endpoint != null)
            {
                ret.Endpoint = other.Endpoint;
            }
            if (other.Protocol != null)
            {
                ret.Protocol = other.Protocol;
            }
            if (other.TimeoutInMillis.HasValue)
            {
                ret.TimeoutInMillis = other.TimeoutInMillis;
            }
            if (other.ReadWriteTimeoutInMillis.HasValue)
            {
                ret.ReadWriteTimeoutInMillis = other.ReadWriteTimeoutInMillis;
            }
            if (other.ProxyHost != null)
            {
                ret.ProxyHost = other.ProxyHost;
            }
            if (other.ProxyPort.HasValue)
            {
                ret.ProxyPort = other.ProxyPort;
            }
            if (other.ProxyCredentials != null)
            {
                ret.ProxyCredentials = other.ProxyCredentials;
            }
            if (other.UseNagleAlgorithm.HasValue)
            {
                ret.UseNagleAlgorithm = other.UseNagleAlgorithm;
            }
            if (other.MaxIdleTimeInMillis.HasValue)
            {
                ret.MaxIdleTimeInMillis = other.MaxIdleTimeInMillis;
            }
            if (other.ConnectionLimit.HasValue)
            {
                ret.ConnectionLimit = other.ConnectionLimit;
            }
            if (other.SocketBufferSizeInBytes.HasValue)
            {
                ret.SocketBufferSizeInBytes = other.SocketBufferSizeInBytes;
            }
            if (other.Signer != null)
            {
                ret.Signer = other.Signer;
            }
            if (other.SignOptions != null)
            {
                ret.SignOptions = other.SignOptions;
            }
            if (other.RetryPolicy != null)
            {
                ret.RetryPolicy = other.RetryPolicy;
            }
            if (other.CnameEnabled.HasValue)
            {
                ret.CnameEnabled = other.CnameEnabled;
            }
            if (other.PathStyleEnabled.HasValue)
            {
                ret.PathStyleEnabled = other.PathStyleEnabled;
            }
            return ret;
        }

        internal static BceClientConfiguration CreateWithDefaultValues()
        {
            var config = new BceClientConfiguration();
            config.Region = BceConstants.Region.Beijing;
            config.Protocol = BceConstants.Protocol.Http;
            config.TimeoutInMillis = BceClientConfiguration.DefaultTimeoutInMillis;
            config.ReadWriteTimeoutInMillis = BceClientConfiguration.DefaultReadWriteTimeoutInMillis;
            config.UseNagleAlgorithm = true;
            config.MaxIdleTimeInMillis = BceClientConfiguration.DefaultMaxIdleTimeInMillis;
            config.ConnectionLimit = BceClientConfiguration.DefaultConnectionLimit;
            config.SocketBufferSizeInBytes = BceClientConfiguration.DefaultSocketBufferSizeInBytes;
            config.CnameEnabled = false;
            config.PathStyleEnabled = false;
            config.Signer = new BceV1Signer();
            config.SignOptions = new SignOptions();
            config.RetryPolicy = new DefaultRetryPolicy();
            return config;
        }

        private static string GenerateDefaultUserAgent()
        {
            string osVersion = "Unknown";
            try
            {
                osVersion = Environment.OSVersion.ToString();
            }
            catch
            {
                // Ignore all exceptions, use "Unknown".
            }
            string runtimeVersion = string.Format(
                CultureInfo.InvariantCulture, "{0}.{1}", Environment.Version.Major, Environment.Version.MajorRevision);
            string userAgent = string.Format("bce-sdk-dotnet/{0}/Framework:{1}/Runtime:{2}/OS:{3}",
                Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                GetFrameworkVersion(),
                runtimeVersion,
                osVersion);

            // Space is evil, avoid it
            return new Regex(@"\s+").Replace(userAgent, "_");
        }

        // see https://msdn.microsoft.com/en-us/library/hh925568(v=vs.110).aspx
        // How to: Determine Which .NET Framework Versions Are Installed
        private static string GetFrameworkVersion()
        {
            using (RegistryKey ndpKey =
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                if (ndpKey == null)
                {
                    return "Unknown(NDP key not found)";
                }
                using (RegistryKey v4Key = ndpKey.OpenSubKey("v4"))
                {
                    if (Environment.Version.Major >= 4 && v4Key != null)
                    {
                        return BceClientConfiguration.GetFrameworkVersionAfter4(v4Key);
                    }
                }
                using (RegistryKey v35Key = ndpKey.OpenSubKey("v3.5"))
                {
                    if (v35Key != null)
                    {
                        return "3.5";
                    }
                }
                using (RegistryKey v30Key = ndpKey.OpenSubKey("v3.0"))
                {
                    if (v30Key != null)
                    {
                        return "3.0";
                    }
                }
                using (RegistryKey v20Key = ndpKey.OpenSubKey("v2.0.50727"))
                {
                    if (v20Key != null)
                    {
                        return "2.0";
                    }
                }
            }
            return "Unknown";
        }

        private static string GetFrameworkVersionAfter4(RegistryKey v4Key)
        {
            using (RegistryKey fullKey = v4Key.OpenSubKey("Full"))
            {
                if (fullKey != null)
                {
                    object release = fullKey.GetValue("Release");
                    if (release != null)
                    {
                        int releaseValue = Convert.ToInt32(release);
                        if (releaseValue > 393273)
                        {
                            return ">4.6RC";
                        }
                        if (releaseValue == 393273)
                        {
                            return "4.6RC";
                        }
                        if (releaseValue >= 379893)
                        {
                            return "4.5.2";
                        }
                        if (releaseValue >= 378675)
                        {
                            return "4.5.1";
                        }
                        if (releaseValue >= 378389)
                        {
                            return "4.5";
                        }
                    }
                }
            }
            return "4.0";
        }
    }
}