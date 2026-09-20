#region "Imports"
using UnityEngine;
#endregion


namespace RoadArchitect
{
    [System.Serializable]
    public class TrafficLightController
    {
        //Enums for controller:
        public enum iLightControllerEnum { Regular, LeftTurn, MasterLeft1, MasterLeft2, Red }
        //Enums for actual lights:
        public enum iLightStatusEnum { Regular, LeftTurn, MasterLeft, Red, RightTurn }
        public enum iLightSubStatusEnum { Green, Yellow, Red }
        public enum iLightYieldSubStatusEnum { Green, Yellow, Red, YellowTurn, GreenTurn }


        #region "Vars"
        public GameObject lightLeftObject;
        public GameObject lightRightObject;
        public GameObject[] lightsObjects;

        public MeshRenderer leftMR;
        public MeshRenderer rightMR;
        public MeshRenderer[] mainMRStorage;
        public MeshRenderer mainMR;

        public Light lightLeftR;
        public Light lightLeftY;
        public Light lightLeftG;

        public Light lightRightR;
        public Light lightRightY;
        public Light lightRightG;

        public Light[] lightsR;
        public Light[] lightsY;
        public Light[] lightsG;

        public iLightStatusEnum lightStatus = iLightStatusEnum.Red;
        public iLightSubStatusEnum lightSubStatus = iLightSubStatusEnum.Green;

        private bool isLeft = false;
        private bool isRight = false;
        private bool isMain = false;
        private bool isUsingSharedMaterial = false;
        private bool isLeftTurnYieldOnGreen = true;
        private bool isLightsEnabled = true;
        #endregion


        public TrafficLightController(ref GameObject _LightLeft, ref GameObject _LightRight, ref GameObject[] _Lights, ref MeshRenderer _MR_Left, ref MeshRenderer _MR_Right, ref MeshRenderer[] MR_Mains)
        {
            lightLeftObject = _LightLeft;
            lightRightObject = _LightRight;
            lightsObjects = _Lights;

            leftMR = _MR_Left;
            rightMR = _MR_Right;
            mainMRStorage = MR_Mains;
            mainMR = MR_Mains[0];

            FindDirectionalLights(lightLeftObject, out lightLeftR, out lightLeftY, out lightLeftG);
            FindDirectionalLights(lightRightObject, out lightRightR, out lightRightY, out lightRightG);
            CacheMainLights();
        }


        private static void FindDirectionalLights(GameObject lightObject, out Light red, out Light yellow, out Light green)
        {
            red = null;
            yellow = null;
            green = null;
            if (lightObject == null)
            {
                return;
            }

            foreach (Light light in lightObject.transform.GetComponentsInChildren<Light>())
            {
                string lightName = light.transform.name.ToLower();
                if (lightName.Contains("redlight"))
                {
                    red = light;
                }
                if (lightName.Contains("yellowlight"))
                {
                    yellow = light;
                }
                if (lightName.Contains("greenlight"))
                {
                    green = light;
                }
            }
        }


        private void CacheMainLights()
        {
            int lightCount = lightsObjects.Length;
            lightsR = new Light[lightCount];
            lightsY = new Light[lightCount];
            lightsG = new Light[lightCount];
            for (int index = 0; index < lightCount; index++)
            {
                FindDirectionalLights(lightsObjects[index], out lightsR[index], out lightsY[index], out lightsG[index]);
            }
        }


        #region "Update"
        public void UpdateLights(iLightStatusEnum _lightStatus, iLightSubStatusEnum _lightSubStatus, bool _isLightsEnabled)
        {
            isLightsEnabled = _isLightsEnabled;
            lightStatus = _lightStatus;
            lightSubStatus = _lightSubStatus;
            isUsingSharedMaterial = false;
            switch (lightStatus)
            {
                case iLightStatusEnum.Regular:
                    TriggerRegular();
                    break;
                case iLightStatusEnum.LeftTurn:
                    TriggerLeftTurn();
                    break;
                case iLightStatusEnum.MasterLeft:
                    TriggerMasterLeft();
                    break;
                case iLightStatusEnum.Red:
                    TriggerRed();
                    break;
                case iLightStatusEnum.RightTurn:
                    TriggerRightTurn();
                    break;
            }
        }
        #endregion


