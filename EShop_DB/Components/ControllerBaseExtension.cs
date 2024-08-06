using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace EShop_DB.Components;

internal static class ControllerBaseExtension
{
    internal static string GetControllerName(this ControllerBase controller)
    {
        return controller.GetType().Name.Replace("Controller", "");
    }

    internal static string GetFormatedControllerName(this ControllerBase controller)
    {
        string controllerName = controller.GetControllerName();
        
        string formattedName = Regex.Replace(controllerName, "([a-z])([A-Z])", "$1 $2");
        
        formattedName = Regex.Replace(formattedName, @"\s([A-Z])", m => " " + char.ToLower(m.Groups[1].Value[0]));

        return formattedName;
    }
}