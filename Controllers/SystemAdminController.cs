using FurryFriendFinder.Models.Data;
using FurryFriendFinder.Models.LogicModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FurryFriendFinder.Controllers
{
    
    [Authorize(Roles = nameof(Rol.SystemAdmin))]
    public class SystemAdminController : Controller
    {
        private readonly FurryFriendFinderDbContext _context;
        public SystemAdminController(FurryFriendFinderDbContext context)
        {
            _context = context;
        }

        // GET: Users
        //[Authorize(Roles = "RUsers")]
        public async Task<IActionResult> User()
        {
            var proyectContext = _context.Users.Include(p => p.IdRhNavigation);
            return View(await proyectContext.ToListAsync());
        }

        public async Task<IActionResult> DetailsUser(int id)
        {

            var proyectContext = await _context.Users.FindAsync(id);
            return View(proyectContext);
        }


        // GET: Users/Create
        [AllowAnonymous]
        public IActionResult CreateUser()
        {
            ViewBag.Id_Role = _context.Roles.ToList();
            ViewBag.Rhs = new SelectList(_context.Rhs, "IdRh", "RhType");
            return View();
        }

        public int GetRole()
        {
            var Role = Convert.ToInt32(HttpContext.User.Identities.First().Actor);
            return (Role);
        }



        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.s
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("IdUser,Name,State,BirthDate,Rh,IdRole,Phones")] User user, List<long> Phones, List<string> Address, List<string> Email, List<string> Password, List<int> Role)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                foreach (var p in Phones)
                {
                    _context.Phones.Add(new Phone { Phone1 = p, IdUserNavigation = user });
                }
                foreach (var p in Address)
                {
                    _context.Addresses.Add(new Address { Address1 = p, IdUserNavigation = user });
                }
                var num = 0;
                foreach (var p in Password)
                {
                    _context.Accesses.Add(new Access { Email = Email[num], Password = Encrypt.GetSHA256(Password[num]), IdRole = Role[num], IdUserNavigation = user });
                    num++;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> EditUser(int? id)
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
            ViewBag.Rhs = new SelectList(_context.Rhs, "IdRh", "RhType");
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, [Bind("IdUser,Name,State,BirthDate,Rh,IdRole")] User user, List<int> Phones, List<string> Address, List<string> Email, List<string> Password1, List<string> Password, List<int> role)
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

            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> DeleteUser(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                //.Include(u => u.IdRoleNavigation)
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ProyectContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
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
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.IdUser == id)).GetValueOrDefault();
        }
    }
}
