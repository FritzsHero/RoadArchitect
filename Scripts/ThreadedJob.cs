namespace RoadArchitect.Threading
{
    public class ThreadedJob
    {
        [UnityEngine.Serialization.FormerlySerializedAs("m_IsDone")]
        private bool isDone = false;
        [UnityEngine.Serialization.FormerlySerializedAs("m_Handle")]
        private object handle = new object();
        [UnityEngine.Serialization.FormerlySerializedAs("m_Thread")]
        private System.Threading.Thread thread = null;


        public bool IsDone
        {
            get
            {
                bool tmp;
                lock (handle)
                {
                    tmp = isDone;
                }
                return tmp;
            }
            set
            {
                lock (handle)
                {
                    isDone = value;
                }
            }
        }


        public virtual void Start()
        {
            thread = new System.Threading.Thread(Run);
            thread.Start();
        }


        public virtual void Abort()
        {
            thread.Abort();
        }


        protected virtual void ThreadFunction() { }


        protected virtual void OnFinished() { }


        public virtual bool Update()
        {
            if (IsDone)
            {
                OnFinished();
                return true;
            }
            return false;
        }


        private void Run()
        {
            ThreadFunction();
            IsDone = true;
        }
    }
}
