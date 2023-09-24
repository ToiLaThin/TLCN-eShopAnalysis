using eShopAnalysis.ProductCatalogAPI.Application.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace eShopAnalysis.ProductCatalogAPI.Utilities.Behaviors
{
    //Ipipeline behavior & mediatR is another option
    //this is an IActionFilter(not have attribute) so it will executed before and after the action
    //will have Ilogger inject since it is not an attribute
    //will require ServiceFilter

    //should only be applied if return type of controller is ResponseDto<class>
    public class LoggingBehaviorActionFilter : IActionFilter
    {
        private readonly ILogger<LoggingBehaviorActionFilter> _logger;
        public LoggingBehaviorActionFilter(ILogger<LoggingBehaviorActionFilter> logger)
        {
            _logger = logger;
        }

        

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var controllerStr = context.Controller.ToString();
            var controllerName = controllerStr.Substring(controllerStr.LastIndexOf('.') + 1).ToUpper();
            var actionDescriptor = context.ActionDescriptor;
            var actionName = actionDescriptor.DisplayName;
            var actionRoute = actionDescriptor.AttributeRouteInfo.Template;
            var responseDtoTryCast  = (context.Result as ObjectResult).Value as ResponseDto<string>; //unboxing but with check https://stackoverflow.com/a/13405826
            if (responseDtoTryCast != null) //can be cast to ResponseDto
            {
            

                if (responseDtoTryCast.IsFailed)
                {
                    _logger.LogError(
                        "Request {@ControllerName} \nAt action {@ActionName} \nAt route {@RouteName} \nError: {@Error} \nAt {@DateTime}",
                        controllerName,
                        actionName,
                        actionRoute,
                        responseDtoTryCast.Error,
                        DateTime.UtcNow
                    );
                }
                if (responseDtoTryCast.IsException)
                {
                    _logger.LogCritical(
                        "Request {@ControllerName} \nAt action {@ActionName} \nAt route {@RouteName} \nException: {@ExceptionError} \nAt {@DateTime}",
                        controllerName,
                        actionName,
                        actionRoute,
                        responseDtoTryCast.Error,
                        DateTime.UtcNow
                    );
                }

                _logger.LogInformation(
                    "Completed controller {@ControllerName} \nAt action {@ActionName} \nAt route {@RouteName} \nAt {@DateTime}",
                    controllerName,
                    actionName,
                    actionRoute,
                    DateTime.UtcNow
                    );
            }
            else
            {
                _logger.LogInformation(
                    "Done controller {@ControllerName} \nAt action {@ActionName} \nAt route {@RouteName} \nAt {@DateTime} without knowing successful or not",
                    controllerName,
                    actionName,
                    actionRoute,
                    DateTime.UtcNow
                    );
            }
            //todo check context.Result with different type to find a way to know if this have error or not for structure loggin
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerStr = context.Controller.ToString();
            var controllerName = controllerStr.Substring(controllerStr.LastIndexOf('.') + 1).ToUpper();
            var actionDescriptor = context.ActionDescriptor;
            var actionName = actionDescriptor.DisplayName;
            var actionRoute = actionDescriptor.AttributeRouteInfo.Template;
            _logger.LogInformation(
                "Handling request from controller {@ControllerName} \nAt action {@ActionName} \nAt route {@RouteName} \nAt {@DateTime}",
                controllerName, actionName, actionRoute,DateTime.UtcNow
                );
        }
    }
}
