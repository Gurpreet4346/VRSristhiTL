using UnityEngine;

public class LevelSwitcher : MonoBehaviour {
    [SerializeField] LevelList SelectedLevel;
    public void LoadLevel() {
        CustomSceneLoader.Instance.LoadScene(SelectedLevel);
    }
}
