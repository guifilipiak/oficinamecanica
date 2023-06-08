using AutoMapper;
using Parcker.Domain;
using Parcker.Models;
using Parcker.Repository.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parcker.Controllers
{
    public class UsuarioController : BaseController
    {
        // GET: Usuario
        public ActionResult Index()
        {
            ViewBag.ListTipoPessoa = ListaTipoPessoa();
            ViewBag.ListUF = ListaUF();

            using (var entity = new Entity())
            {
                var usuario = entity.All<Usuario>().Where(x => x.Nome == User.Identity.Name).FirstOrDefault();
                if (usuario == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (usuario.Pessoa == null)
                {
                    usuario.Pessoa = new Pessoa();
                }

                var usuarioModel = Mapper.Map<UsuarioModel>(usuario);
                usuarioModel.ConfiguracaoEmail = Mapper.Map<ConfiguracaoEmailModel>(entity.All<ConfiguracaoEmail>().FirstOrDefault()) ??
                    new ConfiguracaoEmailModel();

                return View(usuarioModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UsuarioModel model)
        {
            if (ModelState.IsValid)
            {
                //inicia gravacao
                using (var entity = new Entity())
                {
                    try
                    {
                        entity.BeginTransaction();

                        var usuario = Mapper.Map<Usuario>(model);

                        if (model.Id != 0)
                        {
                            //atualiza pessoa
                            #region Validacao Manual
                            bool error = false;
                            //valida campos
                            if (model.Pessoa.Tipo == 1)
                            {
                                if (string.IsNullOrEmpty(model.Pessoa.CPF))
                                {
                                    ModelState.AddModelError("Pessoa_CPF", "Campo CPF é obrigatório.");
                                    error = true;
                                }

                                if (string.IsNullOrEmpty(model.Pessoa.Nome))
                                {
                                    ModelState.AddModelError("Pessoa_Nome", "Campo Nome é obrigatório.");
                                    error = true;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(model.Pessoa.CNPJ))
                                {
                                    ModelState.AddModelError("Pessoa_CNPJ", "Campo CNPJ é obrigatório.");
                                    error = true;
                                }

                                if (string.IsNullOrEmpty(model.Pessoa.RazaoSocial))
                                {
                                    ModelState.AddModelError("Pessoa_RazaoSocial", "Campo RazaoSocial é obrigatório.");
                                    error = true;
                                }
                            }

                            if (error)
                            {
                                return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);
                            }
                            #endregion

                            if (usuario.Pessoa.Id != 0)
                            {
                                //atualiza pessoa
                                entity.Update(usuario.Pessoa);
                            }
                            else
                            {
                                if (entity.All<Usuario>().Where(x => (x.Pessoa.CPF == model.Pessoa.CPF && !string.IsNullOrEmpty(model.Pessoa.CPF)) || (x.Pessoa.CNPJ == model.Pessoa.CNPJ && !string.IsNullOrEmpty(model.Pessoa.CNPJ))).Any())
                                {
                                    //ja existe um cliente cadastrado com este documento
                                    return Json(new { IsValid = false, Message = "Já existe um cliente cadastro com este número de documento, verifique e tente novamente!" }, JsonRequestBehavior.AllowGet);
                                }

                                //adiciona pessoa
                                entity.Add(usuario.Pessoa);
                            }

                            //adiciona cliente
                            usuario.IdPessoa = usuario.Pessoa.Id;

                            //re atribui senha
                            if (string.IsNullOrEmpty(model.Senha))
                            {
                                var modelDB = entity.GetById<Usuario>(model.Id);
                                usuario.Senha = modelDB.Senha;
                            }

                            //atualiza usuario
                            var merge = entity.Merge(usuario);
                            entity.Update(merge);

                            var configuracao = entity.All<ConfiguracaoEmail>().FirstOrDefault();

                            if (configuracao == null)
                                configuracao = configuracao = new ConfiguracaoEmail();

                            configuracao.Remetente = model.ConfiguracaoEmail.Remetente;
                            configuracao.Servidor = model.ConfiguracaoEmail.Servidor;
                            configuracao.Remetente = model.ConfiguracaoEmail.Remetente;
                            configuracao.Usuario = model.ConfiguracaoEmail.Usuario;
                            configuracao.Porta = model.ConfiguracaoEmail.Porta;

                            if (!string.IsNullOrEmpty(model.ConfiguracaoEmail.Senha))
                            {
                                configuracao.Senha = model.ConfiguracaoEmail.Senha;
                            }

                            entity.SaveOrUpdate(configuracao);

                            entity.Commit();
                        }
                        else
                        {
                            entity.Rollback();
                        }

                        return Json(new { IsValid = true, Message = MensagemSalvo, Data = Mapper.Map<UsuarioModel>(usuario) }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        entity.Rollback();
                        ModelState.AddModelError("", ex);
                        return Json(new { IsValid = false, Message = MensagemErro }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
                return Json(new { IsValid = false, Message = MensagemValidacao }, JsonRequestBehavior.AllowGet);
        }
    }
}