        #region "Triggers"
        private void TriggerRegular()
        {
            SetMainStatus(lightSubStatus);
            if (isLeft)
            {
                if (isLeftTurnYieldOnGreen)
                {
                    if (lightSubStatus == iLightSubStatusEnum.Green)
                    {
                        MRChangeLeftYield(ref leftMR, iLightYieldSubStatusEnum.Green);
                    }
                    else if (lightSubStatus == iLightSubStatusEnum.Yellow)
                    {
                        MRChangeLeftYield(ref leftMR, iLightYieldSubStatusEnum.Yellow);
                    }
                }
                else
                {
                    SetLeftStatus(iLightSubStatusEnum.Red);
                }
            }
            if (isRight)
            {
                SetRightStatus(iLightSubStatusEnum.Red);
            }
        }


        private void TriggerLeftTurn()
        {
            SetMainStatus(iLightSubStatusEnum.Red);
            if (isLeft)
            {
                if (isLeftTurnYieldOnGreen)
                {
                    if (lightSubStatus == iLightSubStatusEnum.Green)
                    {
                        MRChangeLeftYield(ref leftMR, iLightYieldSubStatusEnum.GreenTurn);
                    }
                    else if (lightSubStatus == iLightSubStatusEnum.Yellow)
                    {
                        MRChangeLeftYield(ref leftMR, iLightYieldSubStatusEnum.YellowTurn);
                    }
                    LightChange(1, lightSubStatus);
                }
                else
                {
                    SetLeftStatus(lightSubStatus);
                }
            }
            if (isRight)
            {
                SetRightStatus(iLightSubStatusEnum.Red);
            }
        }


        private void TriggerMasterLeft()
        {
            SetMainStatus(lightSubStatus);
            if (isLeft)
            {
                if (lightSubStatus == iLightSubStatusEnum.Green)
                {
                    MRChangeLeftYield(ref leftMR, iLightYieldSubStatusEnum.GreenTurn);
                }
                else if (lightSubStatus == iLightSubStatusEnum.Yellow)
                {
                    MRChangeLeftYield(ref leftMR, iLightYieldSubStatusEnum.YellowTurn);
                }
                LightChange(1, lightSubStatus);
            }
            if (isRight)
            {
                SetRightStatus(lightSubStatus);
            }
        }


        private void TriggerRightTurn()
        {
            SetMainStatus(iLightSubStatusEnum.Red);
            if (isLeft)
            {
                SetLeftStatus(iLightSubStatusEnum.Red);
            }
            if (isRight)
            {
                SetRightStatus(lightSubStatus);
            }
        }


        private void TriggerRed()
        {
            SetMainStatus(iLightSubStatusEnum.Red);
            if (isLeft)
            {
                SetLeftStatus(iLightSubStatusEnum.Red);
            }
            if (isRight)
            {
                SetRightStatus(iLightSubStatusEnum.Red);
            }
        }
        #endregion


        private void SetMainStatus(iLightSubStatusEnum status)
        {
            if (!isMain)
            {
                return;
            }

            MRChange(ref mainMR, status);
            for (int index = 1; index < mainMRStorage.Length; index++)
            {
                MRChange(ref mainMRStorage[index], status);
            }
            LightChange(0, status);
        }


        private void SetLeftStatus(iLightSubStatusEnum status)
        {
            MRChange(ref leftMR, status);
            LightChange(1, status);
        }


        private void SetRightStatus(iLightSubStatusEnum status)
        {
            MRChange(ref rightMR, status);
            LightChange(2, status);
        }


        /// <summary> Changes _MR mainTextureOffset of the material based on _lightYieldSub </summary>
        private void MRChange(ref MeshRenderer _MR, iLightSubStatusEnum _lightSub)
        {
            Material meshMaterial; 

            if (isUsingSharedMaterial)
            {
                meshMaterial = _MR.sharedMaterial;
            }
            else
            {
                meshMaterial = _MR.material;
            }


            if (_lightSub == iLightSubStatusEnum.Green)
            {
                meshMaterial.mainTextureOffset = new Vector2(0.667f, 0f);
            }
            else if (_lightSub == iLightSubStatusEnum.Yellow)
            {
                meshMaterial.mainTextureOffset = new Vector2(0.334f, 0f);
            }
            else if (_lightSub == iLightSubStatusEnum.Red)
            {
                meshMaterial.mainTextureOffset = new Vector2(0f, 0f);
            }
        }


