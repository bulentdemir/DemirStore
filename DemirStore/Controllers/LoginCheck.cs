using DemirStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemirStore.Controllers
{
    public class LoginCheck
    {
        DemirStoreDBEntities dbModel;
        public LoginCheck()
        {
            dbModel = new DemirStoreDBEntities();
        }

        public bool IsLoginSuccess(tblUsers userModel)
        {
            var crypto = new SimpleCrypto.PBKDF2();
            var user = dbModel.tblUsers.Where(x => x.Email == userModel.Email).FirstOrDefault();

            if(user != null)
            {
                if(user.Pswd == crypto.Compute(userModel.Pswd, user.PswdSalt))
                {
                    return true;
                }
            }
            return false;
        }

        public bool AdminIsLoginSuccess(tblUsers userModel)
        {
            var crypto = new SimpleCrypto.PBKDF2();
            var user = dbModel.tblUsers.Where(x => x.Email == userModel.Email && x.isVerified == true).FirstOrDefault();

            if (user != null)
            {
                if (user.Pswd == crypto.Compute(userModel.Pswd, user.PswdSalt))
                {
                    return true;
                }
            }
            return false;
        }
    }
}