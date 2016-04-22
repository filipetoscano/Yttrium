using NDesk.Options;
using System;
using System.Collections.Generic;

namespace Yttrium.Xslt
{
    public class CommandLine
    {
        public string InputFile
        {
            get;
            set;
        }

        public string TransformFile
        {
            get;
            set;
        }

        public string OutputFile
        {
            get;
            set;
        }

        public bool MultipleOutput
        {
            get;
            set;
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
                { "i",          v => this.InputFile = v },
                { "input=",     v => this.InputFile = v },

                { "x",          v => this.TransformFile = v },
                { "t",          v => this.TransformFile = v },
                { "xslt=",      v => this.TransformFile = v },
                { "transform=", v => this.TransformFile = v },

                { "o",          v => this.OutputFile = v },
                { "output=",    v => this.OutputFile = v },

                { "m|multiple", v => this.MultipleOutput = true },
                { "h|help",     v => this.Help = true },
            };

            List<string> extra = p.Parse( args );

            //if ( extra.Count > 0 )
            //{
            //    Console.Error.WriteLine( "error: aditional unrecognized arguments specified." );
            //    return false;
            //}


            // TODO

            return true;
        }


        public void HelpShow()
        {
            throw new NotImplementedException();
        }
    }
}

/* eof */