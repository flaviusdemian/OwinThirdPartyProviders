using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using EmailUtilsRazor;
using Newtonsoft.Json;
using Owin.Security.Providers;
using OwinOAuthProvidersDemo.Models;

namespace OwinOAuthProvidersDemo.Controllers
{
    public partial class HomeController : Controller
    {
        String token = "734253fe7f1742e11060992a8597ab95435adf56";
        //private String token = "147a9066d2f30d590ce1217ff90fbf887eec7c0f";
        private HttpClient client = null;
        List<String> currentTrackTeamCombinations = new List<string>();
        public async virtual Task<ActionResult> Index()
        {
            //TODO: check if list name is unique -> if equal (1)
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
                        string[] partsOfCurrentLines = currentLine.Split(new string[] { "," }, StringSplitOptions.None);
                        if (partsOfCurrentLines != null && partsOfCurrentLines.Length > 0)
                        {
                            string currentUserGithubUsername = null;
                            string currentUserEmail = null;
                            string currentUserName = null;
                            CreateRepoModel currentRepoToCreate = new CreateRepoModel();
                            List<UserToSendEmailTo> usersToSendEmailTo = new List<UserToSendEmailTo>();
                            // track name  
                            currentRepoToCreate.TrackName = partsOfCurrentLines[1].Trim();
                            //team name
                            currentRepoToCreate.TeamName = partsOfCurrentLines[2].Trim();
                            //mentor name
                            currentRepoToCreate.MentorName = partsOfCurrentLines[3].Trim();

                            List<string> collaboratorsEntry = partsOfCurrentLines.ToList();
                            for (int l = 0; l < 4; l++)
                            {
                                collaboratorsEntry.RemoveAt(0);
                            }

                            for (int j = 0; j < collaboratorsEntry.ToArray().Length; j++)
                            {
                                if (j % 4 == 0)
                                {
                                    try
                                    {
                                        currentUserName = collaboratorsEntry.ElementAt(j + 1);
                                        currentUserEmail = collaboratorsEntry.ElementAt(j + 1);
                                        currentUserGithubUsername = collaboratorsEntry.ElementAt(j + 2);
                                        if (String.IsNullOrWhiteSpace(currentUserGithubUsername) == false)
                                        {
                                            currentRepoToCreate.Collaboratos.Add(currentUserGithubUsername, currentUserEmail);
                                        }
                                        else
                                        {
                                            usersToSendEmailTo.Add(new UserToSendEmailTo()
                                            {
                                                Email = currentUserEmail,
                                                Name = currentUserName
                                            });
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.ToString();
                                    }
                                }
                            }
                            String trackAndTeamName = String.Format("{0}-{1}", currentRepoToCreate.TrackName, currentRepoToCreate.TeamName);
                            if (currentTrackTeamCombinations.Contains(trackAndTeamName) == true)
                            {
                                currentRepoToCreate.TeamName = trackAndTeamName = String.Format("{0}-{1}", currentRepoToCreate.TeamName, RandomStringGenerator.RandomString(10));
                            }
                            currentTrackTeamCombinations.Add(trackAndTeamName);

                            reposToCreate.Add(currentRepoToCreate);
                            await CreateRepoAndAddCollaborators(currentRepoToCreate, usersToSendEmailTo);
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

        private void SendEmailToUserWithoutGithubAccount(UserToSendEmailTo userInfo)
        {
            try
            {
                bool result = EmailHelper.SendHtmlTemplatedEmailFromPath(userInfo.Email, "HackTM Notice", "Views/Templates/Emails/UserSendEmailNoGitAccount.cshtml", true, userInfo);
                if (result == false)
                {
                    int x = 0;
                    x++;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        private async Task CreateRepoAndAddCollaborators(CreateRepoModel currentRepoToCreate, List<UserToSendEmailTo> usersToSendEmailTo)
        {
            try
            {
                if (currentRepoToCreate != null && String.IsNullOrWhiteSpace(currentRepoToCreate.TrackName) == false
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
                        if (currentRepoToCreate.Collaboratos != null && currentRepoToCreate.Collaboratos.Count > 0)
                        {
                            foreach (var collaboratorsEntry in currentRepoToCreate.Collaboratos)
                            {
                                try
                                {
                                    result = await client.PutAsync(new Uri(
                                     String.Format("https://api.github.com/repos/{0}/collaborators/{1}?access_token={2}", parsedObject.FullName, collaboratorsEntry.Key.Trim(), token)), null);
                                    result.EnsureSuccessStatusCode();
                                }
                                catch (Exception ex)
                                {
                                    ex.ToString();
                                }
                            }
                        }

                        if (usersToSendEmailTo != null && usersToSendEmailTo.Count > 0)
                        {
                            foreach (var userToSendEmailToEntry in usersToSendEmailTo)
                            {
                                userToSendEmailToEntry.Email = "flavius_praf@yahoo.com";
                                userToSendEmailToEntry.RepoLink = parsedObject.RepoUrl;
                                SendEmailToUserWithoutGithubAccount(userToSendEmailToEntry);
                            }
                        }
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