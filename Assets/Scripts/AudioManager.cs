using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource playerSound;

   public static AudioManager instance{get; private set;}

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There is two or more Audio Managers");
        }
    }
    public void PlayerSound(AudioClip audioClip)
    {
        playerSound.clip = audioClip;
        playerSound.Play();
    }
}
