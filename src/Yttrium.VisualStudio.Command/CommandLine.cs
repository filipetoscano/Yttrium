using NDesk.Options;
using System;
using System.Collections.Generic;

namespace Yttrium.VisualStudio.Command
{
    public class CommandLine
    {
        public string Tool
        {
            get;
            private set;
        }


        public string File
        {
            get;
            private set;
        }


        public string Namespace
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
                { "tool=",      v => this.Tool = v },
                { "t=",         v => this.Tool = v },

                { "file=",      v => this.File = v },
                { "f=",         v => this.File = v },

                { "namespace=", v => this.Namespace = v },
                { "ns=",        v => this.Namespace = v },

                { "h|help",     v => this.Help = true },
            };

            List<string> extra = p.Parse( args );

            if ( extra.Count > 0 )
            {
                Console.Error.WriteLine( "error: aditional unrecognized arguments specified." );
                return false;
            }


            if ( string.IsNullOrEmpty( this.Tool ) == true )
            {
                Console.Error.WriteLine( "error: tool parameter is mandatory (use --tool=TOOL)." );
                return false;
            }

            if ( string.IsNullOrEmpty( this.File ) == true )
            {
                Console.Error.WriteLine( "error: file parameter is mandatory (use --file=FILE)." );
                return false;
            }

            return true;
        }


        public void HelpShow()
        {
            throw new NotImplementedException();
        }
    }
}

/* eof */