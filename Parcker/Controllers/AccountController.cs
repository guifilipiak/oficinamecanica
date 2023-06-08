using Parcker.Domain;
using Parcker.Models;
using Parcker.Repository.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Parcker.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (var entity = new Entity())
                {
                    var user = entity.All<Usuario>()
                        .Where(x => (x.Nome.ToLower() == model.Email.ToLower() && x.Senha == model.Senha) || x.Pessoa.Email.ToLower() == model.Email.ToLower())
                        .FirstOrDefault();

                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(user.Nome, model.LembrarMe);
                        Session["UsuarioLogado"] = user;

                        if (this.Url.IsLocalUrl(returnUrl))
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
                        this.ModelState.AddModelError("", "Usuário ou senha incorreto.");
                    }
                }
            }

            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}