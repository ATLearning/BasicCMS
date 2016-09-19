using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for UserRoles
/// </summary>
/// 
//PURPOSE OF CLASS IT TO MITIGATE RISK OF ACCIDENTALY VALIDATING WRONG ROLE
public class UserRoles
{
    public UserRoles()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //We can use UserRoles.Admin to refrence the user role
    //We dont have to worry about mistyping admin role name

    //Admin can do everything
    public const string Admin = "admin";

    //Can create, edit, delete posts
    public const string Editor = "editor";

    //Author can only edit, create own posts
    public const string Author = "author";
}