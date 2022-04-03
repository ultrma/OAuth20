using Microsoft.AspNetCore.Mvc;
using OAuth20.Web.Services;
using OAuth20.Web.Pages;
using OAuth20.Web.Models;

namespace OAuth20.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery(Name = "code")] string code, [FromQuery(Name = "state")] string state)
        {
            try
            {
                var user = await _userService.Login(code, state);
                return View(user);
            }
            catch (Exception)
            {
                return Redirect("/Error");
            }
        }

        [HttpGet("subscribe")]
        public async Task<IActionResult> Dashboard([FromQuery(Name = "code")] string code, [FromQuery(Name = "state")] string state)
        {
            try
            {
                var user = await _userService.Subscribe(code, state);
                return View(user);
            }
            catch (Exception)
            {
                return Redirect("/Error");
            }
        }

        [HttpGet("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([FromQuery(Name ="accessToken")] string accessToken)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(accessToken))
                    return Redirect("/Error");

                await _userService.Unsubscribe(accessToken);
                return Redirect("/Index");
            }
            catch (Exception)
            {
                return Redirect("/Error");
            }
        }
    }
}
