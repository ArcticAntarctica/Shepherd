using UnityEngine;

public class LevelActions : MonoBehaviour
{
    private SoundManager _soundManager;

    void Start()
    {
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        _soundManager.SetButtonSfx(null);
    }
}
