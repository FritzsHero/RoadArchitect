/*
Based on ObjExporter.cs, this "wrapper" lets you export to .OBJ directly from the editor menu.
 
Use by selecting the objects you want to export, and select the appropriate menu item from "Custom->Export".
Exported models are put in a folder called "ExportedObj" in the root of your project.
Textures should also be copied and placed in the same folder.
*/

#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
#endregion


namespace RoadArchitect
{
    public class ObjExporter : ScriptableObject
    {
        private static int vertexOffset = 0;
        private static int normalOffset = 0;
        private static int uvOffset = 0;


        //User should probably be able to change this. It is currently left as an excercise for
        //the reader.
        private static string targetFolder = "ExportedObj";


        private struct ObjMaterial
        {
            public string name;
            public string textureName;
        }


        private static string MeshToString(MeshFilter _meshFilter, Dictionary<string, ObjMaterial> _materialList)
        {
            Mesh mesh = _meshFilter.sharedMesh;
            Renderer renderer = _meshFilter.GetComponent<Renderer>();
            //Material[] mats = mf.renderer.sharedMaterials;
            Material[] materials = renderer.sharedMaterials;

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("g ").Append(_meshFilter.name).Append("\n");
            foreach (Vector3 lv in mesh.vertices)
            {
                Vector3 wv = _meshFilter.transform.TransformPoint(lv);

                //This is sort of ugly - inverting x-component since we're in
                //a different coordinate system than "everyone" is "used to".
                stringBuilder.Append(string.Format("v {0} {1} {2}\n", -wv.x, wv.y, wv.z));
            }
            stringBuilder.Append("\n");

            foreach (Vector3 lv in mesh.normals)
            {
                Vector3 wv = _meshFilter.transform.TransformDirection(lv);

                stringBuilder.Append(string.Format("vn {0} {1} {2}\n", -wv.x, wv.y, wv.z));
            }
            stringBuilder.Append("\n");

            foreach (Vector3 v in mesh.uv)
            {
                stringBuilder.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }

            for (int material = 0; material < mesh.subMeshCount; material++)
            {
                stringBuilder.Append("\n");
                stringBuilder.Append("usemtl ").Append(materials[material].name).Append("\n");
                stringBuilder.Append("usemap ").Append(materials[material].name).Append("\n");

                //See if this material is already in the materiallist.
                try
                {
                    ObjMaterial objMaterial = new ObjMaterial();

                    objMaterial.name = materials[material].name;

                    if (materials[material].mainTexture)
                    {
                        objMaterial.textureName = EngineIntegration.GetAssetPath(materials[material].mainTexture);
                    }
                    else
                    {
                        objMaterial.textureName = null;
                    }

                    _materialList.Add(objMaterial.name, objMaterial);
                }
                catch (ArgumentException)
                {
                    //Already in the dictionary
                }


                int[] triangles = mesh.GetTriangles(material);
                for (int index = 0; index < triangles.Length; index += 3)
                {
                    //Because we inverted the x-component, we also needed to alter the triangle winding.
                    stringBuilder.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
                        triangles[index] + 1 + vertexOffset, triangles[index + 1] + 1 + normalOffset, triangles[index + 2] + 1 + uvOffset));
                }
            }

            vertexOffset += mesh.vertices.Length;
            normalOffset += mesh.normals.Length;
            uvOffset += mesh.uv.Length;

