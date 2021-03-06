// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MS-PL license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Runtime.InteropServices;

namespace Ankura
{
    // http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.audio.noaudiohardwareexception.aspx
    [Serializable]
    public sealed class NoAudioHardwareException : ExternalException
    {
        public NoAudioHardwareException()
        {
        }

        public NoAudioHardwareException(string message)
            : base(message)
        {
        }

        public NoAudioHardwareException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
