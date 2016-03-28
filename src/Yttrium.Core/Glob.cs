using System;
using System.Collections.Generic;
using System.IO;

namespace Yttrium
{
    public static class Glob
    {
        public static string[] Do( string[] patterns )
        {
            /*
             * 
             */
            if ( patterns == null )
                return null;

            if ( patterns.Length == 0 )
                return null;


            /*
             * 
             */
            List<string> list = new List<string>();

            foreach ( string pattern in patterns )
            {
                string[] partial = Do( pattern );

                if ( partial != null )
                {
                    foreach ( string file in partial )
                    {
                        if ( list.Contains( file ) == false )
                            list.Add( file );
                    }
                }
            }

            list.Sort();
            return list.ToArray();
        }


        public static string[] Do( string pattern )
        {
            #region Validations

            if ( pattern == null )
                throw new ArgumentNullException( "pattern" );

            #endregion

            /*
             * 
             */
            string rempat;
            string baseDirectory;
            string directoryPattern = null;
            string filePattern;
            int ix;


            /*
             * Does the path begin from the root? Either of the current drive (a single slash),
             * or from the different drive? Otherwise, the pattern refers to a relative path
             * from the working directory.
             */
            if ( Path.IsPathRooted( pattern ) == true )
            {
                baseDirectory = Path.GetPathRoot( pattern );
                rempat = pattern.Substring( baseDirectory.Length );
            }
            else
            {
                baseDirectory = Environment.CurrentDirectory;
                rempat = pattern;
            }


            /*
             * Remove the file pattern.
             */
            ix = rempat.LastIndexOf( Path.DirectorySeparatorChar );

            if ( ix < 0 )
            {
                filePattern = rempat;
                rempat = null;
            }
            else
            {
                filePattern = rempat.Substring( ix + 1 );
                rempat = rempat.Substring( 0, ix );
            }


            /*
             * Consume the remainder of the initial path (ie, without the root and the file
             * pattern) determining whether we should append it to the base directory pattern
             * (since it is fixed) or to another pattern (which we will use to match 
             * directories).
             */
            while ( rempat != null )
            {
                ix = rempat.IndexOf( Path.DirectorySeparatorChar );

                string iter;

                if ( ix < 0 )
                {
                    iter = rempat;
                    rempat = null;
                }
                else
                {
                    iter = rempat.Substring( 0, ix );
                    rempat = rempat.Substring( ix + 1 );
                }

                if ( iter == "**" && rempat != null )
                {
                    throw new ApplicationException( "Once ** is specified, no additional directories may be present in pattern." );
                }
                else if ( iter.Contains( "*" ) == true || iter.Contains( "?" ) == true )
                {
                    if ( rempat == null )
                        directoryPattern = iter;
                    else
                        directoryPattern = Path.Combine( iter, rempat );
                    break;
                }
                else
                {
                    baseDirectory = Path.Combine( baseDirectory, iter );
                }
            }

            return DoGlob( baseDirectory, directoryPattern, filePattern );
        }


        private static string[] DoGlob( string baseDirectory, string directoryPattern, string filePattern )
        {
            #region Validations

            if ( baseDirectory == null )
                throw new ArgumentNullException( "baseDirectory" );

            if ( filePattern == null )
                throw new ArgumentNullException( "filePattern" );

            #endregion

            /*
             * Easy bit: we're already at the leaf of the tree (ie, null directoryPattern) or
             * we're at a point where we can recurse through all of the sub-directories
             * looking for the desired file (ie, ** == full recurse).
             */
            if ( directoryPattern == null || directoryPattern == "**" )
            {
                SearchOption option = SearchOption.TopDirectoryOnly;

                if ( directoryPattern == "**" )
                    option = SearchOption.AllDirectories;

                string[] files = null;

                try
                {
                    files = Directory.GetFiles( baseDirectory, filePattern, option );
                }
                catch ( DirectoryNotFoundException )
                {
                    // not a problem!
                }
                catch ( UnauthorizedAccessException )
                {
                    // not a problem!
                }

                return files;
            }


            /*
             * Not so easy bit (but not exactly hard either): we need to pop the first 
             * directory from the directory pattern and...
             */
            int ix = directoryPattern.IndexOf( Path.DirectorySeparatorChar );

            string searchPattern;
            string remainderPattern;

            if ( ix < 0 )
            {
                searchPattern = directoryPattern;
                remainderPattern = null;
            }
            else
            {
                searchPattern = directoryPattern.Substring( 0, ix );
                remainderPattern = directoryPattern.Substring( ix + 1 );
            }


            /*
             * ... we recurse into all directories which match the (current) search pattern.
             * All of the new calls to DoGlob() will have a larger base directory, while
             * having a smaller directory pattern (until it becomes empty, or **).
             */
            string[] dirs = null;

            try
            {
                dirs = Directory.GetDirectories( baseDirectory, searchPattern, SearchOption.TopDirectoryOnly );
            }
            catch ( DirectoryNotFoundException )
            {
                // not a problem!
            }
            catch ( UnauthorizedAccessException )
            {
                // not a problem!
            }

            if ( dirs == null || dirs.Length == 0 )
                return null;

            List<string> agg = new List<string>();

            foreach ( string dir in dirs )
            {
                string newBase = Path.Combine( baseDirectory, dir );

                string[] f = DoGlob( newBase, remainderPattern, filePattern );

                if ( f != null )
                    agg.AddRange( f );
            }

            return agg.ToArray();
        }
    }
}

/* eof */