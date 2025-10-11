using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
namespace WebApplication2.Controllers
{
    [AllowAnonymous]
    public class SurveyController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View(new SurveyViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(SurveyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // TODO: 저장/전달 로직 (DB, API 호출 등)
            TempData["Ok"] = "설문이 접수되었습니다. 감사합니다!";
            return RedirectToAction(nameof(Index));
        }
    }
}

