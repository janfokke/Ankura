// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MS-PL license. See LICENSE file in the Git repository root directory for full license information.

using System;

namespace Ankura
{
    [Flags]
    public enum ClearOptions
    {
        Target = 1,
        DepthBuffer = 2,
        Stencil = 4
    }
}
