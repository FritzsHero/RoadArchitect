using UnityEngine;


namespace RoadArchitect.Threading
{
    public class RoadMeshSurfaceDataJob : ThreadedJob
    {
        private object handle = new object();
        private RoadConstructorBufferMaker RCS;


        public void Setup(ref RoadConstructorBufferMaker _RCS)
        {
            RCS = _RCS;
        }


        protected override void ThreadFunction()
        {
            try
            {
                RoadConstruction.BuildSurfaceData(RCS);
            }
            catch (System.Exception exception)
            {
                lock (handle)
                {
                    RCS.road.isEditorError = true;
                    RCS.road.exceptionError = exception;
                }
            }
        }


        public RoadConstructorBufferMaker GetRCS()
        {
            RoadConstructorBufferMaker tRCS;
            lock (handle)
            {
                tRCS = RCS;
            }
            return tRCS;
        }
    }
}
