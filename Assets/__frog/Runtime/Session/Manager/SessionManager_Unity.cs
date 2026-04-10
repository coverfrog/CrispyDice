using Steamworks;

public partial class SessionManager
{
    public override void Awake()
    {
        base.Awake();
        
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        gameObject.name = "Session Manager";
    }

    public override void Start()
    {
        base.Start();
        
        m_onLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
        m_onLobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
    }

    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        IsQuitting = true;
    }
}