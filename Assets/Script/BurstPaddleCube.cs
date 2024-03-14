using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstPaddleCube : MonoBehaviour
{
    private bool canBurst = true;
    private MainManager mainManager;

    [SerializeField] private GameObject gameObjects;
    [SerializeField] private GameObject vfxExplode;
    [SerializeField] private AudioSource soundEffectBurstBall;

    void Start()
    {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        soundEffectBurstBall.volume = mainManager.settings.gameSoundEffectF * 0.5f;
    }


    //窱疾浪代
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Brick") && canBurst)
        {
            soundEffectBurstBall.Play();

            //砞竚采
            GameObject vfx = Instantiate(vfxExplode, transform.position, Quaternion.identity);
            var particleSystem = vfx.GetComponent<ParticleSystem>();
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
            short burstsCount = (short)(mainManager.settings.effectsVFX * 20.0);
            bursts[0].time = 0.0f; // 眖笲︽秨﹍ミ祇甮
            bursts[0].count = burstsCount; //采计秖
            particleSystem.emission.SetBursts(bursts);


            // 畖4絛瞅ず浪琩ㄤン
            Vector3 box = new Vector3(50f, 1.5f, 1.5f);
            Collider[] colliders = Physics.OverlapBox(transform.position, box);
            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Brick"))
                {
                    Brick brick = col.GetComponent<Brick>();
                    if (brick != null)
                    {
                        brick.BrickCollision();
                    }
                }
            }
            canBurst = false;
            Destroy(gameObjects);
        }

    }
}
