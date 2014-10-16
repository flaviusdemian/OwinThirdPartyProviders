using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OwinOAuthProvidersDemo.Models
{
    public class UserToSendEmailTo
    {
        public String Name { get; set; }
        public String Email { get; set; }
        public String RepoLink { get; set; }
    }
}