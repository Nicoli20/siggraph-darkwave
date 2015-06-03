﻿using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour 
{
	bool inLitArea = false;
	public GameObject[] enemies;
	public int enemyChoice, timeBetweenSpawns;
	Vector3 enemySpawnPosition;
	Quaternion enemySpawnRotation;


	// Use this for initialization
	void Start () 
	{

		enemySpawnRotation = this.transform.rotation;
		InvokeRepeating("SpawnEnemies", 1, timeBetweenSpawns);
	}

	void SpawnEnemies()
	{
		enemySpawnPosition = transform.position + new Vector3(Random.Range(-2,2),0,Random.Range(-2,2));
		if(gameObject.GetComponentInParent<GameController>().enemiesLeft > 0  && !inLitArea)
		{
			gameObject.GetComponentInParent<GameController>().enemiesLeft--;
			Instantiate(enemies[enemyChoice], enemySpawnPosition, enemySpawnRotation);
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag == "LitArea")
		{
			inLitArea=true;
		}
	}
	void OnTriggerExit(Collider col)
	{
		if(col.gameObject.tag == "LitArea")
		{
			inLitArea=false;
		}
	}
}
