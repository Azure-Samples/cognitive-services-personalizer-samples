using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerTravelAgencyDemo.Repositories
{
    public class IActionRepository
    {
        IList<Models.Action> GetArticles();
        Models.Action GetArticle(string id);
    }
}
