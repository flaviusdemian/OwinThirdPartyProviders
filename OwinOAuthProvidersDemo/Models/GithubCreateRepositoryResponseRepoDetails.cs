using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace OwinOAuthProvidersDemo.Models
{
    public class GithubCreateRepositoryResponseRepoDetails
    {
        [JsonProperty(PropertyName = "full_name")]
        public String FullName { get; set; }

        [JsonProperty(PropertyName = "id")]
        public String Id { get; set; }

        [JsonProperty(PropertyName = "html_url")]
        public String RepoUrl { get; set; }

    }
}