using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace BrainWave.Pages.Controllers
{
    public class PageController : Controller
    {
        private readonly ApplicationPartManager _partManager;

        public PageController(ApplicationPartManager partManager)
        {
            _partManager = partManager;
        }

        public ActionResult Index()
        {
            var controllerFeature = new ControllerFeature();
            _partManager.PopulateFeature(controllerFeature);
            var a = controllerFeature.Controllers;
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
