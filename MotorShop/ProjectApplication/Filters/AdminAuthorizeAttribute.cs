using System.Web.Mvc;

public class AdminAuthorizeAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (filterContext.HttpContext.Session["AdminId"] == null)
        {
            filterContext.Result = new RedirectResult("~/Login/Login");
        }
        base.OnActionExecuting(filterContext);
    }
}
