using Opc.Ua;
using Opc.Ua.Configuration;
using System.Threading.Channels;
using neulib;
using BenchmarkDotNet.Attributes;

namespace neuserver
{
    [MemoryDiagnoser]
    public class UaServer
    {
        private readonly ApplicationInstance _application;
        private readonly NeuServer _server;
        private readonly string _user;
        private readonly string _password;
        private readonly ValueWrite _write;

        public Channel<Msg>? DataChannel { get; }
        public bool Running { get; private set; }

        private readonly Task _task;

        public UaServer(string uri, string user, string password, ValueWrite write)
        {
            _user = user;
            _password = password;
            _write = write;

            Running = false;
            DataChannel = Channel.CreateUnbounded<Msg>();

            var securityConfig = new ServerSecurityPolicy
            {
                SecurityMode = MessageSecurityMode.None,
                SecurityPolicyUri = SecurityPolicies.None
            };

            var tokenPolicies = new List<UserTokenPolicy>
            {
                new UserTokenPolicy(UserTokenType.Anonymous),
                //new UserTokenPolicy(UserTokenType.Certificate)
            };

            var serverConfiguration = new ServerConfiguration()
            {
                BaseAddresses = { uri },
                MinRequestThreadCount = 20,
                MaxRequestThreadCount = 200,
                MaxQueuedRequestCount = 400,
                UserTokenPolicies = new UserTokenPolicyCollection(tokenPolicies),
                SecurityPolicies = new ServerSecurityPolicyCollection
                {
                    securityConfig
                }

            };

            var securityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = @"Directory",
                    StorePath =
                        @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault",
                    SubjectName = Utils.Format(
                        @"CN={0}, DC={1}",
                        "neuopc",
                        System.Net.Dns.GetHostName()
                    )
                },
                TrustedIssuerCertificates = new CertificateTrustList
                {
                    StoreType = @"Directory",
                    StorePath =
                        @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities"
                },
                TrustedPeerCertificates = new CertificateTrustList
                {
                    StoreType = @"Directory",
                    StorePath =
                        @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications"
                },
                RejectedCertificateStore = new CertificateTrustList
                {
                    StoreType = @"Directory",
                    StorePath =
                        @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates"
                },
                AutoAcceptUntrustedCertificates = true,
                AddAppCertToTrustedStore = true
            };

            var config = new ApplicationConfiguration
            {
                ApplicationName = "neuopc",
                ApplicationUri = Utils.Format(@"urn:{0}:neuopc", System.Net.Dns.GetHostName()),
                ApplicationType = ApplicationType.Server,
                ServerConfiguration = serverConfiguration,
                SecurityConfiguration = securityConfiguration,
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
                TraceConfiguration = new TraceConfiguration(),
            };

            config.TransportQuotas.MaxArrayLength = 65535;
            config.TransportQuotas.MaxStringLength = 65535;
            config.TransportQuotas.MaxMessageSize = 4 * 1024 * 1024;
            config.TransportQuotas.MaxByteStringLength = 4 * 1024 * 1024;


            config.Validate(ApplicationType.Server).GetAwaiter().GetResult();
            if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                config.CertificateValidator.CertificateValidation += (s, e) =>
                {
                    e.Accept = (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted);
                };
            }

            _application = new ApplicationInstance
            {
                ApplicationName = "neuopc",
                ApplicationType = ApplicationType.Server,
                ApplicationConfiguration = config
            };

            _server = new NeuServer(_write);

            //_task = new Task(async () =>
            //{
            //    while (await DataChannel.Reader.WaitToReadAsync())
            //    {
            //        if (DataChannel.Reader.TryRead(out var msg))
            //        {
            //            if (null != msg && null != msg.Items)
            //            {
            //                _server.UpdateNodes(msg.Items);
            //            }
            //        }
            //    }
            //});
            //_task.Start();

            // 替换 _task 初始化部分
            _task = Task.Run(async () =>
            {
                const int batchSize = 500;
                await foreach (var msg in DataChannel.Reader.ReadAllAsync())
                {
                    if (msg?.Items != null && msg.Items.Count > 0)
                    {
                        var items = msg.Items;
                        for (int i = 0; i < items.Count; i += batchSize)
                        {
                            var batch = items.Skip(i).Take(batchSize).ToList();
                            _server.UpdateNodes(batch);
                        }
                    }
                }
            });

        }

        [Benchmark]
        public void Start()
        {
            if (Running)
            {
                return;
            }

            bool certOk = _application.CheckApplicationInstanceCertificate(false, 0).Result;
            if (!certOk)
            {
                System.Diagnostics.Debug.WriteLine("check application instacnce cert failed");
            }

            try
            {
                _server.SetUser(_user, _password);
                _application.Start(_server).Wait();
                Running = true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Benchmark]
        public void Stop()
        {
            if (Running)
            {
                try
                {
                    _application.Stop();
                }
                catch (Exception)
                {
                    throw;
                }

                Running = false;
            }

            System.Diagnostics.Debug.WriteLine("ua server stoped");
        }
    }
}
