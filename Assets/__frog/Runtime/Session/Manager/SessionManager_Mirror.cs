using FrogLibrary;
using Mirror;
using UnityEngine.SceneManagement;

public partial class SessionManager
{
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        SceneManager.LoadScene("__project/1_scenes/Lobby");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        
    }
}