using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace EShop_DB.Common.Extensions;

internal static class ControllerBaseExtension
{
    /// <summary>
    /// Get the class name and removes the Controller prefix
    /// </summary>
    /// <returns>Class name without Controller prefix</returns>
    internal static string GetControllerName(this ControllerBase controller)
    {
        return controller.GetType().Name.Replace("Controller", "");
    }

    /// <summary>
    /// Separates words with spaces and makes the first letter of the second and subsequent words lowercase
    /// </summary>
    /// <returns>ExampleControllerName => Example controller name</returns>
    internal static string GetFormatedControllerName(this ControllerBase controller)
    {
        string formattedName = Regex.Replace(controller.GetControllerName(), "([a-z])([A-Z])", "$1 $2");
        
        formattedName = Regex.Replace(formattedName, @"\s([A-Z])", m => " " + char.ToLower(m.Groups[1].Value[0]));

        return formattedName;
    }
}