using NDesk.Options;
using System;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Yttrium.DbManifest
{
    public class CommandLine
    {
        public string Server { get; private set; }
        public string DatabaseName { get; private set; }
        public string AuthUserName { get; private set; }
        public string AuthPassword { get; private set; }
        public bool AuthTrusted { get; private set; }
        public bool IncludeComments { get; private set; }
        public bool ExportCode { get; private set; }
        public string OutputFileName { get; private set; }
        public bool Verbose { get; private set; }
        public bool Help { get; private set; }
        public string ConnectionString { get; private set; }


        public bool Parse( string[] args )
        {
            this.Server = ".";


            /*
             * 
             */
            var p = new OptionSet()
            {
                { "S=|server=",     v => this.Server = v },
                { "d=|database=",   v => this.DatabaseName = v },
                { "U=|user=",       v => this.AuthUserName = v },
                { "P=|password=",   v => this.AuthPassword = v },
                { "E|trusted",      v => this.AuthTrusted = true },
                { "c|comments",     v => this.IncludeComments = true },
                { "k|code",         v => this.ExportCode = true },
                { "o=|output=",     v => this.OutputFileName = v },
                { "v|verbose",      v => this.Verbose = true },
                { "h|help",         v => this.Help = true },
            };

            p.Parse( args );


            /*
             * Stop parsing the remainder of the options, if the user has
             * specified that he wishes to view the help.
             */
            if ( this.Help == true )
                return true;


            /*
             * Validate the authentication modes
             */
            if ( this.AuthTrusted == false && string.IsNullOrEmpty( this.AuthUserName ) == true )
            {
                Console.Error.WriteLine( "error: specify one authentication mode, either by -E or by -U/-P" );
                return false;
            }

            if ( this.AuthTrusted == true && string.IsNullOrEmpty( this.AuthUserName ) == false )
            {
                Console.Error.WriteLine( "error: specify one authentication mode, either by -E or by -U/-P" );
                return false;
            }

            if ( string.IsNullOrEmpty( this.AuthUserName ) == false && string.IsNullOrEmpty( this.AuthPassword ) == true )
            {
                Console.Error.WriteLine( "error: username specified, but password not specified with -P" );
                return false;
            }


            /*
             * Build connection string
             * Database=Auth;Data Source=localhost;User Id=user;Password=password
             */
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat( CultureInfo.InvariantCulture, "Server={0}", this.Server );

            if ( string.IsNullOrEmpty( this.DatabaseName ) == false )
                sb.AppendFormat( CultureInfo.InvariantCulture, ";Initial Catalog={0}", this.DatabaseName );

            if ( this.AuthTrusted == true )
                sb.AppendFormat( CultureInfo.InvariantCulture, ";Integrated Security=SSPI" );
            else
                sb.AppendFormat( CultureInfo.InvariantCulture, ";User ID={0};Password={1};Trusted_Connection=False", this.AuthUserName, this.AuthPassword );

            this.ConnectionString = sb.ToString();

            return true;
        }


        public void HelpShow()
        {
            Console.WriteLine( "usage: dbmanifest [OPTION] [files]" );
            Console.WriteLine( "  -S, --server=SERVER   Host-name of the database server, default=." );
            Console.WriteLine( "  -d, --database=DBNAME Name of the server database. Required." );
            Console.WriteLine( "  -U, --user=USER       Login name for SQL authentication" );
            Console.WriteLine( "  -P, --password=PWORD  Password, required if using -U" );
            Console.WriteLine( "  -E, --trusted         Use trusted connection" );
            Console.WriteLine( "  -k, --code            Whether to export the code to stored procedures and UDF" );
            Console.WriteLine( "  -o, --output          Emit manifest to output file, otherwise to console" );
            Console.WriteLine( "  -h, --help            Print this help page" );
        }


        public string ToolVersionGet()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString( 4 );
        }
    }
}

/* eof */