using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommunicationLogging.ServiceProxies
{
    public class Service1Proxy : BaseServiceProxy<IService1>, IService1
    {
        public Service1Proxy(string endpointUrl)
            :base(endpointUrl, null, null)
        {
        }

        public Service1Proxy(string endpointUrl, string username, string password)
            :base(endpointUrl, username, password)
        {
        }

        public Task<string> GetDataAsync(int value)
        {
            return this.Channel.GetDataAsync(value);
        }

        public ServiceCommunicationLogging.ServiceProxies.CompositeType GetDataUsingDataContract(ServiceCommunicationLogging.ServiceProxies.CompositeType composite)
        {
            return this.Channel.GetDataUsingDataContract(composite);
        }
    }
}
