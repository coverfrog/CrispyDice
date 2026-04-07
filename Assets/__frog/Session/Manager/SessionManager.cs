using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine;

[RequireComponent(typeof(KcpTransport))]
[RequireComponent(typeof(FizzySteamworks))]
[RequireComponent(typeof(SteamManager))]
public partial class SessionManager : NetworkManager
{
    private const string k_hostAddress = "HostAddress";
    
    public static SessionManager Instance { get; private set; }
    
    public bool IsQuitting { get; private set; }
    
    public CSteamID CurrentLobbyId { get; private set; }

    public int PlayerSpawnCount { get; private set; }

    // ->

    [Header("Session")]
    [SerializeField] private SessionMode m_sessionMode;

    public SessionMode SessionMode => m_sessionMode;

    private bool m_isSessionApply;
    
    // -> 함수 안에서 초기화
    private Callback<LobbyCreated_t> m_onLobbyCreated;
    private Callback<LobbyEnter_t> m_onLobbyEnter;

    // -> 전역 초기화
    private Callback<GameLobbyJoinRequested_t> m_onLobbyJoinRequested;
    private Callback<LobbyChatUpdate_t> m_onLobbyChatUpdate;

}
