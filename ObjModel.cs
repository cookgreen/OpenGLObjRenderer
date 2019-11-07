﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjRenderer
{
	public class ObjModel
	{
		public struct Vector
		{
			public float X;
			public float Y;
			public float Z;
		}
		public struct FacePoint
		{
			public int VertexIndex;
			public int TextureIndex;
			public int NormalIndex;
		}
		public struct Face
		{
			public FacePoint[] Points;
			public bool IsQuad;
		}
		public Vector[] Vertics;
		public Vector[] VertexNormals;
		public Vector[] VertexTextures;
		public Face[] Faces;

		public ObjModel(string filename)
		{
			Analyze(File.ReadAllText(filename));
		}

		private void Analyze(string content)
		{
			content = content.Replace('\r', ' ').Replace('\t', ' ');
			var lines = content.Split('\n');
			var vertexList = new List<Vector>();
			var vertexNormalList = new List<Vector>();
			var vertexTextureList = new List<Vector>();
			var faceList = new List<Face>();
			for (int i = 0; i < lines.Length; i++)
			{
				var currentLine = lines[i];
				if (currentLine.StartsWith("#") || currentLine.Length == 0)
				{
					continue;
				}
                if (currentLine.StartsWith("v "))
                {
                    var splitInfo = currentLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    vertexList.Add(new Vector() { X = float.Parse(splitInfo[1]), Y = float.Parse(splitInfo[2]), Z = float.Parse(splitInfo[3]) });
                }
                else if (currentLine.StartsWith("vt "))
                {
                    var splitInfo = currentLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (splitInfo.Length == 4)
                    {
                        vertexTextureList.Add(new Vector() { X = float.Parse(splitInfo[1]), Y = float.Parse(splitInfo[2]), Z = float.Parse(splitInfo[3]) });
                    }
                    else if (splitInfo.Length == 3)
                    {
                        vertexTextureList.Add(new Vector() { X = float.Parse(splitInfo[1]), Y = float.Parse(splitInfo[2]), Z = 0 });
                    }
                }
                else if (currentLine.StartsWith("vn "))
                {
                    var splitInfo = currentLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    vertexNormalList.Add(new Vector() { X = float.Parse(splitInfo[1]), Y = float.Parse(splitInfo[2]), Z = float.Parse(splitInfo[3]) });
                }
                else if (currentLine.StartsWith("f "))
                {
                    var splitInfo = currentLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var isQuad = splitInfo.Length > 4;
                    var face1 = splitInfo[1].Split('/');
                    var face2 = splitInfo[2].Split('/');
                    var face3 = splitInfo[3].Split('/');
                    var face4 = isQuad ? splitInfo[4].Split('/') : null;
                    var face = new Face();
                    face.Points = new FacePoint[4];
                    face.Points[0] = new FacePoint() { VertexIndex = int.Parse(face1[0]), TextureIndex = int.Parse(face1[1]), NormalIndex = int.Parse(face1[2]) };
                    face.Points[1] = new FacePoint() { VertexIndex = int.Parse(face2[0]), TextureIndex = int.Parse(face2[1]), NormalIndex = int.Parse(face2[2]) };
                    face.Points[2] = new FacePoint() { VertexIndex = int.Parse(face3[0]), TextureIndex = int.Parse(face3[1]), NormalIndex = int.Parse(face3[2]) };
                    face.Points[3] = isQuad ? new FacePoint() { VertexIndex = int.Parse(face4[0]), TextureIndex = int.Parse(face4[1]), NormalIndex = int.Parse(face4[2]) } : default(FacePoint);
                    face.IsQuad = isQuad;
                    faceList.Add(face);
                }
			}
			Vertics = vertexList.ToArray();
			VertexNormals = vertexNormalList.ToArray();
			VertexTextures = vertexTextureList.ToArray();
			Faces = faceList.ToArray();
		}
	}
}
