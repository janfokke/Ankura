// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MS-PL license. See LICENSE file in the Git repository root directory for full license information.

using System;

namespace Ankura
{
    public class PreparingDeviceSettingsEventArgs : EventArgs
    {
        public GraphicsDeviceInformation GraphicsDeviceInformation { get; }

        public PreparingDeviceSettingsEventArgs(GraphicsDeviceInformation graphicsDeviceInformation)
        {
            GraphicsDeviceInformation = graphicsDeviceInformation;
        }
    }
}
