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
                { "in=",        v => this.InputFile = v },
                { "input=",     v => this.InputFile = v },

                { "x",          v => this.TransformFile = v },
                { "t",          v => this.TransformFile = v },
                { "xslt=",      v => this.TransformFile = v },
                { "transform=", v => this.TransformFile = v },

                { "o",          v => this.OutputFile = v },
                { "out=",       v => this.OutputFile = v },
                { "output=",    v => this.OutputFile = v },

                { "m|multiple", v => this.MultipleOutput = true },
                { "h|help",     v => this.Help = true },
            };

            List<string> extra = p.Parse( args );

            /*
             * Stop parsing the remainder of the options, if the user has
             * specified that he wishes to view the help.
             */
            if ( this.Help == true )
                return true;


            /*
             * .Input
             */
            if ( string.IsNullOrEmpty( this.InputFile ) == true )
            {
                Console.Error.WriteLine( "error: input file is mandatory (use --input=INPUT)." );
                return false;
            }


            /*
             * .Transform
             */
            if ( string.IsNullOrEmpty( this.TransformFile ) == true )
            {
                Console.Error.WriteLine( "error: transformation file is mandatory (use --xslt=XSLT)." );
                return false;
            }


            /*
             * .Output
             */
            if ( string.IsNullOrEmpty( this.OutputFile ) == true )
            {
                Console.Error.WriteLine( "error: output file is mandatory (use --output=OUTPUT)." );
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