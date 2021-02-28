namespace RoadArchitect
{
    public class TrafficLightSequence
    {
        [UnityEngine.Serialization.FormerlySerializedAs("bLightMasterPath1")]
        public bool isLightMasterPath1 = true;
        [UnityEngine.Serialization.FormerlySerializedAs("iLightController")]
        public TrafficLightController.iLightControllerEnum lightController = TrafficLightController.iLightControllerEnum.Regular;
        [UnityEngine.Serialization.FormerlySerializedAs("iLightSubcontroller")]
        public TrafficLightController.iLightSubStatusEnum lightSubcontroller = TrafficLightController.iLightSubStatusEnum.Green;
        [UnityEngine.Serialization.FormerlySerializedAs("tTime")]
        public float time = 10f;


        public TrafficLightSequence(bool _isPath1, TrafficLightController.iLightControllerEnum _lightController, TrafficLightController.iLightSubStatusEnum _lightSubcontroller, float _time)
        {
            isLightMasterPath1 = _isPath1;
            lightController = _lightController;
            lightSubcontroller = _lightSubcontroller;
            time = _time;
        }


        public string ToStringRA()
        {
            return "Path1: " + isLightMasterPath1 + " iLightController: " + lightController.ToString() + " iLightSubcontroller: " + lightSubcontroller.ToString() + " tTime: " + time.ToString("0F");
        }
    }
}
