// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MS-PL license. See LICENSE file in the Git repository root directory for full license information.

using System.Numerics;

namespace Ankura.Samples
{
    internal struct Vertex : IVertexType
    {
        public Vector3 Position;

        public static readonly VertexDeclaration Declaration;

        VertexDeclaration IVertexType.VertexDeclaration => Declaration;

        static Vertex()
        {
            var elements = new[]
            {
                new VertexElement(
                    0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            };
            Declaration = new VertexDeclaration(elements);
        }
    }
}
