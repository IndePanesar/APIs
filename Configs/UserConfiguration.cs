using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AutoMapper;
using RESTAPITest.JsonClasses.CommomRequests;

namespace RESTAPITest.Configuration
{
    public class UserConfiguration
    {
        private readonly Dictionary<string, UserConfigurationElement> _usersByUserName;

        static UserConfiguration()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<UserConfigurationElement, User>());
        }

        public UserConfiguration()
        {
            var section = (UsersConfigurationSection)ConfigurationManager.GetSection("users");

            var users = (from object configurationElement in section.Users
                select (configurationElement as UserConfigurationElement)
                into userConfigurationElement
                select Mapper.Map<UserConfigurationElement>(userConfigurationElement)
            ).ToList();

            _usersByUserName = users.ToDictionary(x => x.PolicyHolder);
        }

        public UserConfigurationElement GetUser(string policyHolder)
        {
            return _usersByUserName[policyHolder];
        }
    }
}
