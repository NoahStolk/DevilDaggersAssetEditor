using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Chunks;
using DevilDaggersAssetEditor.Data;
using DevilDaggersAssetEditor.Utils;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.VertexBuffers;
using SharpGL.WPF;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls
{
	public partial class ModelPreviewerControl : UserControl
	{
		private VertexBufferArray _vertexBufferArray;

		private List<Vector3> _vertices;
		private List<Vector2> _texCoords;
		private List<Vector3> _normals;
		private List<VertexReference> _indices;

		public ModelPreviewerControl()
		{
			InitializeComponent();
		}

		public void Initialize(ModelAsset asset)
		{
			TextureName.Text = asset.AssetName;
			DefaultVertexCount.Text = asset.DefaultVertexCount.ToString(CultureInfo.InvariantCulture);
			DefaultIndexCount.Text = asset.DefaultIndexCount.ToString(CultureInfo.InvariantCulture);

			bool isPathValid = File.Exists(asset.EditorPath);

			FileName.Text = isPathValid ? Path.GetFileName(asset.EditorPath) : GuiUtils.FileNotFound;

			if (isPathValid)
			{
				string[] lines = File.ReadAllLines(asset.EditorPath);
				int v = 0;
				int vt = 0;
				int vn = 0;
				int f = 0;
				foreach (string line in lines)
				{
					switch (line.Split(' ')[0])
					{
						case "v": v++; break;
						case "vt": vt++; break;
						case "vn": vn++; break;
						case "f": f++; break;
					}
				}

				FileVertexCount.Text = new[] { v, vt, vn }.Max().ToString(CultureInfo.InvariantCulture);
				FileIndexCount.Text = f.ToString(CultureInfo.InvariantCulture);

				ModelChunk.ReadObj(asset.EditorPath, out _vertices, out _texCoords, out _normals, out _indices);
			}
			else
			{
				FileVertexCount.Text = "N/A";
				FileIndexCount.Text = "N/A";
			}
		}

		private void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
		{
			OpenGL gl = args.OpenGL;

			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

			if (_vertices == null)
				return;

			_vertexBufferArray = new VertexBufferArray();
			_vertexBufferArray.Create(gl);
			_vertexBufferArray.Bind(gl);

			uint vertexAttributeLocation = 0;
			uint normalAttributeLocation = 1;

			CreateVertexNormalBuffer(gl, vertexAttributeLocation, normalAttributeLocation);
			CreateIndexBuffer(gl);

			_vertexBufferArray.Unbind(gl);

			gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, FromSliders(GlobalAmbientR, GlobalAmbientG, GlobalAmbientB));
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, FromSliders(LightPositionX, LightPositionY, LightPositionZ, LightPositionW));
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, FromSliders(LightAmbientR, LightAmbientG, LightAmbientB));
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, FromSliders(LightDiffuseR, LightDiffuseG, LightDiffuseB));
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, FromSliders(LightSpecularR, LightSpecularG, LightSpecularB));

			gl.LoadIdentity();
			gl.Translate(0.0f, 0.0f, -Zoom.Value);

			gl.Rotate(RotationX.Value, 1, 0, 0);
			gl.Rotate(RotationY.Value, 0, 1, 0);
			gl.Rotate(RotationZ.Value, 0, 0, 1);

			gl.PushAttrib(OpenGL.GL_POLYGON_BIT);
			gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);

			gl.Begin(BeginMode.Triangles);
			foreach (VertexReference index in _indices)
				gl.Vertex(_vertices[(int)index.PositionReference - 1].X, _vertices[(int)index.PositionReference - 1].Y, _vertices[(int)index.PositionReference - 1].Z);
			gl.End();

			gl.PopAttrib();
		}

		private void OpenGLControl_OpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
		{
			OpenGL gl = args.OpenGL;

			gl.Enable(OpenGL.GL_DEPTH_TEST);
			gl.Enable(OpenGL.GL_LIGHTING);
			gl.Enable(OpenGL.GL_LIGHT0);

			gl.ShadeModel(OpenGL.GL_SMOOTH);
		}

		private static float[] FromSliders(params Slider[] sliders)
			=> sliders.Select(s => (float)s.Value).ToArray();

		private void CreateVertexNormalBuffer(OpenGL gl, uint vertexAttributeLocation, uint normalAttributeLocation)
		{
			VertexBuffer vertexBuffer = new VertexBuffer();
			vertexBuffer.Create(gl);
			vertexBuffer.Bind(gl);
			vertexBuffer.SetData(gl, vertexAttributeLocation, _vertices.SelectMany(v => new[] { v.X, v.Y, v.Z }).ToArray(), false, 3);

			VertexBuffer normalBuffer = new VertexBuffer();
			normalBuffer.Create(gl);
			normalBuffer.Bind(gl);
			normalBuffer.SetData(gl, normalAttributeLocation, _normals.SelectMany(v => new[] { v.X, v.Y, v.Z }).ToArray(), false, 3);
		}

		private void CreateIndexBuffer(OpenGL gl)
		{
			IndexBuffer indexBuffer = new IndexBuffer();
			indexBuffer.Create(gl);
			indexBuffer.Bind(gl);
			indexBuffer.SetData(gl, _indices.Select(i => (ushort)i.PositionReference).ToArray());
		}
	}
}