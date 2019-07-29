using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerTravelAgencyDemo.Repositories
{
    public interface IActionRepository
    {
        IList<Models.Action> GetActions();
        Models.Action GetAction(string id);
    }
}
