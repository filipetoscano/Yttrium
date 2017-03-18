using System;
using System.Collections.Generic;
using System.IO;

namespace Yttrium.Scaffold
{
    /// <summary />
    public class ExecutionContext
    {
        /// <summary>
        /// List of key-value pairs which we are replacing in runtime.
        /// </summary>
        public Dictionary<string, string> Values { get; set; }


        /// <summary>
        /// Root directory of the template.
        /// </summary>
        public DirectoryInfo FromRoot { get; set; }


        /// <summary>
        /// Root directory to which we are mirroring to.
        /// </summary>
        public DirectoryInfo ToRoot { get; set; }


        /// <summary>
        /// Returns the relative path of <see cref="path" /> to the 
        /// output directory.
        /// </summary>
        public string Relative( string path )
        {
            #region Validations

            if ( path == null )
                throw new ArgumentNullException( nameof( path ) );

            #endregion

            var s = Path2.RelativePath( this.ToRoot.FullName, path );

            return s.Substring( s.IndexOf( "/" ) + 1 );
        }
    }
}
