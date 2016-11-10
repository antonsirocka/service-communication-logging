using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ServiceCommunicationLogging.ServiceProxies;
using Microsoft.Extensions.Options;
using LogsDataAccess.AzureImplementation;
using Microsoft.Extensions.Logging;

namespace ServiceCommunicationLogging.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptions<ServiceEndpoints> serviceEndpoints;
        private ILogger<HomeController> logger;

        public HomeController(IOptions<ServiceEndpoints> serviceEndpoints, ILoggerFactory loggerFactory)
        {
            this.serviceEndpoints = serviceEndpoints;
            this.logger = loggerFactory.CreateLogger<HomeController>();
        }

        public async Task<IActionResult> Index()
        {
            using (var serviceProxy = new Service1Proxy(this.serviceEndpoints?.Value?.Service1))
            {
                string test = await serviceProxy.GetDataAsync(1);
            }

            logger.LogTrace("Test trace logging " + DateTime.Now.ToString());

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
