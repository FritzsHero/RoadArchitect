#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
#endregion


[CustomEditor(typeof(GSDRoadSystem))]
public class GSDRoadSystemEditor : Editor
{
    #region "Vars"
    //Main target for this editor file:
    private GSDRoadSystem roadSystem;

    //Serialized properties:
    private SerializedProperty isTempMultithreading;
    private SerializedProperty isTempSaveMeshAssets;

    //Editor only variables:
    private bool isUpdateGlobalMultithread = false;
    private bool isUpdateGlobalSaveMesh = false;

    //	//Editor only camera variables:
    //	private GSDRoadIntersection[] tInters = null;
    //	private int tInterIndex = 0;
    //	private GSDSplineN[] tBridges = null;
    //	private int tBridgesIndex = 0;
    //	private bool bHasBridgeInit = false;
    //	private bool bHasInterInit = false;
    //	private bool bHasDoneEither = false;
    //	private bool bFlipEditorCamera = false;
    //	private float CameraZoomFactor = 1f;
    //	private float CameraHeightOffset = 1f;
    //	private bool bCameraCustomRot = false;
    //	private Vector3 CameraCustomRot = new Vector3(0.5f,0f,-0.5f);

    //Editor only graphic variables:
    private Texture2D loadButtonBG = null;
    private Texture2D loadButtonBGGlow = null;
    private Texture2D warningLabelBG;
    private GUIStyle warningLabelStyle;
    private GUIStyle loadButton = null;
    #endregion


