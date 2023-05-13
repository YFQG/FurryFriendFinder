using FurryFriendFinder.Models.Data;
using FurryFriendFinder.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Net;
using System;
using Newtonsoft.Json.Linq;

namespace FurryFriendFinder.Controllers
{

        [Authorize(Roles = nameof(Rol.PetSitter))]

    // This "PetSitterController" class contains different methods for Caregiver views which you are inheriting from "Controller".

    public class PetSitterController : Controller
        {
            private readonly FurryFriendFinderDbContext _context;

        //This constructor is used to initialize a "PetSitterController" instance with a "FurryFriendFinderDbContext" object.
        public PetSitterController(FurryFriendFinderDbContext context)
        {
            _context = context;
        }
        [HttpPost]
            //Is a controller action that is invoked when an HTTP request is made to a specific endpoint.which calls the parameters "user" and "pet"
            [HttpPost]
            public async Task<IActionResult> AdoptionCertificate([FromForm] string user, [FromForm] string pet)
            {
                var validate = _context.Adoptions.Where(a => a.IdPetNavigation.PetName == pet).FirstOrDefault();

                if (validate == null)
                {
                    Models.Data.User oUser = _context.Users
                    .Include(p => p.Accesses)
                    .Include(p => p.Addresses)
                    .Include(p => p.Phones)
                    .FirstOrDefault(u => u.Name == user);
                    Pet oPet = _context.Pets.FirstOrDefault(p => p.PetName == pet);

                DateTime currentDate = DateTime.Today;
                AdoptionDate adoptionDate = _context.AdoptionDates.FirstOrDefault(ad => ad.RegisterAdoption.HasValue && ad.RegisterAdoption.Value.Date == currentDate);


            //checks if both oUser and oPet are not void. to issue the adoption certificate
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
                }else return RedirectToAction("Pets");

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
        // The GetUsers method performs a user search based on a given term and returns the names of the matching users in JSON format.

        public IActionResult GetUsers(string term)
            {
                var userlist = (from u in _context.Users.ToList()
                                where u.Name.Contains(term, System.StringComparison.CurrentCultureIgnoreCase)
                                select new { value = u.Name });
                return Json(userlist);
            }

        //The GetNames method takes a term parameter, which represents the search term used to filter products by name.       
        public IActionResult GetNames(string term) { 
                var products = (from u in _context.Products.ToList()
                                    //Make a LINQ query in the _context. Products collection for a list of products that meet the following criteria:
                                where u.ProductName.Contains(term, System.StringComparison.CurrentCultureIgnoreCase)
                                select new { value = u.ProductName });
                return Json(products);
            }
        //Action controller used to get a list of animal types that match a given search term.
        public IActionResult GetTypes(string term)
            {
                var products = (from u in _context.AnimalTypes.ToList()
                                    //query LINQ in the context.AnimalTypes collection for a list of animal types
                                where u.Type.Contains(term, System.StringComparison.CurrentCultureIgnoreCase)
                                select new { value = u.Type });
                return Json(products);
            }
        // Action controller used to get a list of product packaging

        public IActionResult GetPackings(string term)
            {
                var products = (from u in _context.Packings.ToList()
                                    //consulta LINQ en la colección _context. Packings para obtener una lista de empaques
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
            // Consult inventory collection including relationships with other entities
            var pf1Context = _context.Inventories
                    .Include(i => i.IdProductNavigation)
                    .Include(i => i.IdProductNavigation.IdAnimalTypeNavigation)
                    .Include(i => i.IdProductNavigation.IdBrandNavigation)
                    .Include(i => i.IdProductNavigation.IdPackingNavigation)
                    .Include(i => i.IdProductNavigation.IdAnimalTypeNavigation);

            // Returns "Inventories" view by passing inventory list as view model
            return View(await pf1Context.ToListAsync());
            }
            public async Task<IActionResult> InventoryDetails(int? id)
            {
            // Check if the 'id' parameter is null or if the 'Inventories' collection is null
            if (id == null || _context.Inventories == null)
                {
                    return NotFound();
                }
            // Obtain the inventory corresponding to the 'id' provided, including related entities
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
            // Return a view with the found inventory
            return View(inventory);
            }
        // GET: Inventories/Create

        // "InventoryCreate" is a controller action that displays the view to create an inventory of a specific product.
        public IActionResult InventoryCreate(int? id)

            {
            // Check if the id parameter is null
            if (id == null)
                    return View();
            //Get the values associated with product
                var Producto = _context.Products.Find(id);
                var packing = _context.Packings.Where(x => x.IdPacking == Producto.IdPacking).First();
                var Brand = _context.Brands.Where(x => x.IdBrand == Producto.IdBrand).First();
                var type = _context.AnimalTypes.Where(x => x.IdAnimalType == Producto.IdAnimalType).First();

            // // Return the view with a new ProductInvent object containing the obtained data
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

        //processes the received data, verifies the validity of them and perform the corresponding operations in the database
        public async Task<IActionResult> InventoryCreate([Bind("IdInventory,Quantity,IdProduct")] Inventory inventory, 
                             Product product, AnimalType type, Brand brand, Packing packing, Movement movement)
        {

            var zero = Convert.ToInt32(_context.Constants.Where(x => x.Description == "Zero").First().Value);
            // Check if the animal type or inventory quantity is zero
            if (type is null || inventory.Quantity is null)
                {
                    return View();
                }

                if (ModelState.IsValid)
                {
                    var b1 = _context.Brands.Where(x => x.NameBrand == brand.NameBrand).FirstOrDefault();
                    var p2 = _context.Packings.Where(x => x.TypePacking == packing.TypePacking).FirstOrDefault();
                    var a = _context.AnimalTypes.Where(x => x.Type == type.Type).FirstOrDefault();
                    bool lastest = false;

                        var p1 = new Product();
                // Check if a product already exists with the same brand, packaging and type of animal
                if (b1 is not null && p2 is not null && a is not null)
                            p1 = _context.Products.Where(x => x.ProductName == product.ProductName && x.IdPacking == p2.IdPacking && x.IdBrand == b1.IdBrand && a.IdAnimalType == x.IdAnimalType).FirstOrDefault();
                       
                    if (p1 !=null || inventory.Quantity >= zero)
                    { if (b1 is null)
                        {
                            _context.Add(brand);
                            _context.SaveChanges();
                            lastest = true;
                        }
                        else
                        {
                             brand = b1;
                        }
                        if (p2 is null)
                        {
                            _context.Add(packing);
                            _context.SaveChanges();
                            lastest = true;
                        }
                            else
                        {
                            packing = p2;
                        }
                        if (a is null)
                        {
                            _context.Add(type);
                            _context.SaveChanges();
                            lastest = true;
                        }
                            else
                        {
                            type = a;
                        }
                        if (p1 is null)
                            lastest = true;
                        if (lastest)
                        {

                            product.IdBrand = brand.IdBrand;
                            product.IdPacking = packing.IdPacking;
                            product.IdAnimalType = type.IdAnimalType;
                            _context.Add(product);
                            await _context.SaveChangesAsync();
                            inventory.IdProductNavigation = product;
                            _context.Add(inventory);
                            await _context.SaveChangesAsync();
                        
                        // Configure product and inventory on the move

                        movement.IdProduct = product.IdProduct;
                            movement.IdInventary = inventory.IdInventory;
                        }
                        if (movement.Date == default) movement.Date = DateTime.Today;
                        movement.Quantity = inventory.Quantity;
                        if (!lastest)
                        {
                        // If not a new record (editing an existing inventory)
                        // Find the product p1 inventory
                        var inv = _context.Inventories.Where(x => x.IdProduct == p1.IdProduct).First();
                            movement.IdInventary = inv.IdInventory;
                            movement.IdProduct = p1.IdProduct;
                            inv.Quantity += inventory.Quantity;
                            _context.Update(inv);
                            await _context.SaveChangesAsync();
                            if (inventory.Quantity < zero) movement.MovementType = false;
                        }
                        if (inventory.Quantity >= zero) movement.MovementType = true;
                        _context.Add(movement);
                        await _context.SaveChangesAsync();
                    // Redirect to action "Inventories"
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
                var genderValues = new List<string> { "M", "F" };
                ViewBag.GenderValues = genderValues.Select(g => new SelectListItem { Text = g, Value = g });
                var castratedValues = new List<string> { "Yes", "No" };
                ViewBag.CastratedValues = castratedValues.Select(c => new SelectListItem { Text = c, Value = c });
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
            public async Task<IActionResult> PetCreate([Bind("IdPet,PetImage,PetName,Gender,BirthYear,IdAnimalType,IdAdoption,IdBreed, IdStateHealth")] Pet pet, IFormFile PetImagee, 
                List<string> Vaccine, List<DateTime> VaccinationDate, StateHealth stateHealth, string castrated)
            {
            // Check if a pet image has been provided
            if (PetImagee != null)
                {
                    var stream = new MemoryStream();
                    PetImagee.CopyTo(stream);
                    pet.PetImage = stream.ToArray();


                    if (ModelState.IsValid)
                    {
                        if(castrated == "yes") stateHealth.Castrated = true;
                        else stateHealth.Castrated = false;

                        _context.StateHealths.Add(stateHealth);
                        await _context.SaveChangesAsync();
                        //if(stateHealth.IdStateHealth == "Yes")
                        pet.IdStateHealth = stateHealth.IdStateHealth;
                    // Add pet to database and save changes
                    _context.Add(pet);

                        await _context.SaveChangesAsync();
                        var num = 0;

                    foreach (var p in Vaccine)
                        {
                            _context.Vaccines.Add(new Vaccine { TypeVaccine = Vaccine[num], VaccinationDate = VaccinationDate[num], IdPetNavigation = pet });
                            num++;
                        }

                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Pets));
                    }


                }
                else
                    ViewBag.MessageError = "Insert image ";
  
              // Prepare the necessary data for the view
                var genderValues = new List<string> { "M", "F" };
                ViewBag.GenderValues = genderValues.Select(g => new SelectListItem { Text = g, Value = g });
                var castratedValues = new List<string> { "Yes", "No" };
                ViewBag.CastratedValues = castratedValues.Select(c => new SelectListItem { Text = c, Value = c });
                ViewData["IdAnimalType"] = new SelectList(_context.AnimalTypes, "IdAnimalType", "Type", pet.IdAnimalType);
                ViewData["IdBreed"] = new SelectList(_context.Breeds, "IdBreed", "Breed1", pet.IdBreed);
                ViewData["IdStateHealth"] = new SelectList(_context.StateHealths, "IdStateHealth", "IdStateHealth", pet.IdStateHealth);
            // Devolver la vista con el modelo de datos de mascota y los datos adicionales

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
                var genderValues = new List<string> { "M", "F" };
                ViewBag.GenderValues = genderValues.Select(g => new SelectListItem { Text = g, Value = g });
                var castratedValues = new List<string> { "Yes", "No" };
                ViewBag.CastratedValues = castratedValues.Select(c => new SelectListItem { Text = c, Value = c });
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
            public async Task<IActionResult> PetEdit(int id, [Bind("IdPet,PetImage,PetName,Gender,BirthYear,IdAnimalType,IdAdoption,IdStateHealth,IdBreed")] Pet pet, 
                IFormFile? PetImagee, List<string> Vaccine, List<DateTime> VaccinationDate, StateHealth stateHealth, Breed breed, string castrated)
            {
            // Check if a new pet image has been provided

            if (PetImagee != null)
                {
                // Copy image to memory sequence
                var stream = new MemoryStream();
                    PetImagee.CopyTo(stream);
                    pet.PetImage = stream.ToArray();
                }
            // Check if the id provided matches the pet id
                if (id != pet.IdPet)
                {
                    return NotFound();
                }
            // Find the health status in the database that matches the values provided
            var sthealth = _context.StateHealths.Where(x => x.Castrated == stateHealth.Castrated && x.State == stateHealth.State).First();
                if (sthealth == null)
                {
                    if (castrated == "yes") stateHealth.Castrated = true;
                    else stateHealth.Castrated = false;
                    _context.StateHealths.Add(stateHealth);
                    await _context.SaveChangesAsync();
                // Establish the relationship between the pet and the newly created state of health

                pet.IdStateHealthNavigation = stateHealth;
                }
                else
                {
                // Establish the relationship between the pet and the existing state of health
                pet.IdStateHealthNavigation = sthealth;

                }
                if (ModelState.IsValid && pet.PetImage != null)
                {

                    try
                    {
                    // Update the pet in the database
                    _context.Update(pet);
                        await _context.SaveChangesAsync();
                    // Eliminate all existing vaccines associated with the pet
                    var Vacciene = _context.Vaccines.ToList();
                        Vacciene = Vacciene.FindAll(x => x.IdPet == pet.IdPet);
                        if (Vacciene != null)
                        {
                            _context.Vaccines.RemoveRange(Vacciene);
                            await _context.SaveChangesAsync();
                        }

                        var num = 0;
                    // Add the new vaccines associated with the pet
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
                // Redirect to action "Pets"
                return RedirectToAction(nameof(Pets));
                }
                else
                {
                    ViewBag.MessageError = "The model is not valid ";
            }
                var genderValues = new List<string> { "M", "F" };
                ViewBag.GenderValues = genderValues.Select(g => new SelectListItem { Text = g, Value = g });
                var castratedValues = new List<string> { "Yes", "No" };
                ViewBag.CastratedValues = castratedValues.Select(c => new SelectListItem { Text = c, Value = c });
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
        // Check if the set of 'Pets' entities is null
        public async Task<IActionResult> PetDelete(int id)
            {
                if (_context.Pets == null)
                {
                    return Problem("Entity set 'ProyectPetsContext.Pets'  is null.");
                }
            // Find the pet with the specified id
            var pet = await _context.Pets.FindAsync(id);

                if (pet != null)
                {
                // Remove the pet from the context
                _context.Pets.Remove(pet);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Pets));
            }

            private bool PetExists(int id)
            {
            // Check if a pet exists with the id specified in the context
            return (_context.Pets?.Any(e => e.IdPet == id)).GetValueOrDefault();
            }
        }
    }


