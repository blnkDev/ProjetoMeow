using UnityEngine;
using UnityEngine.Serialization;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine.Rendering;

public class MeowSwitcher : MonoBehaviour
{
    [FormerlySerializedAs("GatoAzul")]
    [SerializeField] private Character gatoAzul;

    [FormerlySerializedAs("GatoVermelho")]
    [SerializeField] private Character gatoVermelho;

    private bool _focadoNoAzul = true;

    [SerializeField] private Volume deadVolume;
    [SerializeField] private float deadVolumeTransitionDuration = 0.2f;
    private float _targetDeadVolumeWeight;

    [Header("Áudio")]
    [FormerlySerializedAs("SomTransicao")]
    [SerializeField] private AudioClip transicaoDeathParaAlive;

    [SerializeField] private AudioClip transicaoAliveParaDeath;

    // Campo legado para compatibilidade com cenas/prefabs antigos
    [FormerlySerializedAs("somTransicao")]
    [SerializeField] private AudioClip somTransicaoLegado;

    [FormerlySerializedAs("SomAlive")]
    [SerializeField] private AudioClip somAlive;

    [FormerlySerializedAs("SomDeath")]
    [SerializeField] private AudioClip somDeath;

    // IDs fixos para controlar os fades por som
    private const int AliveSoundId = 1001;
    private const int DeathSoundId = 1002;

    private void Start()
    {
        HabilitarPersonagem(gatoAzul, true);
        HabilitarPersonagem(gatoVermelho, false);

        _targetDeadVolumeWeight = _focadoNoAzul ? 0f : 1f;
        if (deadVolume != null)
        {
            deadVolume.weight = _targetDeadVolumeWeight;
        }

        IniciarMusicas();
    }

    private void IniciarMusicas()
    {
        // Disparamos as duas músicas ao mesmo tempo para manter o sincronismo
        // Volume inicial: Alive 100%, Death 0%
        if (somAlive != null)
        {
            MMSoundManagerSoundPlayEvent.Trigger(
                somAlive,
                MMSoundManager.MMSoundManagerTracks.Music,
                transform.position,
                loop: true,
                volume: 1f,
                ID: AliveSoundId);
        }

        if (somDeath != null)
        {
            MMSoundManagerSoundPlayEvent.Trigger(
                somDeath,
                MMSoundManager.MMSoundManagerTracks.Music,
                transform.position,
                loop: true,
                volume: 0f,
                ID: DeathSoundId);
        }
    }

    private void Update()
    {
        AtualizarDeadVolumeGradual();

        if (Input.GetKeyDown(KeyCode.P))
        {
            // Curto (0.1s a 0.2s) e intenso
            MMCameraShakeEvent.Trigger(
                duration: 0.15f,
                amplitude: 2.5f,
                frequency: 40f,
                amplitudeX: 0f,
                amplitudeY: 0f,
                amplitudeZ: 0f,
                infinite: false,
                channelData: null,
                useUnscaledTime: false);
            AlternarFoco();
        }
    }

    private void AtualizarDeadVolumeGradual()
    {
        if (deadVolume == null)
        {
            return;
        }

        float duration = Mathf.Max(0.01f, deadVolumeTransitionDuration);
        float speed = 1f / duration;
        deadVolume.weight = Mathf.MoveTowards(deadVolume.weight, _targetDeadVolumeWeight, speed * Time.deltaTime);
    }

    private void AlternarFoco()
    {
        _focadoNoAzul = !_focadoNoAzul;

        _targetDeadVolumeWeight = _focadoNoAzul ? 0f : 1f;

        if (gatoAzul?.ConditionState != null)
        {
            gatoAzul.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
        }

        if (gatoVermelho?.ConditionState != null)
        {
            gatoVermelho.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
        }
        
        TocarSfxTransicaoDirecional(_focadoNoAzul);

        if (_focadoNoAzul)
        {
            HabilitarPersonagem(gatoAzul, true);
            HabilitarPersonagem(gatoVermelho, false);

            // Crossfade: Alive (Sobe para 1), Death (Desce para 0) em 0.5 segundos
            MMSoundManagerSoundFadeEvent.Trigger(MMSoundManagerSoundFadeEvent.Modes.PlayFade, AliveSoundId, 0.5f, 1f, MMTweenType.DefaultEaseInCubic);
            MMSoundManagerSoundFadeEvent.Trigger(MMSoundManagerSoundFadeEvent.Modes.PlayFade, DeathSoundId, 0.5f, 0f, MMTweenType.DefaultEaseInCubic);
        }
        else
        {
            HabilitarPersonagem(gatoVermelho, true);
            HabilitarPersonagem(gatoAzul, false);

            // Crossfade: Alive (Desce para 0), Death (Sobe para 1) em 0.5 segundos
            MMSoundManagerSoundFadeEvent.Trigger(MMSoundManagerSoundFadeEvent.Modes.PlayFade, AliveSoundId, 0.5f, 0f, MMTweenType.DefaultEaseInCubic);
            MMSoundManagerSoundFadeEvent.Trigger(MMSoundManagerSoundFadeEvent.Modes.PlayFade, DeathSoundId, 0.5f, 1f, MMTweenType.DefaultEaseInCubic);
        }
    }

    private void HabilitarPersonagem(Character gato, bool estado)
    {
        if (gato == null)
        {
            return;
        }

        var mov = gato.FindAbility<CharacterHorizontalMovement>();
        var jump = gato.FindAbility<CharacterJump>();

        if (mov != null)
        {
            mov.AbilityPermitted = estado;
        }

        if (jump != null)
        {
            jump.AbilityPermitted = estado;
        }

        if (!estado)
        {
            if (mov != null)
            {
                mov.SetHorizontalMove(0f);
            }

            var corgiController = gato.GetComponent<CorgiController>();
            if (corgiController != null)
            {
                corgiController.SetForce(Vector2.zero);
            }
        }
    }

    private void TocarSfxTransicaoDirecional(bool indoParaAlive)
    {
        AudioClip clip = indoParaAlive ? transicaoDeathParaAlive : transicaoAliveParaDeath;

        // Fallback para preservar comportamento em setups antigos
        if (clip == null)
        {
            clip = somTransicaoLegado;
        }

        if (clip != null)
        {
            MMSoundManagerSoundPlayEvent.Trigger(clip, MMSoundManager.MMSoundManagerTracks.Sfx, transform.position, volume: 0.35f);
        }
    }
}