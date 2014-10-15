using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Owin.Security.Providers;
using OwinOAuthProvidersDemo.Models;

namespace OwinOAuthProvidersDemo.Controllers
{
    public partial class HomeController : Controller
    {
        String token = "734253fe7f1742e11060992a8597ab95435adf56";
        private HttpClient client = null;
        public async virtual Task<ActionResult> Index()
        {
            var lines = FileReaderBusinessLogic.ReadFiles(Server.MapPath("/Input/Input.txt"));
            if (lines != null && lines.Length > 1)
            {
                string currentLine = null;
                List<CreateRepoModel> reposToCreate = new List<CreateRepoModel>();
                for (int i = 0; i < lines.Length; i++)
                {
                    try
                    {
                        currentLine = lines[i];
                        if (i == 0)
                        {
                            continue;
                        }
                        string[] partsOfCurrentLines = currentLine.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (partsOfCurrentLines != null && partsOfCurrentLines.Length > 0)
                        {
                            string currentUserGithubUsername = null;
                            string currentUserEmail = null;
                            CreateRepoModel currentRepoToCreate = new CreateRepoModel();
                            for (int j = 0; j < partsOfCurrentLines.Length; j++)
                            {
                                if (j == 0)
                                {
                                    continue;
                                }
                                if (j == 1) // track name team name mentor name
                                {
                                    currentRepoToCreate.TrackName = partsOfCurrentLines[j];
                                }
                                if (j == 2)
                                {
                                    currentRepoToCreate.TeamName = partsOfCurrentLines[j];
                                }
                                if (j == 3)
                                {
                                    currentRepoToCreate.MentorName = partsOfCurrentLines[j];
                                }
                                if (j > 3 && j % 2 == 0)
                                {
                                    currentUserGithubUsername = partsOfCurrentLines[j];
                                    try
                                    {
                                        currentUserEmail = partsOfCurrentLines[j + 1];
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.ToString();
                                    }
                                    currentRepoToCreate.Collaboratos.Add(currentUserGithubUsername, currentUserEmail);
                                }
                            }
                            await CreateRepoAndAddCollaborators(currentRepoToCreate);
                            //reposToCreate.Add(currentRepoToCreate);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
            }
            return View();
        }

        private async Task CreateRepoAndAddCollaborators(CreateRepoModel currentRepoToCreate)
        {
            try
            {
                if (currentRepoToCreate != null && currentRepoToCreate.Collaboratos != null &&
                    String.IsNullOrWhiteSpace(currentRepoToCreate.TrackName) == false
                    && String.IsNullOrWhiteSpace(currentRepoToCreate.TeamName) == false
                    && String.IsNullOrWhiteSpace(currentRepoToCreate.MentorName) == false)
                {
                    client = new HttpClient();
                    GithubCreateRepositoryRequest model = new GithubCreateRepositoryRequest()
                    {
                        Name = String.Format("{0}-{1}", currentRepoToCreate.TrackName, currentRepoToCreate.TeamName),
                        Description = String.Format("Mentor-{0}", currentRepoToCreate.MentorName),
                        Homepage = "https://github.com",
                        Private = false,
                        HasIssues = true,
                        HasWiki = true,
                        HasDownloads = true
                    };

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "https://api.github.com/meta");

                    try
                    {
                        var result = await client.PostAsync(new Uri(String.Format("https://api.github.com/user/repos?access_token={0}", token)),
                                     new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
                        result.EnsureSuccessStatusCode();
                        string content = await result.Content.ReadAsStringAsync();
                        var parsedObject = JsonConvert.DeserializeObject<GithubCreateRepositoryResponseRepoDetails>(content);

                        foreach (var collaboratorsEntry in currentRepoToCreate.Collaboratos)
                        {
                            try
                            {
                                result = await client.PutAsync(new Uri(
                                 String.Format("https://api.github.com/repos/{0}/collaborators/{1}?access_token={2}", parsedObject.FullName, collaboratorsEntry.Key, token)), null);
                                result.EnsureSuccessStatusCode();
                            }
                            catch (Exception ex)
                            {
                                ex.ToString();
                            }
                        }
                        //https://api.github.com/repos/slown1/GRWXUZEMIJ/collaborators/vladisac
                        //string owner = "slown1";
                        //string projectId = "GRWXUZEMIJ";
                        //string usertoInvite = "denisdenes";


                        //result = await client.Get()
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
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