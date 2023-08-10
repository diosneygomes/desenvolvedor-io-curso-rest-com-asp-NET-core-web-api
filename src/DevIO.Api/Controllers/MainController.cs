﻿using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DevIO.Api.Controllers
{
    [ApiController]
    public abstract class MainController : Controller
    {
        private readonly INotificador _notificador;
        public readonly IUser _appUser;

        protected Guid UsuarioId { get; set; }
        protected bool UsuarioAutenticado { get; set; }

        public MainController(
            INotificador _notificador,
            IUser appUser)
        {
            this._notificador = _notificador;
            this._appUser = appUser;

            if (appUser.IsAuthenticated())
            {
                this.UsuarioId = appUser.GetUserId();
                this.UsuarioAutenticado = true;
            }
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        protected ActionResult CustomResponse(object? result = null)
        {
            if (this.OperacaoValida())
            {
                return Ok(
                    new
                    {
                        success= true,
                        data = result
                    });
            }

            return BadRequest(
                new
                {
                    success = false,
                    errors = _notificador
                        .ObterNotificacoes()
                        .Select(n => n.Mensagem)
                });
        }

        protected ActionResult CustomResponse (ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                this.NotificarErroModelInvalida(modelState);
            }

            return View(modelState);
        }

        protected void NotificarErroModelInvalida (ModelStateDictionary modelState)
        {
            var erros = modelState.Values
                .SelectMany(e => e.Errors);

            foreach (var erro in erros)
            {
                var erroMsg = erro.Exception is null
                    ? erro.ErrorMessage
                    : erro.Exception.Message;

                NotificarErro(erroMsg);
            }
        }

        protected void NotificarErro(string mensagem)
        {
            this._notificador
                .Handle(new Notificacao(mensagem));
        }
    }
}
