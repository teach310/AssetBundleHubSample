using System;
using System.Collections;
using System.Collections.Generic;
using AssetBundleHubEditor;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEngine;

public class AssetBundleBuilder
{
    static Dictionary<string, string> GetParametersFromCommandLineArgs(string[] args)
    {
        var parameters = new Dictionary<string, string>();
        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg[0] == '-')
            {
                var key = arg.Substring(1);
                parameters[key] = args[i + 1];
            }
        }
        return parameters;
    }

    [MenuItem("Test/GetParametersFromCommandLineArgs")]
    static void TestGetParametersFromCommandLineArgs()
    {
        var args = new string[] { "SomeApp", "-arg1", "ABC", "-arg2", "DEF" };
        var parameters = GetParametersFromCommandLineArgs(args);
        UnityEngine.Debug.Assert(parameters["arg1"] == "ABC");
        UnityEngine.Debug.Assert(parameters["arg2"] == "DEF");
        UnityEngine.Debug.Log("finished");
    }

    // -outputFolder ex) AssetBundles/StandaloneOSX
    // -target StandaloneOSX
    public static void BatchBuild()
    {
        var parameters = GetParametersFromCommandLineArgs(System.Environment.GetCommandLineArgs());
        if (!parameters.TryGetValue("outputFolder", out var outputFolder))
        {
            throw new ArgumentException("outputFolderをCommandLine引数で渡してください");
        }

        if (!parameters.TryGetValue("target", out var target))
        {
            throw new ArgumentException("targetをCommandLine引数で渡してください");
        }
        var loadPath = "Assets/AssetBundleResources";

        var buildParameters = CreateBundleBuildParameters(outputFolder, target);
        var buildMap = new BuildMapFactory().Create(loadPath, false);
        AssetBundleHubEditor.BuildPipeline.BuildAssetBundles(buildParameters, buildMap);
    }

    static BundleBuildParameters CreateBundleBuildParameters(string outputFolder, string target)
    {
        BundleBuildParameters buildParameters = null;

        switch (target)
        {
            case "StandaloneOSX":
                buildParameters = new BundleBuildParameters(BuildTarget.StandaloneOSX, BuildTargetGroup.Standalone, outputFolder);
                break;
            default:
                throw new ArgumentException("unknown target");
        }

        buildParameters.UseCache = false;
        buildParameters.BundleCompression = BuildCompression.LZ4;
        buildParameters.AppendHash = false; // ファイル名にHashを加えるのはAssetBundleの機能ではなくAssetBundleHub側で行う。
        return buildParameters;
    }
}
