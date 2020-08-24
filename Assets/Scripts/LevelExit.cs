using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 0f;
    [SerializeField] bool progressForward = true;
    [SerializeField] bool loopToMainMenu = false;
    
    Collider2D myCollider;

    const string PLAYER_LAYER = "Player";

    private void Start()
    {
        myCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Exit is colliding with something");

        //StartCoroutine(LoadNextLevel());
            var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if(loopToMainMenu)
                SceneManager.LoadScene(0);
            else if (progressForward)
                SceneManager.LoadScene(currentSceneIndex + 1);
            else
                SceneManager.LoadScene(currentSceneIndex - 1);
    }

    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex +1);
    }
}
