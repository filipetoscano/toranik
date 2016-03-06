using System.Data.SQLite;
using System.Globalization;
using System.Web;
using System.Web.Http;
using WebApplication.Properties;

namespace WebApplication
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            /*
             * Ensure Sqlite database exists and has necessary tables
             * created.
             */
            SQLiteConnection.CreateFile( DbPath );

            using ( SQLiteConnection conn = new SQLiteConnection( WebApiApplication.DbConnectionString ) )
            {
                conn.Open();

                SQLiteCommand cmd = new SQLiteCommand( Resources.Schema, conn );
                cmd.ExecuteNonQuery();

                conn.Close();
            }


            /*
             * WebAPI configuration
             */
            GlobalConfiguration.Configure( WebApiConfig.Register );
        }


        /// <summary>
        /// Gets the physical location of the SQLite database file.
        /// </summary>
        public static string DbPath
        {
            get
            {
                return HttpContext.Current.Server.MapPath( @"~/App_Data/TwoFactor.sqlite" );
            }
        }


        /// <summary>
        /// Gets the connection string to the SQLite database.
        /// </summary>
        public static string DbConnectionString
        {
            get
            {
                return string.Format( CultureInfo.InvariantCulture, "Data Source={0};Version=3;", DbPath );
            }
        }
    }
}
