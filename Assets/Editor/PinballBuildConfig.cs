using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class PinballBuildConfig : MonoBehaviour
{
    [MenuItem("Pinball/Build Android APK")]
    public static void BuildAndroid()
    {
        string[] scenes = new string[]
        {
            "Assets/Scenes/MainMenu",
            "Assets/Scenes/GameScene"
        };

        string path = "Builds/Android/PinballGame3D.apk";

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = path,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.stripEngineCode = true;

        BuildPipeline.BuildPlayer(options);
        Debug.Log($"Build complete: {path}");
    }

    [MenuItem("Pinball/Build iOS")]
    public static void BuildIOS()
    {
        string[] scenes = new string[]
        {
            "Assets/Scenes/MainMenu",
            "Assets/Scenes/GameScene"
        };

        string path = "Builds/iOS";

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = path,
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };

        PlayerSettings.iOS.targetOSVersionString = "13.0";
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);

        BuildPipeline.BuildPlayer(options);
        Debug.Log($"Build complete: {path}");
    }

    [MenuItem("Pinball/Build WebGL")]
    public static void BuildWebGL()
    {
        string[] scenes = new string[]
        {
            "Assets/Scenes/MainMenu",
            "Assets/Scenes/GameScene"
        };

        string path = "Builds/WebGL";

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = path,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
        PlayerSettings.WebGL.codeOptimization = WebGLCodeOptimization.Speed;

        BuildPipeline.BuildPlayer(options);
        Debug.Log($"Build complete: {path}");
    }
}
#endif
