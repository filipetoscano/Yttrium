
namespace Yttrium.DbManifest
{
    public class XslExtensionObject
    {
        public string pad( string value, int totalWidth )
        {
            if ( value == null )
                value = "";

            return value.PadRight( totalWidth, ' ' );
        }
    }
}

/* eof */