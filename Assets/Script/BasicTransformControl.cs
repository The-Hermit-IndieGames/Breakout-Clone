using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTransformControl : MonoBehaviour
{
    //����
    [SerializeField] public bool isMove;
    [SerializeField] private float xMoveSpeed;
    [SerializeField] private float yMoveSpeed;
    [SerializeField] private float zMoveSpeed;


    //����
    [SerializeField] public bool isRotation;
    [SerializeField] private float xRotationSpeed;      //�b:����
    [SerializeField] private float yRotationSpeed;      //�b:����
    [SerializeField] private float zRotationSpeed;      //�b:����


    //�Y��
    [SerializeField] public bool isScale;
    [SerializeField] private float xScaleSpeed;
    [SerializeField] private float yScaleSpeed;
    [SerializeField] private float zScaleSpeed;


    void FixedUpdate()
    {
        //���ʱ���
        if (isMove)
        {
            Vector3 movement = new Vector3(xMoveSpeed, yMoveSpeed, zMoveSpeed) * Time.fixedDeltaTime;
            transform.Translate(movement);
        }

        //���౱��
        if (isRotation)
        {
            Vector3 rotation = new Vector3(xRotationSpeed, yRotationSpeed, zRotationSpeed) * Time.fixedDeltaTime;
            transform.Rotate(rotation);
        }

        //�Y�񱱨�
        if (isScale)
        {
            Vector3 scale = new Vector3(xScaleSpeed, yScaleSpeed, zScaleSpeed) * Time.fixedDeltaTime;
            transform.localScale += scale;
        }
    }
}