    private void OnEnable()
    {
        roadSystem = (GSDRoadSystem)target;

        isTempMultithreading = serializedObject.FindProperty("isMultithreaded");
        isTempSaveMeshAssets = serializedObject.FindProperty("isSavingMeshes");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        isUpdateGlobalMultithread = false;
        isUpdateGlobalSaveMesh = false;
        EditorStyles.label.wordWrap = true;
        InitChecks();

        //Add road button:
        RoadArchitect.EditorUtilities.DrawLine();
        if (GUILayout.Button("Add road", loadButton, GUILayout.Width(128f)))
        {
            // || GUILayout.Button(btnLoadText,GSDImageButton,GUILayout.Width(16f))){
            Selection.activeObject = roadSystem.AddRoad();
        }
        RoadArchitect.EditorUtilities.DrawLine();

        //Update all roads button:
        if (GUILayout.Button("Update all roads", EditorStyles.miniButton, GUILayout.Width(120f)))
        {
            roadSystem.UpdateAllRoads();
        }

        //Multi-threading input:
        isTempMultithreading.boolValue = EditorGUILayout.Toggle("Multi-threading enabled", roadSystem.isMultithreaded);
        if (isTempMultithreading.boolValue != roadSystem.isMultithreaded)
        {
            isUpdateGlobalMultithread = true;
        }

        //Save mesh assets input:
        isTempSaveMeshAssets.boolValue = EditorGUILayout.Toggle("Save mesh assets: ", roadSystem.isSavingMeshes);
        if (isTempSaveMeshAssets.boolValue != roadSystem.isSavingMeshes)
        {
            isUpdateGlobalSaveMesh = true;
        }
        if (roadSystem.isSavingMeshes || isTempSaveMeshAssets.boolValue)
        {
            GUILayout.Label("WARNING: Saving meshes as assets is very slow and can increase road generation time by several minutes.", warningLabelStyle);
        }

        //Online manual button:
        GUILayout.Space(4f);
        if (GUILayout.Button("Online manual", EditorStyles.miniButton, GUILayout.Width(120f)))
        {
            // formerly http://microgsd.com/Support/RoadArchitectManual.aspx
            Application.OpenURL("https://github.com/MicroGSD/RoadArchitect/wiki");
        }

        //Offline manual button:
        GUILayout.Space(4f);
        if (GUILayout.Button("Offline manual", EditorStyles.miniButton, GUILayout.Width(120f)))
        {
            Application.OpenURL(GSD.Roads.GSDRoadUtilityEditor.GetRoadArchitectApplicationPath() + "/RoadArchitectManual.htm");
        }

        if (roadSystem.editorPlayCamera == null)
        {
            roadSystem.EditorCameraSetSingle();
        }
        RoadArchitect.EditorUtilities.DrawLine();

        //		bHasDoneEither = false;

        //		//View intersection
        //		DoInter();

        //View bridges
        //		DoBridges();
        //		if(bHasDoneEither){
        //			EditorGUILayout.LabelField("* Hotkeys only function when this RoadArchitectSystem object is selected", EditorStyles.miniLabel);
        //		}

        //Hotkey check:
        DoHotKeyCheck();

        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();

            //Multithreading global change:
            if (isUpdateGlobalMultithread)
            {
                roadSystem.UpdateAllRoadsMultiThreadedOption();
            }

            //Save mesh assets global change:
            if (isUpdateGlobalSaveMesh)
            {
                roadSystem.UpdateAllRoadsSavingMeshesOption();
            }
        }
    }


    private void InitChecks()
    {
        string basePath = GSD.Roads.GSDRoadUtilityEditor.GetBasePath();

        RoadArchitect.EditorUtilities.CheckLoadTexture(ref warningLabelBG, basePath + "/Editor/Icons/WarningLabelBG.png");
        RoadArchitect.EditorUtilities.CheckLoadTexture(ref loadButtonBG, basePath + "/Editor/Icons/otherbg.png");
        RoadArchitect.EditorUtilities.CheckLoadTexture(ref loadButtonBGGlow, basePath + "/Editor/Icons/otherbg2.png");

        if (loadButton == null)
        {
            loadButton = new GUIStyle(GUI.skin.button);
            loadButton.contentOffset = new Vector2(0f, 1f);
            loadButton.normal.textColor = new Color(1f, 1f, 1f, 1f);
            loadButton.normal.background = loadButtonBG;
            loadButton.active.background = loadButtonBGGlow;
            loadButton.focused.background = loadButtonBGGlow;
            loadButton.hover.background = loadButtonBGGlow;
            loadButton.fixedHeight = 16f;
            loadButton.fixedWidth = 128f;
            loadButton.padding = new RectOffset(0, 0, 0, 0);
        }

        if (warningLabelStyle == null)
        {
            warningLabelStyle = new GUIStyle(GUI.skin.textArea);
            warningLabelStyle.normal.textColor = Color.red;
            warningLabelStyle.active.textColor = Color.red;
            warningLabelStyle.hover.textColor = Color.red;
            warningLabelStyle.normal.background = warningLabelBG;
            warningLabelStyle.active.background = warningLabelBG;
            warningLabelStyle.hover.background = warningLabelBG;
            warningLabelStyle.padding = new RectOffset(8, 8, 8, 8);
        }
    }


    //	private void DoInter(){
    //		//View intersection
    //		if(!bHasInterInit){
    //			bHasInterInit = true;
    //			tInters = (GSDRoadIntersection[])GameObject.FindObjectsOfType(typeof(GSDRoadIntersection));	
    //			if(tInters == null || tInters.Length < 1){
    //				tInterIndex = -1;	
    //				tInters = null;
    //			}
    //		}
    //		if(tInters != null && tInters.Length > 0 && tInterIndex > -1){
    //			EditorGUILayout.BeginHorizontal();
    //			if(GUILayout.Button("View next intersection",GUILayout.Width(150f))){
    //				IncrementIntersection();
    //			}
    //			EditorGUILayout.LabelField("Hotkey K");
    //			EditorGUILayout.EndHorizontal();
    //			bHasDoneEither = true;
    //		}
    //	}
    //	
    //
    //	private void IncrementIntersection(){
    //		if(tInters != null && tInters.Length > 0){
    //			tInterIndex+=1;
    //			if(tInterIndex >= tInters.Length){ tInterIndex = 0; }
    //			ShowIntersection(tInterIndex);
    //		}
    //	}
    //	
    //
    //	private void DoBridges(){
    //		//View bridges
    //		if(!bHasBridgeInit){
    //			bHasBridgeInit = true;
    //			GSDSplineN[] tSplineN = (GSDSplineN[])GameObject.FindObjectsOfType(typeof(GSDSplineN));	
    //			List<GSDSplineN> tSplineNList = new List<GSDSplineN>();
    //			foreach(GSDSplineN tNode in tSplineN){
    //				if(tNode.bIsBridgeStart && tNode.bIsBridgeMatched){
    //					tSplineNList.Add(tNode);
    //				}
    //			}
    //			tBridges = tSplineNList.ToArray();
    //			tBridgesIndex = 0;
    //			if(tBridges == null || tBridges.Length < 1){ 
    //				tBridgesIndex = -1; 
    //				tBridges = null;
    //			}
    //		}
    //		
    //		if(tBridges != null && tBridges.Length > 0 && tBridgesIndex > -1){
    //			EditorGUILayout.BeginHorizontal();
    //			if(GUILayout.Button("View next bridge",GUILayout.Width(150f))){
    //				IncrementBridge();
    //			}
    //			EditorGUILayout.LabelField("Hotkey L");
    //			EditorGUILayout.EndHorizontal();
    //			if(EditorApplication.isPlaying){
    //				bool bChangeChecker = EditorGUILayout.Toggle("Flip camera Y:",bFlipEditorCamera);	
    //				if(bChangeChecker != bFlipEditorCamera){
    //					bFlipEditorCamera = bChangeChecker;
    //					ShowBridge(tBridgesIndex);
    //				}
    //			}
    //			
    //			if(EditorApplication.isPlaying){
    //				float ChangeChecker = EditorGUILayout.Slider("Zoom factor:",CameraZoomFactor,0.02f,10f);
    //				if(!GSDRootUtil.IsApproximately(ChangeChecker,CameraZoomFactor,0.001f)){
    //					CameraZoomFactor = ChangeChecker;
    //					ShowBridge(tBridgesIndex);
    //				}
    //				ChangeChecker = EditorGUILayout.Slider("Height offset:",CameraHeightOffset,0f,8f);
    //				if(!GSDRootUtil.IsApproximately(ChangeChecker,CameraHeightOffset,0.001f)){
    //					CameraHeightOffset = ChangeChecker;
    //					ShowBridge(tBridgesIndex);
    //				}
    //				
    //				bool bChangeChecker = EditorGUILayout.Toggle("Custom camera rot:",bCameraCustomRot);	
    //				if(bChangeChecker != bCameraCustomRot){
    //					bCameraCustomRot = bChangeChecker;
    //					ShowBridge(tBridgesIndex);
    //				}
    //				if(bCameraCustomRot){
    //					Vector3 vChangeChecker = default(Vector3);
    //					vChangeChecker.x = EditorGUILayout.Slider("Rotation X:",CameraCustomRot.x,-1f,1f);
    //					vChangeChecker.z = EditorGUILayout.Slider("Rotation Z:",CameraCustomRot.z,-1f,1f);
    //
    //					if(vChangeChecker != CameraCustomRot){
    //						CameraCustomRot = vChangeChecker;
    //						ShowBridge(tBridgesIndex);
    //					}
    //				}
    //			}
    //			
    //			bHasDoneEither = true;
    //		}
    //	}
    //	
    //
    //	private void IncrementBridge(){
    //		if(tBridges != null && tBridges.Length > 0){
    //			tBridgesIndex+=1;
    //			if(tBridgesIndex >= tBridges.Length){ tBridgesIndex = 0; }
    //			ShowBridge(tBridgesIndex);
    //		}
    //	}
    //	
    //
    //	private void ShowIntersection(int i){	
    //		if(EditorApplication.isPlaying && GSDRS.EditorPlayCamera != null){
    //			GSDRS.EditorPlayCamera.transform.position = tInters[i].transform.position + new Vector3(-40f,20f,-40f);
    //			GSDRS.EditorPlayCamera.transform.rotation = Quaternion.LookRotation(tInters[i].transform.position - (tInters[i].transform.position + new Vector3(-40f,20f,-40f)));
    //		}else{
    //	        SceneView.lastActiveSceneView.pivot = tInters[i].transform.position;
    //	        SceneView.lastActiveSceneView.Repaint();
    //		}
    //	}
    //	
    //
    //	private void ShowBridge(int i){
    //		if(EditorApplication.isPlaying && GSDRS.EditorPlayCamera != null){
    //			Vector3 tBridgePos = ((tBridges[i].pos - tBridges[i].BridgeCounterpartNode.pos)*0.5f)+tBridges[i].BridgeCounterpartNode.pos;
    //			float tBridgeLength = Vector3.Distance(tBridges[i].pos,tBridges[i].BridgeCounterpartNode.pos);
    //			
    //			//Rotation:
    //			Vector3 tCameraRot = Vector3.Cross((tBridges[i].pos - tBridges[i].BridgeCounterpartNode.pos),Vector3.up);
    //			if(bCameraCustomRot){
    //				tCameraRot = CameraCustomRot;
    //			}else{
    //				tCameraRot = tCameraRot.normalized;	
    //			}
    //
    //			//Calc offset:
    //			Vector3 tBridgeOffset = tCameraRot * (tBridgeLength * 0.5f * CameraZoomFactor);
    //			
    //			//Height offset:
    //			tBridgeOffset.y = Mathf.Lerp(20f,120f,(tBridgeLength*0.001f)) * CameraZoomFactor * CameraHeightOffset;
    //			
    //			GSDRS.EditorPlayCamera.transform.position = tBridgePos + tBridgeOffset;
    //			GSDRS.EditorPlayCamera.transform.rotation = Quaternion.LookRotation(tBridgePos - (tBridgePos + tBridgeOffset));
    //		}else{
    //        	SceneView.lastActiveSceneView.pivot = tBridges[i].transform.position;
    //        	SceneView.lastActiveSceneView.Repaint();
    //		}
    //	}


    //	bool bCtrl = false;
    public void OnSceneGUI()
    {
        DoHotKeyCheck();
    }


    private void DoHotKeyCheck()
    {
        bool isUsed = false;
        Event current = Event.current;
        int controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);

        //		if(current.type == EventType.KeyDown){
        //			if(current.keyCode == KeyCode.K){
        //				IncrementIntersection();
        //				bUsed = true;
        //			}else if(current.keyCode == KeyCode.L){
        //				IncrementBridge();
        //				bUsed = true;
        //			}
        //		}

        if (isUsed)
        {
            switch (current.type)
            {
                case EventType.Layout:
                    HandleUtility.AddDefaultControl(controlID);
                    break;
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(roadSystem);
        }
    }
}
#endif