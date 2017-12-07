using LoginAndRegistrationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LoginAndRegistrationSystem.Controllers
{
    public class UserController : Controller
    {
        //Registration
        // GET: User
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        //Registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "isEmailVerified , activationCode")] User user)
        {
            Boolean status = false;
            string message = "";

            if (ModelState.IsValid)
            {
                //Email is already exist
                var isExist = isEmailExist(user.email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email is already exist !");
                    return View(user);
                }

                //Generate Activation code
                user.activationCode = Guid.NewGuid();

                //password encoding
                user.password = Crypto.Hash(user.password);
                user.confirmPassword = Crypto.Hash(user.confirmPassword);

                user.isEmailVerified = false;

                //save to database
                using(MyDbEntities db = new MyDbEntities())
                {
                    db.Users.Add(user);
                    db.SaveChanges();

                    //send Email
                    sendVerificationLinkEmail(user.email, user.activationCode.ToString());
                    message = "For activation check your email.";
                    status = true;
                }
            }
            else
            {
                message = "Invalid Request !";
            }
            ViewBag.Message = message;
            ViewBag.Status = status;

            return View(user);
        }


        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            Boolean status = false;
            using(MyDbEntities db = new MyDbEntities())
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                var validActivation = db.Users.Where(a => a.activationCode == new Guid(id)).FirstOrDefault();

                if(validActivation != null)
                {
                    validActivation.isEmailVerified = true;
                    db.SaveChanges();
                    status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request !";
                }
            }

            ViewBag.Status = status;
            return View();

        }

        
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }   

        [HttpPost]
        public ActionResult Login(UserLogin userLogin , string returnUrl = "")
        {
            string message = "";
            using(MyDbEntities db = new MyDbEntities())
            {
                var user = db.Users.Where(a => a.email == userLogin.email).FirstOrDefault();
                if(user != null)
                {
                    if(string.Compare(Crypto.Hash(userLogin.password),user.password) == 0)
                    {
                        int timeOut = userLogin.RememberMe ? 525600 : 20;
                        var ticket = new FormsAuthenticationTicket(userLogin.email, userLogin.RememberMe, timeOut);
                        string encripted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encripted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeOut);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        message = "Invalid credential Provided";
                    }

                }
                else
                {
                    message = "Invalid credential Provided";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        [Authorize]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }


        [NonAction]
        public Boolean isEmailExist(string email)
        {
            using(MyDbEntities db = new MyDbEntities())
            {
                var existState = db.Users.Where(a => a.email == email).FirstOrDefault();
                return existState != null;
            }
        }

        [NonAction]
        public void sendVerificationLinkEmail(string email , string activatonCode)
        {
            var verifyUrl = "/User/VerifyAccount/" + activatonCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("thrinduchulle@gmail.com","thrindu chulle");//your email address
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "*****";//your email password
            string subject = "Your account is successdully created !";

            string body = "<br/><br/>Click <a href='" + link + "'>here<a/>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                try
                {
                    smtp.Send(message);
                    
                }
                catch(Exception e)
                {
                    
                }
                
        }
    }
}