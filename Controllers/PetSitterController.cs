using FurryFriendFinder.Models.Data;
using FurryFriendFinder.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

namespace FurryFriendFinder.Controllers
{

        [Authorize(Roles = nameof(Rol.PetSitter))]
        public class PetSitterController : Controller
        {
            private readonly FurryFriendFinderDbContext _context;

            public PetSitterController(FurryFriendFinderDbContext context)
            {
                _context = context;
            }

            [HttpPost]
            public async Task<IActionResult> AdoptionCertificate([FromForm] string user, [FromForm] string pet)
            {
                Models.Data.User oUser = _context.Users
                    .Include(p => p.Accesses)
                    .Include(p => p.Addresses)
                    .Include(p => p.Phones)
                    .FirstOrDefault(u => u.Name == user);
                Pet oPet = _context.Pets.FirstOrDefault(p => p.PetName == pet);

                DateTime currentDate = DateTime.Today;
                AdoptionDate adoptionDate = _context.AdoptionDates.FirstOrDefault(ad => ad.RegisterAdoption.HasValue && ad.RegisterAdoption.Value.Date == currentDate);

                if (oUser != null && oPet != null)
                {
                    Certificate model = new Certificate();
                    model.User = oUser;
                    model.Pet = oPet;

                Adoption adoption = new Adoption()
                {
                    IdUserNavigation = oUser,
                    IdPetNavigation = oPet
                };

                    if (adoptionDate == null)
                    {
                    adoptionDate = new AdoptionDate
                    {
                        RegisterAdoption = currentDate
                    };
                    adoption.IdAdoptionDateNavigation = adoptionDate;
                    _context.Add(adoptionDate);
                    }
                    _context.Add(adoption);
                    await _context.SaveChangesAsync();

                    return new ViewAsPdf("AdoptionCertificate", model)
                    {
                        FileName = $"{oUser.Name}AdoptionCertificate.pdf",
                        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                        PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        ViewData = ViewData
                        
                    };
                }
                else
                {
                    return RedirectToAction("Pets");
                }
            }

            public async Task<IActionResult> CreateAdoption(int? id)
            {
                if (id == null || _context.Pets == null)
                {
                    return NotFound();
                }

                var pet = await _context.Pets
                    .Include(p => p.IdAnimalTypeNavigation)
                    .Include(p => p.IdStateHealthNavigation)
                    .FirstOrDefaultAsync(m => m.IdPet == id);
                if (pet == null)
                {
                    return NotFound();
                }

                return View(pet);
            }

            public IActionResult GetUsers(string term)
            {
                var userlist = (from u in _context.Users.ToList()
                                where u.Name.Contains(term, System.StringComparison.CurrentCultureIgnoreCase)
                                select new { value = u.Name });
                return Json(userlist);
            }







            public IActionResult GetNames(string term)
            {
                var products = (from u in _context.Products.ToList()
                                where u.ProductName.Contains(term, System.StringComparison.CurrentCultureIgnoreCase)
                                select new { value = u.ProductName });
                return Json(products);
            }
            public IActionResult GetTypes(string term)
            {
                var products = (from u in _context.AnimalTypes.ToList()
                                where u.Type.Contains(term, System.StringComparison.CurrentCultureIgnoreCase)
                                select new { value = u.Type });
                return Json(products);
            }
            public IActionResult GetPackings(string term)
            {
                var products = (from u in _context.Packings.ToList()
                                where u.TypePacking.Contains(term, System.StringComparison.CurrentCultureIgnoreCase)
                                select new { value = u.TypePacking });
                return Json(products);
            }
            public IActionResult GetBrands(string term)
            {
                var products = (from u in _context.Brands.ToList()
                                where u.NameBrand.Contains(term, System.StringComparison.CurrentCultureIgnoreCase)
                                select new { value = u.NameBrand });
                return Json(products);
            }


