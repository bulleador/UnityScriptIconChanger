using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ChangeScriptIconWindow : EditorWindow
{
    private Texture2D _selectedIcon;
    private Vector2 _scrollPosition;
    private static IEnumerable<MonoScript> SelectedScripts => Selection.objects.Select(x => x as MonoScript).Where(x => x != null);
    
    [MenuItem("Window/Change script icons")]
    private static void ShowWindow()
    {
        ChangeScriptIconWindow window = (ChangeScriptIconWindow)GetWindow(typeof(ChangeScriptIconWindow));
        window.minSize = new Vector2(250f, 100f);
        window.Show();
    }

    [MenuItem("Assets/Change icons for selected scripts", false, 0)]
    private static void ShowWindowFromAssetMenu()
    {
        ShowWindow();
    }
    
    [MenuItem("Assets/Change icons for selected scripts", true, 0)]
    private static bool ShowWindowFromAssetMenuValidation()
    {
        return SelectedScripts.Any();
    }

    private void OnGUI()
    {
        
        GUILayout.Label("Select Icon:");
        
        _selectedIcon = EditorGUILayout.ObjectField(_selectedIcon, typeof(Texture2D), false) as Texture2D;
        GUI.enabled = _selectedIcon != null;
        
        var selectedScripts = SelectedScripts.ToArray();
        if (GUILayout.Button("Apply to Selected Scripts")) 
            SetIconForScriptAssets(selectedScripts, _selectedIcon);
        
        GUILayout.Space(10);
        
        GUILayout.Label("Affected Scripts:", EditorStyles.centeredGreyMiniLabel);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, EditorStyles.helpBox, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

        if (!selectedScripts.Any())
            EditorGUILayout.LabelField("No scripts selected.");
        else
        {
            foreach (var obj in Selection.objects)
            {
                EditorGUILayout.LabelField(obj.name);
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private static void SetIconForScriptAssets(IEnumerable<MonoScript> scriptAssets, Texture2D icon)
    {
        if (scriptAssets == null)
            throw new System.ArgumentNullException(nameof(scriptAssets));

        if (icon == null)
            throw new System.ArgumentNullException(nameof(icon));
                
        foreach (var obj in scriptAssets) 
            SetIconForScriptAsset(obj, icon);
    }
    
    private static void SetIconForScriptAsset(Object asset, Texture2D icon)
    {
        var scriptImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(asset)) as MonoImporter;
        if (scriptImporter == null)
        {
            Debug.LogWarning($"Could not find script importer for asset {asset.name}.");
            return;
        }
        
        scriptImporter.SetIcon(icon);
        scriptImporter.SaveAndReimport();
    }
}