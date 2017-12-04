using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public string DamageTargetTag;

    // Use this for initialization
    void Start()
    {
        if (DamageTargetTag == null)
            Debug.LogError("AttackArea.cs : DamageTargetTag를 설정해주세요");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == DamageTargetTag)
        {
            //other.SendMessage()  ** 겟 컴포넌트와 센드 메세지의 속도 및 가비지 생성정도를 테스트 해보는 것도 재미있을듯.
            var enemyCon = other.GetComponent<CharacterBase>();
            enemyCon.Damage(30);
        }
    }
}
