using System.Collections.Generic;
using Octokit;

namespace githubMVCApplication.Models
{
    public class IndexViewModel
    {
        public IndexViewModel(IEnumerable<Repository> repositories, Octokit.User user)
        {
            Repositories = repositories;
            User = user;
            
        }

        public IEnumerable<Repository> Repositories { get; private set; }
        public Octokit.User User { get; private set; }
    }
}