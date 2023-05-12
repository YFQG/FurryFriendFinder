using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FurryFriendFinder.Models.Data;
using FurryFriendFinder.Models.LogicModels;
using FurryFriendFinder.Models.ViewModels;
namespace FurryFriendFinder.Controllers
{

        [Authorize(Roles = nameof(Rol.Client))]
        public class ClientController : Controller
        {

            private readonly FurryFriendFinderDbContext _context;

            public ClientController(FurryFriendFinderDbContext context)
            {
                _context = context;
                if (Id == 0)
                {
                    Index();
                }
            }

            public int Id
            {
                get
                {
                    if (UserRol.User != null)
                        ViewBag.id = UserRol.User.IdUser;
                    else
                        return 0;
                    return ViewBag.id;
                }

            }



            public async Task<IActionResult> Index()
            {
                if (Id == 0)
                {
                    return RedirectToAction("TryLogIn", "Login");
                }

            var proyectContext = _context.Pets.Include(x => x.Adoptions).Include(x => x.Vaccines).Include(x =>x.IdAnimalTypeNavigation).Include(x => x.IdBreedNavigation).Include(x => x.IdStateHealthNavigation).Include(x => x.Vaccines).ToList();
            var p = new List<Pet>();
            foreach(var i in proyectContext)
            {
                if (i.Adoptions.Count == 0)
                    p.Add(i);
            }

                return View(p);
            }
            public async Task<IActionResult> DetailsPet(int? id)
            {
                if (id == null || _context.Pets == null)
                {
                    return NotFound();
                }

            var adopt = _context.Adoptions.Include(p=> p.IdAdoptionDateNavigation).Where(x => x.IdPet == id).ToList();
                var Pet = await _context.Pets
                .Include(p => p.Vaccines)
                .Include(p => p.IdAnimalTypeNavigation)
                .Include(p => p.IdStateHealthNavigation)
                .Include(p => p.IdBreedNavigation)

                    .FirstOrDefaultAsync(m => m.IdPet == id);

                if (Pet == null)
                {
                    return NotFound();
                }
            Pet.Adoptions = adopt;
                return View(Pet);
            }

            // GET: Publications/Details/5
            public async Task<IActionResult> PetsLostDetails(int? id)
            {
                if (id == null || _context.Publications == null)
                {
                    return NotFound();
                }

                var publication = await _context.Publications
                .Include(p => p.IdUserNavigation)
                    .FirstOrDefaultAsync(m => m.IdPublication == id);


                if (publication == null)
                {
                    return NotFound();
                }

                return View(publication);
            }
            public IActionResult ClientDetails(int id)
            {

                var proyectContext = _context.Users.Include(id => id.IdRhNavigation).Where(x => x.IdUser == id).First();
                //.Include(u => u.IdRoleNavigation);
                return View(proyectContext);
            }

