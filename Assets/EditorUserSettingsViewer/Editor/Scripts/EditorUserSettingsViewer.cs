using System;
using System.IO;  
using UnityEditor;  
using UnityEngine;  
  
public class EditorUserSettingsViewer : EditorWindow
{
    private static readonly string WindowTitle = "EditorUserSettings Viewer";
    private static readonly Vector2 WindowMinSize = new Vector2(350, 200);

    private Vector2 _scroll;
    private string _filterText = "";
    private string[] _cachedLines;

    private bool _wrapLabelField;

#if UNITY_2017_1_OR_NEWER
    [MenuItem("Window/OkaneGames/", priority = Int32.MaxValue)]
#endif
    [MenuItem("Window/OkaneGames/EditorUserSettings Viewer")]
    [MenuItem("OkaneGames/EditorUserSettings Viewer")]
    private static void CreateWindow()
    {
        var window = CreateInstance<EditorUserSettingsViewer>();
        window.titleContent = new GUIContent(WindowTitle);
        window.minSize = WindowMinSize;
        window.Show();
    }

    void OnEnable()
    {
        ReloadEditorUserSettingsAsset();
    }

    private void ReloadEditorUserSettingsAsset()
    {
        string settingsPath = Application.dataPath.Replace("/Assets", "/UserSettings/EditorUserSettings.asset");
        if (File.Exists(settingsPath))
        {
            _cachedLines = File.ReadAllLines(settingsPath);
        }
        else
        {
            _cachedLines = new string[0];
        }
    }

    void OnGUI()
    {
        // EditorUserSettings.asset 関連
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("EditorUserSettings.asset");
            if (GUILayout.Button(new GUIContent("Reload", "Reload the 'EditorUserSettings.asset' file and reflect it in the Viewer."), GUILayout.Width(55)))
            {
                ReloadEditorUserSettingsAsset();
            }

            var message = @"Open '/UserSettings/EditorUserSettings.asset'.
                    Warning!!
Direct editing of this file is not recommended.
Use EditorUserSettings.SetConfigValue(key, value).
";
            var tempBGColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button(new GUIContent("Open", message), GUILayout.Width(40)))
            {
                var path = Application.dataPath.Replace("/Assets", "/UserSettings/EditorUserSettings.asset");
                System.Diagnostics.Process.Start(path);
            }
            GUI.backgroundColor = tempBGColor;
        }
        EditorGUILayout.EndScrollView();

        // Filter 関連
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Filter", GUILayout.Width(35));
            _filterText = GUILayout.TextField(_filterText);
            if (GUILayout.Button("Clear", GUILayout.Width(45)))
            {
                _filterText = "";
            }
        }
        EditorGUILayout.EndScrollView();

        _wrapLabelField = EditorGUILayout.Toggle("Wrap Label Text", _wrapLabelField);

        // 行データ
        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        {
            var count = 0;
            bool foundEditorUserSettings = false;
            foreach (string line in _cachedLines)
            {
                // EditorUserSettings: で始まる行までは表示する必要のないデータなので飛ばす
                if (!foundEditorUserSettings)
                {
                    if (line.Contains("EditorUserSettings")) foundEditorUserSettings = true;
                    continue;
                }

                // フィルタリング処理
                if (_filterText.Length > 0 && !line.Contains(_filterText))
                {
                    continue;
                }

                count++;

                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                GUIContent labelText = new GUIContent(line, line);
                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                if (_wrapLabelField)
                {
                    labelStyle.wordWrap = true;
                }
                EditorGUILayout.LabelField(labelText, labelStyle);
            }

            if(count == 0)
            {
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.LabelField("No data matches the filter '" + _filterText + "'.");
            }
        }
        EditorGUILayout.EndScrollView();

        DrawZeniganeLink();
    }

    private void DrawZeniganeLink()
    {
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("(C) 2023 OkaneGames / zenigane");
            if (GUILayout.Button(new GUIContent("GitHub", ""), GUILayout.Width(50)))
            {
                Application.OpenURL("https://github.com/zenigane138");
            }
            if (GUILayout.Button(new GUIContent("Blog", ""), GUILayout.Width(35)))
            {
                Application.OpenURL("https://zenigane138.hateblo.jp/?from=ab1");
            }
            if (GUILayout.Button(new GUIContent("Twitter", ""), GUILayout.Width(55)))
            {
                Application.OpenURL("https://twitter.com/zenigane138");
            }
        }
        GUILayout.EndHorizontal();
    }
}