            public async Task<IActionResult> Inventories()
            {
                var pf1Context = _context.Inventories
                    .Include(i => i.IdProductNavigation)
                    .Include(i => i.IdProductNavigation.IdAnimalTypeNavigation)
                    .Include(i => i.IdProductNavigation.IdBrandNavigation)
                    .Include(i => i.IdProductNavigation.IdPackingNavigation)
                    .Include(i => i.IdProductNavigation.IdAnimalTypeNavigation);
                return View(await pf1Context.ToListAsync());
            }
            public async Task<IActionResult> InventoryDetails(int? id)
            {
                if (id == null || _context.Inventories == null)
                {
                    return NotFound();
                }

                var inventory = await _context.Inventories
                    .Include(i => i.IdProductNavigation)
                    .Include(i => i.IdProductNavigation.IdAnimalTypeNavigation)
                    .Include(i => i.IdProductNavigation.IdBrandNavigation)
                    .Include(i => i.IdProductNavigation.IdPackingNavigation)
                    .Include(i => i.IdProductNavigation.IdAnimalTypeNavigation)
                    .FirstOrDefaultAsync(m => m.IdProduct == id);
                if (inventory == null)
                {
                    return NotFound();
                }

                return View(inventory);
            }
            // GET: Inventories/Create
            public IActionResult InventoryCreate(int? id)
            {
                if (id == null)
                    return View();

                var Producto = _context.Products.Find(id);
                var packing = _context.Packings.Where(x => x.IdPacking == Producto.IdPacking).First();
                var Brand = _context.Brands.Where(x => x.IdBrand == Producto.IdBrand).First();
                var type = _context.AnimalTypes.Where(x => x.IdAnimalType == Producto.IdAnimalType).First();
                return View(new ProductInvent()
                {
                    product = Producto,
                    packing = packing,
                    brand = Brand,
                    type = type

                });
            }


        // POST: Inventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InventoryCreate([Bind("IdInventory,Quantity,IdProduct")] Inventory inventory, Product product, AnimalType type, Brand brand, Packing packing, Movement movement)
        {

            var zero = Convert.ToInt32(_context.Constants.Where(x => x.Description == "Zero").First().Value);
            if (type is null || inventory.Quantity is null)
                {
                    return View();
                }

                if (ModelState.IsValid)
                {
                    var b1 = _context.Brands.Where(x => x.NameBrand == brand.NameBrand).FirstOrDefault();
                    var p2 = _context.Packings.Where(x => x.TypePacking == packing.TypePacking).FirstOrDefault();
                    var a = _context.AnimalTypes.Where(x => x.Type == type.Type).FirstOrDefault();
                    bool nuevo = false;

                        var p1 = new Product();
                        if (b1 is not null && p2 is not null && a is not null)
                            p1 = _context.Products.Where(x => x.ProductName == product.ProductName && x.IdPacking == p2.IdPacking && x.IdBrand == b1.IdBrand && a.IdAnimalType == x.IdAnimalType).FirstOrDefault();
                       
                    if (p1 !=null || inventory.Quantity >= zero)
                    { if (b1 is null)
                        {
                            _context.Add(brand);
                            _context.SaveChanges();
                            nuevo = true;
                        }
                        else
                        {
                             brand = b1;
                        }
                        if (p2 is null)
                        {
                            _context.Add(packing);
                            _context.SaveChanges();
                            nuevo = true;
                        }
                            else
                        {
                            packing = p2;
                        }
                        if (a is null)
                        {
                            _context.Add(type);
                            _context.SaveChanges();
                            nuevo = true;
                        }
                            else
                        {
                            type = a;
                        }
                        if (p1 is null)
                            nuevo = true;
                        if (nuevo)
                        {

                            product.IdBrand = brand.IdBrand;
                            product.IdPacking = packing.IdPacking;
                            product.IdAnimalType = type.IdAnimalType;
                            _context.Add(product);
                            await _context.SaveChangesAsync();
                            inventory.IdProductNavigation = product;
                            _context.Add(inventory);
                            await _context.SaveChangesAsync();
                            movement.IdProduct = product.IdProduct;
                            movement.IdInventary = inventory.IdInventory;
                        }

                        movement.Quantity = inventory.Quantity;
                        if (!nuevo)
                        {

                            var inv = _context.Inventories.Where(x => x.IdProduct == p1.IdProduct).First();

                            movement.IdProduct = p1.IdProduct;
                            movement.IdInventary = inv.IdInventory;
                            inv.Quantity += inventory.Quantity;
                            _context.Update(inv);
                            await _context.SaveChangesAsync();
                            if (inventory.Quantity < zero)
                            {
                                movement.MovementType = false;
                                _context.Add(movement);
                            }

                        }
                        if (inventory.Quantity >= zero)
                        {
                            movement.MovementType = true;
                            _context.Add(movement);
                        }

                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Inventories));
                    }
                }

