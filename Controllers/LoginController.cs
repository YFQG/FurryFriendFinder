using FurryFriendFinder.Models.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using FurryFriendFinder.Models.ViewModels;
using FurryFriendFinder.Models.LogicModels;
using Microsoft.EntityFrameworkCore;

namespace FurryFriendFinder.Controllers
{
    public enum Rol // Add a enum to set the roles id
    {
        n, SystemAdmin, CenterAdmin, PetSitter, Client
    }
    public class LoginController : Controller
    {
        private readonly FurryFriendFinderDbContext _context;

        public LoginController(FurryFriendFinderDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Access _access) //Method that recive mapped object from the access class
        {
            var user = UserValidation(_access.Email, _access.Password); 
            if (user != null) 
            {
                var info = _context.Users.Where(u => u.IdUser == user.IdUser).FirstOrDefault(); //Search for the user that match with email and password
                var claims = new List<Claim> 
                {
                    new Claim(ClaimTypes.Name, user.IdUser.ToString()), //Create
                    new Claim(ClaimTypes.Email, user.Email)
                };
                var rol = (from r in _context.Roles
                              where r.IdRole == user.IdRole
                              select r.RoleType).First();

                claims.Add(new Claim(ClaimTypes.Role, rol));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)); //Crear la cookie en la sesion de logeo
                UserRol.User = info;
                return user.IdRole switch
                {
                    (int)Rol.Client => RedirectToAction("Index", "Client"),
                    (int)Rol.CenterAdmin => RedirectToAction("Index", "CenterAdmin"),
                    (int)Rol.SystemAdmin => RedirectToAction("User", "SystemAdmin"),
                    (int)Rol.PetSitter => RedirectToAction("Inventories", "PetSitter"),
                    _ => RedirectToAction("Index", "Home"),
                };
            }
            else
            {
                return View();
            }
        }

        public IActionResult ClientRegister()
        {
            ViewBag.Rhs = new SelectList(_context.Rhs, "IdRh", "RhType");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClientRegister([Bind("IdUser,Name,BirthDate,IdRh,IdRole,Phones")] User user, List<long> Phones, List<string> Address, List<string> Email, List<string> Password, List<string> RepeatPassword)
        {
            if (ModelState.IsValid && Email.Count > 0)
            {
                bool EmailExist = false;
                bool IncorrectPass = false;
                foreach (var mail in Email)
                {
                    if (_context.Accesses.Where(x => x.Email == mail).FirstOrDefault() != null)
                        EmailExist = true;
                }
                int num = 0;
                if (!EmailExist)
                {
                    for (int i = 1; i < Email.Count; i++)
                    {
                        for (int i1 = 0; i1 < Email.Count; i1++)
                        {
                            if (Email[i1] == Email[i] && i1 != i)
                                EmailExist = true;
                            if (EmailExist)
                                break;
                        }

                        if (EmailExist)
                            break;
                    }
                }
                num = 0;
                foreach (var Pass in Password)
                {
                    if (Pass != RepeatPassword[num])
                        IncorrectPass = true;
                    num++;
                }
                if (!EmailExist)
                {
                    if (!IncorrectPass)
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
                        num = 0;
                        foreach (var p in Password)
                        {
                            _context.Accesses.Add(new Access { Email = Email[num], Password = Encrypt.GetSHA256(Password[num]), IdRole = (int)Rol.Client, IdUserNavigation = user });
                            num++;
                        }
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Login));
                    }
                    ViewBag.MessageError = _context.Constants.Where(x => x.Description == "MessageError8").First().Value;
                    return View(user);
                }
                ViewBag.MessageError = _context.Constants.Where(x => x.Description == "MessageError7").First().Value;
                return View(user);

            }

            ViewBag.MessageError = _context.Constants.Where(x => x.Description == "MessageError5").First().Value;
            // ViewData["IdRole"] = new SelectList(_context.Roles, "IdRole", "RoleType", user.IdRole);
            return View(user);
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }

        public Access UserValidation(string email, string password) //returns the occess object that match with the email and password of the form
        {
            return _context.Accesses.Where(u => u.Email == email && u.Password == Encrypt.GetSHA256(password)).FirstOrDefault();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromForm] string email)
        {
            string password = _context.Accesses
                    .Where(a => a.Email == email)
                    .Select(a => a.Password)
                    .FirstOrDefault()?.ToString();
            if (password == null)
            {
                TempData["ErrorMessage"] = "El correo electrónico ingresado no existe.";
                return RedirectToAction("ForgotPassword", "Login");
            }
            var emailSender = new FFFEmail();
            await emailSender.SendEmailAsync(email, "Cambio de contraseña", "Hola, con esta encriptacion puedes ingresarla en el formulario que se te ha desplegado despues" +
                $" de enviar el correo.\n Encriptacion: {password}");

            return RedirectToAction("PasswordRecovery");
        }

        public IActionResult PasswordRecovery()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordRecovery(string encrypt, string password, string confirmation)
        {
            if (password != confirmation)
            {
                TempData["ErrorMessage"] = "Las contraseñas no coinciden.";
                return RedirectToAction("PasswordRecovery");
            }

            Access access = _context.Accesses.FirstOrDefault(a => a.Password == encrypt);

            if (access != null)
            {
                access.Password = Encrypt.GetSHA256(password);
                _context.Update(access);
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["ErrorMessage"] = "El codigo de encriptacion no coindide.";
                return RedirectToAction("PasswordRecovery");
            }

            return RedirectToAction("Login");
        }
    }
}
