using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Yttrium.DbConfig
{
    public class CommandLine
    {
        public string Environment { get; private set; }
        public bool Enforce { get; private set; }
        public string Locale { get; private set; }
        public bool VerboseOutput { get; private set; }
        public bool SimpleOutputName { get; private set; }
        public List<string> Parameters { get; private set; }
        public List<string> FilePatterns { get; private set; }
        public bool IncludeStackTrace { get; private set; }
        public bool Help { get; private set; }


        public bool Parse( string[] args )
        {
            this.Locale = "en-GB";
            this.Parameters = new List<string>();


            /*
             * 
             */
            var p = new OptionSet()
            {
                { "e=|env=",      v => this.Environment = v },
                { "f|enforce",    v => this.Enforce = true },
                { "l=|locale=",   v => this.Locale = v },
                { "k|kiss",       v => this.SimpleOutputName = true },
                { "v|verbose",    v => this.VerboseOutput = true },
                { "s|stack",      v => this.IncludeStackTrace = true },
                { "p=|param=",    v => this.Parameters.Add( v ) },
                { "h|help",       v => this.Help = true },
            };

            this.FilePatterns = p.Parse( args );


            /*
             * Stop parsing the remainder of the options, if the user has
             * specified that he wishes to view the help.
             */
            if ( this.Help == true )
                return true;


            /*
             * Expect/demand at least one file/pattern to be provided.
             */
            if ( this.FilePatterns.Count == 0 )
            {
                Console.Error.WriteLine( "error: file/pattern is mandatory." );
                return false;
            }


            /*
             * KISS mode (where the output file always has the same file-name)
             * requires that the environment flag always be set -- even if
             * it isn't a multi environment transformation.
             */
            if ( this.SimpleOutputName == true && this.Environment == null )
            {
                Console.Error.WriteLine( "error: when specifying --kiss, the environment is mandatory." );
                return false;
            }

            return true;
        }


        public void HelpShow()
        {
            Console.WriteLine( "usage: dbconfig [OPTION] [files]" );
            Console.WriteLine( "  -e, --env=ENV         Target environment for which file is generated. If not specified (and file" );
            Console.WriteLine( "                        is environment specific) then all target files will be generated." );
            Console.WriteLine( "  -f, --enforce         Whether the transformation will receive Enforce=true." );
            Console.WriteLine( "  -l, --locale=LOCALE   Sets value of Locale value which is passed to transformation" );
            Console.WriteLine( "  -v, --verbose         Tool should emit verbose console information" );
            Console.WriteLine( "  -s, --stack           In case of failure, emit full stack to console" );
            Console.WriteLine( "  -p, --param=K=V       Additional key/values which are passed to transformation" );
            Console.WriteLine( "  -h, --help            Print this help page" );
        }


        public string ToolVersionGet()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString( 4 );
        }
    }
}

/* eof */