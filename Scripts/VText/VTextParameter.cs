using UnityEngine;

// ReSharper disable StringLiteralTypo
namespace EthansGameKit.VText
{
	/// <summary>
	///     VText parameter.
	///     change requires rebuild of glyp meshes
	/// </summary>
	[System.Serializable]
	class VTextParameter
	{
		bool m_modified;
		/// <summary>
		///     The depth of the glyphs.
		///     getter setter
		/// </summary>
		public float Depth
		{
			get => m_depth;
			set
			{
				var v = value < 0.0f ? 0.0f : value;
				if (m_depth != v)
				{
					m_depth = v;
					m_modified = true;
				}
			}
		}
		public bool Is3D => Depth != 0 || Bevel != 0;
		/// <summary>
		///     The crease angle to generate sides and bevel
		///     getter setter
		///     range [10..45]
		/// </summary>
		public float Crease
		{
			get => m_crease;
			set
			{
				var v = Mathf.Clamp(value, 10f, 45f);
				if (m_crease != v)
				{
					m_crease = v;
					m_modified = true;
				}
			}
		}
		/// <summary>
		///     The bevel frame of the glyphs.
		///     getter setter
		///     range [0..1] where 1 is max factor of 1/10 width of glyph
		/// </summary>
		public float Bevel
		{
			get => m_bevel;
			set
			{
				var v = Mathf.Clamp01(value);
				if (m_bevel != v)
				{
					m_bevel = v;
					m_modified = true;
				}
			}
		}
		/// <summary>
		///     Flag generate backface
		///     getter setter
		/// </summary>
		public bool Backface
		{
			get => m_backface;
			set
			{
				if (m_backface != value)
				{
					m_backface = value;
					m_modified = true;
				}
			}
		}
		/// <summary>
		///     Flag generate tangents
		///     getter setter
		/// </summary>
		public bool GenerateTangents
		{
			get => m_needTangents;
			set
			{
				if (m_needTangents != value)
				{
					m_needTangents = value;
					m_modified = true;
				}
			}
		}
		/// <summary>
		///     Fontname
		///     getter setter
		/// </summary>
		public string Fontname
		{
			get => m_fontname;
			set
			{
				if (m_fontname != value)
				{
					m_fontname = value;
					m_modified = true;
				}
			}
		}
		public bool CheckClearModified()
		{
			if (m_modified)
			{
				m_modified = false;
				return true;
			}
			return false;
		}
		#region parameter
		/// <summary>
		///     The depth of the glyphs.
		/// </summary>
		[SerializeField] float m_depth;
		/// <summary>
		///     The bevel frame of the glyphs.
		///     range [0..1] where 1 is max factor of 1/10 width of glyph
		/// </summary>
		[SerializeField] float m_bevel;
		/// <summary>
		///     The need tangents property
		///     If set, tangents will be generated for Mesh
		/// </summary>
		[SerializeField] bool m_needTangents;
		/// <summary>
		///     create backface
		///     If set, backface will be generated for Mesh
		/// </summary>
		[SerializeField] bool m_backface;
		/// <summary>
		///     crease angle
		///     in degree for smoothing sides and bevel.
		/// </summary>
		[SerializeField] float m_crease = 35.0f;
		/// <summary>
		///     The fontname must specify a font available in StreamingAsset
		///     folder.
		///     Accepted formats are:
		///     - ttf
		///     - otf
		///     - ps (Postscript)
		/// </summary>
		[SerializeField] string m_fontname = "mittelschrift.otf";
		#endregion
	}
}
