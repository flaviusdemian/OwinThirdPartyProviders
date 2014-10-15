using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OwinOAuthProvidersDemo.Controllers
{
    public partial class HomeController : Controller
    {
        public async virtual Task<ActionResult> Index()
        {
            var client = new HttpClient();
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("name", "my-new-repo"),
                new KeyValuePair<string, string>("description", "my new repo description")
            };

            var content = new FormUrlEncodedContent(pairs);
            //content.Headers.Add("User-Agent", "https://api.github.com/meta");
            //content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            try
            {
                string token = "366f97126feadcdb2008701e4a13294d3684dc03";
                var result = await client.PostAsync(new Uri("https://api.github.com/user?access_token=" + token), content);
                result.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return View();
        }

        public virtual ActionResult SigninGithub()
        {
            return RedirectToAction("Index");
        }

        public virtual ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public virtual ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}