using Google.Authenticator;
using System;
using System.Data;
using System.Data.SQLite;
using System.Web;
using System.Web.Http;
using WebApplication.Properties;

namespace WebApplication.Controllers
{
    public class TwoFactorController : ApiController
    {
        /// <summary>
        /// Returns Two Factor authentication information.
        /// </summary>
        /// <returns>Setup information.</returns>
        [HttpGet]
        public SetupResponse SetupGet()
        {
            var user = UserSetup();
            string app = "MyApplication";

            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var setupInfo = tfa.GenerateSetupCode( app, user.UserId, user.Secret, 300, 300 );

            string code = setupInfo.ManualEntryKey;
            string provisionUrl = setupInfo.QrCodeSetupImageUrl;

            return new SetupResponse()
            {
                UserId = user.UserId,
                ManualCode = code,
                ImageUrl = provisionUrl
            };
        }


        /// <summary>
        /// Validates a TFA code.
        /// </summary>
        /// <param name="request">TFA code.</param>
        /// <returns>Whether code is valid or not.</returns>
        [HttpPost]
        public ValidateResponse Validate( ValidateRequest request )
        {
            var user = UserGet();

            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            bool isValid = tfa.ValidateTwoFactorPIN( user.Secret, request.Code );

            return new ValidateResponse()
            {
                IsValid = isValid
            };
        }



        /// <summary>
        /// Setups two factor configuration for the current user.
        /// If the user already has two factor setup, this will
        /// overwrite it.
        /// </summary>
        /// <returns>Two factor configuration.</returns>
        private TwoFactorUserConfiguration UserSetup()
        {
            var identity = HttpContext.Current.User.Identity;
            string secret = Guid.NewGuid().ToString();

            using ( SQLiteConnection conn = new SQLiteConnection( WebApiApplication.DbConnectionString ) )
            {
                conn.Open();

                SQLiteCommand cmd = new SQLiteCommand( Resources.UserSetup, conn );
                cmd.Parameters.Add( "@UserId", DbType.String ).Value = identity.Name;
                cmd.Parameters.Add( "@Secret", DbType.String ).Value = secret;

                cmd.ExecuteNonQuery();

                conn.Close();
            }

            return new TwoFactorUserConfiguration()
            {
                UserId = identity.Name,
                Secret = secret
            };
        }


        /// <summary>
        /// Gets two factor configuration for the current user.
        /// </summary>
        /// <returns>Two factor configuration.</returns>
        private TwoFactorUserConfiguration UserGet()
        {
            var identity = HttpContext.Current.User.Identity;
            string secret;

            using ( SQLiteConnection conn = new SQLiteConnection( WebApiApplication.DbConnectionString ) )
            {
                conn.Open();

                SQLiteCommand cmd = new SQLiteCommand( Resources.UserGet, conn );
                cmd.Parameters.Add( "@UserId", DbType.String ).Value = identity.Name;
                secret = (string) cmd.ExecuteScalar();

                conn.Close();
            }


            return new TwoFactorUserConfiguration()
            {
                UserId = identity.Name,
                Secret = secret
            };
        }
    }


    /// <summary>
    /// Two factor configuration for a user.
    /// </summary>
    internal class TwoFactorUserConfiguration
    {
        public string UserId { get; set; }
        public string Secret { get; set; }
    }


    /// <summary>
    /// Response for TwoFactor/Setup method.
    /// </summary>
    public class SetupResponse
    {
        public string UserId { get; set; }
        public string ManualCode { get; set; }
        public string ImageUrl { get; set; }
    }


    /// <summary>
    /// Request for TwoFactor/Validate method.
    /// </summary>
    public class ValidateRequest
    {
        public string Code { get; set; }
    }


    /// <summary>
    /// Response for TwoFactor/Validate method.
    /// </summary>
    public class ValidateResponse
    {
        public bool IsValid { get; set; }
    }
}