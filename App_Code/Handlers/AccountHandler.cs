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
public class AccountHandler : IHttpHandler, IReadOnlySessionState
{
    

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
        //User who are not authenticated or do not have admin role
        //Cant do anything from this accounthandler
        AntiForgery.Validate();

        if (!WebUser.IsAuthenticated)
        {
            throw new HttpException(401, "You must login to do this");
        }

        if (!WebUser.HasRole(UserRoles.Admin))
        {
            throw new HttpException(401, "You do not have permission");
        }

        


        var mode = context.Request.Form["mode"];
        var username = context.Request.Form["accountName"];
        var password1 = context.Request.Form["accountPassword1"];
        var password2 = context.Request.Form["accountPassword2"];
        var id = context.Request.Form["accountId"];
        var email = context.Request.Form["accountEmail"];
        var userRoles = context.Request.Form["accountRoles"];
        var resourceItem = context.Request.Form["resourceItem"];

        IEnumerable<int> roles = new int[] { };
        if (!string.IsNullOrEmpty(userRoles))
        {
             roles = userRoles.Split(',').Select(v => Convert.ToInt32(v));

        }

        //Delete is first because we need to check user input 
        if ( mode == "delete")
        {
            Delete(username ?? resourceItem);
        }

        
        else
        {
            if(password1 != password2)
            {
                throw new Exception("Passwords do not match");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("Email cannot be blank");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new Exception("Username cannot be blank");
            }

            //Once our checks pass we can begin to see if edit/new

            if(mode == "edit")
            {
                Edit(Convert.ToInt32(id), username, password1, email, roles);
            }

            else if(mode == "new")
            {
                Create(username, password1, email, roles);
            }
        }


        if (string.IsNullOrEmpty(resourceItem))
        {
            context.Response.Redirect("~/admin/account/");

        }
    }


    //
    private static void Create(string username, string password, 
        string email, IEnumerable<int> roles)
    {
        
        //Check to see if password blank
        //Password is needed
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new Exception("Password cant be blank");
        }

        //Then check to see if another user with the same username
        var result = AccountRepository.Get(username);
        if (result != null)
        {
            throw new HttpException(409, "User already exists");
        }

        AccountRepository.Add(username, Crypto.HashPassword(password), email, roles);
    }


    //For editing an existing user
    //We get the user from there id, if not throw exception
    private static void Edit(int id, string username, string password, 
        string email, IEnumerable<int> roles)
    {
        var result = AccountRepository.Get(id);
       

        if (result == null)
        {
            throw new HttpException(404, "User does not exist");
        }

        //Need to decide to change password or not
        //If password in form is not blank
        var updatedPassword = result.Password;

        if (!string.IsNullOrWhiteSpace(password))
        {
            updatedPassword = Crypto.HashPassword(password);
        }

        //Warning Username SHOULDNT change
        AccountRepository.Edit(id, result.Username, updatedPassword, email, roles);
    }

    private static void Delete(string username)
    {
        AccountRepository.Remove(username);
    }

   
}
