using UnityEngine;


namespace RoadArchitect.Threading
{
    public class RoadMeshGeometryJob : ThreadedJob
    {
        private object handle = new object();
        private RoadConstructorBufferMaker RCS;
        private Road road;


        public void Setup(ref RoadConstructorBufferMaker _RCS, ref Road _road)
        {
            RCS = _RCS;
            road = _road;
        }


        protected override void ThreadFunction()
        {
            try
            {
                RoadConstruction.BuildPreliminaryGeometry(road);
                RoadConstruction.BuildGeometry(RCS);
            }
            catch (System.Exception exception)
            {
                lock (handle)
                {
                    road.isEditorError = true;
                    road.exceptionError = exception;
                }
                throw exception;
            }
        }


        public RoadConstructorBufferMaker GetRCS()
        {
            RoadConstructorBufferMaker refrenceRCS;
            lock (handle)
            {
                refrenceRCS = RCS;
            }
            return refrenceRCS;
        }
    }
}
