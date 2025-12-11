using UnityEngine;

namespace ShieldWall.Visual
{
    public static class PrimitiveMeshGenerator
    {
        public static Mesh CreateCapsuleMesh(float height, float radius, int segments = 16)
        {
            Mesh mesh = new Mesh { name = "Generated Capsule" };
            
            int vertexCount = segments * 4 + 2;
            Vector3[] vertices = new Vector3[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];
            
            float halfHeight = height * 0.5f;
            
            vertices[0] = new Vector3(0, halfHeight + radius, 0);
            normals[0] = Vector3.up;
            uvs[0] = new Vector2(0.5f, 1f);
            
            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                float u = (float)i / segments;
                
                vertices[i * 4 + 1] = new Vector3(x, halfHeight, z);
                normals[i * 4 + 1] = new Vector3(x, 0, z).normalized;
                uvs[i * 4 + 1] = new Vector2(u, 0.75f);
                
                vertices[i * 4 + 2] = new Vector3(x, halfHeight, z);
                normals[i * 4 + 2] = new Vector3(x, 0, z).normalized;
                uvs[i * 4 + 2] = new Vector2(u, 0.75f);
                
                vertices[i * 4 + 3] = new Vector3(x, -halfHeight, z);
                normals[i * 4 + 3] = new Vector3(x, 0, z).normalized;
                uvs[i * 4 + 3] = new Vector2(u, 0.25f);
                
                vertices[i * 4 + 4] = new Vector3(x, -halfHeight, z);
                normals[i * 4 + 4] = new Vector3(x, 0, z).normalized;
                uvs[i * 4 + 4] = new Vector2(u, 0.25f);
            }
            
            vertices[vertexCount - 1] = new Vector3(0, -halfHeight - radius, 0);
            normals[vertexCount - 1] = Vector3.down;
            uvs[vertexCount - 1] = new Vector2(0.5f, 0f);
            
            int triangleCount = segments * 12;
            int[] triangles = new int[triangleCount];
            int triIndex = 0;
            
            for (int i = 0; i < segments; i++)
            {
                int next = (i + 1) % segments;
                
                triangles[triIndex++] = 0;
                triangles[triIndex++] = i * 4 + 1;
                triangles[triIndex++] = next * 4 + 1;
                
                triangles[triIndex++] = i * 4 + 2;
                triangles[triIndex++] = next * 4 + 3;
                triangles[triIndex++] = i * 4 + 3;
                
                triangles[triIndex++] = i * 4 + 2;
                triangles[triIndex++] = next * 4 + 2;
                triangles[triIndex++] = next * 4 + 3;
                
                triangles[triIndex++] = vertexCount - 1;
                triangles[triIndex++] = next * 4 + 4;
                triangles[triIndex++] = i * 4 + 4;
            }
            
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            
            return mesh;
        }
        
        public static Mesh CreateCubeMesh(float size)
        {
            Mesh mesh = new Mesh { name = "Generated Cube" };
            
            float half = size * 0.5f;
            
            Vector3[] vertices = {
                new Vector3(-half, -half, -half),
                new Vector3(half, -half, -half),
                new Vector3(half, half, -half),
                new Vector3(-half, half, -half),
                new Vector3(-half, half, half),
                new Vector3(half, half, half),
                new Vector3(half, -half, half),
                new Vector3(-half, -half, half),
            };
            
            int[] triangles = {
                0, 2, 1, 0, 3, 2,
                2, 3, 4, 2, 4, 5,
                1, 2, 5, 1, 5, 6,
                0, 7, 4, 0, 4, 3,
                5, 4, 7, 5, 7, 6,
                0, 6, 7, 0, 1, 6
            };
            
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }
        
        public static Mesh CreateSphereMesh(float radius, int segments = 16)
        {
            Mesh mesh = new Mesh { name = "Generated Sphere" };
            
            int vertexCount = (segments + 1) * (segments + 1);
            Vector3[] vertices = new Vector3[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];
            
            int index = 0;
            for (int lat = 0; lat <= segments; lat++)
            {
                float theta = lat * Mathf.PI / segments;
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);
                
                for (int lon = 0; lon <= segments; lon++)
                {
                    float phi = lon * 2 * Mathf.PI / segments;
                    float sinPhi = Mathf.Sin(phi);
                    float cosPhi = Mathf.Cos(phi);
                    
                    Vector3 normal = new Vector3(cosPhi * sinTheta, cosTheta, sinPhi * sinTheta);
                    vertices[index] = normal * radius;
                    normals[index] = normal;
                    uvs[index] = new Vector2((float)lon / segments, (float)lat / segments);
                    index++;
                }
            }
            
            int triangleCount = segments * segments * 6;
            int[] triangles = new int[triangleCount];
            int triIndex = 0;
            
            for (int lat = 0; lat < segments; lat++)
            {
                for (int lon = 0; lon < segments; lon++)
                {
                    int first = lat * (segments + 1) + lon;
                    int second = first + segments + 1;
                    
                    triangles[triIndex++] = first;
                    triangles[triIndex++] = second;
                    triangles[triIndex++] = first + 1;
                    
                    triangles[triIndex++] = second;
                    triangles[triIndex++] = second + 1;
                    triangles[triIndex++] = first + 1;
                }
            }
            
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            
            return mesh;
        }
    }
}
