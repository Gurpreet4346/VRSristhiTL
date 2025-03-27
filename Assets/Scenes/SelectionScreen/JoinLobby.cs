using UnityEngine;
using Photon.Pun;
using TMPro;
public class JoinLobby: MonoBehaviourPunCallbacks
{
    [SerializeField] LevelList SelectedLevel=LevelList.LobbyManager;
    [SerializeField] TMP_InputField UsernameInputField;
    [SerializeField] GameObject ConnectionStatusUI;
    [SerializeField] TMP_Text ConnectionStatusText;
    bool ShowConnectionStatus;


    public void LobbyEnter() {
        if (UsernameInputField.text != null) {
            PhotonNetwork.NickName = UsernameInputField.text;
        } else { PhotonNetwork.NickName = ("Cipher" + Random.Range(1, 100)); };

        PhotonNetwork.ConnectUsingSettings();
        ConnectionStatusUI.SetActive(true);
        ShowConnectionStatus = true;
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        base.OnJoinedLobby();
        LoadLevel();
    }

    public void LoadLevel() {
        CustomSceneLoader.Instance.LoadScene(SelectedLevel);

    }
    private void Update() {
        if (ShowConnectionStatus) {
            ConnectionStatusText.text = PhotonNetwork.NetworkClientState.ToString();
        }
    }
}
