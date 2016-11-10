using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SampleService
{
    public class Service1 : IService1
    {
        public Service1()
        {
            XmlConfigurator.Configure();
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof(Service1));

        public string GetData(int value)
        {
            Log.Debug("Timestamp " + DateTime.Now.ToString());

            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
