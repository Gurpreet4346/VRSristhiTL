using UnityEngine;
using System.Collections;
using System.Timers;
using TMPro;
using UnityEngine.UI;

public class RotateCharacterChoice : MonoBehaviour
{

    [SerializeField] float FullRotationPerSecond=0.8f;
    int SelectedCharacter =0;
    bool OngoingRotation=false;
    int NumberOfDCharacters = System.Enum.GetValues(typeof(CharacterList)).Length;

    private void Awake()
    {
    }
    public void Rotate(float angle)
    {
        if (!OngoingRotation)
        {
            StartCoroutine(SlerpItDown(angle));

        }

    }

    IEnumerator SlerpItDown(float angle)
    {
        if (angle > 0) { SelectedCharacter++; }else { SelectedCharacter--; }
        if (SelectedCharacter < 0 || SelectedCharacter >= NumberOfDCharacters)
        {
            SelectedCharacter = ((SelectedCharacter % NumberOfDCharacters) + NumberOfDCharacters) % NumberOfDCharacters;
        }

        OngoingRotation = true;
        float alpha = 0;
        Quaternion startRotation = transform.rotation;
        Quaternion finalRotation = transform.rotation * Quaternion.Euler(0, angle, 0);
        while (alpha < 1.0f)
        {
            transform.rotation = Quaternion.Slerp(startRotation, finalRotation, alpha);
            alpha += FullRotationPerSecond * Time.deltaTime;
            yield return null;
        }
        transform.rotation = finalRotation; //for precision
        OngoingRotation = false;
 //       PlayerData.Instance.SetDragonChoice((DragonType)SelectionDragon);

    }


}

