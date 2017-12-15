using System.Configuration;

namespace RESTAPITest.Configuration
{
    public class UserConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("policyHolder", IsKey = true, IsRequired = true)]
        public string PolicyHolder
        {
            get { return (string)this["policyHolder"]; }
            set { this["policyHolder"] = value; }
        }

        [ConfigurationProperty("oan", IsRequired = true)]
        public string Oan
        {
            get { return (string)this["oan"]; }
            set { this["oan"] = value; }
        }

        [ConfigurationProperty("userName", IsRequired = true)]
        public string UserName
        {
            get { return (string)this["userName"]; }
            set { this["userName"] = value; }
        }
    }
}
