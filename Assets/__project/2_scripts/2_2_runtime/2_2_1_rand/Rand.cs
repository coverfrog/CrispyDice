public static class Rand
{
    public static void Seed(int seed)
    {
        UnityEngine.Random.InitState(seed);
    }

    public static int Next(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

    public static float Next(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
}
