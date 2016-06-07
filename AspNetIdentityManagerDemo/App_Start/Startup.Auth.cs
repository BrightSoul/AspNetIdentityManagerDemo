using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using AspNetIdentityManagerDemo.Models;
using IdentityManager.Configuration;
using IdentityManager;
using Microsoft.AspNet.Identity.EntityFramework;
using IdentityManager.AspNetIdentity;

namespace AspNetIdentityManagerDemo
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});

            #region IdentityManagerConfiguration
            var factory = new IdentityManagerServiceFactory();

            // In questo specifico esempio, usiamo Entity Framework 
            // e perciò registriamo le classi UserStore<TUser> e 
            // RoleStore<TRole> dal namespace Microsoft.AspNet.Identity.EntityFramework
            factory.IdentityManagerService = new Registration<IIdentityManagerService>(
              resolver =>
              {
                  var userManager =
        new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                  var roleManager =
        new RoleManager<IdentityRole>(new RoleStore<IdentityRole>());
                  return new AspNetIdentityManagerService<
        IdentityUser, string, IdentityRole, string>(userManager, roleManager);
              });

            // Creiamo l'oggetto di configurazione
            var managerOptions = new IdentityManagerOptions
            {
                // Consentiamo l'accesso solo dalla macchina locale
                SecurityConfiguration = new LocalhostSecurityConfiguration {
                    RequireSsl = false
                },

                // Potremmo decidere di fare a meno dell'interfaccia grafica, 
                // se volessimo sfruttare la Web API sottostante, esposta 
                // su /identitymanager/api
                DisableUserInterface = false,

                //Indichiamo la factory creata in precedenza
                Factory = factory
            };

            // Infine, registriamo il middleware indicando il percorso
            // da cui desideriamo accedere al pannello di gestione
            app.Map("/identitymanager", map =>
            {
                map.UseIdentityManager(managerOptions);
            });

            #endregion

        }
    }
}