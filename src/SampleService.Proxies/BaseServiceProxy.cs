using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ServiceCommunicationLogging.ServiceProxies
{
    public abstract class BaseServiceProxy<TChannel> : IDisposable
    {
        public BaseServiceProxy(string endpointUrl, string username, string password)
        {
            this.EndpointUrl = endpointUrl;
            this.UserName = username;
            this.Password = password;
            this.CreateChannel();
        }

        ~BaseServiceProxy()
        {
            // Finalizer calls Dispose(false)
            this.Dispose(false);
        }

        #region Properties

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; private set; }

        /// <summary>
        /// Gets the name of the endpoint configuration.
        /// </summary>
        /// <value>
        /// The name of the endpoint configuration.
        /// </value>
        public string EndpointUrl { get; private set; }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        public TChannel Channel { get; protected set; }

        #endregion Properties

        private void CreateChannel()
        {
            try
            {
                var endpoint = new EndpointAddress(this.EndpointUrl);

                var binding = new NetTcpBinding();

                binding.Security.Mode = SecurityMode.None;
                //binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                //binding.Security.Message.ClientCredentialType = MessageCredentialType.None;

                ChannelFactory<TChannel> channelFactory = new ChannelFactory<TChannel>(binding, endpoint);

                //channelFactory.Credentials.ClientCertificate.SetCertificate(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName, "localhost");
                //channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;

                //channelFactory.Credentials.UserName.UserName = this.UserName;
                //channelFactory.Credentials.UserName.Password = this.Password;

                this.Channel = channelFactory.CreateChannel();

                SetupChannel();
            }
            catch (TimeoutException ex)
            {
                this.DisposeChannel();
            }
            catch (CommunicationException ex)
            {
                this.DisposeChannel();
            }
        }

        /// <summary>
        /// Setups the channel.
        /// do things here to setup anything related to channel
        /// </summary>
        private void SetupChannel()
        {
            var channel = this.Channel as IClientChannel;

            channel.Open();
        }

        #region IDisposeable Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// Dispose() calls Dispose(true)
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// The bulk of the clean-up code is implemented in Dispose(bool)
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                this.DisposeChannel();
            }
        }

        /// <summary>
        /// Disposes the channel.
        /// </summary>
        protected void DisposeChannel()
        {
            var channel = this.Channel as IClientChannel;

            if (channel == null)
            {
                return;
            }

            var success = false;
            try
            {
                if (channel.State != CommunicationState.Faulted)
                {
                    channel.Close();
                    success = true;
                }
            }
            catch (TimeoutException ex)
            {
                // Hide communication exceptions on disposing channel as we are disposing underline channel in finally block
            }
            catch (CommunicationException ex)
            {
                // Hide communication exceptions on disposing channel as we are disposing underline channel in finally block
            }
            catch (Exception ex)
            {
                // Hide any exceptions on disposing channel as we are disposing underline channel in finally block
            }
            finally
            {
                if (!success)
                {
                    channel.Abort();
                }
            }
        }

        #endregion IDisposeable Methods
    }
}
