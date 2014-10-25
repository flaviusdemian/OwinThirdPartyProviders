using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace OwinOAuthProvidersDemo.Models
{
    public class CommitModel
    {

        [JsonProperty(PropertyName = "sha")]
        public String Sha { get; set; }
    }
}