using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    Renderer rend;
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;
    Vector3 angularVelocity;
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    private void Update()
    {
        time += Time.deltaTime;
        // ��ʱ����ٶ��� ����ֵΪ0
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));

        // ��һ����С�ڶ������Ҳ�  pulse * time��ʾ����
        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

        //���䷢�͵���ɫ��
        rend.material.SetFloat("_WobbleX", wobbleAmountX);
        rend.material.SetFloat("_WobbleZ", wobbleAmountZ);

        //�ٶ� ����
        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;


        // �ڶ��ٶ�ƽ��
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        // �������λ��
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }



}