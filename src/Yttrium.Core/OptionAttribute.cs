using System;

namespace Yttrium.Core
{
    [AttributeUsage( AttributeTargets.Property )]
    public class OptionAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the short command-line flag for the current
        /// option.
        /// </summary>
        public char Short
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the long command-line flag for the current
        /// option.
        /// </summary>
        public string Long
        {
            get;
            set;
        }


        public string Description
        {
            get;
            set;
        }
    }
}
