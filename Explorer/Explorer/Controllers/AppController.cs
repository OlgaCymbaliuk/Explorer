using System.Web.Http;
using Explorer.Models;

namespace Explorer.Controllers
{
    public class AppController : ApiController
    {
        private string Patch(string value) { return value.Replace("||", ":\\").Replace("|", "\\"); }

        [HttpGet]
        public object Get()
        {          
            return ExplorerObj.GoToParentDirectory(string.Empty);
        }

        [HttpGet]
        public object Get(string path)
        {
            return ExplorerObj.GoToParentDirectory(Patch(path));
        }

        // GET: api/App/5
        [HttpGet]
        public object Get(string path, string value)
        {
            return ExplorerObj.GoToChild(Patch(path), Patch(value));
        }
    }
}
