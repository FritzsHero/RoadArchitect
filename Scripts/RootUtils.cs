#region "Imports"
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
#endregion


namespace RoadArchitect
{
    public static class RootUtils
    {
        /// <summary>
        /// Smooths the input parameter t.
        /// If less than k1 ir greater than k2, it uses a sin.
        /// Between k1 and k2 it uses linear interp.
        /// </summary>
        public static float Ease(float _t, float _k1, float _k2)
        {
            float f;
            float s;


            f = _k1 * 2 / Mathf.PI + _k2 - _k1 + (1.0f - _k2) * 2 / Mathf.PI;

            if (_t < _k1)
            {
                s = _k1 * (2 / Mathf.PI) * (Mathf.Sin((_t / _k1) * Mathf.PI / 2 - Mathf.PI / 2) + 1);
            }
            else if (_t < _k2)
            {
                s = (2 * _k1 / Mathf.PI + _t - _k1);
            }
            else
            {
                s = 2 * _k1 / Mathf.PI + _k2 - _k1 + ((1 - _k2) * (2 / Mathf.PI)) * Mathf.Sin(((_t - _k2) / (1.0f - _k2)) * Mathf.PI / 2);
            }

            return (s / f);
        }


        /// <summary> Returns true if the lines intersect, otherwise false.  </summary>
        /// <param name="_line1S">Line 1 start.</param>
        /// <param name="_line1E">Line 1 end.</param>
        /// <param name="_line2S">Line 2 start.</param>
        /// <param name="_line2E">Line 2 end.</param>
        /// <param name="_intersectionPoint">If the lines intersect, intersectionPoint holds the intersection point.</param>
        public static bool Intersects2D(ref Vector2 _line1S, ref Vector2 _line1E, ref Vector2 _line2S, ref Vector2 _line2E, out Vector2 _intersectionPoint)
        {
            float firstLineSlopeX, firstLineSlopeY, secondLineSlopeX, secondLineSlopeY;

            firstLineSlopeX = _line1E.x - _line1S.x;
            firstLineSlopeY = _line1E.y - _line1S.y;

            secondLineSlopeX = _line2E.x - _line2S.x;
            secondLineSlopeY = _line2E.y - _line2S.y;

            float s, t;
            s = (-firstLineSlopeY * (_line1S.x - _line2S.x) + firstLineSlopeX * (_line1S.y - _line2S.y)) / (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);
            t = (secondLineSlopeX * (_line1S.y - _line2S.y) - secondLineSlopeY * (_line1S.x - _line2S.x)) / (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                float intersectionPointX = _line1S.x + (t * firstLineSlopeX);
                float intersectionPointY = _line1S.y + (t * firstLineSlopeY);

                // Collision detected
                _intersectionPoint = new Vector2(intersectionPointX, intersectionPointY);
                return true;
            }

            _intersectionPoint = Vector2.zero;
            // No collision
            return false;
        }


        /// <summary> Gives the asset path to this object </summary>
        public static string GetPrefabString(GameObject _object)
        {
            string path = "";
            #if UNITY_EDITOR
            if (_object != null)
            {
                path = UnityEditor.AssetDatabase.GetAssetPath(_object);
                if (path == null || path.Length < 1)
                {
                    Object parentObject;
                    #if UNITY_2018_2_OR_NEWER
                    parentObject = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(_object);
                    #else
                    parentObject = UnityEditor.PrefabUtility.GetPrefabParent(_object);
                    #endif
                    path = UnityEditor.AssetDatabase.GetAssetPath(parentObject);
                }
            }
            #endif
            return path;
        }


        #region "Float comparisons"
        public static bool IsApproximately(float _a, float _b, float _tolerance = 0.01f)
        {
            return Mathf.Abs(_a - _b) < _tolerance;
        }
        #endregion


