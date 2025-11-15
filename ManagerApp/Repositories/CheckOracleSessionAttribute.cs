using System;
using System.Web;
using System.Web.Mvc;
using ManagerApp.Repositories;

public class CheckOracleSessionAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var session = HttpContext.Current.Session;

        if (session["user"] == null || session["password"] == null)
            return;

        string username = session["user"].ToString();
        string password = session["password"].ToString();

        var userService = new UserService("OracleDbContext");
        bool alive = userService.CheckOracleSession(username, password);

        if (!alive)
        {
            session.Clear();
            session.Abandon();
            filterContext.Result = new RedirectResult("/Home/Login");
        }
    }

}



