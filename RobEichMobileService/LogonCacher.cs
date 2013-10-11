using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace RobEichMobileService
{
    class LogonCacher
    {

        private const string RESOURCE = "RobEichMobileService"; 

        public static void SaveCredential(string userName, string password)
        {
            PasswordVault passwordVault = new PasswordVault();
            PasswordCredential passwordCredential = new PasswordCredential(RESOURCE, userName, password);

            passwordVault.Add(passwordCredential); 
        }

        public static PasswordCredential GetCredential()
        {
            PasswordCredential passwordCredential = null; 
            PasswordVault passwordVault = new PasswordVault();
            try 
            {
                passwordCredential = passwordVault.FindAllByResource(RESOURCE).FirstOrDefault();
                if (passwordCredential != null)
                {
                    passwordCredential.Password = passwordVault.Retrieve(RESOURCE, passwordCredential.UserName).Password; 
                }
            }
            catch (Exception)
            { 
                //TODO: implement error handling
            }

            return passwordCredential; 
        }


    }
}
