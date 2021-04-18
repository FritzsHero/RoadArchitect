using UnityEngine;


namespace RoadArchitect.Threading
{
    public class RoadCalcs2 : ThreadedJob
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
                RoadCreationT.RoadJob2(ref RCS);
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
