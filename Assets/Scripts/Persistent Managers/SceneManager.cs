using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    private Animator _sceneTransition;
    private AudioManager _audioManager;
    private float _sceneTransitionTime;
    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (arg0, mode) =>  OnSceneLoaded(arg0, mode);
        //LoadScene(1);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    //Load Main Loop scene 
    public void LoadScene(int levelIndex)
    {
        StartCoroutine(TransitionScene(levelIndex));
    }

    IEnumerator TransitionScene(int levelIndex)
    {
        _sceneTransition.SetTrigger("Start");
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelIndex);
    }

    public IEnumerator WaitTimeToLoad(float time, int scene)
    {
        yield return new WaitForSeconds(time);
        LoadScene(scene);
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameObject.FindGameObjectWithTag("SceneTransitionEffect"))
        {
            _sceneTransition = GameObject.FindGameObjectWithTag("SceneTransitionEffect").GetComponent<Animator>();   
        }
    }
}
