using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using PersonalizerTravelAgencyDemo.Models;

namespace PersonalizerTravelAgencyDemo.Repositories
{
    public class ActionRepository : IActionRepository
    {
        private IList<Action> _actions;

        public ActionRepository(IHostingEnvironment hostingEnvironment)
        {
            var fileProvider = hostingEnvironment.ContentRootFileProvider;
            var fileContent = System.IO.File.ReadAllText(fileProvider.GetFileInfo("Actions/actions.json").PhysicalPath);
            _actions = JsonConvert.DeserializeObject<List<Action>>(fileContent);
        }

        public Action GetAction(string id)
        {
            return _actions.FirstOrDefault(action => action.Id == id);
        }

        public IList<Action> GetActions()
        {
            return _actions.ToList();
        }
    }
}
