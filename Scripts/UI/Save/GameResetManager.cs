using _02.Scripts.Resource;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameResetManager : MonoBehaviour
{
    [Header("로비 씬 이름")]
    public string lobbySceneName = "LobbyScene";

    public void ResetGameAndGoToLobby()
    {
        Time.timeScale = 1f;
        ClearDontDestroyOnLoad();

        SceneManager.LoadScene(lobbySceneName, LoadSceneMode.Single);
    }

    private void ClearDontDestroyOnLoad()
    {
        var objs = FindObjectsOfType<GameObject>(true);
        foreach (var obj in objs)
        {
            if (obj.scene.buildIndex == -1)
            {
                if (obj.name.Contains("ResourceController"))
                {
                    Destroy(obj);
                }
            }
        }
    }
}
