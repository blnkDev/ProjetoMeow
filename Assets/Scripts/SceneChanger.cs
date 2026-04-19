using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void QuitGame()
    {
        // Se estiver rodando no Editor da Unity (para você testar)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // Se for o jogo final compilado (.exe ou .app)
            Application.Quit();
#endif
        
        Debug.Log("O jogo foi fechado!");
    }
}