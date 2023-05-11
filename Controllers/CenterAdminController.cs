using FurryFriendFinder.Models.Data;
using FurryFriendFinder.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace FurryFriendFinder.Controllers
{
    [Authorize(Roles = nameof(Rol.CenterAdmin))]
    public class CenterAdminController : Controller
    {
        private readonly FurryFriendFinderDbContext _context;

        public CenterAdminController(FurryFriendFinderDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        //It is a method dedicated to creating a Pdf on Inventory Movements in a range of dates
        [HttpPost]
        public IActionResult PrintMovement([FromForm] DateTime first, [FromForm] DateTime last)
        {
            if (first != default && last != default)
            {
                ViewData["DataRange"] = $"{first.ToShortDateString()} - {last.ToShortDateString()}";
                var model = _context.Movements
                    .Where(m => m.Date >= first && m.Date <= last)
                    .Select(m => new Movement()
                    {
                        IdMovement = m.IdMovement,
                        IdProduct = m.IdProduct,
                        Quantity = m.Quantity,
                        MovementType = m.MovementType,
                        MovementTypeString = m.MovementType == true ? "Entrada" : "Salida",
                        IdProductNavigation = m.IdProductNavigation // Include the Product entity reference
                    })
                    .ToList();
                return new ViewAsPdf("PrintMovement", model)
                {
                    FileName = $"Movements.pdf",
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    ViewData = ViewData
                };
            }
            else return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult PrintAdoption([FromForm] DateTime first, [FromForm] DateTime last)
        {
            if (first != default && last != default)
            {
                var fecha = _context.Adoptions.Where(a => a.IdAdoptionDateNavigation.RegisterAdoption >= first && a.IdAdoptionDateNavigation.RegisterAdoption <= last).FirstOrDefault();
                ViewBag.DateRange = $"{first.ToShortDateString()} - {last.ToShortDateString()}";
                var model = _context.Adoptions
                        .Where(a => a.IdAdoptionDateNavigation.RegisterAdoption >= first && a.IdAdoptionDateNavigation.RegisterAdoption <= last)
                        .Select(a => new AdoptionTable
                        {
                            IdAdoption = a.IdAdoption,
                            UserName = a.IdUserNavigation.Name,
                            PetName = a.IdPetNavigation.PetName,
                            RegisterAdoption = (DateTime)a.IdAdoptionDateNavigation.RegisterAdoption
                        })
                        .ToList();
                return new ViewAsPdf("PrintAdoption", model)
                {
                    FileName = $"Adoptions.pdf",
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    ViewData = ViewData
                };
            }
            else return RedirectToAction("Index");
        }

    }
}
