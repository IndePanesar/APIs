using System.Configuration;

namespace RESTAPITest.Configuration
{
    [ConfigurationCollection(typeof(UserConfigurationElement))]
    public sealed class UserConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new UserConfigurationElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((UserConfigurationElement)element).PolicyHolder;
        }
    }
}
