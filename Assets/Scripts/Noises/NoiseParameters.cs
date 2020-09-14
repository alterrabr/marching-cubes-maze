public static class NoiseParameters
{
    // Noise parameter
    private static float scale = 0.2f;
    private static float smooth = 0.3f;
    private static int mult = 10;

    // Noise usage for blend
    private static bool classicNoise = false;
    private static bool perlinNoise = false;
    private static bool simplexNoise = true;

    private static bool use3DNoise = false;

    // Iso level also here for more vary mazes
    private static float isoLevel = 0.5f;

    public static float Scale
    {
        get { return scale; }
        set { scale = value; }
    }

    public static float Smooth
    {
        get { return smooth; }
        set { smooth = value; }
    }

    public static int Mult
    {
        get { return mult; }
        set { mult = value; }
    }

    public static bool ClassicNoise
    {
        get { return classicNoise; }
        set { classicNoise = value; }
    }

    public static bool PerlinNoise
    {
        get { return perlinNoise; }
        set { perlinNoise = value; }
    }

    public static bool SimplexNoise
    {
        get { return simplexNoise; }
        set { simplexNoise = value; }
    }

    public static bool Use3DNoise
    {
        get { return use3DNoise; }
        set { use3DNoise = value; }
    }

    public static float IsoLevel
    {
        get { return isoLevel; }
        set { isoLevel = value; }
    }
}