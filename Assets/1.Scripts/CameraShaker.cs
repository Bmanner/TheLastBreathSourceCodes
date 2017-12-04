using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public float ShakeDecay = 0.01f;
    public float ShakeIntensity = 0.05f;

    Vector3 originPosition;
    Quaternion originRotation;

    float shakeDecay;
    float shakeIntensity;
    float bivrateIntensity;
    float bivrateMultiplier;

    bool isShaking = false;
    bool isBivrating = false;

    bool frameSkipper = false;
    bool switchTemp = false;

    /*
    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 40, 120, 40), "Shake"))
        {
            Shake();
        }
    }
    */

    // TODO : Frame에 관계없이 일정한 흔들림을 갖도록 추후 수정.
    // FollowCam.cs와의 연계를 위해 lateUpdate로 변경
    void LateUpdate()
    {
        if (!frameSkipper)
            OneHalfLateUpdate();

        frameSkipper = !frameSkipper;
    }

    void OneHalfLateUpdate()
    {
        if (isBivrating)
        {
            transform.position = transform.position + Random.insideUnitSphere * ShakeIntensity * bivrateMultiplier;
            /*
            if (isAngleShakeOn)
            {
                transform.rotation = new Quaternion(
                                originRotation.x + Random.Range(-ShakeIntensity, ShakeIntensity) * .005f * bivrateMultiplier,
                                originRotation.y + Random.Range(-ShakeIntensity, ShakeIntensity) * .005f * bivrateMultiplier,
                                originRotation.z + Random.Range(-ShakeIntensity, ShakeIntensity) * .005f * bivrateMultiplier,
                                originRotation.w + Random.Range(-ShakeIntensity, ShakeIntensity) * .005f * bivrateMultiplier);
            }
            */
        }

        if (isShaking && shakeIntensity > 0)
        {
            //transform.position = transform.position + Random.insideUnitSphere * shake_intensity;
            //transform.position = transform.position + new Vector3(Random.insideUnitSphere.x * shake_intensity,0,0);
            transform.position = transform.position + new Vector3(switchTemp ? shakeIntensity : -shakeIntensity, 0, 0);
            
            if (switchTemp)
                shakeIntensity -= shakeDecay;
        }
        else
            isShaking = false;

        switchTemp = !switchTemp;
    }


    public void Shake()
    {
        isShaking = true;

        originPosition = transform.position;
        //originRotation = transform.rotation;
        shakeIntensity = ShakeIntensity;
        shakeDecay = ShakeDecay;
    }

    public void SetBivrate(float biveAmount)
    {
        bivrateMultiplier = biveAmount * .35f;

        if (bivrateMultiplier <= 0)
            isBivrating = false;
        else
            isBivrating = true;
    }
}