        #region "XML"
        public static void CreateXML<T>(ref string _path, object _object)
        {
            // New function to write better xml style with utf8 encoding
            FileStream fs = new FileStream(_path, FileMode.Create);
            XmlSerializer xs = new XmlSerializer(typeof(T));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(fs, Encoding.UTF8);
            xmlTextWriter.Formatting = Formatting.Indented;
            xs.Serialize(xmlTextWriter, _object);

            fs.Close();
        }


        /// <summary> returns serialization of this object as string </summary>
        public static string GetString<T>(object _object)
        {
            string data = SerializeObject<T>(ref _object);
            return data;
        }


        /// <summary> Loads object from _path as T </summary>
        public static T LoadXML<T>(ref string _path)
        {
            StreamReader reader = File.OpenText(_path);
            string _info = reader.ReadToEnd();
            reader.Close();
            T loadedObject = DeserializeObject<T>(_info);
            return loadedObject;
        }


        /// <summary> Deserializes _info into an object of T </summary>
        public static T LoadData<T>(ref string _info)
        {
            T loadedObject = DeserializeObject<T>(_info);
            return loadedObject;
        }


        /// <summary> Deletes _name from library folder </summary>
        public static void DeleteLibraryXML(string _name, bool _isExtrusion)
        {
            string path;
            if (_isExtrusion)
            {
                path = Application.dataPath + "/RoadArchitect/Library/ESO" + _name + ".rao";
            }
            else
            {
                path = Application.dataPath + "/RoadArchitect/Library/EOM" + _name + ".rao";
            }
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }


        private static string SerializeObject<T>(ref object _object)
        {
            string XmlizedString = null;
            MemoryStream memoryStream = new MemoryStream();

            XmlSerializer xs = new XmlSerializer(typeof(T));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xmlTextWriter.Formatting = Formatting.Indented;
            xs.Serialize(xmlTextWriter, _object);

            memoryStream = (MemoryStream) xmlTextWriter.BaseStream;
            XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());

            return XmlizedString;
        }


