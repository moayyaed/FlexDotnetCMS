﻿using FrameworkLibrary;
using System.Web.Services;

namespace WebApplication.Admin.Views.Roles
{
    /// <summary>
    /// Summary description for ItemServices
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
    [System.Web.Script.Services.ScriptService]
    public class ItemServices : System.Web.Services.WebService
    {
        [WebMethod]
        public string DeleteItemPermanently(long id)
        {
            Role item = BaseMapper.GetObjectFromContext(RolesMapper.GetByID(id));

            if (item != null)
            {
                Return returnObj = RolesMapper.DeletePermanently(item);

                if (returnObj.IsError)
                    return jGrowlHelper.GenerateCode(new jGrowlMessage("Error", "Error deleting item permanently", jGrowlMessage.jGrowlMessageType.Error, returnObj.Error), true);
                else
                    return jGrowlHelper.GenerateCode(new jGrowlMessage("Success", "Item was successfully deleted permanently", jGrowlMessage.jGrowlMessageType.Success), false);
            }

            return "";
        }
    }
}