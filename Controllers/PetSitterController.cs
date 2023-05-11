using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriendFinder.Controllers
{
    public class PetSitterController : Controller
    {
        // GET: PetSitterController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PetSitterController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PetSitterController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PetSitterController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PetSitterController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PetSitterController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PetSitterController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PetSitterController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
