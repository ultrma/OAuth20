using Microsoft.AspNetCore.Mvc;
using OAuth20.Web.Services;
using OAuth20.Web.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace OAuth20.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        public UserController(IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult Login()
        {
            try
            {
                var state = _userService.GetLineAPIState();
                _httpContextAccessor.HttpContext.Session.SetString("state", state);
                var url = _userService.GetLineLoginAuthorizeURL(state);
                return Redirect(url);
            }
            catch (Exception)
            {
                return Redirect("/Error");
            }
        }


        [AllowAnonymous]
        [HttpGet("logincallback")]
        public async Task<IActionResult> LoginCallback([FromQuery(Name = "code")] string code, [FromQuery(Name = "state")] string state)
        {
            try
            {
                if (_httpContextAccessor.HttpContext.Session.GetString("state") != state)
                       return BadRequest();

                var loginUser = await _userService.LoginCallback(code, state, _httpContextAccessor.HttpContext.User.Identity.Name);
                var sessionKey = LoginUsers.GetSessionKey(loginUser.Id);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, sessionKey),
                    new Claim("FullName", loginUser.Name),
                    new Claim(ClaimTypes.Role, loginUser.IsAdmin ? "Admin" : "User"),
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await _httpContextAccessor.HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

                return View("Profile", loginUser);
            }
            catch (Exception)
            {
                return Redirect("/Error");
            }
        }


        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            try
            {
                var loginUser = LoginUsers.GetValue(_httpContextAccessor.HttpContext.User.Identity.Name);
                if (loginUser == null)
                    return Redirect("/");

                return View(loginUser);
            }
            catch (Exception)
            {
                return Redirect("/Error");
            }
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                LoginUsers.RemoveBySessionKey(_httpContextAccessor.HttpContext.User.Identity.Name);
                await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/");
            }
            catch (Exception)
            {
                return Redirect("/Error");
            }
        }


        [AllowAnonymous]
        [HttpGet("accessdenied")]
        public IActionResult AccessDenined([FromQuery(Name = "code")] string code, [FromQuery(Name = "state")] string state)
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                return Redirect("/Error");
            }
        }

        [Authorize]
        [HttpGet("subscribe")]
        public IActionResult Subscribe()
        {
            try
            {
                var state = _userService.GetLineAPIState();
                _httpContextAccessor.HttpContext.Session.SetString("state", state);
                var url = _userService.GetLineNotifiyAuthorizeURL(state);

                return Redirect(url);
            }
            catch (Exception)
            {
                return Redirect("/Error");
            }
        }

        [HttpGet("notifycallback")]
        public async Task<IActionResult> NotifyCallBack([FromQuery(Name = "code")] string code, [FromQuery(Name = "state")] string state)
        {
            try
            {
                if (_httpContextAccessor.HttpContext.Session.GetString("state") != state)
                    return BadRequest();

                var user = await _userService.Subscribe(code, state, _httpContextAccessor.HttpContext.User.Identity.Name); 
                return View("Profile", user);
            }
            catch (Exception)
            {
                return Redirect("/Error");
            }
        }

        [Authorize]
        [HttpGet("unsubscribe")]
        public async Task<IActionResult> Unsubscribe()
        {
            try
            {
                await _userService.Unsubscribe(_httpContextAccessor.HttpContext.User.Identity.Name);
                return Redirect("/user/profile");
            }
            catch (Exception)
            {
                return Redirect("/Error");
            }
        }
    }
}