                ViewBag.MessageError = _context.Constants.Where(x => x.Description == "MessageError5").First().Value;
                return View(new ProductInvent()
                {
                    movement = movement,
                    product = product,
                    type = type,
                    packing = packing,
                    brand = brand,
                    inventory = inventory
                });
            }



            public async Task<IActionResult> Pets()
            {
                var proyectPetsContext = _context.Pets
                    .Include(p => p.Adoptions)
                    .Include(p => p.IdAnimalTypeNavigation)
                    .Include(p => p.IdBreedNavigation)
                    .Include(p => p.IdStateHealthNavigation);
                return View(await proyectPetsContext.ToListAsync());
            }

            // GET: Pets/Details/5
            public async Task<IActionResult> PetDetails(int? id)
            {
                if (id == null || _context.Pets == null)
                {
                    return NotFound();
                }

                var pet = await _context.Pets
                    .Include(p => p.Adoptions)
                    .Include(p => p.IdAnimalTypeNavigation)
                    .Include(p => p.IdBreedNavigation)
                    .Include(p => p.Vaccines)
                    .Include(p => p.IdStateHealthNavigation)
                    .FirstOrDefaultAsync(m => m.IdPet == id);
                if (pet == null)
                {
                    return NotFound();
                }

                return View(new extraPet(pet));
            }

            // GET: Pets/Create
            public IActionResult PetCreate()
            {



                ViewData["IdAnimalType"] = new SelectList(_context.AnimalTypes, "IdAnimalType", "Type");
                ViewData["IdBreed"] = new SelectList(_context.Breeds, "IdBreed", "Breed1");
                ViewData["IdStateHealth"] = new SelectList(_context.StateHealths, "IdStateHealth", "IdStateHealth");
                return View();
            }

            // POST: Pets/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> PetCreate([Bind("IdPet,PetImage,PetName,Gender,BirthYear,IdAnimalType,IdAdoption,IdBreed, IdStateHealth")] Pet pet, IFormFile PetImagee, List<string> Vaccine, List<DateTime> VaccinationDate, StateHealth stateHealth)
            {




                if (PetImagee != null)
                {
                    var stream = new MemoryStream();
                    PetImagee.CopyTo(stream);
                    pet.PetImage = stream.ToArray();


                    if (ModelState.IsValid)
                    {
                        _context.StateHealths.Add(stateHealth);
                        await _context.SaveChangesAsync();
                        pet.IdStateHealth = stateHealth.IdStateHealth;


                        _context.Add(pet);



                        await _context.SaveChangesAsync();
                        var num = 0;




                        foreach (var p in Vaccine)
                        {


                            // _context.Vaccines.Add(new Vaccine {  TypeVaccine = p, IdPetNavigation = pet });
                            _context.Vaccines.Add(new Vaccine { TypeVaccine = Vaccine[num], VaccinationDate = VaccinationDate[num], IdPetNavigation = pet });
                            num++;
                        }

                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Pets));
                    }


                }
                else
                    ViewBag.MessageError = "Insert image ";



                ViewData["IdAnimalType"] = new SelectList(_context.AnimalTypes, "IdAnimalType", "Type", pet.IdAnimalType);
                ViewData["IdBreed"] = new SelectList(_context.Breeds, "IdBreed", "Breed1", pet.IdBreed);
                ViewData["IdStateHealth"] = new SelectList(_context.StateHealths, "IdStateHealth", "IdStateHealth", pet.IdStateHealth);

                return View(new extraPet(pet));
            }

            // GET: Pets/Edit/5
            public async Task<IActionResult> PetEdit(int? id)
            {
                if (id == null || _context.Pets == null)
                {
                    return NotFound();
                }

                var pet = await _context.Pets.FindAsync(id);
                if (pet == null)
                {
                    return NotFound();
                }
                ViewData["IdAnimalType"] = new SelectList(_context.AnimalTypes, "IdAnimalType", "Type");
                ViewData["IdBreed"] = new SelectList(_context.Breeds, "IdBreed", "Breed1");
                ViewData["IdStateHealth"] = new SelectList(_context.StateHealths, "IdStateHealth", "IdStateHealth", pet.IdStateHealth);
                return View(new extraPet(pet));
            }

            // POST: Pets/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> PetEdit(int id, [Bind("IdPet,PetImage,PetName,Gender,BirthYear,IdAnimalType,IdAdoption,IdStateHealth,IdBreed")] Pet pet, IFormFile? PetImagee, List<string> Vaccine, List<DateTime> VaccinationDate, StateHealth stateHealth, Breed breed)
            {

                if (PetImagee != null)
                {
                    var stream = new MemoryStream();
                    PetImagee.CopyTo(stream);
                    pet.PetImage = stream.ToArray();
                }

                if (id != pet.IdPet)
                {
                    return NotFound();
                }
                var sthealth = _context.StateHealths.Where(x => x.Castrated == stateHealth.Castrated && x.State == stateHealth.State).FirstOrDefault();
                if (sthealth == null)
                {
                    _context.StateHealths.Add(stateHealth);
                    await _context.SaveChangesAsync();

                    pet.IdStateHealth = stateHealth.IdStateHealth;
                }
                else
                {
                    pet.IdStateHealthNavigation = sthealth;

                }
                if (ModelState.IsValid && pet.PetImage != null)
                {

                    try
                    {
                        _context.Update(pet);
                        await _context.SaveChangesAsync();

                        var Vacciene = _context.Vaccines.ToList();
                        Vacciene = Vacciene.FindAll(x => x.IdPet == pet.IdPet);
                        if (Vacciene != null)
                        {
                            _context.Vaccines.RemoveRange(Vacciene);
                            await _context.SaveChangesAsync();
                        }

                        var num = 0;

                        foreach (var p in Vaccine)
                        {
                            _context.Vaccines.Add(new Vaccine { TypeVaccine = Vaccine[num], VaccinationDate = VaccinationDate[num], IdPetNavigation = pet });
                            num++;
                        }


                        _context.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PetExists(pet.IdPet))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Pets));
                }
                else
                {
                    ViewBag.MessageError = "The model is not valid ";
            }
            ViewData["IdBreed"] = new SelectList(_context.Breeds, "IdBreed", "Breed1");
            ViewData["IdAnimalType"] = new SelectList(_context.AnimalTypes, "IdAnimalType", "IdAnimalType", pet.IdAnimalType);
                ViewData["IdStateHealth"] = new SelectList(_context.StateHealths, "IdStateHealth", "IdStateHealth", pet.IdStateHealth);
                return View(new extraPet(pet));
            }

            // GET: Pets/Delete/5
            public async Task<IActionResult> PetDelete(int? id)
            {
                if (id == null || _context.Pets == null)
                {
                    return NotFound();
                }

                var pet = await _context.Pets
                    .Include(p => p.Adoptions)
                    .Include(p => p.IdAnimalTypeNavigation)
                    .Include(p => p.IdStateHealthNavigation)
                    .FirstOrDefaultAsync(m => m.IdPet == id);
                if (pet == null)
                {
                    return NotFound();
                }

                return View(pet);
            }

            // POST: Pets/Delete/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> PetDelete(int id)
            {
                if (_context.Pets == null)
                {
                    return Problem("Entity set 'ProyectPetsContext.Pets'  is null.");
                }
                var pet = await _context.Pets.Include(p=>p.Adoptions).Include(p => p.Vaccines).Include(p => p.AppointmentUsers).FirstAsync(x=>x.IdPet==id);
                if (pet != null)
                {
                    _context.Pets.Remove(pet);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Pets));
            }

            private bool PetExists(int id)
            {
                return (_context.Pets?.Any(e => e.IdPet == id)).GetValueOrDefault();
            }
        }
    }


