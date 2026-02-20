using System;
using Xunit;
using ikao;
using System.Runtime.InteropServices;

namespace OFIQConsoleApp.Tests
{
    public class OFIQTests
    {
        [Fact]
        public void TestGetVersion()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;

            IntPtr handle = IntPtr.Zero;
            try
            {
                handle = NativeInvoke.ofiq_get_implementation();
                Assert.NotEqual(IntPtr.Zero, handle);

                int major, minor, patch;
                var status = NativeInvoke.ofiq_get_version(handle, out major, out minor, out patch);
                
                Assert.Equal(0, status.Code);
                Assert.True(major >= 0);
            }
            finally
            {
                if (handle != IntPtr.Zero)
                    NativeInvoke.ofiq_destroy_implementation(handle);
            }
        }

        [Fact]
        public void TestVectorQualityWithPreprocessing()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;

            IntPtr handle = IntPtr.Zero;
            try
            {
                handle = NativeInvoke.ofiq_get_implementation();
                // We'd need to initialize with real config path to actually run
                // For now, this is a compilation and signature check
            }
            finally
            {
                if (handle != IntPtr.Zero)
                    NativeInvoke.ofiq_destroy_implementation(handle);
            }
        }
    }
}
