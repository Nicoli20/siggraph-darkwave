﻿using UnityEngine;
using System.Collections;

/*
 * This code serves as the base for all game assets that will change dynamically during play.
 * Any variables created but not assigned in this script are either set in the child scripts or in the editor.
 * At the moment energy and max energy are not implemented but the plan is to have each use of an attack drain 
 * energy that will be replenished over time by the status update function.
*/
public class Entity : MonoBehaviour 
{
	//Status Variables
	public float health;
	public float maxHealth;	//set in editor
	public int touchDamage;	//set in editor
	public int aggroValue=1;
	public int stun=0;

	//Movement variables
	public float baseSpeed;	//set in editor
	
	internal float yMove = 0;
	
	public void EntityStart()
	{
		health = maxHealth;
	}

	//Function used to update entity status. Called from the fixed update of the child object
	public void EntityUpdate()
	{
		if(stun > 0) stun--;
	}

	//Stub function for implementation of an animation controller
	protected virtual void AnimationController()
	{
		//if(stun) this.gameObject.
	}

	/*Function recognizes if an entity hits a Death object. Collisions don't occur between objects if they 
	 * are both on the player or enemy layers. There may be a better way to implement entities being affected 
	 * by terrain movement
	*/
	void OnCollisionStay(Collision col)
	{
		if(col.gameObject.tag == "Death")
		{
			health = 0;
		}
	}
}