            public async Task<IActionResult> ClientEdit(int? id)
            {
                if (id == null || _context.Users == null)
                {
                    return NotFound();
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                ViewData["IdRh"] = new SelectList(_context.Rhs, "IdRh", "RhType", user.IdRh);
                return View(user);
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ClientEdit(int id, [Bind("IdUser,Name,State,BirthDate,IdRh,IdRole")] User user, List<long> Phones, List<string> Address, List<string> Email, List<string> Password1, List<string> Password, List<int> role)
            {
                if (id != user.IdUser)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                        var Phone = _context.Phones.ToList();
                        Phone = Phone.FindAll(x => x.IdUser == user.IdUser);
                        if (Phone != null)
                        {
                            _context.Phones.RemoveRange(Phone);
                            await _context.SaveChangesAsync();
                        }
                        var Addres = _context.Addresses.ToList();
                        Addres = Addres.FindAll(x => x.IdUser == user.IdUser);
                        if (Addres != null)
                        {
                            _context.Addresses.RemoveRange(Addres);
                            await _context.SaveChangesAsync();
                        }
                        var Access = _context.Accesses.ToList();
                        Access = Access.FindAll(x => x.IdUser == user.IdUser);
                        if (Access != null)
                        {
                            _context.Accesses.RemoveRange(Access);
                            await _context.SaveChangesAsync();
                        }

                        foreach (var p in Phones)
                        {
                            _context.Phones.Add(new Phone { Phone1 = p, IdUserNavigation = user });
                        }
                        foreach (var p in Address)
                        {
                            _context.Addresses.Add(new Address { Address1 = p, IdUserNavigation = user });
                        }
                        var num = 0;
                        var num2 = 0;
                        foreach (var p in Email)
                        {
                            if (Password1[num] != null)
                                _context.Accesses.Add(new Access { Email = Email[num], Password = Password1[num], IdRole = role[num], IdUserNavigation = user });
                            else
                            {
                                _context.Accesses.Add(new Access { Email = Email[num], Password = Encrypt.GetSHA256(Password[num2]), IdRole = role[num], IdUserNavigation = user });
                                num2++;
                            }
                            num++;
                        }
                        _context.SaveChanges();
                        UserRol.User = user;
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UserExists(user.IdUser))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                //  ViewData["IdRole"] = new SelectList(_context.Roles, "IdRole", "RoleType", user.IdRole);
                return View(user);
            }
            public async Task<IActionResult> ClientDelete(int? id)
            {
                if (id == null || _context.Users == null)
                {
                    return NotFound();
                }

                var user = await _context.Users.Include(u => u.IdRhNavigation)
                    .FirstOrDefaultAsync(m => m.IdUser == id);
                if (user == null)
                {
                    return NotFound();
                }

                return View(user);
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ClientDelete(int id)
            {
                if (_context.Users == null)
                {
                    return Problem("Entity set 'ProyectContext.Users'  is null.");
                }
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    user.State = false;
                    _context.Update(user);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("LogOut", "Login");
            }
            private bool UserExists(int id)
            {
                return (_context.Users?.Any(e => e.IdUser == id)).GetValueOrDefault();
            }
            // GET: Publications/Create
            public IActionResult PetsLostCreate()
            {
                Publication publication = new()
                {
                    IdUser = Id
                };
                return View();
            }

            // POST: Publications/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> PetsLostCreate([Bind("IdPublication,Desription,Date,Image,IdUser")] Publication publication, IFormFile Imagee)
            {

                var stream = new MemoryStream();
                Imagee.CopyTo(stream);
                publication.Image = stream.ToArray();
                if (ModelState.IsValid)
                {
                    publication.IdUserNavigation = await _context.Users.FindAsync(publication.IdUser);
                    _context.Add(publication);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(PetsLost));
                }
                ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "Name", publication.IdUser);
                return View(publication);
            }
            public async Task<IActionResult> PetsLostEdit(int? id)
            {
                if (id == null || _context.Publications == null)
                {
                    return NotFound();
                }

                var publication = await _context.Publications.FindAsync(id);
                if (publication == null)
                {
                    return NotFound();
                }
                if (publication.IdUser != Id)
                {
                    ViewBag.MessageError = _context.Constants.Where(x => "MessageError6" == x.Description).FirstOrDefault().Value;
                    return RedirectToAction(nameof(PetsLost));
                }
                ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "Name", publication.IdUser);
                return View(publication);
            }

