using System.Runtime.InteropServices;

namespace EthansGameKit.VText
{
	/// <summary>
	///     Font bounds.
	///     Returned from GetFontBounds.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	struct FontBounds
	{
		/// <summary>
		///     The minimum x.
		/// </summary>
		public readonly float minX;
		/// <summary>
		///     The max x.
		/// </summary>
		public readonly float maxX;
		/// <summary>
		///     The minimum y.
		/// </summary>
		public readonly float minY;
		/// <summary>
		///     The max y.
		/// </summary>
		public readonly float maxY;
	}
}
