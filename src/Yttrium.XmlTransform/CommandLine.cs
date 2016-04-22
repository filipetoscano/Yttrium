using NDesk.Options;
using System;
using System.Collections.Generic;

namespace Yttrium.XmlTransform
{
    public class CommandLine
    {
        public string Input
        {
            get;
            private set;
        }

        public string Transform
        {
            get;
            private set;
        }

        public string Output
        {
            get;
            private set;
        }

        public bool RemoveComments
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
                { "i=",            v => this.Input = v },
                { "input=",        v => this.Input = v },

                { "t=",            v => this.Transform = v },
                { "xdt=",          v => this.Transform = v },

                { "o=",            v => this.Output = v },
                { "output=",       v => this.Output = v },

                { "rm-comment",    v => this.RemoveComments = true },
                { "h|help",        v => this.Help = true },
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
            if ( string.IsNullOrEmpty( this.Input ) == true )
            {
                Console.Error.WriteLine( "error: input file is mandatory (use --input=INPUT)." );
                return false;
            }


            /*
             * .Transform
             */
            if ( string.IsNullOrEmpty( this.Transform ) == true )
            {
                Console.Error.WriteLine( "error: transformation file is mandatory (use --xdt=XDT)." );
                return false;
            }


            /*
             * .Output
             */
            if ( string.IsNullOrEmpty( this.Output ) == true )
            {
                Console.Error.WriteLine( "error: output file is mandatory (use --output=OUTPUT)." );
                return false;
            }

            return true;
        }


        public void HelpShow()
        {
            Console.WriteLine( "usage: yxdt [OPTIONS] --input=[INPUT] --xdt=[XDT] --output=[OUTPUT]" );
            Console.WriteLine( "  --rm-comment        Removes all comments from the input file" );
            Console.WriteLine( "  --help              This help message. :-)" );
        }
    }
}

/* eof */