            // POST: Publications/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> PetsLostEdit(int id, [Bind("IdPublication,Desription,Date,Image,IdUser")] Publication publication)
            {
                if (id != publication.IdPublication)
                {
                    return NotFound();
                }
                if (publication.IdUser != Id)
                {
                    ViewBag.MessageError = _context.Constants.Where(x => "MessageError6" == x.Description).FirstOrDefault().Value;
                    return RedirectToAction(nameof(PetsLost));
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(publication);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PublicationExists(publication.IdPublication))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(PetsLost));
                }
                ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "Name", publication.IdUser);
                return View(publication);
            }

            public async Task<IActionResult> PetsLostDelete(int? id)
            {
                if (id == null || _context.Publications == null)
                {
                    return NotFound();
                }

                var publication = await _context.Publications
                    .Include(p => p.IdUserNavigation)
                    .FirstOrDefaultAsync(m => m.IdPublication == id);
                if (publication == null)
                {
                    return NotFound();
                }
                if (publication.IdUser != Id)
                {
                    ViewBag.MessageError = _context.Constants.Where(x => "MessageError6" == x.Description).FirstOrDefault().Value;
                    return RedirectToAction(nameof(PetsLost));
                }
                return View(publication);
            }

            // POST: Publications/Delete/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> PetsLostDelete(int id)
            {
                if (_context.Publications == null)
                {
                    return Problem("Entity set 'ProyectContext.Publications'  is null.");
                }
                var comments = await _context.Comments.ToListAsync();
                var comments1 = comments.FindAll(x => x.IdPublication == id);
                foreach (var comment in comments1)
                {
                    _context.Comments.Remove(comment);
                }
                var publication = await _context.Publications.FindAsync(id);
                if (publication != null)
                {
                    if (publication.IdUser != Id)
                    {
                        ViewBag.MessageError = _context.Constants.Where(x => "MessageError6" == x.Description).FirstOrDefault().Value;
                        return RedirectToAction(nameof(PetsLost));
                    }
                    _context.Publications.Remove(publication);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(PetsLost));
            }

            private bool PublicationExists(int id)
            {
                return (_context.Publications?.Any(e => e.IdPublication == id)).GetValueOrDefault();
            }

            public async Task<IActionResult> PetsLostMess(int id)
            {
                if (id != 0)
                {
                    var publi = await _context.Publications.FindAsync(id);
                    if (publi == null)
                        return RedirectToAction(nameof(PetsLost));
                    if (publi.Description != null)
                        ViewBag.Description = publi.Description;
                    ViewBag.Date = publi.Date;
                    ViewBag.Image = publi.Image;
                    ViewBag.IdUserNavigation = publi.IdUserNavigation;
                    ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "Name");

                    Comment comment = new()
                    {
                        IdUser = Id,
                        IdPublication = id
                    };
                    return View(comment);
                }

                return RedirectToAction(nameof(PetsLost));
                //

            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> PetsLostMess(int id, [Bind("Comment1,IdUser,IdPublication")] Comment comment)
            {
                comment.PublicationDate = DateTime.Now;
                if (ModelState.IsValid)
                {
                    _context.Add(comment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(PetsLost));
                }
                var publi = await _context.Publications.FindAsync(id);
                if (publi == null)
                    return RedirectToAction(nameof(PetsLost));
                ViewBag.Description = publi.Description;
                ViewBag.Date = publi.Date;
                ViewBag.Image = publi.Image;
                ViewBag.IdUserNavigation = publi.IdUserNavigation;
                ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "Name");
                comment = new()
                {
                    IdPublication = id
                };

                return View(comment);
            }
            public async Task<IActionResult> PetsLost()
            {
                var proyectContext = _context.Publications.Include(p => p.IdUserNavigation);
            var comments = _context.Comments.Include(p => p.IdUserNavigation);
                return View(new PubliComment(await proyectContext.ToListAsync(), await comments.ToListAsync()));
            }

            public async Task<IActionResult> Appointments()
            {

                var proyectContext = _context.AppointmentUsers.Where(x => x.IdUser == Id);
                return View(await proyectContext.ToListAsync());
            }

            // GET: AppointmentUsers/Create
            public IActionResult CreateAppointment(int id)
            {

                ViewData["IdPet"] = new SelectList(_context.Pets, "IdPet", "PetName", id);
                AppointDate app = new(new Appointment(), new AppointmentUser() { IdUser = Id });
                return View();
            }

            // POST: AppointmentUsers/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> CreateAppointment(AppointmentUser appointmentUser, Appointment appointment)
            {

                //declaring constants
                var constants = _context.Constants.ToList();
                var Space = constants.Find(x => x.Description == "Space").Value;
                var TwoPoints = constants.Find(x => x.Description == "TwoPoints").Value;
                var HoursConsult = constants.Find(x => x.Description == "HoursConsult").Value;
                var ForwardSlash = constants.Find(x => x.Description == "ForwardSlash").Value;
                var Am = constants.Find(x => x.Description == "Am").Value;
                var Pm = constants.Find(x => x.Description == "Pm").Value;
                var Zero = constants.Find(x => x.Description == "Zero").Value;
                var One = constants.Find(x => x.Description == "One").Value;
                var Two = constants.Find(x => x.Description == "Two").Value;
                var Twelve = constants.Find(x => x.Description == "Twelve").Value;
                var Three = constants.Find(x => x.Description == "Three").Value;
                var HourI = constants.Find(x => x.Description == "HourI").Value;
                var HourF = constants.Find(x => x.Description == "HourF").Value;


                var MessageE1 = constants.Find(x => x.Description == "MessageError1").Value;
                var MessageE2 = constants.Find(x => x.Description == "MessageError2").Value;
                var MessageE3 = constants.Find(x => x.Description == "MessageError3").Value;
                var MessageE4 = constants.Find(x => x.Description == "MessageError4").Value;
                var MessageE5 = constants.Find(x => x.Description == "MessageError5").Value;


                if (appointment.Date != null)
                {
                    if (appointment.Date > DateTime.Now)
                    {

                        //sum of consultation hours that the appointment takes
                        var h = Convert.ToString(appointment.Date);
                        var h1 = h.Split(Space);
                        var h2 = h1[Convert.ToInt32(One)].Split(TwoPoints);
                        int h3 = Convert.ToInt32(h2[Convert.ToInt32(Zero)]) + Convert.ToInt32(HoursConsult);
                        int h4 = Convert.ToInt32(h2[Convert.ToInt32(Zero)]) - Convert.ToInt32(HoursConsult);

                        int th3 = h3;
                        int th1 = Convert.ToInt32(h2[Convert.ToInt32(Zero)]);
                        var tst = h1[2];
                        if (h1[Convert.ToInt32(Two)].Equals(Pm))
                        {
                            th3 += Convert.ToInt32(Twelve);
                            th1 += Convert.ToInt32(Twelve);
                            if (th3.Equals(Convert.ToInt32(HourF)) && Convert.ToInt32(h2[Convert.ToInt32(One)]) > 0)
                                th3++;
                        }

                        if (th1 >= Convert.ToInt32(HourI) && th3 <= Convert.ToInt32(HourF))
                        {
                            //Hacemos los calculos para validar los tiempos
                            var h13 = new string[Convert.ToInt32(Three)];


                            h1.CopyTo(h13, Convert.ToInt32(Zero));

                            if (h3 > Convert.ToInt32(Twelve))
                            {
                                h3 -= Convert.ToInt32(Twelve);
                                if (h13[Convert.ToInt32(Two)] == Am)
                                {
                                    h13[Convert.ToInt32(Two)] = Pm;
                                }
                            }

                            var TimeI = "";
                            var TimeF = "";
                            if (h4 <= Convert.ToInt32(Zero))
                            {
                                h4 += Convert.ToInt32(Twelve);
                                if (h1[2] == Pm)
                                {
                                    h1[2] = Am;

                                }
                            }
                            TimeI = h1[Convert.ToInt32(Zero)] + Space + h4.ToString() + TwoPoints + h2[Convert.ToInt32(One)] + TwoPoints + h2[Convert.ToInt32(Two)] + Space + h1[Convert.ToInt32(Two)];
                            TimeF = h13[Convert.ToInt32(Zero)] + Space + h3.ToString() + TwoPoints + h2[Convert.ToInt32(One)] + TwoPoints + h2[Convert.ToInt32(Two)] + Space + h13[Convert.ToInt32(Two)];


                            var Dates = _context.Appointments.Where(x => x.Date >= Convert.ToDateTime(TimeI) && x.Date <= Convert.ToDateTime(TimeF)).FirstOrDefault();
                            if (Dates == null)
                            {
                                if (ModelState.IsValid)
                                {
                                    _context.Appointments.Add(appointment);
                                    await _context.SaveChangesAsync();
                                    appointmentUser.IdAppointment = appointment.IdAppointment;
                                    _context.Add(appointmentUser);
                                    await _context.SaveChangesAsync();
                                    return RedirectToAction(nameof(Appointments));
                                }
                                else
                                {

                                    ViewBag.MessageError = MessageE5;
                                }
                            }
                            else
                            {
                                ViewBag.MessageError = MessageE1;
                            }

                        }
                        else
                        {
                            ViewBag.MessageError = MessageE2;
                        }
                    }
                    else
                    {
                        ViewBag.MessageError = MessageE3;

                    }
                }
                else
                {
                    ViewBag.MessageError = MessageE4;

                }
                ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "Name", appointmentUser.IdUser);
                ViewData["IdPet"] = new SelectList(_context.Pets, "IdPet", "PetName", appointmentUser.IdPet);
                return View(new AppointDate(appointment, appointmentUser));
            }

            // GET: AppointmentUsers/Edit/5
            public async Task<IActionResult> EditAppointment(int? id)
            {
                if (id == null || _context.AppointmentUsers == null)
                {
                    return NotFound();
                }

                var appointmentUser = await _context.AppointmentUsers.FindAsync(id);
                if (appointmentUser == null)
                {
                    return NotFound();
                }
                if (appointmentUser.IdUser != Id)
                {
                    ViewBag.MessageError = _context.Constants.Where(x => "MessageError6" == x.Description).FirstOrDefault().Value;
                    return RedirectToAction(nameof(PetsLost));
                }
                ViewData["IdPet"] = new SelectList(_context.Pets, "IdPet", "PetName", appointmentUser.IdAppointment);
                ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "Name", appointmentUser.IdUser);

                Appointment appoint = await _context.Appointments.FindAsync(appointmentUser.IdAppointment);
                var AppointDate = new AppointDate(appoint, appointmentUser);
                return View(AppointDate);
            }

            // POST: AppointmentUsers/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> EditAppointment(int id, [Bind("IdAppointmentUser,IdUser,IdAppointment,IdPet")] AppointmentUser appointmentUser, Appointment appointment)
            {

                if (appointmentUser.IdUser != Id)
                {
                    ViewBag.MessageError = _context.Constants.Where(x => "MessageError6" == x.Description).FirstOrDefault().Value;
                    return RedirectToAction(nameof(PetsLost));
                }

                //declaring constants
                var constants = _context.Constants.ToList();
                var Space = constants.Find(x => x.Description == "Space").Value;
                var TwoPoints = constants.Find(x => x.Description == "TwoPoints").Value;
                var HoursConsult = constants.Find(x => x.Description == "HoursConsult").Value;
                var ForwardSlash = constants.Find(x => x.Description == "ForwardSlash").Value;
                var Am = constants.Find(x => x.Description == "Am").Value;
                var Pm = constants.Find(x => x.Description == "Pm").Value;
                var Zero = constants.Find(x => x.Description == "Zero").Value;
                var One = constants.Find(x => x.Description == "One").Value;
                var Two = constants.Find(x => x.Description == "Two").Value;
                var Twelve = constants.Find(x => x.Description == "Twelve").Value;
                var Three = constants.Find(x => x.Description == "Three").Value;
                var HourI = constants.Find(x => x.Description == "HourI").Value;
                var HourF = constants.Find(x => x.Description == "HourF").Value;

                var MessageE1 = constants.Find(x => x.Description == "MessageError1").Value;
                var MessageE2 = constants.Find(x => x.Description == "MessageError2").Value;
                var MessageE3 = constants.Find(x => x.Description == "MessageError3").Value;
                var MessageE4 = constants.Find(x => x.Description == "MessageError4").Value;


                if (appointment.Date != null)
                {
                    if (appointment.Date > DateTime.Now)
                    {

                        //sum of consultation hours that the appointment takes
                        var h = Convert.ToString(appointment.Date);
                        var h1 = h.Split(Space);
                        var h2 = h1[Convert.ToInt32(One)].Split(TwoPoints);
                        int h3 = Convert.ToInt32(h2[Convert.ToInt32(Zero)]) + Convert.ToInt32(HoursConsult);
                        int h4 = Convert.ToInt32(h2[Convert.ToInt32(Zero)]) - Convert.ToInt32(HoursConsult);

                        int th3 = h3;
                        int th1 = Convert.ToInt32(h2[Convert.ToInt32(Zero)]);
                        var tst = h1[2];
                        if (h1[Convert.ToInt32(Two)].Equals(Pm))
                        {
                            th3 += Convert.ToInt32(Twelve);
                            th1 += Convert.ToInt32(Twelve);
                            if (th3.Equals(Convert.ToInt32(HourF)) && Convert.ToInt32(h2[Convert.ToInt32(One)]) > 0)
                                th3++;
                        }

                        if (th1 >= Convert.ToInt32(HourI) && th3 <= Convert.ToInt32(HourF))
                        {
                            //Hacemos los calculos para validar los tiempos
                            var h13 = new string[Convert.ToInt32(Three)];


                            h1.CopyTo(h13, Convert.ToInt32(Zero));

                            if (h3 > Convert.ToInt32(Twelve))
                            {
                                h3 -= Convert.ToInt32(Twelve);
                                if (h13[Convert.ToInt32(Two)] == Am)
                                {
                                    h13[Convert.ToInt32(Two)] = Pm;
                                }
                            }

                            var TimeI = "";
                            var TimeF = "";
                            if (h4 <= Convert.ToInt32(Zero))
                            {
                                h4 += Convert.ToInt32(Twelve);
                                if (h1[2] == Pm)
                                {
                                    h1[2] = Am;

                                }
                            }
                            TimeI = h1[Convert.ToInt32(Zero)] + Space + h4.ToString() + TwoPoints + h2[Convert.ToInt32(One)] + TwoPoints + h2[Convert.ToInt32(Two)] + Space + h1[Convert.ToInt32(Two)];
                            TimeF = h13[Convert.ToInt32(Zero)] + Space + h3.ToString() + TwoPoints + h2[Convert.ToInt32(One)] + TwoPoints + h2[Convert.ToInt32(Two)] + Space + h13[Convert.ToInt32(Two)];


                            var Dates = _context.Appointments.Where(x => x.Date >= Convert.ToDateTime(TimeI) && x.Date <= Convert.ToDateTime(TimeF)).ToList();
                            if (Dates.Count <= Convert.ToInt32(One))
                            {
                                if (Dates.First().IdAppointment == appointment.IdAppointment || Dates.Count == Convert.ToInt32(Zero))
                                    Dates = null;
                            }
                            if (Dates == null)
                            {
                                _context.Update(appointment);
                                await _context.SaveChangesAsync();
                                appointmentUser.IdAppointment = appointment.IdAppointment;
                                if (ModelState.IsValid)
                                {
                                    _context.Update(appointmentUser);
                                    await _context.SaveChangesAsync();
                                    return RedirectToAction(nameof(Index));
                                }
                            }
                            else
                            {
                                ViewBag.MessageError = MessageE1;
                            }

                        }
                        else
                        {
                            ViewBag.MessageError = MessageE2;
                        }
                    }
                    else
                    {
                        ViewBag.MessageError = MessageE3;

                    }
                }
                else
                {
                    ViewBag.MessageError = MessageE4;

                }
                ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "Name", appointmentUser.IdUser);
                ViewData["IdPet"] = new SelectList(_context.Pets, "IdPet", "PetName", appointmentUser.IdPet);
                return View(new AppointDate(appointment, appointmentUser));


            }

            // GET: AppointmentUsers/Delete/5
            public async Task<IActionResult> DeleteAppointment(int? id)
            {
                if (id == null || _context.AppointmentUsers == null)
                {
                    return NotFound();
                }

                var appointmentUser = await _context.AppointmentUsers.FindAsync(id);
                if (appointmentUser == null)
                {
                    return NotFound();
                }

                if (appointmentUser.IdUser != Id)
                {
                    ViewBag.MessageError = _context.Constants.Where(x => "MessageError6" == x.Description).FirstOrDefault().Value;
                    return RedirectToAction(nameof(PetsLost));
                }
                ViewData["IdPet"] = new SelectList(_context.Pets, "IdPet", "PetName", appointmentUser.IdAppointment);
                ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "Name", appointmentUser.IdUser);

                _context.Publications.Include(p => p.IdUserNavigation);


                Appointment appoint = _context.Appointments.Where(x => x.IdAppointment == appointmentUser.IdAppointment).First();
                var pet = _context.Pets.Where(x => x.IdPet == appointmentUser.IdPet).First();
                var user = _context.Users.Where(x => x.IdUser == appointmentUser.IdUser).First();
                if (appoint == null)
                {
                    return NotFound();
                }
                var AppointDate = new AppointDate(appoint, appointmentUser, pet, user);


                return View(AppointDate);
            }

            // POST: AppointmentUsers/Delete/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteAppointment(int id)
            {
                if (_context.AppointmentUsers == null)
                {
                    return Problem("Entity set 'ProyectContext.AppointmentUsers'  is null.");
                }

                var appointmentUser = await _context.AppointmentUsers.FindAsync(id);

                if (appointmentUser == null)
                {
                    return NotFound();
                }

                if (appointmentUser.IdUser != Id)
                {
                    ViewBag.MessageError = _context.Constants.Where(x => "MessageError6" == x.Description).FirstOrDefault().Value;
                    return RedirectToAction(nameof(PetsLost));
                }
                Appointment appoint = await _context.Appointments.FindAsync(appointmentUser.IdAppointment);

                if (appointmentUser != null && appoint != null)
                {
                    _context.Appointments.Remove(appoint);
                    _context.AppointmentUsers.Remove(appointmentUser);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool AppointmentUserExists(int id)
            {
                return (_context.AppointmentUsers?.Any(e => e.IdAppointmentUser == id)).GetValueOrDefault();
            }
        }
    }


