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
                    //背景 恆定發射
                    emission.rateOverTime = MainManager.settingFile.backgroundVFX * 0.1f * ratio;
                    break;

                case 1:
                    //特效 恆定發射
                    emission.rateOverTime = MainManager.settingFile.effectsVFX * 0.1f * ratio;
                    break;

                case 2:
                    //特效 爆發發射-立即
                    ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
                    bursts[0].time = 0.0f;
                    //粒子數量
                    bursts[0].count = (short)MainManager.settingFile.effectsVFX * 0.1f * ratio;
                    particleSystem.emission.SetBursts(bursts);
                    break;

                case -1:
                    break;

                default:
                    Debug.LogWarning("未知的 ControlType 類型: " + controlType + "  (0 = 背景 恆定發射; 1 = 特效 恆定發射; 2 = 特效 爆發發射-立即)");
                    break;
            }
        }
    }
}
