using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public Animator sceneTransition;
    private float _sceneTransitionTime;
    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (arg0, mode) =>  OnSceneLoaded(arg0, mode);
        LoadScene(1);
    }

    //Load Main Loop scene 
    public void LoadScene(int levelIndex)
    {
        StartCoroutine(TransitionScene(levelIndex));
        //Delete another existing Game Manager to prevent duplication
        if (GameObject.FindGameObjectWithTag("Managers").GetComponentInChildren<GameManager>())
        {
            Destroy(GameObject.FindGameObjectWithTag("Managers").GetComponentInChildren<GameManager>().gameObject);
        }
    }

    IEnumerator TransitionScene(int levelIndex)
    {
        sceneTransition.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
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
            sceneTransition = GameObject.FindGameObjectWithTag("SceneTransitionEffect").GetComponent<Animator>();   
        }
        sceneTransition.SetTrigger("End");
    }
}
