using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplicationAPI.Authorize
{
    public class AttributeRole : ActionFilterAttribute
    {
        private readonly string[] _roles;
        public AttributeRole(string roles)
        {
            _roles = roles.Split(',').Select(r => r.Trim()).ToArray();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessionRole = context.HttpContext.Session.GetString("UserRole");

            if (sessionRole == null ||
                !_roles.Contains(sessionRole))
            {
                context.Result = new ObjectResult(new { error = "Unauthorized", message = "You do not have permission to access this resource." })
                {
                    StatusCode = 403
                };
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
