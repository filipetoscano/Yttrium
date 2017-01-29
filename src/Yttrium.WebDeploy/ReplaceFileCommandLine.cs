using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Yttrium.WebDeploy
{
    /// <summary />
    public class ReplaceFileCommandLine
    {
        public string Package { get; private set; }
        public string Replace { get; private set; }
        public string With { get; private set; }
        public bool Help { get; private set; }


        /// <summary />
        public bool Parse( string[] args )
        {
            /*
             * 
             */
            var p = new OptionSet()
            {
                { "p=|package=",  v => this.Package = v },
                { "f=|file=",     v => this.Replace = v },
                { "w=|with=",     v => this.With = v },
                { "h",            v => this.Help = true }
            };

            List<string> extra = p.Parse( args );

            if ( extra.Count > 0 )
            {
                Console.Error.WriteLine( "err: too many arguments." );
                return false;
            }


            /*
             * 
             */
            if ( this.Package == null )
            {
                Console.Error.WriteLine( "err: package is mandatory argument." );
                return false;
            }

            if ( this.Replace == null )
            {
                Console.Error.WriteLine( "err: file is mandatory argument." );
                return false;
            }

            if ( this.With == null )
            {
                Console.Error.WriteLine( "err: with is mandatory argument." );
                return false;
            }


            /*
             * Stop parsing the remainder of the options, if the user has
             * specified that he wishes to view the help.
             */
            if ( this.Help == true )
                return true;

            return true;
        }


        /// <summary />
        public void HelpShow()
        {
            Console.WriteLine( "usage: ywebdeploy replace [OPTIONS]" );
            Console.WriteLine( "  -p, --package   Path to the generated web deploy package." );
            Console.WriteLine( "  -f, --file      Path, relative to root, of file to replace." );
            Console.WriteLine( "  -w, --with      Path to the file which will replace it." );
            Console.WriteLine( "  -h, --help      Print this help page" );
        }


        /// <summary />
        public string ToolVersionGet()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString( 4 );
        }
    }
}

/* eof */