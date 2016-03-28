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


        public string Project
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


        public bool DryRun
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

                { "project=",   v => this.Project = v },
                { "p=",         v => this.Project = v },
                { "d|dry",      v => this.DryRun = true },

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


            /*
             * Either the user specifies .Project, and the tool will crawl through the
             * .csproj file and apply the custom tools defined -OR- .Tool and .File
             * need to be specified.
             */
            if ( string.IsNullOrEmpty( this.Project ) == false )
            {
                if ( string.IsNullOrEmpty( this.Tool ) == false )
                {
                    Console.Error.WriteLine( "error: if using --project, using --tool is not permitted." );
                    return false;
                }

                if ( string.IsNullOrEmpty( this.File ) == false )
                {
                    Console.Error.WriteLine( "error: if using --project, using --file is not permitted." );
                    return false;
                }
            }
            else if ( string.IsNullOrEmpty( this.File ) == false )
            {
                if ( string.IsNullOrEmpty( this.Tool ) == true )
                {
                    Console.Error.WriteLine( "error: tool parameter is mandatory (use --tool=TOOL)." );
                    return false;
                }
            }
            else
            {
                Console.Error.WriteLine( "error: file or project parameter is mandatory (use --file or --project)." );
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