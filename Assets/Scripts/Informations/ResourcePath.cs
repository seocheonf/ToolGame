using UnityEngine;

public static class ResourcePath
{
    public const string audioMixerPath = "Sounds/AudioMixer";

    public static string[] prefabPathArray =
    {
        
        "UIs/BasicCanvas",

        #region Object Prefab List
        "TestPrefab",
        "Monologue",
        "CapCap",

        "UniqueTool/Umbrella",
        "UniqueTool/BigLeaf",

        "UniqueTool/AboutUniqueTools/UmbrellaHookTarget",

        "SpecialInteractions/WindA",
        "SpecialInteractions/WindB",


        #endregion


        #region UI Prefab List

        "UIs/UI_Fixed_FixedUITest",
        "UIs/UI_Floating_FloatingUITest",

        "UIs/UI_Fixed_PlayableInputUI",
        "UIs/UI_Fixed_PlayableInputUIBlock"


        #endregion

    };
    public static string[] bgmPathArray =
    {
        
    };
    public static string[] sfxPathArray =
    {
        
    };

    public static string[] meshPathArray =
    {
        #region Umbrella
        "Meshes/Umbrellas/Mesh_UmbrellaOpen",
        "Meshes/Umbrellas/Mesh_UmbrellaClosed",
        #endregion
    };

    public static string[] materialPathArray =
    {
        #region Umbrella
        "Materials/Umbrellas/Material_Umbrella1",
        "Materials/Umbrellas/Material_Umbrella2",
        "Materials/Umbrellas/Material_Umbrella3",
        "Materials/Umbrellas/Material_Umbrella4",
        "Materials/Umbrellas/Material_Umbrella5",
        "Materials/Umbrellas/Material_Umbrella6"
        #endregion
    };
};