using Steamworks;

public partial class SessionManager
{
    public override void Awake()
    {
        base.Awake();
        
        gameObject.name = "Session Manager";
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
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