            return stringBuilder.ToString();
        }


        private static void Clear()
        {
            vertexOffset = 0;
            normalOffset = 0;
            uvOffset = 0;
        }


        private static Dictionary<string, ObjMaterial> PrepareFileWrite()
        {
            Clear();

            return new Dictionary<string, ObjMaterial>();
        }


        private static void MaterialsToFile(Dictionary<string, ObjMaterial> _materialList, string _folder, string _fileName)
        {
            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(_folder, _fileName) + ".mtl"))
            {
                foreach (KeyValuePair<string, ObjMaterial> kvp in _materialList)
                {
                    streamWriter.Write("\n");
                    streamWriter.Write("newmtl {0}\n", kvp.Key);
                    streamWriter.Write("Ka  0.6 0.6 0.6\n");
                    streamWriter.Write("Kd  0.6 0.6 0.6\n");
                    streamWriter.Write("Ks  0.9 0.9 0.9\n");
                    streamWriter.Write("d  1.0\n");
                    streamWriter.Write("Ns  0.0\n");
                    streamWriter.Write("illum 2\n");

                    if (kvp.Value.textureName != null)
                    {
                        string destinationFile = kvp.Value.textureName;

                        int stripIndex = destinationFile.LastIndexOf(Path.PathSeparator);

                        if (stripIndex >= 0)
                        {
                            destinationFile = destinationFile.Substring(stripIndex + 1).Trim();
                        }


                        string relativeFile = destinationFile;

                        destinationFile = Path.Combine(_folder, destinationFile);

                        //Debug.Log("Copying texture from " + kvp.Value.textureName + " to " + destinationFile);

                        try
                        {
                            //Copy the source file
                            File.Copy(kvp.Value.textureName, destinationFile);
                        }
                        catch
                        {

                        }


                        streamWriter.Write("map_Kd {0}", relativeFile);
                    }

                    streamWriter.Write("\n\n\n");
                }
            }
        }


        private static void MeshToFile(MeshFilter _meshFilter, string _folder, string _fileName)
        {
            Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(_folder, _fileName) + ".obj"))
            {
                streamWriter.Write("mtllib ./" + _fileName + ".mtl\n");

                streamWriter.Write(MeshToString(_meshFilter, materialList));
            }

            MaterialsToFile(materialList, _folder, _fileName);
        }


        private static void MeshesToFile(MeshFilter[] _meshFilter, string _folder, string _fileName)
        {
            Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(_folder, _fileName) + ".obj"))
            {
                streamWriter.Write("mtllib ./" + _fileName + ".mtl\n");

                for (int index = 0; index < _meshFilter.Length; index++)
                {
                    streamWriter.Write(MeshToString(_meshFilter[index], materialList));
                }
            }

            MaterialsToFile(materialList, _folder, _fileName);
        }


        private static bool CreateTargetFolder()
        {
            try
            {
                System.IO.Directory.CreateDirectory(targetFolder);
            }
            catch
            {
                EditorUtility.DisplayDialog("Error!", "Failed to create target folder!", "");
                return false;
            }

            return true;
        }


        [MenuItem("Window/Road Architect/Export/Export all MeshFilters in selection to separate OBJs")]
        private static void ExportSelectionToSeparate()
        {
            if (!CreateTargetFolder())
            {
                return;
            }

            Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);

            if (selection.Length == 0)
            {
                EditorUtility.DisplayDialog("No source object selected!", "Please select one or more target objects", "");
                return;
            }

            int exportedObjects = 0;

            for (int index = 0; index < selection.Length; index++)
            {
                Component[] meshfilter = selection[index].GetComponentsInChildren<MeshFilter>();

                for (int m = 0; m < meshfilter.Length; m++)
                {
                    exportedObjects++;
                    MeshToFile((MeshFilter)meshfilter[m], targetFolder, selection[index].name + "_" + index + "_" + m);
                }
            }

            if (exportedObjects > 0)
            {
                EditorUtility.DisplayDialog("Objects exported", "Exported " + exportedObjects + " objects", "");
            }
            else
            {
                EditorUtility.DisplayDialog("Objects not exported", "Make sure at least some of your selected objects have mesh filters!", "");
            }
        }


        [MenuItem("Window/Road Architect/Export/Export whole selection to single OBJ")]
        private static void ExportWholeSelectionToSingle()
        {
            if (!CreateTargetFolder())
            {
                return;
            }


            Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);

            if (selection.Length == 0)
            {
                EditorUtility.DisplayDialog("No source object selected!", "Please select one or more target objects", "");
                return;
            }

            int exportedObjects = 0;

            ArrayList mfList = new ArrayList();

            for (int index = 0; index < selection.Length; index++)
            {
                Component[] meshfilter = selection[index].GetComponentsInChildren<MeshFilter>();

                for (int m = 0; m < meshfilter.Length; m++)
                {
                    exportedObjects++;
                    mfList.Add(meshfilter[m]);
                }
            }

            if (exportedObjects > 0)
            {
                MeshFilter[] meshFilters = new MeshFilter[mfList.Count];

                for (int index = 0; index < mfList.Count; index++)
                {
                    meshFilters[index] = (MeshFilter)mfList[index];
                }


                string sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
                //string filename = EditorApplication.currentScene + "_" + exportedObjects;
                string filename = sceneName + "_" + exportedObjects;

                int stripIndex = filename.LastIndexOf(Path.PathSeparator);

                if (stripIndex >= 0)
                {
                    filename = filename.Substring(stripIndex + 1).Trim();
                }

                MeshesToFile(meshFilters, targetFolder, filename);


                EditorUtility.DisplayDialog("Objects exported", "Exported " + exportedObjects + " objects to " + filename, "");
            }
            else
                EditorUtility.DisplayDialog("Objects not exported", "Make sure at least some of your selected objects have mesh filters!", "");
        }


        [MenuItem("Window/Road Architect/Export/Export each selected to single OBJ")]
        private static void ExportEachSelectionToSingle()
        {
            if (!CreateTargetFolder())
            {
                return;
            }

            Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);

            if (selection.Length == 0)
            {
                EditorUtility.DisplayDialog("No source object selected!", "Please select one or more target objects", "");
                return;
            }

            int exportedObjects = 0;


            for (int index = 0; index < selection.Length; index++)
            {
                Component[] meshfilter = selection[index].GetComponentsInChildren<MeshFilter>();

                MeshFilter[] mf = new MeshFilter[meshfilter.Length];

                for (int m = 0; m < meshfilter.Length; m++)
                {
                    exportedObjects++;
                    mf[m] = (MeshFilter)meshfilter[m];
                }

                MeshesToFile(mf, targetFolder, selection[index].name + "_" + index);
            }

            if (exportedObjects > 0)
            {
                EditorUtility.DisplayDialog("Objects exported", "Exported " + exportedObjects + " objects", "");
            }
            else
            {
                EditorUtility.DisplayDialog("Objects not exported", "Make sure at least some of your selected objects have mesh filters!", "");
            }
        }


        [MenuItem("Window/Road Architect/Export/Exporters by Hrafnkell Freyr Hlooversson from Unity3D wiki")]
        private static void OpenLink()
        {
            Application.OpenURL("http://wiki.unity3d.com/index.php?title=ObjExporter");
        }
    }
}
#endif
