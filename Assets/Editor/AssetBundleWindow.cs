using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Linq;
using AssetBundleHubEditor;

public class AssetBundleWindow : EditorWindow
{
    static AssetBundleWindow window;

    [MenuItem("Tools/AssetBundleWindow")]
    static void Open()
    {
        if (window == null)
            window = CreateInstance<AssetBundleWindow>();
        window.ShowUtility();
    }

    void OnGUI()
    {
        // if (GUILayout.Button("Export BuildMap"))
        // {
        //     var buildMap = CreateBuildMap(useFileNameHash);
        //     var exporter = new BuildMapExporter();
        //     exporter.Export(buildMap);
        //     Debug.Log($"export buildMap {exporter.ExportPath}");
        // }

        if (GUILayout.Button("Export BuildMap"))
        {
            AssetBundleHubEditor.BuildPipeline.ExportBuildMap();
        }

        if (GUILayout.Button("Build"))
        {
            Build();
        }
    }

    public void Build()
    {
        // シーン未保存だと実行できないため
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }
        AssetDatabase.SaveAssets();

        var outputFolder = "AssetBundles/StandaloneOSX";
        if (Directory.Exists(outputFolder))
        {
            Directory.Delete(outputFolder, true);
        }
        Directory.CreateDirectory(outputFolder);
        AssetBundleHubEditor.BuildPipeline.BuildAssetBundlesStandaloneOSX(outputFolder);
    }
}
