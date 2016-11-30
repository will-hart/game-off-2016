#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
 
public class NotePad : EditorWindow {
    public int fontSize;
    public string note;
    public Texture icon;
    private Vector2 scroll;
    private GUIStyle style;
    [MenuItem( "Window/Notepad" )]
    static void Init(){
        var window = (NotePad)EditorWindow.GetWindow( typeof( NotePad ) );
        //window.maxSize = new Vector2( 200, 100 );
        window.Show();
    }
    public string GetDataPath (){
        string path = Application.dataPath;
        path = Path.Combine(path,"Standard Assets");
        path = Path.Combine(path,"Editor");
        path = Path.Combine(path,"NotePad");
        path = Path.Combine(path,"Notes.txt");
        return path;
    }
    public void SaveNote(){
        File.WriteAllText(GetDataPath(), note);
    }
    public void LoadNote(){
        note = File.ReadAllText(GetDataPath());
    }
	public void OnApplicationQuit(){
		SaveNote();
	}
	public void OnLostFocus(){
		SaveNote();
	}
	public void OnDestroy(){
		SaveNote();
	}
	public void OnFocus(){
		LoadNote();
	}
    public void OnEnable(){
		LoadNote();
        titleContent.image = icon;
        titleContent.tooltip = "A place to write down notes for your project.";
    }
    public void ApplyTextStyle(){
        style = new GUIStyle(GUI.skin.textArea);
        style.wordWrap = true;
        style.richText = true;
        style.fontSize = fontSize;
    }
    public void OnGUI(){

    	EditorGUI.BeginChangeCheck();

        fontSize = EditorGUILayout.IntSlider("Font Size",fontSize,11,18);
        scroll = EditorGUILayout.BeginScrollView(scroll,false,false);
        ApplyTextStyle();
        //GUI.SetNextControlName("TextArea");
        string tempNote = EditorGUILayout.TextArea(note, style, GUILayout.MaxHeight(position.height - 30));
        //GUI.FocusControl("TextArea"); //do this, or else the textarea can't switch
/*
        //Code to move the caret into place would go here. But this doesn't work. :(
        TextEditor te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
        if(te != null){ //move caret into remembered position
            //te.cursorIndex = 0;
            //te.selectIndex = 0;
            te.SelectNone();
            te.MoveTextEnd();
        }
*/
        EditorGUILayout.EndScrollView();
        if(EditorGUI.EndChangeCheck()){
			Undo.RecordObject(this, "Change Note");
			if(!note.Equals(tempNote)){
				note = tempNote;
				EditorUtility.SetDirty(this);
			}
		}
    }
}
#endif