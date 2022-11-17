using Microsoft.AspNetCore.Mvc;

namespace VKR.Common.Extensions
{
    public static class ControllerExtention
    {
        public static String? ControllerAction<T>(this IUrlHelper urlHelper, string name, object? arg) where T : ControllerBase
        {
            var controllerType = typeof(T);
            var method = controllerType.GetMethod(name);
            if (method == null)
                return null;
            var controller = controllerType.Name.Replace("Controller", string.Empty);
            var action = urlHelper.Action(name, controller, arg);
            return action;
        }

    }
}
