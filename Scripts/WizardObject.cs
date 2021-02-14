using UnityEngine;


namespace RoadArchitect
{
    public class WizardObject
    {
        [UnityEngine.Serialization.FormerlySerializedAs("Thumb")]
        public Texture2D thumb;
        [UnityEngine.Serialization.FormerlySerializedAs("ThumbString")]
        public string thumbString;
        [UnityEngine.Serialization.FormerlySerializedAs("DisplayName")]
        public string displayName;
        [UnityEngine.Serialization.FormerlySerializedAs("Desc")]
        public string desc;
        [UnityEngine.Serialization.FormerlySerializedAs("bIsDefault")]
        public bool isDefault;
        [UnityEngine.Serialization.FormerlySerializedAs("bIsBridge")]
        public bool isBridge;
        [UnityEngine.Serialization.FormerlySerializedAs("FileName")]
        public string fileName;
        [UnityEngine.Serialization.FormerlySerializedAs("FullPath")]
        public string FullPath;
        public int sortID = 0;


        public string ConvertToString()
        {
            WizardObjectLibrary WOL = new WizardObjectLibrary();
            WOL.LoadFrom(this);
            return RootUtils.GetString<WizardObjectLibrary>(WOL);
        }


        public void LoadDataFromWOL(WizardObjectLibrary _wizardObjLib)
        {
            thumbString = _wizardObjLib.thumbString;
            displayName = _wizardObjLib.displayName;
            desc = _wizardObjLib.desc;
            isDefault = _wizardObjLib.isDefault;
            fileName = _wizardObjLib.fileName;
            isBridge = _wizardObjLib.isBridge;
        }


        public static WizardObject LoadFromLibrary(string _path)
        {
            string tData = System.IO.File.ReadAllText(_path);
            string[] tSep = new string[2];
            tSep[0] = RoadUtility.FileSepString;
            tSep[1] = RoadUtility.FileSepStringCRLF;
            string[] tSplit = tData.Split(tSep, System.StringSplitOptions.RemoveEmptyEntries);
            int tSplitCount = tSplit.Length;
            WizardObjectLibrary WOL = null;
            for (int i = 0; i < tSplitCount; i++)
            {
                WOL = WizardObject.WizardObjectLibrary.WOLFromData(tSplit[i]);
                if (WOL != null)
                {
                    WizardObject WO = new WizardObject();
                    WO.LoadDataFromWOL(WOL);
                    return WO;
                }
            }
            return null;
        }


        [System.Serializable]
        public class WizardObjectLibrary
        {
            [UnityEngine.Serialization.FormerlySerializedAs("ThumbString")]
            public string thumbString;
            [UnityEngine.Serialization.FormerlySerializedAs("DisplayName")]
            public string displayName;
            [UnityEngine.Serialization.FormerlySerializedAs("Desc")]
            public string desc;
            [UnityEngine.Serialization.FormerlySerializedAs("bIsDefault")]
            public bool isDefault;
            [UnityEngine.Serialization.FormerlySerializedAs("bIsBridge")]
            public bool isBridge;
            [UnityEngine.Serialization.FormerlySerializedAs("FileName")]
            public string fileName;


            public void LoadFrom(WizardObject _wizardObj)
            {
                thumbString = _wizardObj.thumbString;
                displayName = _wizardObj.displayName;
                desc = _wizardObj.desc;
                isDefault = _wizardObj.isDefault;
                fileName = _wizardObj.fileName;
                isBridge = _wizardObj.isBridge;
            }


            public static WizardObjectLibrary WOLFromData(string _data)
            {
                try
                {
                    WizardObjectLibrary WOL = RootUtils.LoadData<WizardObjectLibrary>(ref _data);
                    return WOL;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