        /// <summary> Changes _MR mainTextureOffset of the material based on _lightYieldSub </summary>
        private void MRChangeLeftYield(ref MeshRenderer _MR, iLightYieldSubStatusEnum _lightYieldSub)
        {
            Material meshMaterial;

            if (isUsingSharedMaterial)
            {
                meshMaterial = _MR.sharedMaterial;
            }
            else
            {
                meshMaterial = _MR.material;
            }


            if (_lightYieldSub == iLightYieldSubStatusEnum.Green)
            {
                meshMaterial.mainTextureOffset = isUsingSharedMaterial ? new Vector2(0.667f, 0f) : new Vector2(0.4f, 0f);
            }
            else if (_lightYieldSub == iLightYieldSubStatusEnum.Yellow)
            {
                meshMaterial.mainTextureOffset = isUsingSharedMaterial ? new Vector2(0.334f, 0f) : new Vector2(0.2f, 0f);
            }
            else if (_lightYieldSub == iLightYieldSubStatusEnum.Red)
            {
                meshMaterial.mainTextureOffset = new Vector2(0f, 0f);
            }
            else if (_lightYieldSub == iLightYieldSubStatusEnum.YellowTurn)
            {
                meshMaterial.mainTextureOffset = new Vector2(0.6f, 0f);
            }
            else if (_lightYieldSub == iLightYieldSubStatusEnum.GreenTurn)
            {
                meshMaterial.mainTextureOffset = new Vector2(0.8f, 0f);
            }
        }


        /// <summary> Change lights to current light status </summary>
        private void LightChange(int _index, iLightSubStatusEnum _lightSub)
        {
            if (!isLightsEnabled)
            {
                int meshCount = mainMRStorage.Length;
                for (int index = 0; index < meshCount; index++)
                {
                    lightsR[index].enabled = false;
                    lightsY[index].enabled = false;
                    lightsG[index].enabled = false;
                }
                if (lightLeftR != null)
                {
                    lightLeftR.enabled = false;
                }
                if (lightLeftY != null)
                {
                    lightLeftY.enabled = false;
                }
                if (lightLeftG != null)
                {
                    lightLeftG.enabled = false;
                }
                if (lightRightR != null)
                {
                    lightRightR.enabled = false;
                }
                if (lightRightY != null)
                {
                    lightRightY.enabled = false;
                }
                if (lightRightG != null)
                {
                    lightRightG.enabled = false;
                }
                return;
            }

            if (_index == 0)
            {
                //Main:
                int mCount = mainMRStorage.Length;
                for (int index = 0; index < mCount; index++)
                {
                    LightChangeHelper(ref lightsR[index], ref lightsY[index], ref lightsG[index], _lightSub);
                }
            }
            else if (_index == 1)
            {
                //Left:
                LightChangeHelper(ref lightLeftR, ref lightLeftY, ref lightLeftG, _lightSub);
            }
            else if (_index == 2)
            {
                //Right:
                LightChangeHelper(ref lightRightR, ref lightRightY, ref lightRightG, _lightSub);
            }
        }


        /// <summary> Change active light </summary>
        private void LightChangeHelper(ref Light _red, ref Light _yellow, ref Light _green, iLightSubStatusEnum _lightSub)
        {
            if (_lightSub == iLightSubStatusEnum.Green)
            {
                _red.enabled = false;
                _yellow.enabled = false;
                _green.enabled = true;
            }
            else if (_lightSub == iLightSubStatusEnum.Yellow)
            {
                _red.enabled = false;
                _yellow.enabled = true;
                _green.enabled = false;
            }
            else if (_lightSub == iLightSubStatusEnum.Red)
            {
                _red.enabled = true;
                _yellow.enabled = false;
                _green.enabled = false;
            }
        }


        #region "Setup"
        public void Setup(bool _isLeftYield)
        {
            SetupMainObjects();
            isLeft = (leftMR != null);
            isRight = (rightMR != null);
            isMain = (mainMR != null);
            isLeftTurnYieldOnGreen = _isLeftYield;
        }


        private void SetupMainObjects()
        {
            if (mainMR == null)
            {
                return;
            }
            int meshCount = mainMRStorage.Length;
            if (meshCount <= 1)
            {
                return;
            }
            if (isUsingSharedMaterial)
            {
                for (int index = 1; index < meshCount; index++)
                {
                    mainMRStorage[index].sharedMaterial = mainMR.sharedMaterial;
                }
            }
            else
            {
                Material[] materials = new Material[1];
                materials[0] = mainMR.materials[0];
                for (int index = 1; index < meshCount; index++)
                {
                    mainMRStorage[index].materials = materials;
                }
            }
        }
        #endregion
    }
}
