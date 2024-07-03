using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstPaddleCube : MonoBehaviour
{
    private bool canBurst = true;

    [SerializeField] private GameObject gameObjects;
    [SerializeField] private GameObject vfxExplode;
    [SerializeField] private AudioSource soundEffectBurstBall;

    void Start()
    {
        soundEffectBurstBall.volume = MainManager.settingFile.gameSoundEffectF * 0.5f;
    }


    //�I���˴�
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Brick") && canBurst)
        {
            soundEffectBurstBall.Play();

            //�]�m�ɤl
            GameObject vfx = Instantiate(vfxExplode, transform.position, Quaternion.identity);


            // �b�d���ˬd��L����
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
