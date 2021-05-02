using UnityEngine;


namespace RoadArchitect
{
    public class SplinePreviewNode
    {
        #region "Vars"
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 tangent;
        [UnityEngine.Serialization.FormerlySerializedAs("EaseIO")]
        public Vector2 easeIO;
        [UnityEngine.Serialization.FormerlySerializedAs("tTime")]
        public float time = 0f;
        [UnityEngine.Serialization.FormerlySerializedAs("OldTime")]
        public float oldTime = 0f;

        public string name = "Node-1";

        [UnityEngine.Serialization.FormerlySerializedAs("tempTime")]
        public bool isTempTime = false;

        public float tempSegmentTime = 0f;
        public float tempMinDistance = 5000f;
        public float tempMinTime = 0f;

        public int idOnSpline;
        [UnityEngine.Serialization.FormerlySerializedAs("GSDSpline")]
        public SplineC spline;
        [UnityEngine.Serialization.FormerlySerializedAs("bDestroyed")]
        public bool isDestroyed = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bPreviewNode")]
        public bool isPreviewNode = false;
        #endregion


        public void Setup(Vector3 _p, Quaternion _q, Vector2 _io, float _time, string _name)
        {
            pos = _p;
            rot = _q;
            easeIO = _io;
            time = _time;
            name = _name;
        }
    }
}
