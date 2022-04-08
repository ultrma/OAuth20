using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAuth20.Web.Models;
using OAuth20.Web.Services;

namespace OAuth20.Web.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        public AdminController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet]
        public ActionResult Subscriptions()
        {
            var subscriptions = _userService.GetSubscriptions();
            var subscriptionResponse = new SubscriptionsResponse { Subscriptions = subscriptions };
            return View("Subscriptions", subscriptionResponse);
        }

        [HttpPost("message")]
        public async Task<IActionResult> Notify([FromForm] Message message)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Redirect("/Error");

                await _userService.Notify(message.Content);

                return Subscriptions();
            }
            catch (Exception)
            {
                return Redirect("/Error");
            }
        }

        [HttpGet("revoke/{sessionKey}")]
        public async Task<ActionResult> Revoke([FromRoute] string sessionKey)
        {
            await _userService.Unsubscribe(sessionKey);
            return Subscriptions();

        }

    }
}
