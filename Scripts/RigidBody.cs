#region "Imports"
using UnityEngine;
#endregion


namespace RoadArchitect
{
    public class RigidBody : MonoBehaviour
    {
        [UnityEngine.Serialization.FormerlySerializedAsAttribute("MinCollVelocity")]
        public float minCollisionVelocity = 2f;
        //[UnityEngine.Serialization.FormerlySerializedAsAttribute("bIsForcedSleeping")]
        //bool isForcedSleeping = false;
        [UnityEngine.Serialization.FormerlySerializedAs("RB")]
        private Rigidbody rigidBody;
        //[UnityEngine.Serialization.FormerlySerializedAsAttribute("bIgnoreRB")]
        //bool isIgnoringRigidBody = false;


        private void Awake()
        {
            rigidBody = transform.GetComponent<Rigidbody>();
            if (rigidBody != null)
            {
                DestroyImmediate(rigidBody);
            }
        }


        /*
        private void OnCollisionEnter(Collision collision)
        {
            if ( isIgnoringRigidBody || !isForcedSleeping )
            {
                return;
            }
            Debug.Log( collision.relativeVelocity.magnitude );
            if ( rigidbody != null )
            {
                if ( collision.relativeVelocity.magnitude <= minCollisionVelocity )
                {
                    rigidbody.Sleep();
                }
                else
                {
                    //RB.isKinematic = false;
                    isForcedSleeping = false;
                    //RB.AddForce(collision.relativeVelocity*collision.relativeVelocity.magnitude*(RB.mass*0.3f));
                }
            }
        }


        private void OnCollisionExit(Collision collisionInfo)
        {
            if ( isIgnoringRigidBody || !isForcedSleeping )
            {
                return;
            }
            if ( isForcedSleeping && rigidbody != null )
            {
                rigidbody.Sleep();
            }
        }


        float TimerMax = 0.1f;
        float TimerNow = 0f;


        private void Update()
        {
            if ( isForcedSleeping )
            {
                TimerNow += Time.deltaTime;
                if ( TimerNow > TimerMax )
                {
                    if ( rigidbody != null && !rigidbody.IsSleeping() )
                    {
                        rigidbody.Sleep();
                    }
                    TimerNow = 0f;
                }
            }
        }
        */
    }
}
