using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OwinOAuthProvidersDemo.Models
{
    public class CreateRepoModel
    {
        public CreateRepoModel()
        {
            Collaboratos = new Dictionary<string, string>();
        }
        public String TrackName { get; set; }
        public String TeamName { get; set; }
        public String MentorName { get; set; }

        public Dictionary<String, String> Collaboratos { get; set; }
    }
}