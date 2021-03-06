using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour 
{

	public bool 
		mainActionFlag = false,
		secondaryActionFlag = false,
		particleFlag;
	bool ready;

	public float 
		baseCooldown,
		augmentedCooldown,
		baseDamage,
		augmentedDamage,
		baseEnergy,
		augmentedEnergy,
		currentEnergy,
		currentCooldown=0,
		energyDrain = 0;
	public GameObject parent; // Entity wielding the weapon.

	Vector3 defaultPosition;
	public Vector3 secondaryPosition;
	internal Vector3 nextPosition;

	// Use this for initialization
	public void WeaponStart () 
	{
		//Sets parent of object
		if(this.transform.parent.gameObject.name != "Main Camera")
		{
			parent = gameObject.transform.parent.gameObject;
		}
		else
		{
			parent = gameObject.transform.parent.parent.gameObject;
		}

		//Determines if a weapon has a particle effect
		particleFlag = gameObject.GetComponentInChildren<ParticleSystem>() != null;
		defaultPosition = transform.localPosition;
		nextPosition=defaultPosition;
		augmentedEnergy=baseEnergy;
		augmentedCooldown=baseCooldown;
		augmentedDamage = baseDamage;
		currentEnergy = augmentedEnergy;
		InvokeRepeating("WeaponTime",0,.25f);

	}

	// Controls the weapon's fire rate and recharge
	protected void WeaponTime()
	{
		//
		//Continuous energy recharge
		if(currentEnergy < augmentedEnergy) 
		{
			currentEnergy++;
			if(gameObject.activeSelf && particleFlag && gameObject.GetComponentInChildren<ParticleSystem>().isStopped) 
				gameObject.GetComponentInChildren<ParticleSystem>().Play();
		}
		else if(gameObject.activeSelf && particleFlag && gameObject.GetComponentInChildren<ParticleSystem>().isPlaying) 
			gameObject.GetComponentInChildren<ParticleSystem>().Stop();

		//Controls time between attacks
		if(currentCooldown <= 0) ready=true;
		else if (parent.GetComponent<Unit>().statusEffects[6]) currentCooldown -= parent.GetComponent<Unit>().actMod; //statusEffects[6] is haste
		else currentCooldown--;
	}

	public void AttackAnimation()
	{
		//Trigger animation built into weapon object
	}

	public void MainActionController()
	{
		mainActionFlag = true;
	}

	public void SecondaryActionController()
	{

		secondaryActionFlag = true;
	}

	public Vector3 DefaultPosition {
		get {
			return defaultPosition;
		}
		set {
			defaultPosition = value;
		}
	}

	public bool Ready {
		get {
			return ready;
		}
		set {
			ready = value;
		}
	}
}
