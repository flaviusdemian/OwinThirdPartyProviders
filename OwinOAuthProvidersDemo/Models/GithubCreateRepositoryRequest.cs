using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace OwinOAuthProvidersDemo.Models
{
    public class GithubCreateRepositoryRequest
    {
        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public String Description { get; set; }

        [JsonProperty(PropertyName = "homepage")]
        public String Homepage { get; set; }

        [JsonProperty(PropertyName = "private")]
        public bool Private { get; set; }

        [JsonProperty(PropertyName = "has_issues")]
        public bool HasIssues { get; set; }

        [JsonProperty(PropertyName = "has_wiki")]
        public bool HasWiki { get; set; }

        [JsonProperty(PropertyName = "has_downloads")]
        public bool HasDownloads { get; set; }
    }
}