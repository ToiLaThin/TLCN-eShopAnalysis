using eShopAnalysis.StockInventoryAPI.Utilities.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;
using System.Reflection;

namespace eShopAnalysis.StockInventoryAPI.Utilities.Behaviors
{
    //Ipipeline behavior & mediatR is another option
    //this is an IActionFilter(not have attribute) so it will executed before and after the action
    //will have Ilogger inject since it is not an attribute
    //will require ServiceFilter
    public class LoggingBehaviorActionFilter : IActionFilter
    {
        private readonly ILogger<LoggingBehaviorActionFilter> _logger;
        public LoggingBehaviorActionFilter(ILogger<LoggingBehaviorActionFilter> logger)
        {
            _logger = logger;
        }

        //backchannel action will return backChannel, other will return ActionResult like Ok, NotFound
        //should not return null dto because will not know it have error on client angular
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var controllerStr = context.Controller.ToString();
            var controllerName = controllerStr.Substring(controllerStr.LastIndexOf('.') + 1)
                                              .ToUpper();

            var actionDescriptor = context.ActionDescriptor;
            var actionRoute = actionDescriptor.AttributeRouteInfo.Template;
            var actionName = actionRoute.Substring(actionRoute.LastIndexOf("/") + 1)
                                        .ToUpper();

            var implicitConvertedResult = (context.Result as ObjectResult);
            if (implicitConvertedResult.StatusCode == StatusCodes.Status404NotFound) {
                _logger.LogError(
                    "Request {@ControllerName} " +
                    "\n\tAt action {@ActionName} " +
                    "\n\tAt route {@RouteName} " +
                    "\n\tError: {@Error} " +
                    "\n\tAt {@DateTime}",
                    controllerName,
                    actionName,
                    actionRoute,
                    implicitConvertedResult.Value.ToString(),
                    DateTime.UtcNow
                );
                return;
            }
            //unboxing but with check https://stackoverflow.com/a/13405826 but it not work this time
            //backChannelResp = implicitConvertedResult.Value as BackChannelResponseDto<object> return null
            //this make sure is of type BackChannelResponseDto<>, then we use dynamic to bypass compiler check
            //because we checked it ourself
            Type resultValueType = implicitConvertedResult.Value.GetType();
            if (resultValueType.Name != typeof(BackChannelResponseDto<>).Name) { 
                return;
            }
            dynamic backChannelResp = implicitConvertedResult.Value;
            if (backChannelResp.IsFailed || backChannelResp.IsException) {
                string error = backChannelResp.Error; //logger not accept dynamic
                _logger.LogError(
                    "BackChannel Request {@ControllerName} " +
                    "\n\tAt action {@ActionName} " +
                    "\n\tAt route {@RouteName} " +
                    "\n\tError: {@Error} " +
                    "\n\tAt {@DateTime}",
                    controllerName,
                    actionName,
                    actionRoute,
                    error,
                    DateTime.UtcNow
                );
                //when this is received by the backchannel sender,
                //it will still know because we switch case the status code
                //and return and backChannelResponseDto.Fail
                context.Result = new NotFoundObjectResult(backChannelResp.Error);
                return;
            }            
        }


        public void OnActionExecuting(ActionExecutingContext context)
        {
            //can short circuit by context.Result = 
        }
    }
}
