using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace CustomToolGenerator
{
    [
        ComImport,
        Guid( "6D5140C1-7436-11CE-8034-00AA006009FA" ),
        InterfaceTypeAttribute( ComInterfaceType.InterfaceIsIUnknown )
    ]
    public interface IOleServiceProvider
    {
        [
            SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "riid" ), 
            SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "ppv" ), SuppressMessage( "Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#" ),
            SuppressMessage( "Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#" ),
            SuppressMessage( "Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#" ), 
            PreserveSig
        ]
        int QueryService( [In]ref Guid guidService, [In] ref Guid riid, out IntPtr ppvObject );
    }
}

/* eof */