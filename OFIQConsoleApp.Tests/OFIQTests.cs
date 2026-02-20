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
            // Skip on non-linux systems for execution, but we check compilation
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
    }
}