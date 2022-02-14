using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingMenu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
                Pause();
                GameIsPaused = true;
        }
        //При нажатии на кнопку F программа останавливается
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        settingMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        // При нажатии на кнопку продолжить (Resume) программа продолжает работу
    }
   public void Pause()
   {
        pauseMenuUI.SetActive(true);
        settingMenu.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Метод использующийся для паузы программы
   }
    public void LoadMenu()
    {
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(false);
        settingMenu.SetActive(true);
        // При нажатии на кнопку появляется меню настроек генерации
    }
    public void LoadMainMenu()
    {
        Time.timeScale = 0f;
        settingMenu.SetActive(false);
        pauseMenuUI.SetActive(true);
        // При нажатии на кнопку появляется главное меню
    }
    public void QuitGame()
    {
        Application.Quit();
        // При нажатии на кнопку программа закрывается
    }


}
