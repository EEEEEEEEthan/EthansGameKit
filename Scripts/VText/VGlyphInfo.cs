using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// ReSharper disable IdentifierTypo

// ReSharper disable CommentTypo
namespace EthansGameKit.VText
{
	namespace VText
	{
		/// <summary>
		///     VGlyph info
		///     contains Mesh and aditional info of specified glyph.
		/// </summary>
		class VGlyphInfo
		{
			readonly char id;
			//Mesh mesh;
			readonly System.IntPtr fh;
			readonly int numContours;
			Vector3[][] contours;

			[StructLayout(LayoutKind.Sequential)]
			struct GlyphInfo
			{
				readonly float sizeX;
				readonly float sizeY;
				readonly float advanceX;
				readonly float advanceY;
				readonly float horizBearingX;
				readonly float horizBearingY;
				readonly float vertBearingX;
				readonly float vertBearingY;
				public readonly int numContours;
			}

			class BaseAttributes
			{
				public readonly Vector3 v;
				public readonly Vector3 n;
				public readonly Vector2 uv;
				public Vector3 bv;
				public BaseAttributes(Vector3 v, Vector3 bv, Vector3 n, float dist)
				{
					this.v = new(v.x, v.y, v.z);
					this.bv = new(bv.x, bv.y, bv.z);
					this.n = new(n.x, n.y, n.z);
					uv = new(0.0f, dist);
				}
			}

#if UNITY_IPHONE || UNITY_XBOX360
			[DllImport("__Internal")]
			private static extern bool GetGlyphInfo([Out] System.IntPtr gi, [In] System.IntPtr fontHandle, [In] uint id);
			[DllImport("__Internal")]
			private static extern int GetGlyphVertices(ref System.IntPtr buffer, [In] System.IntPtr fontHandle, [In] uint id);
			[DllImport("__Internal")]
			private static extern int GetGlyphTriangleIndices(ref System.IntPtr buffer, [In] System.IntPtr fontHandle, [In] uint id);
			[DllImport("__Internal")]
			private static extern void ClearGlyphData([In] System.IntPtr fontHandle, [In] uint id);
			[DllImport("__Internal")]
			private static extern int GetGlyphContour(ref System.IntPtr buffer, [In] System.IntPtr fontHandle, [In] uint id, [In] int index, ref bool odd, ref bool reverse);
#else
			[DllImport("VText")]
			static extern bool GetGlyphInfo([Out] System.IntPtr gi, [In] System.IntPtr fontHandle, [In] uint id);
			[DllImport("VText")]
			static extern int GetGlyphVertices(ref System.IntPtr buffer, [In] System.IntPtr fontHandle, [In] uint id);
			[DllImport("VText")]
			static extern int GetGlyphTriangleIndices(ref System.IntPtr buffer, [In] System.IntPtr fontHandle, [In] uint id);
			[DllImport("VText")]
			static extern void ClearGlyphData([In] System.IntPtr fontHandle, [In] uint id);
			[DllImport("VText")]
			static extern int GetGlyphContour(ref System.IntPtr buffer, [In] System.IntPtr fontHandle, [In] uint id, [In] int index, ref bool odd, ref bool reverse);
#endif
			/// <summary>
			///     Initializes a new instance of the <see cref="VGlyphInfo" /> class.
			/// </summary>
			/// <param name="fontHandle">Font handle.</param>
			/// <param name="id">Glyph ddentifier.</param>
			public VGlyphInfo(System.IntPtr fontHandle, char id)
			{
				this.id = id;
				fh = fontHandle;
				if (System.IntPtr.Zero != fh)
				{
					// Debug.Log("fonthandle " + _fh);
					var gi = new GlyphInfo();
					var rawsize = Marshal.SizeOf(gi);
					// Debug.Log("c: " + _id + " sz: " + rawsize);
					var ip = Marshal.AllocHGlobal(rawsize);
					if (GetGlyphInfo(ip, fh, this.id))
					{
						gi = (GlyphInfo)Marshal.PtrToStructure(ip, typeof(GlyphInfo));
						numContours = gi.numContours;
					}
					Marshal.FreeHGlobal(ip);
				}
			}
			int[] sideIndices;
			int[] bevelIndices;
			readonly Vector3 zVector = new(0.0f, 0.0f, 1.0f);
			static bool FetchNext(ref Vector3 v, IReadOnlyList<Vector3> contour, int startIndex)
			{
				var act = contour[startIndex];
				for (var k = startIndex + 1; k < contour.Count; k++)
				{
					if (!act.Equals(contour[k]))
					{
						v = contour[k];
						return true;
					}
				}
				for (var k = 0; k < startIndex; k++)
				{
					if (!act.Equals(contour[k]))
					{
						v = contour[k];
						return true;
					}
				}
				return false;
			}
			/// <summary>
			///     Creates the sides and or bevel mesh of glyph mesh.
			///     in submesh.
			/// </summary>
			/// <param name="p">VTextParameter</param>
			void CreateSides(Mesh mesh, VTextParameter p)
			{
				var bevel = p.Bevel;
				sideIndices = null;
				bevelIndices = null;
				if (null != contours)
				{
					// side contours
					var aa = new List<List<BaseAttributes>>();
					var crease = Mathf.Cos(p.Crease * Mathf.PI / 180.0f);
					var maxContourLength = 0.0f;
					Vector3 prev;
					var bv1 = new Vector3(); // act bevel vertex
					var bv2 = new Vector3(); // next bevel vertex
					var nnv = new Vector3();
					for (var k = 0; k < contours.Length; k++)
					{
						if (null != contours[k])
						{
							// Debug.Log("---- _contours[" + k + "] " +_contours[k].Length + " -----------");
							if (contours[k].Length > 1)
							{
								var numContourVertices = contours[k].Length;
								if (numContourVertices > 2)
								{
									var attribs = new List<BaseAttributes>();
									prev = zVector;
									var act = contours[k][0];
									var first = contours[k][0];
									for (var j = numContourVertices - 1; j > 0; j--)
									{
										if (!act.Equals(contours[k][j]))
										{
											prev = contours[k][j];
											break;
										}
									}
									var d1 = prev - act;
									var nd1 = Vector3.Normalize(d1);
									var n1 = Vector3.Normalize(Vector3.Cross(nd1, zVector));
									var uvv = 0.0f;
									float dot;
									bool flat;
									var prevSmooth = false;
									Vector3 next;
									Vector3 d2;
									Vector3 n2;
									Vector3 nd2;
									Vector3 ndn;
									Vector3 nnn;
									Vector3 n;
									Vector3 an;
									for (var j = 1; j < numContourVertices; j++)
									{
										next = contours[k][j];
										if (next.Equals(act))
										{
										}
										else
										{
											d2 = act - next;
											nd2 = Vector3.Normalize(d2);
											n2 = Vector3.Normalize(Vector3.Cross(nd2, zVector));
											dot = Vector3.Dot(n1, n2);
											// average normal at vertex j-1
											n = Vector3.Normalize(n1 + n2);
											// act beveled vertex
											bv1.x = act.x + n.x * bevel;
											bv1.y = act.y + n.y * bevel;
											bv1.z = bevel;

											// next beveled vertex
											bv2.x = next.x + n2.x * bevel;
											bv2.y = next.y + n2.y * bevel;
											bv2.z = bevel;
											flat = !(dot > crease);
											if (flat)
											{
												if (prevSmooth)
												{
													attribs.Add(new(act, bv1, n1, uvv));
													prevSmooth = false;
												}
												// use face normal
												attribs.Add(new(act, bv1, n2, uvv));
												uvv += d2.magnitude;
												if (bevel > 0.0f)
												{
													if (FetchNext(ref nnv, contours[k], j))
													{
														ndn = Vector3.Normalize(next - nnv);
														nnn = Vector3.Normalize(Vector3.Cross(ndn, zVector));
														an = Vector3.Normalize(n2 + nnn);
														bv2.x = next.x + an.x * bevel;
														bv2.y = next.y + an.y * bevel;
														bv2.z = bevel;
													}
													else
													{
														Debug.LogWarning("fetch next failed");
													}
													attribs.Add(new(next, bv2, n2, uvv));
												}
												else
												{
													attribs.Add(new(next, next, n2, uvv));
												}
												n1 = n2;
											}
											else
											{
												// smooth
												attribs.Add(new(act, bv1, n, uvv));
												uvv += d2.magnitude;
												prevSmooth = true;
												if (j < numContourVertices - 1)
												{
													n1 = n2;
												}
											}
										}
										act = next;
									}
									// close last edge
									{
										var ridx = numContourVertices - 1;
										act = contours[k][ridx];
										while (act.Equals(first))
										{
											ridx--;
											act = contours[k][ridx];
										}
									}
									for (var j = 0; j < numContourVertices; j++)
									{
										next = contours[k][j];
										if (act.Equals(next))
										{
											// skip if p[n-1] == p[0]
										}
										else
										{
											d2 = act - next;
											nd2 = Vector3.Normalize(d2);
											n2 = Vector3.Normalize(Vector3.Cross(nd2, zVector));
											dot = Vector3.Dot(n1, n2);
											// average normal at vertex j-1
											n = Vector3.Normalize(n1 + n2);
											// act beveled vertex
											bv1.x = act.x + n.x * bevel;
											bv1.y = act.y + n.y * bevel;
											bv1.z = bevel;

											// next beveled vertex
											bv2.x = next.x;
											bv2.y = next.y;
											bv2.z = bevel;
											if (bevel > 0.0f)
											{
												if (FetchNext(ref nnv, contours[k], j))
												{
													ndn = Vector3.Normalize(next - nnv);
													nnn = Vector3.Normalize(Vector3.Cross(ndn, zVector));
													an = Vector3.Normalize(n2 + nnn);
													bv2.x = next.x + an.x * bevel;
													bv2.y = next.y + an.y * bevel;
												}
												else
												{
													Debug.LogWarning("fetch next failed");
												}
											}
											flat = !(dot > crease);
											if (flat)
											{
												// use face normal
												if (prevSmooth)
												{
													attribs.Add(new(act, bv1, n1, uvv));
												}
												else
												{
													n = Vector3.Normalize(n1 + n2);
													// act beveled vertex
													bv1.x = act.x + n.x * bevel;
													bv1.y = act.y + n.y * bevel;
													bv1.z = bevel;
													attribs.Add(new(act, bv1, n2, uvv));
												}
												uvv += d2.magnitude;
												if (next.Equals(first))
												{
													attribs.Add(new(next, bv2, n2, uvv));
												}
											}
											else
											{
												attribs.Add(new(next, bv2, n, uvv));
											}
											// we reached last valid edge
											break;
										}
										act = next;
									}
									if (uvv > maxContourLength)
									{
										maxContourLength = uvv;
									}
									aa.Add(attribs);
								}
							}
						}
					}
					if (aa.Count > 0)
					{
						var totalVertices = mesh.vertices.Length;
						var numSideIndices = 0;
						var numSideVerticesFactor = p.Depth > 0.0f ? 2 : 0;
						numSideVerticesFactor += bevel > 0.0f ? p.Backface ? 4 : 2 : 0;
						for (var j = 0; j < aa.Count; j++)
						{
							var al = aa[j];
							totalVertices += al.Count * numSideVerticesFactor;
							if (al.Count > 0)
							{
								prev = al[0].bv;
								// count required edges
								var numEdges = 0;
								for (var i = 1; i < al.Count; i++)
								{
									var vact = al[i].bv;
									if (!vact.Equals(prev))
									{
										numEdges++;
									}
									prev = vact;
								}
								// Debug.Log("numEdges: " + numEdges + " vs " + (al.Count-1));
								// we require numEdges*6 indices for each edge(two triangles)
								numSideIndices += numEdges * 6;
							}
						}
						// Debug.Log("Expand \'" + _id + "\' " + aa.Count + " (" + _mesh.vertices.Length + ") " + totalVertices);
						var nv = new Vector3[totalVertices];
						var nn = new Vector3[totalVertices];
						var nt = new Vector2[totalVertices];
						var tt = new Vector4[totalVertices]; // tangents
						var k = 0;
						// first copy already tesselated attributes
						var mv = mesh.vertices;
						var mn = mesh.normals;
						var muv = mesh.uv;
						for (; k < mv.Length; k++)
						{
							nv[k] = mv[k];
							nn[k] = mn[k];
							nt[k] = muv[k];
						}
						var mt = mesh.tangents;
						if (null != mt)
						{
							// Debug.Log("copy " + mt.Length + " tangents " + mv.Length);
							for (var tk = 0; tk < mt.Length; tk++)
							{
								tt[tk] = mt[tk];
							}
						}
						var aiv = mv.Length;
						var znAxis = new Vector3(0f, 0f, -1f);
						var ht = new Vector4(0f, 0f, 1f, 1f);
						var b2 = bevel * bevel;
						var uBevelSize = bevel > 0f ? Mathf.Sqrt(b2 + b2) : 0.0f;
						var uTotalSize = uBevelSize + p.Depth;
						const float sideU = 0.1f;
						var buvw = sideU * uBevelSize / uTotalSize;
						var duvw = sideU - (p.Backface ? 2.0f * buvw : buvw);
						var conformant = true;
						if (p.Depth > 0.0f)
						{
							// fill side vertices
							var uOffset = 0.5f;
							var z2 = bevel + p.Depth;
							for (var j = 0; j < aa.Count; j++)
							{
								var al = aa[j];
								if (al.Count > 0)
								{
									for (var i = 0; i < al.Count; i++)
									{
										var ba = al[i];
										/*
										hcp = Vector3.Cross(ba._n, znAxis);
										ht.x = hcp.x;
										ht.y = hcp.y;
										ht.z = hcp.z;
										*/
										nv[k] = ba.bv;
										nn[k] = ba.n;
										nt[k] = conformant ? new(uOffset + buvw, ba.uv.y / maxContourLength) : ba.uv;
										tt[k] = ht;
										k++;
										nv[k] = new(ba.bv.x, ba.bv.y, z2);
										nn[k] = ba.n;
										nt[k] = conformant ? new(uOffset + buvw + duvw, ba.uv.y / maxContourLength) : new Vector2(p.Depth, ba.uv.y);
										tt[k] = ht;
										k++;
									}
								}
								uOffset += sideU;
							}
							// fill side indices
							sideIndices = new int[numSideIndices];
							var ai = 0;
							for (var j = 0; j < aa.Count; j++)
							{
								var al = aa[j];
								if (al.Count > 0)
								{
									prev = al[0].bv;
									for (var i = 1; i < al.Count; i++)
									{
										if (al[i].bv.Equals(prev))
										{
											aiv += 2;
										}
										else
										{
											sideIndices[ai] = aiv;
											sideIndices[ai++] = aiv;
											sideIndices[ai++] = aiv + 1;
											sideIndices[ai++] = aiv + 2;
											sideIndices[ai++] = aiv + 2;
											sideIndices[ai++] = aiv + 1;
											sideIndices[ai++] = aiv + 3;
											aiv += 2;
											prev = al[i].bv;
											// Debug.Log("side edge: " + edge + " max " + (aiv+3) + " alc " + nv.Length);
										}
									}
									aiv += 2;
								}
							}
						}
						if (bevel > 0.0f)
						{
							// Debug.Log("Bevel " + bevel);
							// fill bevel vertex attributes
							var uOffset = 0.5f;
							for (var j = 0; j < aa.Count; j++)
							{
								var al = aa[j];
								if (al.Count > 0)
								{
									for (var i = 0; i < al.Count; i++)
									{
										var ba = al[i];
										/*
										hcp = Vector3.Cross(ba._n, znAxis);
										ht.x = hcp.x;
										ht.y = hcp.y;
										ht.z = hcp.z;
										*/
										nv[k] = ba.v;
										nn[k] = new(0f, 0f, -1f);
										nt[k] = conformant ? new(uOffset, ba.uv.y / maxContourLength) : ba.uv;
										tt[k] = ht;
										k++;
										nv[k] = new(ba.bv.x, ba.bv.y, bevel);
										nn[k] = ba.n;
										nt[k] = conformant ? new(uOffset + buvw, ba.uv.y / maxContourLength) : ba.uv;
										tt[k] = ht;
										k++;
									}
								}
								uOffset += sideU;
							}
							if (p.Backface)
							{
								// backface bevel vertex attributes
								uOffset = 0.5f;
								var z1 = bevel + p.Depth;
								var z2 = bevel * 2f + p.Depth;
								for (var j = 0; j < aa.Count; j++)
								{
									var al = aa[j];
									if (al.Count > 0)
									{
										for (var i = 0; i < al.Count; i++)
										{
											var ba = al[i];
											var hcp = Vector3.Cross(ba.n, znAxis);
											ht.x = hcp.x;
											ht.y = hcp.y;
											ht.z = hcp.z;
											nv[k] = new(ba.bv.x, ba.bv.y, z1);
											nn[k] = ba.n;
											nt[k] = conformant ? new(uOffset + buvw + duvw, ba.uv.y / maxContourLength) : ba.uv;
											tt[k] = ht;
											k++;
											nv[k] = new(ba.v.x, ba.v.y, z2);
											nn[k] = new(0f, 0f, 1f);
											nt[k] = conformant ? new(uOffset + buvw + duvw + buvw, ba.uv.y / maxContourLength) : ba.uv;
											tt[k] = ht;
											k++;
										}
									}
									uOffset += sideU;
								}
							}

							// fill bevel indices
							bevelIndices = new int[p.Backface ? numSideIndices * 2 : numSideIndices];
							var ai = 0;
							for (var j = 0; j < aa.Count; j++)
							{
								var al = aa[j];
								if (al.Count > 0)
								{
									prev = al[0].bv;
									for (var i = 1; i < al.Count; i++)
									{
										if (al[i].bv.Equals(prev))
										{
											aiv += 2;
										}
										else
										{
											bevelIndices[ai] = aiv;
											bevelIndices[ai++] = aiv;
											bevelIndices[ai++] = aiv + 1;
											bevelIndices[ai++] = aiv + 2;
											bevelIndices[ai++] = aiv + 2;
											bevelIndices[ai++] = aiv + 1;
											bevelIndices[ai++] = aiv + 3;
											aiv += 2;
											prev = al[i].bv;
											// Debug.Log("bevel edge: " + edge + " max " + (aiv+3) + " alc " + nv.Length);
										}
									}
									aiv += 2;
								}
							}
							if (p.Backface)
							{
								for (var j = 0; j < aa.Count; j++)
								{
									var al = aa[j];
									if (al.Count > 0)
									{
										prev = al[0].bv;
										for (var i = 1; i < al.Count; i++)
										{
											if (al[i].bv.Equals(prev))
											{
												aiv += 2;
											}
											else
											{
												bevelIndices[ai] = aiv;
												bevelIndices[ai++] = aiv;
												bevelIndices[ai++] = aiv + 1;
												bevelIndices[ai++] = aiv + 2;
												bevelIndices[ai++] = aiv + 2;
												bevelIndices[ai++] = aiv + 1;
												bevelIndices[ai++] = aiv + 3;
												aiv += 2;
												prev = al[i].bv;
												// Debug.Log("bevel edge: " + edge + " max " + (aiv+3) + " alc " + nv.Length);
											}
										}
										aiv += 2;
									}
								}
							}
						}
						// Debug.Log(nv.Length + " " + nt.Length);
						mesh.vertices = nv;
						mesh.uv = nt;
						mesh.normals = nn;
						mesh.tangents = p.GenerateTangents ? tt : null;
					}
				}
				else
				{
					Debug.LogWarning("no contours defined");
				}
			}
			/// <summary>
			///     realize the mesh.
			/// </summary>
			/// <returns>The mesh.</returns>
			/// <param name="shift"></param>
			/// <param name="size"></param>
			/// <param name="parameter"></param>
			public void GetMesh(Mesh mesh, Vector2 shift, Vector2 size, VTextParameter parameter)
			{
				mesh.Clear();
				if (true)
				{
					mesh.name = "c_" + id;
					mesh.subMeshCount = 3;
					if (System.IntPtr.Zero != fh)
					{
						var buffer = System.IntPtr.Zero;
						var vsize = GetGlyphVertices(ref buffer, fh, id);
						// Debug.Log(vsize + " **** glyph vertices **** " + _id);
						if (vsize > 0)
						{
							var res = new float[vsize * 2];
							// fetch xy float array
							Marshal.Copy(buffer, res, 0, vsize * 2);
							var ibuffer = System.IntPtr.Zero;
							// fetch indices
							var isize = GetGlyphTriangleIndices(ref ibuffer, fh, id);
							// Debug.Log(isize + " **** glyph indices **** " + _id);
							if (isize > 0)
							{
								var tisize = parameter.Backface ? isize * 2 : isize;
								var tvsize = parameter.Backface ? vsize * 2 : vsize;
								var ri = new int[isize];
								Marshal.Copy(ibuffer, ri, 0, isize);
								var indices = new int[tisize];
								if (parameter.Backface)
								{
									for (var k = 0; k < isize - 2; k += 3)
									{
										// front face change ccw to cw
										// back face ccw is cw
										var idx = ri[k + 2];
										indices[k + 0] = idx; // front
										indices[isize + k + 2] = idx + vsize; // back
										idx = ri[k + 1];
										indices[k + 1] = idx;
										indices[isize + k + 1] = idx + vsize; // back
										idx = ri[k + 0];
										indices[k + 2] = idx;
										indices[isize + k + 0] = idx + vsize; // back
									}
								}
								else
								{
									// only front faces
									// change ccw to cw
									for (var k = 0; k < isize - 2; k += 3)
									{
										var idx = ri[k + 2];
										indices[k] = idx;
										idx = ri[k + 1];
										indices[k + 1] = idx;
										idx = ri[k + 0];
										indices[k + 2] = idx;
										// Debug.Log(k + "] " + indices[k] + " " + indices[k+1] + " " + indices[k+2]);
									}
								}
								var tvertices = new Vector3[tvsize];
								var tuv = new Vector2[tvsize];
								var tnormals = new Vector3[tvsize];
								var ttangents = new Vector4[tvsize];
								var faceNormal = new Vector3(0f, 0f, -1f);
								var faceTangent = new Vector4(1f, 0f, 0f, 1f);
								// copy front vertex attributes
								for (var k = 0; k < vsize; k++)
								{
									var x = res[k * 2];
									var y = res[k * 2 + 1];
									tvertices[k] = new(x, y, 0f);
									// Debug.Log(k + "] " + x + " " + y + " : " + tvertices[k].ToString("0.00000000"));
									tuv[k] = new(0.5f * (x + shift.x) / size.x, 0.5f * (y + shift.y) / size.y);
									tnormals[k] = faceNormal;
									ttangents[k] = faceTangent;
								}
								if (parameter.Backface)
								{
									// create backface vertex attributes
									faceNormal = new(0f, 0f, 1f);
									for (var k = 0; k < vsize; k++)
									{
										var x = res[k * 2];
										var y = res[k * 2 + 1];
										var offset = vsize + k;
										tvertices[offset] = new(x, y, parameter.Depth + parameter.Bevel * 2f);
										tuv[offset] = new(0.5f * (x + shift.x) / size.x, 0.5f + 0.5f * (y + shift.y) / size.y);
										tnormals[offset] = faceNormal;
										ttangents[offset] = faceTangent;
									}
								}
								mesh.vertices = tvertices;
								mesh.uv = tuv;
								mesh.normals = tnormals;
								mesh.tangents = parameter.GenerateTangents ? ttangents : null;
								if (numContours > 0)
								{
									if (parameter.Is3D)
									{
										// Debug.Log(_numContours + " contours -> depth " + vtext.parameter.Depth + " bevel  " + vtext.parameter.Bevel);
										var odd = false;
										var reverse = false;
										contours = new Vector3[numContours][];
										for (var j = 0; j < numContours; j++)
										{
											var cbuf = System.IntPtr.Zero;
											var csize = GetGlyphContour(ref cbuf, fh, id, j, ref odd, ref reverse);
											// Debug.Log(_numContours + " contour[" + j + "] " + csize);
											if (csize > 0)
											{
												// Debug.Log(csize + " gc[" + j + "] " + (reverse ? "reverse" : "fwd") + " winding " + (odd ? "even_odd" : "nonzero"));
												contours[j] = new Vector3[csize];
												var cvec = new float[csize * 3];
												Marshal.Copy(cbuf, cvec, 0, csize * 3);
												if (reverse)
												{
													for (var z = 0; z < csize; z++)
													{
														var offset = z * 3;
														// Debug.Log(cvec[offset] + " " + cvec[offset+1] + " " + cvec[offset+2]);
														contours[j][csize - z - 1] = new(cvec[offset], cvec[offset + 1], cvec[offset + 2]);
													}
												}
												else
												{
													for (var z = 0; z < csize; z++)
													{
														var offset = z * 3;
														// Debug.Log(cvec[offset] + " " + cvec[offset+1] + " " + cvec[offset+2]);
														contours[j][z] = new(cvec[offset], cvec[offset + 1], cvec[offset + 2]);
													}
												}
											}
										}
										CreateSides(mesh, parameter);
										mesh.SetTriangles(indices, 0);
										if (null != sideIndices)
										{
											mesh.SetIndices(sideIndices, MeshTopology.Triangles, 1);
										}
										if (null != bevelIndices)
										{
											// Debug.Log("bevel ind");
											mesh.SetIndices(bevelIndices, MeshTopology.Triangles, 2);
										}
									}
									else
									{
										mesh.SetTriangles(indices, 0);
									}
									mesh.RecalculateBounds();
								}
								ClearGlyphData(fh, id);
								return;
							}
						}
						ClearGlyphData(fh, id);
						// Debug.Log("no glyph " + size);
					}
					// Debug.Log("ZERO fonthandle " + _fh);
				}
				return;
			}
		}
	}
}
