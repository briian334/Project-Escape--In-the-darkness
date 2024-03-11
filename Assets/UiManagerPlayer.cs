using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManagerPlayer : MonoBehaviour
{
    public static UiManagerPlayer Instance { get; private set; }
    public GameObject GameOverScreen;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        GameOverScreen.SetActive(false);
    }    
    public void FnGameOver()
    {
        GameOverScreen.SetActive(true);
        FnUnlockCursor();
        Time.timeScale = 0;

    }
    public void FnRestartGame()
    {
        GameManager.Instance.booIsPlayerAlive = true;
        Time.timeScale = 1;
        FnLockCursor();
        SceneManager.LoadScene(0);
    }
    public void FnBackToMenu()
    {
        GameManager.Instance.booIsPlayerAlive = true;
        Time.timeScale = 1;
        FnUnlockCursor();
        SceneManager.LoadScene(1);
    }
    public void FnLockCursor()
    {
        //SE BLOQUEA Y SE OCULTA EL CURSOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void FnUnlockCursor()
    {
        //SE DESBLOQUEA Y SE ACTIVA EL CURSOR
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
