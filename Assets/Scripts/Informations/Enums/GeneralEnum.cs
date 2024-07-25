using UnityEngine;

public static class GeneralEnum
{
    public enum ResolutionEnum
    {
        _1920x1080,
        _1440x900,
        _1366x768,
        _1280x720,
        _1024x768,
        _800x600,
    }


    public static Vector2Int ToVector(ResolutionEnum resolution) => resolution switch
    {
        ResolutionEnum._1920x1080 => new Vector2Int(1920, 1080),
        ResolutionEnum._1440x900 => new Vector2Int(1440, 900),
        ResolutionEnum._1366x768 => new Vector2Int(1366, 768),
        ResolutionEnum._1280x720 => new Vector2Int(1280, 720),
        ResolutionEnum._1024x768 => new Vector2Int(1024, 768),
        ResolutionEnum._800x600 => new Vector2Int(800, 600),
        _ => new Vector2Int(1920, 1080)
    };
}
