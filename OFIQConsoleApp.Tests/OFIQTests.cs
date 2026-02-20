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
        public void TestBridgePreprocessingResultInit()
        {
            var preproc = new NativeInvoke.BridgePreprocessingResult
            {
                LandmarkCount = 10,
                Landmarks = (IntPtr)1234,
                SegmentationMask = (IntPtr)5678,
                OcclusionMask = (IntPtr)9012
            };
            Assert.Equal(10, preproc.LandmarkCount);
            Assert.Equal((IntPtr)1234, preproc.Landmarks);
            Assert.Equal((IntPtr)5678, preproc.SegmentationMask);
            Assert.Equal((IntPtr)9012, preproc.OcclusionMask);
        }

        [Fact]
        public void TestBridgeReturnStatusGetInfo()
        {
            var status = new NativeInvoke.BridgeReturnStatus
            {
                Code = 0,
                Info = IntPtr.Zero
            };
            Assert.Equal(string.Empty, status.GetInfo());

            string testInfo = "Test Error Message";
            var ptr = Marshal.StringToHGlobalAnsi(testInfo);
            try {
                status.Info = ptr;
                Assert.Equal(testInfo, status.GetInfo());
            }
            finally {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
