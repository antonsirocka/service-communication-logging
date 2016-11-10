using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogStorage.Interfaces
{
    public interface ILogPersister
    {
        IList<ServiceMessageLog> Get();

        void Save(ServiceMessageLog log);

        void Update(ServiceMessageLog log);
    }
}
