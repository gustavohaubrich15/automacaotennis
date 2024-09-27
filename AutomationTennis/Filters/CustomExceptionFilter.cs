using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AutomationTennis.Exceptions;

namespace AutomationTennis.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            ObjectResult response;

            if (exception is BusinessException)
            {
                response = new ObjectResult(new
                {
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Message = exception.Message ?? "Registro não encontrado."
                })
                {
                    StatusCode = (int)HttpStatusCode.Conflict
                };
            }
            else
            {
                response = new ObjectResult(new
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = $"Erro geral na aplicação : {exception.Message}"
                })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

            context.Result = response;
            context.ExceptionHandled = true;
        }
    }
}
