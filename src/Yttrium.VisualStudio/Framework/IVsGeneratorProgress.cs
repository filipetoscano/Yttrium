using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace CustomToolGenerator
{
    [
        SuppressMessage( "Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Vs" ), ComImport,
        Guid( "BED89B98-6EC9-43CB-B0A8-41D6E2D6669D" ),
        InterfaceTypeAttribute( ComInterfaceType.InterfaceIsIUnknown )
    ]
    public interface IVsGeneratorProgress
    {
        //
        // Communicate errors
        // HRESULT GeneratorError([in] BOOL fWarning,
        //                        [in] DWORD dwLevel,
        //                        [in] BSTR bstrError,
        //                        [in] DWORD dwLine,
        //                        [in] DWORD dwColumn);
        //
        [
            SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "f" ),
            SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dw" ),
            SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "bstr" )
        ]
        void GeneratorError( bool fWarning,
              [MarshalAs( UnmanagedType.U4 )] int dwLevel,
            [MarshalAs( UnmanagedType.BStr )] string bstrError,
              [MarshalAs( UnmanagedType.U4 )] int dwLine,
              [MarshalAs( UnmanagedType.U4 )] int dwColumn );

        //
        // Report progress to the caller.
        // HRESULT Progress([in] ULONG nComplete,        // Current position
        //                  [in] ULONG nTotal);          // Max value
        //
        [
            SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "n" )
        ]
        void Progress(
            [MarshalAs( UnmanagedType.U4 )] int nComplete,
            [MarshalAs( UnmanagedType.U4 )] int nTotal );
    }
}

/* eof */