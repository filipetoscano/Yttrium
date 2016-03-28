using NDesk.Options;
using System;
using System.Collections.Generic;

namespace Yttrium.OraPack
{
    public class CommandLine
    {
        public string PackageName
        {
            get;
            private set;
        }

        public List<string> FilePatterns
        {
            get;
            private set;
        }

        public bool SeparateFiles
        {
            get;
            private set;
        }

        public bool Debug
        {
            get;
            private set;
        }

        public bool KeepMarkedComments
        {
            get;
            private set;
        }

        public bool Help
        {
            get;
            private set;
        }


        public bool Parse( string[] args )
        {
            var p = new OptionSet()
            {
                { "p=",            v => this.PackageName = v },
                { "package=",      v => this.PackageName = v },

                { "d|debug",       v => this.Debug = true },
                { "h|help",        v => this.Help = true },
                { "k|keep",        v => this.KeepMarkedComments = true },
                { "s|split",       v => this.SeparateFiles = true },
            };

            List<string> extra = p.Parse( args );


            /*
             * Stop parsing the remainder of the options, if the user has
             * specified that he wishes to view the help.
             */
            if ( this.Help == true )
                return true;


            /*
             * PackageName
             */
            if ( string.IsNullOrEmpty( this.PackageName ) == true )
            {
                Console.Error.WriteLine( "error: package name is mandatory (use --package=PNAME)." );
                return false;
            }


            /*
             * Arguments
             */
            if ( extra.Count == 0 )
            {
                extra.Add( "*.sql" );
                Console.Out.WriteLine( "warn: assuming *.sql" );
            }


            this.FilePatterns = extra;
            return true;
        }


        public void HelpShow()
        {
            Console.WriteLine( "usage: orapack [OPTION] [files]" );
            Console.WriteLine( "  -p, --package=PNAME   Name of the Oracle package (required)" );
            Console.WriteLine( "  -d, --debug           Turn on DEBUG flag, turning off RELEASE flag" );
            Console.WriteLine( "  -h, --help            Print this help page" );
            Console.WriteLine( "  -k, --keep            Keep comments which were marked for removal" );
            Console.WriteLine( "  -s, --split           Place header/body into separate files (pks, pkb)" );
        }
    }
}

/* eof */