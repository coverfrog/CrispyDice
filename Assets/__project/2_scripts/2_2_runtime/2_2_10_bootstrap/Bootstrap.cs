using Cysharp.Threading.Tasks;
using FrogLibrary;
using UnityEngine;

public static class Bootstrap 
{
    [RuntimeInitializeOnLoadMethod]
    public static void Boot()
    {
        AddressableUtil.InstantiateAsync<DataManager>("manager/data").Forget();
        AddressableUtil.InstantiateAsync<SessionManager>("manager/session").Forget();
        AddressableUtil.InstantiateAsync<UIManager>("manager/ui").Forget();
        AddressableUtil.InstantiateAsync<AudioManager>("manager/audio").Forget();
    }
}
