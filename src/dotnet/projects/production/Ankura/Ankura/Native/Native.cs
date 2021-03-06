// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MS-PL license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ankura
{
    public static class Native
    {
        private static IEnumerable<string>? _librarySearchDirectories;
        private static RuntimePlatform? _platform;

        public static RuntimePlatform RuntimePlatform
        {
            get
            {
                _platform ??= GetRuntimePlatform();
                return _platform.Value;
            }
        }

        public static void SetDllImportResolver(Assembly assembly)
        {
            NativeLibrary.SetDllImportResolver(assembly, Resolver);
        }

        [SuppressMessage("ReSharper", "CommentTypo", Justification = "Flags.")]
        public static IntPtr LoadLibrary(string libraryFilePath)
        {
            return RuntimePlatform switch
            {
                RuntimePlatform.Linux => libdl.dlopen(libraryFilePath, 0x101),
                RuntimePlatform.Windows => Kernel32.LoadLibrary(libraryFilePath),
                RuntimePlatform.macOS => libSystem.dlopen(libraryFilePath, 0x101),
                _ => IntPtr.Zero
            };
        }

        public static bool FreeLibrary(IntPtr libraryHandle)
        {
            return RuntimePlatform switch
            {
                RuntimePlatform.Linux => libdl.dlclose(libraryHandle) == 0,
                RuntimePlatform.Windows => Kernel32.FreeLibrary(libraryHandle) != 0,
                RuntimePlatform.macOS => libSystem.dlclose(libraryHandle) == 0,
                _ => false
            };
        }

        public static IntPtr GetLibraryFunctionPointer(IntPtr libraryHandle, string functionName)
        {
            return RuntimePlatform switch
            {
                RuntimePlatform.Linux => libdl.dlsym(libraryHandle, functionName),
                RuntimePlatform.Windows => Kernel32.GetProcAddress(libraryHandle, functionName),
                RuntimePlatform.macOS => libSystem.dlsym(libraryHandle, functionName),
                _ => IntPtr.Zero
            };
        }

        public static T GetLibraryFunction<T>(IntPtr libraryHandle)
        {
            return GetLibraryFunction<T>(libraryHandle, string.Empty);
        }

        public static T GetLibraryFunction<T>(IntPtr libraryHandle, string functionName)
        {
            if (string.IsNullOrEmpty(functionName))
            {
                functionName = typeof(T).Name;
                if (functionName.StartsWith("d_", StringComparison.Ordinal))
                {
                    functionName = functionName.Substring(2);
                }
            }

            var functionHandle = GetLibraryFunctionPointer(libraryHandle, functionName);
            if (functionHandle == IntPtr.Zero)
            {
                throw new Exception($"Could not find a function with the given name '{functionName}' in the library.");
            }

            return Marshal.GetDelegateForFunctionPointer<T>(functionHandle);
        }

        private static RuntimePlatform GetRuntimePlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return RuntimePlatform.Windows;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return RuntimePlatform.macOS;
            }

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return RuntimePlatform.Linux;
            }

            return RuntimePlatform.Unknown;
        }

        private static string GetLibraryFileExtension(RuntimePlatform platform)
        {
            return platform switch
            {
                RuntimePlatform.Windows => ".dll",
                RuntimePlatform.macOS => ".dylib",
                RuntimePlatform.Linux => ".so",
                RuntimePlatform.Android => throw new NotImplementedException(),
                RuntimePlatform.iOS => throw new NotImplementedException(),
                RuntimePlatform.Unknown => throw new NotSupportedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(platform), platform, null)
            };
        }

        private static string GetRuntimeIdentifier()
        {
            return RuntimePlatform switch
            {
                RuntimePlatform.Windows => Environment.Is64BitProcess ? "win-x64" : "win-x86",
                RuntimePlatform.macOS => "osx-x64",
                RuntimePlatform.Linux => "linux-x64",
                RuntimePlatform.Android => throw new NotImplementedException(),
                RuntimePlatform.iOS => throw new NotImplementedException(),
                RuntimePlatform.Unknown => throw new NotSupportedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(RuntimePlatform), RuntimePlatform, null)
            };
        }

        private static IEnumerable<string> GetSearchDirectories()
        {
            if (_librarySearchDirectories != null)
            {
                return _librarySearchDirectories;
            }

            var runtimeIdentifier = GetRuntimeIdentifier();

            var librarySearchDirectories = new List<string>();

            librarySearchDirectories.Add(Environment.CurrentDirectory);
            librarySearchDirectories.Add(AppDomain.CurrentDomain.BaseDirectory);
            librarySearchDirectories.Add($"libs/{runtimeIdentifier}");
            librarySearchDirectories.Add($"runtimes/{runtimeIdentifier}/native");

            return _librarySearchDirectories = librarySearchDirectories.ToArray();
        }

        private static bool TryGetLibraryPath(string libraryName, out string libraryFilePath)
        {
            var libraryPrefix = RuntimePlatform == RuntimePlatform.Windows ? string.Empty : "lib";
            var libraryFileExtension = GetLibraryFileExtension(RuntimePlatform);
            var libraryFileName = $"{libraryPrefix}{libraryName}";

            var directories = GetSearchDirectories();
            foreach (var directory in directories)
            {
                if (TryFindLibraryPath(directory, libraryFileExtension, libraryFileName, out libraryFilePath))
                {
                    return true;
                }
            }

            libraryFilePath = string.Empty;
            return false;
        }

        private static bool TryFindLibraryPath(
            string directoryPath,
            string libraryFileExtension,
            string libraryFileNameWithoutExtension,
            out string result)
        {
            if (!Directory.Exists(directoryPath))
            {
                result = string.Empty;
                return false;
            }

            var searchPattern = $"*{libraryFileExtension}";
            var filePaths = Directory.EnumerateFiles(directoryPath, searchPattern);
            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                if (fileName.StartsWith(libraryFileNameWithoutExtension))
                {
                    result = filePath;
                    return true;
                }
            }

            result = string.Empty;
            return false;
        }

        private static IntPtr Resolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            IntPtr libraryHandle;

            if (TryGetLibraryPath(libraryName, out var libraryFilePath))
            {
                libraryHandle = LoadLibrary(libraryFilePath);
                return libraryHandle;
            }

            if (NativeLibrary.TryLoad(libraryName, assembly, searchPath, out libraryHandle))
            {
                return libraryHandle;
            }

            throw new Exception($"Could not find the native library: {libraryName}. Did you forget to place a native library in the correct file path?");
        }
    }
}
