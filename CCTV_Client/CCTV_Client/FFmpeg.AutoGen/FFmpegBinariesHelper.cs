using System;
using System.IO;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen.Bindings.DynamicallyLoaded; // additional installation from NuGet
using FFmpeg.AutoGen.Bindings.DynamicallyLinked; // additional installation from NuGet
using FFmpeg.AutoGen.Bindings.StaticallyLinked; // additional installation from NuGet

namespace CCTV_Server
{
    public class FFmpegBinariesHelper {
        // original argument: void
        internal static void RegisterFFmpegBinaries(string binaryPath) {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                /*
                var current = Environment.CurrentDirectory;
                var probe = Path.Combine("FFmpeg", "bin", Environment.Is64BitProcess ? "x64" : "x86");

                while (current != null) {
                    var ffmpegBinaryPath = Path.Combine(current, probe);

                    if (Directory.Exists(ffmpegBinaryPath)) {
                        Console.WriteLine($"FFmpeg binaries found in: {ffmpegBinaryPath}");
                        //DynamicallyLoadedBindings.LibrariesPath = ffmpegBinaryPath;
                        FFmpeg.AutoGen.Bindings.DynamicallyLoaded.DynamicallyLoadedBindings.LibrariesPath = ffmpegBinaryPath;
                        return;
                    }

                    current = Directory.GetParent(current)?.FullName;
                }
                */
                DynamicallyLoadedBindings.LibrariesPath = binaryPath;
                DynamicallyLoadedBindings.Initialize();
            }
            else
                throw new NotSupportedException(); // fell free add support for platform of your choose
        }
    }
}


