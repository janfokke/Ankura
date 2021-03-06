// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MS-PL license. See LICENSE file in the Git repository root directory for full license information.

using System.IO;

namespace Ankura
{
    public static class TitleContainer
    {
        public static Stream OpenStream(string name)
        {
            string safeName = FileHelpers.NormalizeFilePathSeparators(name);
            return File.OpenRead(Path.IsPathRooted(safeName) ? safeName : Path.Combine(TitleLocation.Path, safeName));
        }

        internal static byte[] ReadAllBytes(string name)
        {
            string safeName = FileHelpers.NormalizeFilePathSeparators(name);
            return File.ReadAllBytes(Path.IsPathRooted(safeName) ? safeName : Path.Combine(TitleLocation.Path, safeName));
        }
    }
}