        private static T DeserializeObject<T>(string _xmlString)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(_xmlString));
            return (T)xs.Deserialize(memoryStream);
        }


        private static string UTF8ByteArrayToString(byte[] _characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(_characters);
            return (constructedString);
        }


        private static byte[] StringToUTF8ByteArray(string _xmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteArray = encoding.GetBytes(_xmlString);
            return byteArray;
        }
        #endregion


        #region "Mesh tangents"
        //Thread safe because local scope and by val params
        /// <summary> returns calculated tangents </summary>
        public static Vector4[] ProcessTangents(int[] _tris, Vector3[] _normals, Vector2[] _uvs, Vector3[] _verts)
        {
            int MVL = _verts.Length;
            if (MVL == 0)
            {
                return new Vector4[0];
            }
            // mesh.triangles.Length / 3;
            int triangleCount = _tris.Length;
            Vector3[] tan1 = new Vector3[MVL];
            Vector3[] tan2 = new Vector3[MVL];
            Vector4[] tangents = new Vector4[MVL];
            int i1, i2, i3;
            Vector3 v1, v2, v3;
            Vector2 w1, w2, w3;
            float x1, x2, y1, y2, z1, z2, s1, s2, t1, t2, r;
            Vector3 sdir, tdir;
            float div = 0f;


            for (int a = 0; a < triangleCount; a += 3)
            {
                i1 = _tris[a + 0];
                i2 = _tris[a + 1];
                i3 = _tris[a + 2];

                v1 = _verts[i1];
                v2 = _verts[i2];
                v3 = _verts[i3];

                w1 = _uvs[i1];
                w2 = _uvs[i2];
                w3 = _uvs[i3];

                x1 = v2.x - v1.x;
                x2 = v3.x - v1.x;
                y1 = v2.y - v1.y;
                y2 = v3.y - v1.y;
                z1 = v2.z - v1.z;
                z2 = v3.z - v1.z;

                s1 = w2.x - w1.x;
                s2 = w3.x - w1.x;
                t1 = w2.y - w1.y;
                t2 = w3.y - w1.y;

                //r = 1.0f / (s1 * t2 - s2 * t1);
                div = (s1 * t2 - s2 * t1);
                r = div == 0.0f ? 0.0f : 1.0f / div;

                sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }


            Vector3 n;
            Vector3 t;
            for (int index = 0; index < MVL; index++)
            {
                n = _normals[index];
                t = tan1[index];

                Vector3.OrthoNormalize(ref n, ref t);
                tangents[index].x = t.x;
                tangents[index].y = t.y;
                tangents[index].z = t.z;
                tangents[index].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[index]) < 0.0f) ? -1.0f : 1.0f;

                //tmp = (t - n * Vector3.Dot(n, t)).normalized;
                //tangents[i] = new Vector4(tmp.x, tmp.y, tmp.z);
                //tangents[i].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f;
            }

            return tangents;
        }


        /// <summary> Get copies of mesh values and assign calculated tangents to mesh </summary>
        public static void ProcessTangents(ref Mesh _mesh)
        {
            Vector3[] vertices = _mesh.vertices;
            Vector2[] uv = _mesh.uv;
            Vector3[] normals = _mesh.normals;
            int[] triangles = _mesh.triangles;

            _mesh.tangents = ProcessTangents(triangles, normals, uv, vertices);
        }
        #endregion


        #region "Default directory for library etc"
        /// <summary> Gives the RoadArchitect Library </summary>
        public static string GetDirLibraryBase()
        {
            string libraryPath = Path.Combine(Path.Combine(RoadEditorUtility.GetBasePathForIO(), "Editor"), "Library");
            return libraryPath;
        }


        /// <summary> Returns relative RoadArchitect/Editor/Library with OS compatible directory separator </summary>
        public static string GetDirLibrary()
        {
            string path = GetDirLibraryBase();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }


        /// <summary> Checks for the existing Library folders or creates them if not present </summary>
        public static void CheckCreateSpecialLibraryDirs()
        {
            string libraryPath = GetDirLibraryBase();
            string directoryPath = Path.Combine(libraryPath, "EdgeObjects");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            directoryPath = Path.Combine(libraryPath, "ExtrudedObjects");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            directoryPath = Path.Combine(libraryPath, "W");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            // Creates Groups folder and Default folder in it if they do not exist
            directoryPath = Path.Combine(Path.Combine(libraryPath, "Groups"), "Default");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
        #endregion


        /// <summary> Force Unity to free memory </summary>
        public static void ForceCollection(bool _isWait = false)
        {
            #if UNITY_EDITOR
            System.GC.Collect();
            if (_isWait)
            {
                System.GC.WaitForPendingFinalizers();
            }
            Resources.UnloadUnusedAssets();
            #endif
        }


        /// <summary> Setup a unique ID </summary>
        public static void SetupUniqueIdentifier(ref string _uID)
        {
            if (_uID == null || _uID.Length < 4)
            {
                _uID = System.Guid.NewGuid().ToString();
            }
        }


        #region "Profiling"
        #if UNITY_2018_1_OR_NEWER
        [Unity.Burst.BurstDiscard]
        #endif
        public static void StartProfiling(Road _road, string _profileName)
        {
            if (_road.isProfiling)
            {
                UnityEngine.Profiling.Profiler.BeginSample(_profileName);
            }
        }


        #if UNITY_2018_1_OR_NEWER
        [Unity.Burst.BurstDiscard]
        #endif
        public static void EndProfiling(Road _road)
        {
            if (_road.isProfiling)
            {
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }


        #if UNITY_2018_1_OR_NEWER
        [Unity.Burst.BurstDiscard]
        #endif
        public static void EndStartProfiling(Road _road, string _profileName)
        {
            if (_road.isProfiling)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                UnityEngine.Profiling.Profiler.BeginSample(_profileName);
            }
        }
        #endregion
    }
}
