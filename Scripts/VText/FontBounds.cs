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
		public float minX;
		/// <summary>
		///     The max x.
		/// </summary>
		public float maxX;
		/// <summary>
		///     The minimum y.
		/// </summary>
		public float minY;
		/// <summary>
		///     The max y.
		/// </summary>
		public float maxY;
	}
}
