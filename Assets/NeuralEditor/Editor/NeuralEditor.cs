using UnityEditor;
using UnityEngine;

public class NeuralEditor : MonoBehaviour
{
    public static NeuralEditorWindow currentWindow
    {
        get => EditorWindow.GetWindow<NeuralEditorWindow>(typeof(SceneView));
    }

    [MenuItem("Window/Neural editor")]
    public static void ShowWindow()
    {
        currentWindow.Show();
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
