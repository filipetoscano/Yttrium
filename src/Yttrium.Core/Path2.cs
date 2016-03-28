using System;
using System.Collections.Generic;
using System.IO;
using IoPath = System.IO.Path;

namespace Yttrium
{
    public static class Path2
    {
        /// <summary>
        /// Returns the relative path from the current working directory to the
        /// specified file.
        /// </summary>
        /// <param name="fileName">Target file.</param>
        /// <returns>Relative path.</returns>
        public static string RelativePath( string fileName )
        {
            #region Validations

            if ( fileName == null )
                throw new ArgumentNullException( "fileName" );

            #endregion

            return RelativePath( string.Concat( Environment.CurrentDirectory, Path.DirectorySeparatorChar ), fileName );
        }

        /// <summary>
        /// Returns the relative path from the specified directory to the specified
        /// file.
        /// </summary>
        /// <param name="directoryName">Source directory.</param>
        /// <param name="fileName">Target file.</param>
        /// <returns>Relative path.</returns>
        public static string RelativePath( string directoryName, string fileName )
        {
            #region Validations

            if ( directoryName == null )
                throw new ArgumentNullException( "directoryName" );

            if ( fileName == null )
                throw new ArgumentNullException( "fileName" );

            #endregion

            Uri uriTo = new Uri( fileName );
            Uri uriFrom = new Uri( directoryName );

            Uri rel = uriFrom.MakeRelativeUri( uriTo );

            return Uri.UnescapeDataString( rel.ToString() );
        }

        /// <summary>
        /// Evaluates the list of file patterns and/or file nam,es, and returns the full list 
        /// of unique (matching) files.
        /// </summary>
        /// <param name="paths">List of file patterns.</param>
        /// <returns>List of unique file-names.</returns>
        /// <remarks>
        /// Patterns are evaluated based on the current working directory.
        /// </remarks>
        public static string[] ToUnique( string[] paths )
        {
            if ( paths == null || paths.Length == 0 )
                return new string[] { };


            List<string> files = new List<string>();

            foreach ( string relativePattern in paths )
            {
                string pattern = IoPath.Combine( Environment.CurrentDirectory, relativePattern );

                if ( pattern.IndexOf( '*' ) > -1 || pattern.IndexOf( '?' ) > -1 )
                {
                    string directoryName = IoPath.GetDirectoryName( pattern );
                    string patn = IoPath.GetFileName( pattern );

                    DirectoryInfo directory = new DirectoryInfo( directoryName );

                    if ( directory.Exists == false )
                        continue;

                    foreach ( FileInfo i in directory.GetFiles( patn ) )
                    {
                        if ( files.Contains( i.FullName ) == false )
                            files.Add( i.FullName );
                    }
                }
                else
                {
                    FileInfo file = new FileInfo( pattern );

                    if ( files.Contains( file.FullName ) == false )
                        files.Add( file.FullName );
                }
            }

            return files.ToArray();
        }
    }
}

/* eof */