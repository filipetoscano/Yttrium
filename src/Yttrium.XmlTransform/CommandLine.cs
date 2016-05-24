using NDesk.Options;
using System;
using System.Collections.Generic;

namespace Yttrium.XmlTransform
{
    public class CommandLine
    {
        /// <summary>
        /// Input file.
        /// </summary>
        public string Input
        {
            get;
            private set;
        }

        /// <summary>
        /// XDT transform.
        /// </summary>
        public string Transform
        {
            get;
            private set;
        }

        /// <summary>
        /// Output file.
        /// </summary>
        public string Output
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether whitespace in the source file should be preserved.
        /// </summary>
        public bool PreserveWhitespace
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether to remove all comments.
        /// </summary>
        public bool RemoveComments 
        {
            get;
            private set;
        }

        /// <summary>
        /// Display the help for the current command?
        /// </summary>
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

                { "w|preserve-whitespace", v => this.PreserveWhitespace = true },
                { "c|rm-comment",  v => this.RemoveComments = true },
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