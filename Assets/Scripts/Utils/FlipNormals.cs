using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOT USED

public class FlipNormals : MonoBehaviour
{
    public void FlipMeshNormals()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.mesh;

            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -normals[i];
            }
            mesh.normals = normals;

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int[] triangles = mesh.GetTriangles(i);
                for (int j = 0; j < triangles.Length; j += 3)
                {
                    int temp = triangles[j];
                    triangles[j] = triangles[j + 1];
                    triangles[j + 1] = temp;
                }
                mesh.SetTriangles(triangles, i);
            }

            Debug.Log("Normals flipped successfully");
        }
        else
        {
            Debug.LogError("No MeshFilter found on this GameObject.");
        }
    }
}
