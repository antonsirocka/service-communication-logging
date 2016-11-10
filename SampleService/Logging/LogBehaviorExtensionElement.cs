namespace SampleService.Logging
{
    using System;
    using System.Configuration;
    using System.ServiceModel.Configuration;

    /// <summary>
    /// Log behavior extension element
    /// </summary>
    public class LogBehaviorExtensionElement : BehaviorExtensionElement
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="LogBehaviorExtensionElement"/> class.
        /// </summary>
        public LogBehaviorExtensionElement()
        {
        }

        /// <summary>
        /// Gets or sets the system name.
        /// </summary>
        /// <value>
        /// The systemName.
        /// </value>
        [ConfigurationProperty("systemName", DefaultValue = null, IsRequired = true)]
        public string SystemName
        {
            get { return (string)base["systemName"]; }
            set { base["systemName"] = value; }
        }

        /// <summary>
        /// Gets the type of behavior.
        /// </summary>
        /// <returns>A <see cref="T:System.Type"/>.</returns>
        public override Type BehaviorType
        {
            get
            {
                return typeof(LogEndpointBehavior);
            }
        }

        /// <summary>
        /// Creates a behavior extension based on the current configuration settings.
        /// </summary>
        /// <returns>
        /// The behavior extension.
        /// </returns>
        protected override object CreateBehavior()
        {
            return new LogEndpointBehavior(this.SystemName);
        }
    }
}
