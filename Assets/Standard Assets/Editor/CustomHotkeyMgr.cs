//#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
 
public class CustomHotkeyMgr : Editor {
    
    private static Object selectedGO;
    
    [MenuItem( "Tools/BreakPrefab %b" )]
    public static void Break()
    {
        selectedGO = Selection.activeObject;
        if (selectedGO != null)        
        {           
           PrefabUtility.DisconnectPrefabInstance(selectedGO); 
        }
    }
}