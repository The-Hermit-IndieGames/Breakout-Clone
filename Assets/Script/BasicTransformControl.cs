using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTransformControl : MonoBehaviour
{
    //移動
    [SerializeField] public bool isMove;
    [SerializeField] private float xMoveSpeed;
    [SerializeField] private float yMoveSpeed;
    [SerializeField] private float zMoveSpeed;


    //旋轉
    [SerializeField] public bool isRotation;
    [SerializeField] private float xRotationSpeed;      //軸:水平
    [SerializeField] private float yRotationSpeed;      //軸:垂直
    [SerializeField] private float zRotationSpeed;      //軸:中心


    //縮放
    [SerializeField] public bool isScale;
    [SerializeField] private float xScaleSpeed;
    [SerializeField] private float yScaleSpeed;
    [SerializeField] private float zScaleSpeed;


    void FixedUpdate()
    {
        //移動控制
        if (isMove)
        {
            Vector3 movement = new Vector3(xMoveSpeed, yMoveSpeed, zMoveSpeed) * Time.fixedDeltaTime;
            transform.Translate(movement);
        }

        //旋轉控制
        if (isRotation)
        {
            Vector3 rotation = new Vector3(xRotationSpeed, yRotationSpeed, zRotationSpeed) * Time.fixedDeltaTime;
            transform.Rotate(rotation);
        }

        //縮放控制
        if (isScale)
        {
            Vector3 scale = new Vector3(xScaleSpeed, yScaleSpeed, zScaleSpeed) * Time.fixedDeltaTime;
            transform.localScale += scale;
        }
    }
}
