using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroCutsceneController : MonoBehaviour
{
    private VideoPlayer player;

    private void Awake()
    {
        player = GetComponent<VideoPlayer>();

        //See if player.loopPointReached action is tied to the end of the video
    }

    private void Update()
    {
        if (!player.isPlaying)
        {
            //Load into the hub scene
            Debug.Log("FINISHED");
        }
    }
}
