using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour {

    public int Health = 100;
    protected int hp;

    // Use this for initialization
    void Start () {
        hp = Health;	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void Damage(int dmgAmount)
    {
        hp -= dmgAmount;
        Debug.Log(gameObject.name + " 남은 체력 : " + hp);
    }
}
