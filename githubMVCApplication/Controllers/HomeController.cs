using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Octokit;
using githubMVCApplication.Models;

namespace githubMVCApplication.Controllers
{
    public class HomeController : Controller
    {
        const String CLIENT_ID = "705b86b5adba6afa4363";
        private const string CLIENT_SECRET = "38ae61180a749b2ebc4441c3c6d0046f28f49e28";

        readonly GitHubClient client = new GitHubClient(new ProductHeaderValue("Github-MVC-Demo"), new Uri("https://github.com/"));

        public async Task<ActionResult> Index()
        {
            var accessToken = Session["oAuthToken"] as String;
            if(accessToken != null)
            {
                client.Credentials = new Credentials(accessToken);
            }

            try
            {
                var repositories = await client.Repository.GetAllForCurrent();
                var user = await client.User.Current();
                var model = new IndexViewModel(repositories, user);

                return View(model);
            }
            catch(AuthorizationException)
            {
                return Redirect(getOauthLoginURL());
            }
        }

        public async Task<ActionResult> Authorize(string code, string state)
        {
            if (!String.IsNullOrEmpty(code))
            {
                var expectedState = Session["CSRF:State"] as string;
                if (state != expectedState) throw new InvalidOperationException("SECURITY FAIL!");
                Session["CSRF:State"] = null;

                var token = await client.Oauth.CreateAccessToken(
                    new OauthTokenRequest(CLIENT_ID, CLIENT_SECRET, code));
                Session["OAuthToken"] = token.AccessToken;
            }

            return RedirectToAction("Index");
        }

        private String getOauthLoginURL()
        {
            String csrf = Membership.GeneratePassword(24, 1);
            Session["CSRF:State"] = csrf;

            var request = new OauthLoginRequest(CLIENT_ID)
            {
                Scopes = { "user", "notifications" },
                State = csrf
            };

            var oauthLoginURL = client.Oauth.GetGitHubLoginUrl(request);
            return oauthLoginURL.ToString();
        }

        /*public async Task<ActionResult> Emojis()
        {
            var emojis = await client.Miscellaneous.GetEmojis();
            return View(emojis);
        }*/
    
    }
}