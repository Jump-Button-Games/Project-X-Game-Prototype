using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    private Scene scene;

    void Start() {
        scene = SceneManager.GetActiveScene();
        Debug.Log("Scene Name: " + getSceneName());
    }

     void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            SceneManager.LoadScene (loadNextScene());       
        }
    }

    string getSceneName(){
        return scene.name;
    }

    string loadNextScene(){

        string sceneNamePrefix = "Room-";

        string sceneNumber = getSceneName().Split('-').Last();

        Debug.Log("Scene Number: " + sceneNumber);

        int convertedSceneNumber = int.Parse(sceneNumber); 

        convertedSceneNumber++;

        return sceneNamePrefix + convertedSceneNumber;

    }

}
