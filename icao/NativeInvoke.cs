using System.Runtime.InteropServices;

namespace ikao
{
    public class NativeInvoke
    {
        public enum ReturnCode
        {
            Success = 0,
            ImageReadingError,
            ImageWritingError,
            MissingConfigParamError,
            UnknownConfigParamError,
            FaceDetectionError,
            FaceLandmarkExtractionError,
            FaceOcclusionSegmentationError,
            FaceParsingError,
            UnknownError,
            QualityAssessmentError,
            NotImplemented,
            MissingConfigFileError
        }

        // Эквивалент QualityMeasure из ofiq_structs.h
        public enum QualityMeasure : int
        {
            UnifiedQualityScore = 0x41,
            BackgroundUniformity = 0x42,
            IlluminationUniformity = 0x43,
            LuminanceMean = 0x44,
            LuminanceVariance = 0x45,
            UnderExposurePrevention = 0x46,
            OverExposurePrevention = 0x47,
            DynamicRange = 0x48,
            Sharpness = 0x49,
            CompressionArtifacts = 0x4a,
            NaturalColour = 0x4b,
            SingleFacePresent = 0x4c,
            EyesOpen = 0x4d,
            MouthClosed = 0x4e,
            EyesVisible = 0x4f,
            MouthOcclusionPrevention = 0x50,
            FaceOcclusionPrevention = 0x51,
            InterEyeDistance = 0x52,
            HeadSize = 0x53,
            LeftwardCrop = 0x54,
            RightwardCrop = 0x55,
            MarginAbove = 0x56,
            MarginBelow = 0x57,
            HeadPoseYaw = 0x58,
            HeadPosePitch = 0x59,
            HeadPoseRoll = 0x5a,
            ExpressionNeutrality = 0x5b,
            NoHeadCoverings = 0x5c,

            Luminance = -0x44,
            CropOfTheFaceImage = -0x54,
            HeadPose = -0x58,
            NotSet = -1
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BridgeAssessment
        {
            public int Measure;
            public double RawScore;
            public double Scalar;
            public int Code;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BridgeBoundingBox
        {
            public short X;
            public short Y;
            public short Width;
            public short Height;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct BridgeReturnStatus
        {
            public int Code;
            public IntPtr Info;
            public string GetInfo() => Info == IntPtr.Zero ? string.Empty : Marshal.PtrToStringAnsi(Info) ?? string.Empty;
        }

        private const string LibraryName = "ofiq_bridge";

        [DllImport(LibraryName)] public static extern IntPtr ofiq_get_implementation();
        [DllImport(LibraryName)] public static extern void ofiq_destroy_implementation(IntPtr handle);
        [DllImport(LibraryName)] public static extern BridgeReturnStatus ofiq_initialize(IntPtr handle, string configDir, string configFileName);
        [DllImport(LibraryName)] public static extern void free_status(BridgeReturnStatus status);
        [DllImport(LibraryName)] public static extern BridgeReturnStatus ofiq_get_version(IntPtr handle, out int major, out int minor, out int patch);
        [DllImport(LibraryName)] public static extern BridgeReturnStatus ofiq_vector_quality(IntPtr handle, ushort width, ushort height, byte depth, IntPtr data, [Out] BridgeAssessment[] results, out int count, out BridgeBoundingBox bbox);

        [StructLayout(LayoutKind.Sequential)]
        public struct BridgeLandmark
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BridgePreprocessingResult
        {
            public int LandmarkCount;
            public IntPtr Landmarks;
            public IntPtr SegmentationMask;
            public IntPtr OcclusionMask;
        }

        [DllImport(LibraryName)]
        public static extern BridgeReturnStatus ofiq_vector_quality_with_preprocessing(
            IntPtr handle,
            ushort width, ushort height, byte depth, IntPtr data,
            [Out] BridgeAssessment[] results, out int count,
            out BridgeBoundingBox bbox,
            ref BridgePreprocessingResult preproc);
    }
}
