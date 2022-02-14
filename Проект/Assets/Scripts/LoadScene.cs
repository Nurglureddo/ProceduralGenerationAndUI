using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
   public void Load_Scene(int PreGen)
    {
        SceneManager.LoadScene(PreGen);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
