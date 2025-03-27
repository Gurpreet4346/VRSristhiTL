using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    CharacterList SelectedCharacter=CharacterList.Earth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PhotonNetwork.Instantiate(SelectedCharacter.ToString(), new Vector3(Random.Range(-4f,4f),5f, Random.Range(-4f,4f)), Quaternion.identity );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
