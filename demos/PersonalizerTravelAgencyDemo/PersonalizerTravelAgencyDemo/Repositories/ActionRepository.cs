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
            var contents = fileProvider.GetDirectoryContents("Actions/Layouts");
            _actions = contents
                            .Select(file => System.IO.File.ReadAllText(file.PhysicalPath))
                            .Select(fileContent => JsonConvert.DeserializeObject<Action>(fileContent))
                            .Where(a => a.Enabled)
                            .ToList();
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
