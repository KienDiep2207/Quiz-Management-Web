using Microsoft.AspNetCore.Mvc;

namespace DALTWNC_QUIZ.Models.ViewModels
{
    public class SearchBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
