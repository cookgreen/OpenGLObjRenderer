using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpGL;
using SharpGL.SceneGraph.Assets;

namespace ObjRenderer
{
	public partial class frmMain : Form
	{
		private Texture texture;
        private string ObjFile;
        private string TextureFile;
		private ObjModel model;
		private double currentX = 0;
		private double currentZ = 0;
		private float rotation;
		private double radius = 30;
		private double angle = 0.1f;
		private bool draging = false;
		private Point lastMousePosition;

		public frmMain()
		{
			InitializeComponent();
			MouseWheel += FrmMain_MouseWheel;
		}

		private void FrmMain_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta > 0)
			{
				radius++;
			}
			else
			{
				radius--;
			}
		}

		public bool Ready
        {
            get
            {
                return !string.IsNullOrEmpty(TextureFile) && File.Exists(TextureFile)
                    && !string.IsNullOrEmpty(ObjFile) && File.Exists(ObjFile);
            }
        }

		private void FrmMain_Load(object sender, EventArgs e)
		{
		}

		private void OpenGLControl1_OpenGLInitialized(object sender, EventArgs e)
		{
			var gl = openGLControl1.OpenGL;
			gl.ClearColor(0, 0, 0, 0);
			gl.ShadeModel(OpenGL.GL_SMOOTH);
			var mat_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
			gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, mat_ambient);

			var ambientLight = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, ambientLight);

			texture = new Texture();
            if (Ready)
            {
                texture.Create(gl, TextureFile);
            }

			gl.Enable(OpenGL.GL_TEXTURE_2D);
			gl.Enable(OpenGL.GL_LIGHTING);
			gl.Enable(OpenGL.GL_LIGHT0);
			gl.Enable(OpenGL.GL_LIGHT1);
			gl.Enable(OpenGL.GL_DEPTH_TEST);
		}

		private void OpenGLControl1_OpenGLDraw(object sender, RenderEventArgs args)
		{
			if (Ready)
			{
                model = new ObjModel(ObjFile);

                OpenGL gl = this.openGLControl1.OpenGL;
				gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
				gl.LoadIdentity();

				currentX = Math.Sin(angle) * radius;
				currentZ = Math.Cos(angle) * radius;
				var centerPoint = calculateCenterPoint(model);

				gl.LookAt(
					currentX, 0, currentZ,
					centerPoint.X, centerPoint.Y, centerPoint.Z, 
					0, 1, 0);

				gl.Rotate(-rotation, 0, 1, 0);
				rotation += 3.0f;

				for (int i = 0; i < model.Faces.Length; i++)
				{
					var face = model.Faces[i];
					if (face.IsQuad)
					{
						gl.Begin(OpenGL.GL_QUADS);
						{
							var v1 = model.Vertics[face.Points[0].VertexIndex - 1];
							var vn1 = model.VertexNormals[face.Points[0].NormalIndex - 1];
							var vt1 = model.VertexTextures[face.Points[0].TextureIndex - 1];

							var v2 = model.Vertics[face.Points[1].VertexIndex - 1];
							var vn2 = model.VertexNormals[face.Points[1].NormalIndex - 1];
							var vt2 = model.VertexTextures[face.Points[1].TextureIndex - 1];

							var v3 = model.Vertics[face.Points[2].VertexIndex - 1];
							var vn3 = model.VertexNormals[face.Points[2].NormalIndex - 1];
							var vt3 = model.VertexTextures[face.Points[2].TextureIndex - 1];

							var v4 = model.Vertics[face.Points[3].VertexIndex - 1];
							var vn4 = model.VertexNormals[face.Points[3].NormalIndex - 1];
							var vt4 = model.VertexTextures[face.Points[3].TextureIndex - 1];

							gl.TexCoord(vt1.X, vt1.Y * -1);
							gl.Normal(vn1.X, vn1.Y, vn1.Z);
							gl.Vertex(v1.X, v1.Y, v1.Z);

							gl.TexCoord(vt2.X, vt2.Y * -1);
							gl.Normal(vn2.X, vn2.Y, vn2.Z);
							gl.Vertex(v2.X, v2.Y, v2.Z);

							gl.TexCoord(vt3.X, vt3.Y * -1);
							gl.Normal(vn3.X, vn3.Y, vn3.Z);
							gl.Vertex(v3.X, v3.Y, v3.Z);

							gl.TexCoord(vt4.X, vt4.Y * -1);
							gl.Normal(vn4.X, vn4.Y, vn4.Z);
							gl.Vertex(v4.X, v4.Y, v4.Z);
						}
						gl.End();
					}
					else
					{
						gl.Begin(SharpGL.Enumerations.BeginMode.Triangles);
						{
							var v1 = model.Vertics[face.Points[0].VertexIndex - 1];
							var vn1 = model.VertexNormals[face.Points[0].NormalIndex - 1];
							var vt1 = model.VertexTextures[face.Points[0].TextureIndex - 1];

							var v2 = model.Vertics[face.Points[1].VertexIndex - 1];
							var vn2 = model.VertexNormals[face.Points[1].NormalIndex - 1];
							var vt2 = model.VertexTextures[face.Points[1].TextureIndex - 1];

							var v3 = model.Vertics[face.Points[2].VertexIndex - 1];
							var vn3 = model.VertexNormals[face.Points[2].NormalIndex - 1];
							var vt3 = model.VertexTextures[face.Points[2].TextureIndex - 1];

							gl.TexCoord(vt1.X, vt1.Y * -1);
							gl.Normal(vn1.X, vn1.Y, vn1.Z);
							gl.Vertex(v1.X, v1.Y, v1.Z);

							gl.TexCoord(vt2.X, vt2.Y * -1);
							gl.Normal(vn2.X, vn2.Y, vn2.Z);
							gl.Vertex(v2.X, v2.Y, v2.Z);

							gl.TexCoord(vt3.X, vt3.Y * -1);
							gl.Normal(vn3.X, vn3.Y, vn3.Z);
							gl.Vertex(v3.X, v3.Y, v3.Z);
						}
						gl.End();

					}
					gl.Flush();
				}
			}
		}

		private void OpenGLControl1_Resize(object sender, EventArgs e)
		{
			OpenGL gl = openGLControl1.OpenGL;

			gl.MatrixMode(OpenGL.GL_PROJECTION);

			gl.LoadIdentity();

			gl.Perspective(60.0f, (double)Width / (double)Height, 15, 300.0);

			gl.LookAt(-5, 5, -5, 0, 0, 0, 0, -1, 0);

			gl.MatrixMode(OpenGL.GL_MODELVIEW);
		}

		private void FrmMain_KeyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.S:
					currentZ -= 5;
					break;
				case Keys.W:
					currentZ += 5;
					break;
				case Keys.A:
					currentX += 5;
					break;
				case Keys.D:
					currentX -= 5;
					break;
			}
		}

        private void BtnChooseModel_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Obj Model|*.obj";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtModel.Text = dialog.FileName;
                ObjFile = txtModel.Text;
            }
        }

        private void BtnChooseTexture_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG file|*.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtTexture.Text = dialog.FileName;
                TextureFile = txtTexture.Text;
                UpdateTextureImage();
            }
        }

        private void UpdateTextureImage()
        {
            var gl = openGLControl1.OpenGL;
            if (texture != null)
            {
                texture.Destroy(gl);
                texture.Create(gl, TextureFile);
            }
        }

		private Vector3 calculateCenterPoint(ObjModel model)
		{
			OpenGL gl = openGLControl1.OpenGL;

			var verticsSortByX = from v in model.Vertics
								 orderby v.X descending
								 select v;
			var verticsSortByY = from v in model.Vertics
								 orderby v.Y descending
								 select v;
			var verticsSortByZ = from v in model.Vertics
								 orderby v.Z descending
								 select v;
			var vertexMaxHeightPoint = verticsSortByZ.ElementAt(0);
			var vertexMinHeightPoint = verticsSortByZ.Last();
			var vertexMaxRightPoint = verticsSortByX.ElementAt(0);
			var vertexMinRightPoint = verticsSortByX.Last();
			var vertexMaxFrontPoint = verticsSortByY.ElementAt(0);
			var vertexMinFrontPoint = verticsSortByY.Last();

			var maxPoint = new Vector3() { X = vertexMaxRightPoint.X, Y = vertexMaxFrontPoint.Y, Z = vertexMaxHeightPoint.Z };
			var minPoint = new Vector3() { X = vertexMinRightPoint.X, Y = vertexMinFrontPoint.Y, Z = vertexMinHeightPoint.Z };

			return new Vector3() { X = (maxPoint.X + minPoint.X) / 2, Y = (maxPoint.Y + minPoint.Y) / 2, Z = (maxPoint.Z + minPoint.Z) / 2 };
		}

		private void OpenGLControl1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				draging = true;
				lastMousePosition = new Point(e.X, e.Y);
			}
		}

		private void OpenGLControl1_MouseMove(object sender, MouseEventArgs e)
		{
			if (draging)
			{
				var offset = e.X - lastMousePosition.X;
				if (offset > 0)
				{
					angle -= 0.1f;
				}
				else
				{
					angle += 0.1f;
				}
			}
		}

		private void OpenGLControl1_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				draging = false;
			}
		}
	}
}
