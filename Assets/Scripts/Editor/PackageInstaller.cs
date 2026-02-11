using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;

public class PackageInstaller
{
    [MenuItem("Veins of Malice/Setup/Install Cinemachine")]
    public static void InstallCinemachine()
    {
        Client.Add("com.unity.cinemachine");
    }

    // Optional: Auto-run once to ensure it's installed
    // [InitializeOnLoadMethod]
    // private static void CheckAndInstall()
    // {
    //     // We avoid auto-running blindly to prevent loops, but the menu item is available.
    // }
}
