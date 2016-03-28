using System;

namespace Yttrium.VisualStudio
{
    [Serializable]
    public class ToolException : Exception
    {
        public ToolException( string message )
            : base( message )
        {
        }


        public ToolException( string message, Exception innerException )
            : base( message, innerException )
        {
        }
    }
}

/* eof */