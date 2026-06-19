using FitnessClub_Test.CMS.MVC.Services;
using FitnessClub_Test.Core.Helpers;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Json;
using System.Threading.Tasks;

public abstract class BaseController : Controller
{
    private ApiAuthClient _authClient;
    protected BaseController(ApiAuthClient apiAuthClient)
    {
        _authClient = apiAuthClient;
    }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var token = context.HttpContext.Session.GetString("access");

        if (string.IsNullOrEmpty(token))
        {
            // Session expired → short-circuit the action and redirect
            context.Result = new RedirectToActionResult("Index", "Login", null);
            return; // action will not run
        }

        await LoadLayoutDataAsync();
        await next();
    }

    protected async Task LoadLayoutDataAsync()
    {
        var token = HttpContext.Session.GetString("access");

        var client = await _authClient.GetAuthorizedClientAsync();

        JwtHelper.GetUserID(token, out int UserID);
        var adminInfo = await client.GetFromJsonAsync<AdminAccountDTO>($"admin-account/{UserID}");

        if (adminInfo == null) return;

        ViewBag.LayoutDTO = new LayoutDTO
        {
            FirstName = adminInfo.First_Name,
            LastName = adminInfo.Last_Name,
            Photo = adminInfo.Photo,
            MemberSince = adminInfo.DateCreated?.ToString("MMM yyyy")
        };
    }
}
