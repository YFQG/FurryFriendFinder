using FurryFriendFinder.Models.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using FurryFriendFinder.Models.ViewModels;
using FurryFriendFinder.Models.LogicModels;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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
            //calls the UserValidation method to validate the user's email and password. If a valid user is found,
            //the method returns a User object; otherwise, it returns null
            var user = UserValidation(_access.Email, _access.Password); 
            if (user != null) 
            {
                // retrieves the user's information from the database based on the user's ID.
                var info = _context.Users.Where(u => u.IdUser == user.IdUser).FirstOrDefault(); 
                var claims = new List<Claim> //creates a list of claims for the authenticated user.
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
                return user.IdRole switch //returns a redirect to the appropriate page based on the user's role.
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

        //defines an action method that returns a view to display the client registration form.
        public IActionResult ClientRegister()
        {
            //retrieves a list of blood types from the database and adds it to the view bag
            //to be used in the client registration form.
            ViewBag.Rhs = new SelectList(_context.Rhs, "IdRh", "RhType");
            return View();
        }

        //defines an action method that accepts a User object and several lists of values as parameters and attempts to register a new client.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClientRegister([Bind("IdUser,Name,BirthDate,IdRh,IdRole,Phones")] User user
            , List<long> Phones, List<string> Address, List<string> Email, List<string> Password, List<string> RepeatPassword)
        {
            if (ModelState.IsValid && Email.Count > 0) //checks if the model state is valid and if there is at least one email address provided.
            {
                bool EmailExist = false;
                bool IncorrectPass = false;
                foreach (var mail in Email)
                {
                    //checks if the current email address already exists in the Accesses table of the database
                    if (_context.Accesses.Where(x => x.Email == mail).FirstOrDefault() != null)
                        EmailExist = true;
                }
                int num = 0;
                if (!EmailExist)
                {
                    for (int i = (int)Rol.SystemAdmin; i < Email.Count; i++)
                    {
                        for (int i1 = (int)Rol.n; i1 < Email.Count; i1++)
                        {
                            //checks if the current email address is equal to any other email address in the list
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
                    //checks if the current password does not match the corresponding repeated password.
                    if (Pass != RepeatPassword[num])
                        IncorrectPass = true;
                    num++;
                }
                if (!EmailExist) //indicate that none of the email addresses already exist in the database.
                {
                    if (!IncorrectPass) //Indicate that all the passwords and repeated passwords match.
                    {
                        //Save user, all the phones, the adresses and accesses
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

            ViewBag.MessageError = _context.Constants.Where(x => x.Description == "MessageError5").First().Value;;
            return View(user);
        }

        public async Task<IActionResult> LogOut() //Close the cookie and return to Login
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }

        public Access UserValidation(string email, string password) //returns the occess object that match with the email and password of the form
        {
            return _context.Accesses.Where(u => u.Email == email && u.Password == Encrypt.GetSHA256(password)).FirstOrDefault();
        }

        public IActionResult ForgotPassword() // Show the Forgot password view
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromForm] string email) //Take the email from the form
        {
            //Search the password realted with this email
            string password = _context.Accesses
                    .Where(a => a.Email == email)
                    .Select(a => a.Password)
                    .FirstOrDefault()?.ToString();
            if (password == null) //if the query didn´t find a password realted with this email
            {
                TempData["ErrorMessage"] = "El correo electrónico ingresado no existe.";
                return RedirectToAction("ForgotPassword", "Login");
            }
            //Call an async method to send the encrypted password to the password recovery
            var emailSender = new FFFEmail();
            await emailSender.SendEmailAsync(email, "Cambio de contraseña", "Hola, con esta encriptacion puedes ingresarla en el formulario que se te ha desplegado despues" +
                $" de enviar el correo.\n Encriptacion: {password}");

            return RedirectToAction("PasswordRecovery"); //Send the user to the password recovery view
        }

        public IActionResult PasswordRecovery() //Show password recovery view
        {
            return View();
        }

        //Take the encrypt password sended to the email and the new password with the confirmation of it
        [HttpPost]
        public async Task<IActionResult> PasswordRecovery(string encrypt, string password, string confirmation) 
        {
            if (password != confirmation) // if the new password didn´t match with the cofirmation go back to the view
            {
                TempData["ErrorMessage"] = "Las contraseñas no coinciden.";
                return RedirectToAction("PasswordRecovery");
            }

            Access access = _context.Accesses.FirstOrDefault(a => a.Password == encrypt); //seacrh for the access information that match with the encripted password

            if (access != null) // If that encrypted password exist, encrypt the new password and update this information
            {
                access.Password = Encrypt.GetSHA256(password);
                _context.Update(access);
                await _context.SaveChangesAsync();
            }
            else //if the query is null go back to the view and fill again the inputs
            {
                TempData["ErrorMessage"] = "El codigo de encriptacion no coindide.";
                return RedirectToAction("PasswordRecovery");
            }

            return RedirectToAction("Login"); //Go back to login to fill the new information
        }
    }
}
