using UnityEngine;

public class AssetController
{
    public static AssetController ME = new AssetController();

    public Sprite[] EyeAnimation { get; private set; }
    public Sprite[] GlowAnimation { get; private set; }

    public AssetController()
    {
        EyeAnimation = Resources.LoadAll<Sprite>("Images/eye/");
        GlowAnimation = Resources.LoadAll<Sprite>("Images/glow/");
    }
}
