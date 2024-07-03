using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParticleSystemControl : MonoBehaviour
{
    new ParticleSystem particleSystem = new ParticleSystem();
    [SerializeField] private float ratio = 1.0f;
    [SerializeField] private int controlType;


    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();

        if (particleSystem != null)
        {
            var emission = particleSystem.emission;
            switch (controlType)
            {
                case 0:
                    //�I�� ��w�o�g
                    emission.rateOverTime = MainManager.settingFile.backgroundVFX * 0.1f * ratio;
                    break;

                case 1:
                    //�S�� ��w�o�g
                    emission.rateOverTime = MainManager.settingFile.effectsVFX * 0.1f * ratio;
                    break;

                case 2:
                    //�S�� �z�o�o�g-�ߧY
                    ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
                    bursts[0].time = 0.0f;
                    //�ɤl�ƶq
                    bursts[0].count = (short)MainManager.settingFile.effectsVFX * 0.1f * ratio;
                    particleSystem.emission.SetBursts(bursts);
                    break;

                case -1:
                    break;

                default:
                    Debug.LogWarning("������ ControlType ����: " + controlType + "  (0 = �I�� ��w�o�g; 1 = �S�� ��w�o�g; 2 = �S�� �z�o�o�g-�ߧY)");
                    break;
            }
        }
    }
}
