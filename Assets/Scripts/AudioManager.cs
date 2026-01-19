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
            Destroy(gameObject);
        }
    }
    public void PlayerSound(AudioClip audioClip)
    {
        playerSound.clip = audioClip;
        playerSound.Play();
    }
}
