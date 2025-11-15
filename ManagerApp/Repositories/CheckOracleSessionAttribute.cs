using System;
using System.Web;
using System.Web.Mvc;
using ManagerApp.Repositories;

public class CheckOracleSessionAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var session = HttpContext.Current.Session;

        if (session["user"] == null)
        {
            filterContext.Result = new RedirectResult("/Home/Login");
            return;
        }

        string username = session["user"].ToString();
        var userService = new UserService("OracleDbContext");
        bool alive = userService.CheckOracleSession(username);

        if (!alive)
        {
            session.Clear();
            session.Abandon();
            filterContext.Result = new RedirectResult("/Home/Login");
            return;
        }
        base.OnActionExecuting(filterContext);
    }
}