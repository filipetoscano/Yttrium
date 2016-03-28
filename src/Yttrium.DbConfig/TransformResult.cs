using System;
using System.Collections.Generic;

namespace Yttrium.DbConfig
{
    public class TransformResult
    {
        private List<string> _errors = new List<string>();
        private List<string> _warns = new List<string>();
        private List<string> _info = new List<string>();


        public IList<string> Errors
        {
            get { return _errors; }
        }

        public IList<string> Warnings
        {
            get { return _warns; }
        }

        public IList<string> Info
        {
            get { return _info; }
        }

        public void Add( string message )
        {
            #region Validations

            if ( message == null )
                throw new ArgumentNullException( "message" );

            #endregion

            if ( message.StartsWith( "ERR:", StringComparison.Ordinal ) == true )
            {
                _errors.Add( message.Substring( 4 ) );
            }
            else if ( message.StartsWith( "WRN:", StringComparison.Ordinal ) == true )
            {
                _warns.Add( message.Substring( 4 ) );
            }
            else if ( message.StartsWith( "INF:", StringComparison.Ordinal ) == true )
            {
                _info.Add( message.Substring( 4 ) );
            }
            else
            {
                _errors.Add( message );
            }
        }
    }
}

/* eof */