namespace SampleService.Logging
{
    using System;
    using System.ServiceModel.Channels;

    /// <summary>
    /// Static Class for the Message Header Extensions
    /// </summary>
    public static class MessageHeadersExtension
    {
        /// <summary>
        /// Get the Method Name from the Message Header
        /// </summary>
        /// <param name="headers">The Message Header</param>
        /// <returns>A string containing the Method Name</returns>
        public static string GetMethodName(this MessageHeaders headers)
        {
            string methodName = headers.Action;
            try
            {
                if (headers.Action.Contains("/"))
                {
                    methodName = headers.Action.Substring(headers.Action.LastIndexOf("/") + 1);
                }
            }
            catch (Exception)
            {
            }

            return methodName;
        }

        /// <summary>
        /// Get the Service Name from the Message Header
        /// </summary>
        /// <param name="headers">The Message Header</param>
        /// <returns>A string containing the Method Name</returns>
        public static string GetServiceName(this MessageHeaders headers)
        {
            string serviceName = headers.Action;
            try
            {
                if (headers.Action.Contains("/"))
                {
                    string service = headers.Action.Substring(0, headers.Action.LastIndexOf("/"));
                    serviceName = service.Substring(service.LastIndexOf("/") + 1);
                }
            }
            catch (Exception)
            {
            }

            return serviceName;
        }
    }
}
