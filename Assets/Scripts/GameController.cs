using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int stageNumber;
    public static GameController Instance;
    public GameObject startPanel;
    public GameObject stagePanel;
    public GameObject gamePanel;
    public GameObject gameBkg;
    public GameObject winPanel;
    public GameObject stageButtonPrefab;
    public List<GameObject> tipWindows;
    public bool AllowOperation = false;
    public void GoToStart()
    {
        Time.timeScale = 0;
        startPanel.SetActive(true);
        stagePanel.SetActive(false);
        gamePanel.SetActive(false);
        gameBkg.SetActive(false);
        winPanel.SetActive(false);
        AllowOperation = false;
    }
    public void GoToStage()
    {
        Time.timeScale = 0;
        startPanel.SetActive(false);
        stagePanel.SetActive(true);
        gamePanel.SetActive(false);
        gameBkg.SetActive(false);
        winPanel.SetActive(false);
        AllowOperation = false;
    }
    public void GoToGame()
    {
        Time.timeScale = 1;
        startPanel.SetActive(false);
        stagePanel.SetActive(false);
        gamePanel.SetActive(true);
        gameBkg.SetActive(true);
        winPanel.SetActive(false);
        AllowOperation = true;
    }
    public void GoToWin()
    {
        Time.timeScale = 0;
        startPanel.SetActive(false);
        stagePanel.SetActive(false);
        gamePanel.SetActive(false);
        gameBkg.SetActive(false);
        winPanel.SetActive(true);
        AllowOperation = false;
    }
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        GoToStart();
        RegisterEvents();
    }
    void RegisterEvents()
    {
       
        for (int i = 0; i < MapController.Instance.stageList.Count; i++)
        {
            GameObject g = stagePanel.transform.GetChild(i + 1).gameObject;
            int idx = i;
            g.GetComponent<Button>().onClick.AddListener(delegate { MapController.Instance.SpawnStage(idx); });
            g.GetComponent<Button>().onClick.AddListener(delegate { GoToGame(); });

            
        }
    }
    public void StartGame()
    {
        MapController.Instance.ClearStage();
        GoToStage();
    }
    public void QuiGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    
    public void ShowTipForSecond(int index,float time)
    {
        if (index <= tipWindows.Count)
        {

            Image img = tipWindows[index].transform.GetComponent<Image>();
            //    TextMeshPro tmp = tipWindows[index].transform.GetChild(0).GetComponent<TextMeshPro>();
            TextMeshPro tmp = null;
            StartCoroutine(showtipforsecond(index, time,img,tmp));
        }
        
    }
    
    IEnumerator showtipforsecond(int index,float time,Image img,TextMeshPro tmp)
    {
        Time.timeScale = 0;
        float transition = time / 10f;
        float timer = 0f;
        ShowTipWindow(index);
        if (img != null) img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        if (tmp != null) tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 0);
        while (timer < transition)
        {
            timer += 0.02f;
            if (img != null) img.color = new Color(img.color.r, img.color.g, img.color.b, timer/transition);
            if (tmp != null) tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, timer / transition);
            yield return new WaitForSecondsRealtime(0.02f);
        }
        
        yield return new WaitForSecondsRealtime(time-transition*2f);
        timer = 0f;
        while (timer < transition)
        {
            timer += 0.02f;
            if (img != null) img.color = new Color(img.color.r, img.color.g, img.color.b, 1-timer / transition);
            if (tmp != null) tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b,1- timer / transition);
            yield return new WaitForSecondsRealtime(0.02f);
        }
        Time.timeScale = 1;
        CloseWindow(index);
        
        yield return null;
    }
    public void ShowTipWindow(int index)
    {
        if (index <= tipWindows.Count)
        {
            tipWindows[index].SetActive(true);
            AllowOperation = false;
        }
    }
    public void CloseWindow(int index)
    {
        tipWindows[index].SetActive(false);
        AllowOperation = true;
    }
    public void Win()
    {
        Debug.Log("Win!");
        ShowTipForSecond(3, 2f);
        Invoke("GoToWin",0.02f);
    }
}
