using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Yttrium.FsManifest
{
    public class CommandLine
    {
        public string InputDirectory { get; private set; }
        public string OutputFileName { get; private set; }
        public bool WriteToFile { get { return string.IsNullOrEmpty( this.OutputFileName ) == false; } }
        public bool Help { get; private set; }


        public bool Parse( string[] args )
        {
            /*
             * 
             */
            var p = new OptionSet()
            {
                { "o=|output=",     v => this.OutputFileName = v },
                { "h|help",         v => this.Help = true },
            };

            List<string> extra = p.Parse( args );

            if ( extra.Count > 1 )
            {
                Console.Error.WriteLine( "error: too many arguments." );
                return false;
            }

            if ( extra.Count == 1 )
                this.InputDirectory = extra[ 0 ];
            else
                this.InputDirectory = Environment.CurrentDirectory;


            /*
             * Stop parsing the remainder of the options, if the user has
             * specified that he wishes to view the help.
             */
            if ( this.Help == true )
                return true;

            return true;
        }


        public void HelpShow()
        {
            Console.WriteLine( "usage: fsmanifest [OPTION] [directory]" );
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