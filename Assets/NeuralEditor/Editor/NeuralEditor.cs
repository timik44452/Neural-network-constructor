using UnityEditor;
using UnityEngine;

public class NeuralEditor : MonoBehaviour
{
    public static NeuralEditorWindow currentWindow { get; private set; }

    [MenuItem("Window/Neural editor")]
    public static void ShowWindow()
    {
        if(currentWindow == null)
        {
            currentWindow = EditorWindow.GetWindow<NeuralEditorWindow>();
            currentWindow.ShowWindow();
        }
        else
        {
            currentWindow.Focus();
        }
    }

    [MenuItem("Assets/Neural editor", true)]
    public static bool ShowWindowValidate()
    {
        return Selection.activeObject is NetworkConfiguration;
    }

    [MenuItem("Assets/Neural editor")]
    public static void ShowWindowWithContext()
    {
        ShowWindow();
    }
}
