using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;

public static class MeshSaverEditor {

	[MenuItem("CONTEXT/MeshFilter/Save Mesh...")]
	public static void SaveMeshInPlace (MenuCommand menuCommand) {
		MeshFilter mf = menuCommand.context as MeshFilter;
		Mesh m = mf.sharedMesh;
		Transform trf = mf.transform;
		SaveMesh(trf, m, m.name, false, true);
	}

	[MenuItem("CONTEXT/MeshFilter/Save Mesh As New Instance...")]
	public static void SaveMeshNewInstanceItem (MenuCommand menuCommand) {
		MeshFilter mf = menuCommand.context as MeshFilter;
		Mesh m = mf.sharedMesh;
		Transform trf = mf.transform;
		SaveMesh(trf, m, m.name, true, true);
	}

	public static void SaveMesh (Transform trf, Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh) {
		string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "obj");
		if (string.IsNullOrEmpty(path)) return;
        
		path = FileUtil.GetProjectRelativePath(path);

		//Make mesh
		Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;
		meshToSave.RecalculateNormals();

		if (optimizeMesh)
		    MeshUtility.Optimize(meshToSave);


		//Write Mesh to OBJ
		ObjExporterScript.Start();
 
		StringBuilder meshString = new StringBuilder();
 
		meshString.Append("#" + name + ".obj"
							+ "\n#" + System.DateTime.Now.ToLongDateString() 
							+ "\n#" + System.DateTime.Now.ToLongTimeString()
							+ "\n#-------" 
							+ "\n\n");
  
		meshString.Append("g ").Append(name).Append("\n");
		meshString.Append(ConvertMesh(meshToSave, trf));
 
		WriteToFile(meshString.ToString(), path);
 
		ObjExporterScript.End();
	}
	
	static string ConvertMesh(Mesh m, Transform trf)
	{
		StringBuilder meshString = new StringBuilder();
 
		meshString.Append("#" + trf.name
						+ "\n#-------" 
						+ "\n");
		meshString.Append(ObjExporterScript.MeshToString(m, trf)); 
		return meshString.ToString();
	}
 
	static void WriteToFile(string s, string filename)
	{
		using (StreamWriter sw = new StreamWriter(filename)) 
		{
			sw.Write(s);
		}
	}	
}


public class ObjExporterScript
{
	private static int StartIndex = 0;
 
	public static void Start()
	{
		StartIndex = 0;
	}
	public static void End()
	{
		StartIndex = 0;
	}
 
 
	public static string MeshToString(Mesh m, Transform trf) 
	{	
		Quaternion r 	= trf.localRotation;

		int numVertices = 0;
 
		StringBuilder sb = new StringBuilder();
 
		foreach(Vector3 vv in m.vertices)
		{
			Vector3 v = trf.TransformPoint(vv);
			numVertices++;
			sb.Append(string.Format("v {0} {1} {2}\n",v.x,v.y,-v.z));
		}
		sb.Append("\n");
		foreach(Vector3 nn in m.normals) 
		{
			Vector3 v = r * nn;
			sb.Append(string.Format("vn {0} {1} {2}\n",-v.x,-v.y,v.z));
		}
		sb.Append("\n");
		foreach(Vector3 v in m.uv) 
		{
			sb.Append(string.Format("vt {0} {1}\n",v.x,v.y));
		}
		for (int material=0; material < m.subMeshCount; material ++) 
		{
			sb.Append("\n");
 
			int[] triangles = m.GetTriangles(material);
			for (int i=0;i<triangles.Length;i+=3) {
				sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", 
					triangles[i]+1+StartIndex, triangles[i+1]+1+StartIndex, triangles[i+2]+1+StartIndex));
			}
		}
 
		StartIndex += numVertices;
		return sb.ToString();
	}
}
