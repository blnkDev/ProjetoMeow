using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class LevelChanger : MonoBehaviour
{
    public ExitTrigger exitAlive;
    public ExitTrigger exitDead;

    public Transform aliveSpawnPoint;
    public Transform deadSpawnPoint;

    public UnityEvent onAliveCatTransition;
    public UnityEvent onDeadCatTransition;

    public Camera mainCamera;
    public Vector3 cameraOffset; // O quanto a câmera deve andar (ex: X +30)

    private bool _transitioning = false;

    void Update()
    {
        if (!_transitioning && exitAlive.isCatInside && exitDead.isCatInside)
        {
            StartCoroutine(TransitionRoutine());
        }
    }

    IEnumerator TransitionRoutine()
    {
        _transitioning = true;

        // Dispara os triggers de animacao via UnityEvent
        onAliveCatTransition?.Invoke();
        onDeadCatTransition?.Invoke();

        Debug.Log("Gatos entrando na porta...");

        // Espere o tempo da animação de entrada
        yield return new WaitForSeconds(1.0f);

        // 2. Teleporte os gatos para pontos dedicados
        if (aliveSpawnPoint == null || deadSpawnPoint == null)
        {
            Debug.LogWarning("LevelChanger: defina aliveSpawnPoint e deadSpawnPoint no Inspector.");
            _transitioning = false;
            yield break;
        }

        GameObject alive = GameObject.Find("AliveCat");
        GameObject dead = GameObject.Find("DeadCat");

        if (alive == null || dead == null)
        {
            Debug.LogWarning("LevelChanger: nao foi possivel encontrar AliveCat/DeadCat na cena.");
            _transitioning = false;
            yield break;
        }

        alive.transform.position = aliveSpawnPoint.position;
        dead.transform.position = deadSpawnPoint.position;

        // 3. Desloca a câmera (suavemente)
        Vector3 targetCamPos = mainCamera.transform.position + cameraOffset;
        float elapsed = 0;
        while (elapsed < 1.0f)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCamPos, elapsed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _transitioning = false;
    }
}