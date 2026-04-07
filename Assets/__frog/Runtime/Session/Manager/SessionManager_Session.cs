using System;
using System.Collections;
using System.Threading.Tasks;
using FrogLibrary;
using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine;

public partial class SessionManager
{
    #region [ 스팀 ]

    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Session_JoinRoom(callback.m_steamIDLobby);
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
    {
        if (callback.m_rgfChatMemberStateChange != 0)
        {
            
        }
    }

    #endregion
    
    #region [ 호출 ]

    public void Session_Host(Action onSuccess, Action<byte> onFailure) 
        => StartCoroutine(Co_Session_Host(onSuccess, onFailure));

    public void Session_Client()
        => StartCoroutine(Co_Session_Client());
    
    public void Session_JoinRoom(CSteamID steamIdLobby) 
        => StartCoroutine(Co_Session_JoinRoom(steamIdLobby));

    public void Session_Exit(Action onSuccess, Action<byte> onFailure)
        => StartCoroutine(Co_Session_Exit(onSuccess, onFailure));

    public void Session_ReturnRoom() 
        => StartCoroutine(Co_Session_ReturnRoom());

    public void Session_Joinable(bool isJoinable)
        => SteamMatchmaking.SetLobbyJoinable(CurrentLobbyId, isJoinable);

    #endregion

    #region [ 동작 ]

    private void ApplyTransport()
    {
#if !UNITY_EDITOR
        m_sessionMode = SessionMode.Steam;
#endif
        transport = m_sessionMode switch
        {
            SessionMode.Kcp => GetComponent<KcpTransport>(),
            SessionMode.Steam => GetComponent<FizzySteamworks>(),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        Transport.active = transport;
        
        m_isSessionApply = true;
    }
    
    private IEnumerator Co_Session_Host(Action onSuccess, Action<byte> onFailure)
    {
        // Error Code
        // 0 : UnKnown
        // 1 : 스팀 콜백 에러
        // 2 : 스팀 데이터 에러
        // 3 : 미러 에러
        // 4 : 게임 매니저 스폰 에러
        // 5 : 씬 변경 되지 않음

        // [ 스팀 ]
        if (m_sessionMode == SessionMode.Steam)
        {
            TaskCompletionSource<CSteamID> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

            m_onLobbyCreated = Callback<LobbyCreated_t>.Create(callback =>
            {
                CSteamID result = CSteamID.Nil;

                try
                {
                    if (callback.m_eResult == EResult.k_EResultOK)
                    {
                        result = new CSteamID(callback.m_ulSteamIDLobby);
                    }
                }
                catch(Exception e)
                {
                    Debug.Assert(false, $"[세션] 에러: {e.Message}");
                }
                finally
                {
                    tcs.TrySetResult(result);
                }
            });
            
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivateUnique, maxConnections);
           

            bool isCreated = false;

            for (float t = 0.0f; t < 3.0f; t += Time.deltaTime)
            {
                if (tcs.Task.IsCompleted)
                {
                    isCreated = true;
                    break;
                }

                yield return null;
            }

            m_onLobbyCreated = null;

            if (!isCreated)
            {
                onFailure?.Invoke(1);
                yield break;
            }
            
            CurrentLobbyId = tcs.Task.Result;
            
            if (CurrentLobbyId == CSteamID.Nil)
            {
                onFailure?.Invoke(2);
                yield break;
            }

            SteamMatchmaking.SetLobbyData(CurrentLobbyId, k_hostAddress, SteamUser.GetSteamID().ToString());
            SteamMatchmaking.SetLobbyJoinable(CurrentLobbyId, false);
        }

        // [ 미러 ]
        StartHost();

        bool isHost = false;
        for (float t = 0.0f; t < 3.0f; t += Time.deltaTime)
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                isHost = true;
                break;
            }
            
            yield return null;
        }

        if (!isHost)
        {
            onFailure?.Invoke(3);
            yield break;
        }

        // [ 스팀 ]
        // 실제적으로 로딩이 제대로 된 시점은 이 곳이라서 이 공간에서 초대, 참여 가능하게 설정

        if (m_sessionMode == SessionMode.Steam)
        {
            SteamMatchmaking.SetLobbyType(CurrentLobbyId, ELobbyType.k_ELobbyTypeFriendsOnly);
            SteamMatchmaking.SetLobbyJoinable(CurrentLobbyId, true);
        }
        
        onSuccess?.Invoke();
    }

    private IEnumerator Co_Session_JoinRoom(CSteamID steamIdLobby)
    {
        // Error Code
        // 0 : UnKnown
        // 1 : 스팀 콜백 에러
        // 2 : 스팀 데이터 에러
        // 3 : 미러 에러
        // 4 : 씬 변경 되지 않음

        if (!m_isSessionApply) ApplyTransport();

        // [ 스팀 ]
        TaskCompletionSource<CSteamID> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        
        m_onLobbyEnter = Callback<LobbyEnter_t>.Create(callback =>
        {
            CSteamID result = CSteamID.Nil;

            try
            {
                if ((EChatRoomEnterResponse)callback.m_EChatRoomEnterResponse == EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
                {
                    result = new CSteamID(callback.m_ulSteamIDLobby);
                }
            }
            catch (Exception e)
            {
                Debug.Assert(false, $"[세션] 에러: {e.Message}");
            }
            finally
            {
                tcs.TrySetResult(result);
            }
        });
        
        SteamMatchmaking.JoinLobby(steamIdLobby);

        bool isEnter = false;
        for (float t = 0.0f; t < 3.0f; t += Time.deltaTime)
        {
            if (tcs.Task.IsCompleted)
            {
                isEnter = true;
                break;
            }

            yield return null;
        }

        m_onLobbyEnter = null;

        if (!isEnter)
        {
            yield break;
        }

        CurrentLobbyId = tcs.Task.Result;

        if (CurrentLobbyId == CSteamID.Nil)
        {
            yield break;
        }

        // [ 미러 ]
        networkAddress = SteamMatchmaking.GetLobbyData(CurrentLobbyId, k_hostAddress);

        StartClient();

        bool isConnected = false;
        for (float t = 0.0f; t < 3.0f; t += Time.deltaTime)
        {
            if (NetworkClient.isConnected)
            {
                isConnected = true;
                break;
            }
            
            yield return null;
        }

        if (!isConnected)
        {
            yield break;
        }
    }
    
    private IEnumerator Co_Session_Client()
    {
        if (!m_isSessionApply) ApplyTransport();

        StartClient();

        yield return null;
    }

    private IEnumerator Co_Session_Exit(Action onSuccess, Action<byte> onFailure)
    {
        // [스팀]
        if (CurrentLobbyId != CSteamID.Nil)
        {
            SteamMatchmaking.LeaveLobby(CurrentLobbyId);
            CurrentLobbyId = CSteamID.Nil;
        }

        // [미러]
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            StopClient();
        }

        bool isDisconnect = false;
        for (float t = 0.0f; t < 3.0f; t += Time.deltaTime)
        {
            if (!NetworkClient.isConnected)
            {
                isDisconnect = true;
                break;
            }

            yield return null;
        }

        if (!isDisconnect)
        {
            onFailure?.Invoke(1);
            yield break;
        }

        onSuccess?.Invoke();
    }
    
    private IEnumerator Co_Session_ReturnRoom()
    {
        yield return null;
    }
    
    #endregion
}