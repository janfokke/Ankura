#region License
/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2020 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */

/* Derived from code by the Mono.Xna Team (Copyright 2006).
 * Released under the MIT License. See monoxna.LICENSE for details.
 */
#endregion

using System.Numerics;

namespace Microsoft.Xna.Framework.Content
{
	internal class Vector4Reader : ContentTypeReader<Vector4>
	{
		#region Internal Constructor

		internal Vector4Reader()
		{
		}

		#endregion

		#region Protected Read Method

		protected internal override Vector4 Read(
			ContentReader input,
			Vector4 existingInstance
		) {
			return input.ReadVector4();
		}

		#endregion
	}
}