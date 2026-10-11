using Microsoft.AspNetCore.Mvc;

namespace DALTWNC_QUIZ.Models.ViewComponents
{
    public class AdminSidebarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            string? currentPage = ViewContext.RouteData.Values["Page"]?.ToString();

            ViewBag.CurrentPage = currentPage;

            return View();
        }
    }
}
