using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool booIsPlayerAlive = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void FnPlayerDie()
    {
        if (Int_ObjectSelected.Instance.booIsHiding)
        {
            return;
        }
        booIsPlayerAlive = !booIsPlayerAlive;
        if (booIsPlayerAlive == false)
        {
            UiManagerPlayer.Instance.FnGameOver();
        }        
    }
}
