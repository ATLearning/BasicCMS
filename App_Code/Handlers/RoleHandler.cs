using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.SessionState;
using WebMatrix.Data;

/// <summary>
/// Summary description for PostHandler
/// </summary>
public class RoleHandler : IHttpHandler, IReadOnlySessionState
{
    public RoleHandler()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //We do not want to reuse the object from this class
    //We want to use this handler for every post request to asp
    public bool IsReusable
    {
        get
        { return false; }
    }

    //Allows us to GET information from form.
    public void ProcessRequest(HttpContext context)
    {
        AntiForgery.Validate();

        //User who are not authenticated or do not have admin role
        //Cant do anything from this role handler
        if (!WebUser.IsAuthenticated)
        {
            throw new HttpException(401, "You must login to do this");
        }

        if (!WebUser.HasRole(UserRoles.Admin))
        {
            throw new HttpException(401, "You do not have permission");
        }

        


        var mode = context.Request.Form["mode"];
        var name = context.Request.Form["roleName"];
        var id = context.Request.Form["roleId"];
        var resourceItem = context.Request.Form["resourceItem"];
    
        //We check the mode to see if edit/new/delete

        if( mode == "edit")
        {
            Edit(Convert.ToInt32(id), name);
        }
        else if( mode == "new")
        {
            Create(name);
        }
        else if (mode == "delete")
        {
            Delete(name ?? resourceItem);
        }

        //if no exception redirect to admin/role
        if (string.IsNullOrEmpty(resourceItem))
        {
            context.Response.Redirect("~/admin/role/");

        }
    }

    //try to find an existing role with this name
    //if one exists we dont want to make another
    private static void Create(string name)
    {
        var result = RoleRepository.Get(name);

        if (result != null)
        {
            throw new HttpException(409, "Role is already in use");
        }

       //otherwise add new role
        RoleRepository.Add(name);
    }

    //try to find role with give id
    //if one dosent exist throw exception (cant edit if it dosent exist)
    private static void Edit(int id,string name)
    {
        var result = RoleRepository.Get(id);

        if (result == null)
        {
            throw new HttpException(404, "Post does not exist");
        }

        

        //PostRepository.Add(title, content, slug, published, authorId);
        RoleRepository.Edit(id, name);
    }

    //No error checking because SQL dosent care if it exists or not
    //Because if it dosent exist nothing will change
    private static void Delete(string name)
    {
        RoleRepository.Remove(name);
    }
    
}
