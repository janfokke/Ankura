// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MS-PL license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Ankura
{
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Symbols.")]
    [SuppressMessage("ReSharper", "SA1300", Justification = "Symbols.")]
    internal static class libdl
    {
        private const string LibraryName = "libdl";

        [DllImport(LibraryName, CharSet = CharSet.Ansi)]
        public static extern IntPtr dlopen(string fileName, int flags);

        [DllImport(LibraryName, CharSet = CharSet.Ansi)]
        public static extern IntPtr dlsym(IntPtr handle, string name);

        [DllImport(LibraryName)]
        public static extern int dlclose(IntPtr handle);
    }
}
