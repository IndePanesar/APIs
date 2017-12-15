using System.Configuration;

namespace RESTAPITest.Configuration
{
    public class UsersConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("user")]
        public UserConfigurationCollection Users => base["user"] as UserConfigurationCollection;
    }
}