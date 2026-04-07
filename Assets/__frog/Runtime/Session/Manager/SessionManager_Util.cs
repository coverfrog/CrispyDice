using Steamworks;

public partial class SessionManager
{
    public int MemberCurrentCount
    {
        get
        {
            return SteamMatchmaking.GetNumLobbyMembers(CurrentLobbyId);
        }
    }

    public int MemberLimitCount
    {
        get
        {
            return SteamMatchmaking.GetLobbyMemberLimit(CurrentLobbyId);
        }
    }

    public CSteamID[] MemberIds
    {
        get
        {
            int count = SteamMatchmaking.GetNumLobbyMembers(CurrentLobbyId);

            CSteamID[] result = new CSteamID[count];

            for (int i = 0; i < count; i++)
            {
                CSteamID memberId = SteamMatchmaking.GetLobbyMemberByIndex(CurrentLobbyId, i);
                result[i] = memberId;
            }

            return result;
        }
    }
}