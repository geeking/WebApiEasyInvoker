using Consul;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace WebApiEasyInvoker.Consul.Services
{
    class ConsulServiceRefresh : IConsulServiceRefresh, IDisposable
    {
        private readonly ILogger<ConsulServiceRefresh> _logger;
        private IEnumerable<string> _services;
        private IConsulClient _consul;
        private Timer _timer;
        private static ConcurrentDictionary<string, IEnumerable<ServiceInfo>> _serviceInfoDic;

        public ConsulServiceRefresh(ILogger<ConsulServiceRefresh> logger)
        {
            _logger = logger;
            _serviceInfoDic = new ConcurrentDictionary<string, IEnumerable<ServiceInfo>>();
        }

        

        public IEnumerable<ServiceInfo> GetServiceInfos(string serviceName)
        {
            if (_serviceInfoDic.TryGetValue(serviceName, out var serviceInfos))
            {
                return serviceInfos;
            }
            else
            {
                return null;
            }
        }

        public void LoadServices(IEnumerable<string> services)
        {
            _services = services;
        }

        public void Start(EasyInvokerConsulConfig consulConfig)
        {
            _consul = new ConsulClient(c =>
            {
                c.Address = new Uri(consulConfig.Address);

                if (!string.IsNullOrEmpty(consulConfig.Token))
                {
                    c.Token = consulConfig.Token;
                }
            });


            _timer = new Timer(async x =>
            {
                try
                {
                    foreach (var item in _services)
                    {
                        var queryResult = await _consul.Health.Service(item, string.Empty, true);
                        if (_serviceInfoDic.ContainsKey(item))
                        {
                            var newInfos = new List<ServiceInfo>();
                            var cacheInfos = _serviceInfoDic[item];
                            var entries = queryResult.Response;
                            var haveChanged = false;
                            foreach (var cacheInfo in cacheInfos)
                            {
                                if (entries.Any(x => x.Service.ID == cacheInfo.ID))
                                {
                                    newInfos.Add(cacheInfo);
                                }
                                else
                                {
                                    //local cache should remove
                                    haveChanged = true;
                                }
                            }
                            foreach (var entry in entries)
                            {
                                if (cacheInfos.Any(x => x.ID == entry.Service.ID))
                                {
                                    continue;
                                }
                                else
                                {
                                    //new entry
                                    newInfos.Add(new ServiceInfo
                                    {
                                        ID = entry.Service.ID,
                                        Address = entry.Service.Address,
                                        ServiceName = entry.Service.Service,
                                        Node = entry.Node.Name,
                                        ServicePort = entry.Service.Port
                                    });
                                    haveChanged = true;
                                }
                            }

                            if (haveChanged)
                            {
                                _serviceInfoDic.AddOrUpdate(item, newInfos, (x, y) => newInfos);
                            }
                        }
                        else if (queryResult.Response.Length > 0)
                        {
                            _serviceInfoDic.TryAdd(item, queryResult.Response.Select(e => new ServiceInfo
                            {
                                ID = e.Service.ID,
                                Address = e.Service.Address,
                                ServiceName = e.Service.Service,
                                Node = e.Node.Name,
                                ServicePort = e.Service.Port
                            }).ToArray());
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }

            }, null, consulConfig.RefreshMilliseconds, consulConfig.RefreshMilliseconds);

        }

        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;
        }
    }
}
