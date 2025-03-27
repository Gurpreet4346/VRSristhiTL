using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class HostJoinGame : MonoBehaviourPunCallbacks
{
    [SerializeField] Button CreateButton;
    [SerializeField] Button JoinButton;
    [SerializeField] TMP_InputField GameNameHost;
    [SerializeField] TMP_InputField GameNameJoin;

    private void Awake() {
        CreateButton.interactable = false;
        JoinButton.interactable = false;
    }

    public void EnableCreateButton() {
        if (GameNameHost.text != null) {
            CreateButton.interactable = true;
        }
    }

    public void EnableJoinButton() {
        if (GameNameJoin.text != null) {
            JoinButton.interactable = true;
        }
    }

    public void CreateRoom() {
        PhotonNetwork.CreateRoom(GameNameHost.text, new RoomOptions() {MaxPlayers=4, IsVisible=true, IsOpen=true }, TypedLobby.Default, null);
    }
    public void JoinRoom() {
        PhotonNetwork.JoinRoom(GameNameJoin.text);
    }

    public override void OnJoinedRoom() {
        PhotonNetwork.LoadLevel(LevelList.SampleScene.ToString());
    }

}
