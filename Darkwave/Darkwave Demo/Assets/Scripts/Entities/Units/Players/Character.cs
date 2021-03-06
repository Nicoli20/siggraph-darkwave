﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : Unit 
{
	public int treasures=0;
	//Used in healthController()
	bool inLitArea = true;
	//Used for MoveController()
	CharacterController controller;
	float jumpPower, jumpCounter = 0.0F;
	//Used in CameraController()
	float hRotation = 0F, vRotation = 0F;
	//Used in DeathController()
	int deathCounter = 0;
	float respawnTimer = -99;
	Vector3 respawnPoint;
	//Used in WeaponController()
	public bool causedHeadShot=false; // True if a headshot was made, then sets itself back to false after use.
	public CharacterHUD hud;

	protected void Start()
	{
		UnitStart();
		controller = GetComponent<CharacterController>();
		// Spawn point of the character.
		respawnPoint = new Vector3(
			GameObject.FindGameObjectWithTag("Respawn").transform.position.x+Random.Range(-1,1)*5,
			GameObject.FindGameObjectWithTag("Respawn").transform.position.y,
			GameObject.FindGameObjectWithTag("Respawn").transform.position.z+Random.Range(-1,1)*5);
		InvokeRepeating("healthRegenController",1,1);
        
		/*
		Use these three lines for new effects. Swap "Crippled" for any other effect if needed. The number "10" is duration.
		tempEff = gameObject.AddComponent<Crippled>();
		tempEff.EffectStart(10,this,this);
		NewEffect(tempEff);

		//NewEffectSwitch("Empowered",10,this,this); old; don't use

		Debug.Log ("The longest duration is " + longestEmp.duration);
		*/
	}

	/*
	new public void NewEffectSwitch(string effectName, int duration, Unit sourceUnit, Unit targetUnit)
	{
		base.NewEffectSwitch(effectName, duration, sourceUnit, targetUnit);
	}
	*/
	/// Additionally runs updateEffectTimers.
	new public void NewEffect(Effect newEff)
	{
		Debug.Log ("NewEffect() is running.");
		base.NewEffect(newEff);
		hud.updateEffectTimers(newEff);
	}

	// Called every frame.
	protected void Update() 
	{
		UnitUpdate();
		CameraController();
		MoveController();



		// Runs WeaponController() if character is still alive. Else, it runs DeathController().
		if(health>0)
		{ 
			dying = false;
			aggroValue = baseAggroValue + treasures;
			WeaponController();
		}
		else if(!dying)
		{
			dying=true;
			aggroValue = 0;
			CancelInvoke("healthRegenController");
			InvokeRepeating("DeathController",0,1);
		}
	}

	// Controls Movement(old)
	/*
	void MoveController()
	{
		float jumpSpeed = 20.0F;
		float jumpPower = .5F;

		Vector3 moveDirection = Vector3.zero;

		CharacterController controller = GetComponent<CharacterController>();
		if(health > 0)
		{
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);// makes input directions camera relative
			moveDirection *= baseSpeed * speedMod;

			if (controller.isGrounded) 
			{
				jumpCounter = jumpPower;
				moveDirection *= 2;
			}
			else if(!controller.isGrounded && !Input.GetButton("Jump")) jumpCounter = 0;
			else if(jumpCounter > 0) jumpCounter -= 1*Time.deltaTime;

			if (Input.GetButton("Jump") && jumpCounter > 0) moveDirection.y = jumpSpeed;
		}
		moveDirection.y += Physics.gravity.y;
		controller.Move(moveDirection * Time.deltaTime);
		
	}
*/
	void MoveController()
	{	
		MoveDirection = Vector3.zero;

		if(health > 0)
		{
			MoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			MoveDirection = transform.TransformDirection(MoveDirection);// makes input directions camera relative
			MoveDirection *= (1 + augmentedSpeed);

			if (controller.isGrounded) 
			{
				if (Input.GetButton("Jump"))
				{
					if(jumpCounter == 0)
						jumpCounter = 1.5f;
					else
					jumpCounter+=Time.deltaTime; 
					if(jumpCounter > 2) jumpCounter=2;
				}
				else
				{
					jumpPower = jumpCounter*10;
					jumpCounter=0;
				}
			}
			else
			{
				jumpPower-=Time.deltaTime*5;
				MoveDirection /=2;
			}
		}
		MoveDirection = new Vector3(MoveDirection.x, jumpPower + Physics.gravity.y, MoveDirection.z);
		controller.Move(MoveDirection * Time.deltaTime);
	}

	void CameraController()
	{
		float horizontalSpeed = 7.0F;
		float verticalSpeed = 7.0F;

		//Rotates Player on "X" Axis Acording to Mouse Input
		hRotation = (hRotation + horizontalSpeed * Input.GetAxis("Mouse X"))%360;
		transform.localEulerAngles = new Vector3(0, hRotation, 0);
		
		//Rotates Player on "Y" Axis Acording to Mouse Input
		vRotation = Mathf.Clamp(vRotation - verticalSpeed * Input.GetAxis("Mouse Y"), -90,90);
		Camera.main.transform.localEulerAngles = new Vector3(vRotation, 0, 0);

		RaycastHit hit;

		if(Physics.Raycast(GetComponentInChildren<Camera>().transform.position, 
		                   GetComponentInChildren<Camera>().transform.forward, out hit))
			FocusPoint = hit.point;
		else FocusPoint = Vector3.zero;
		Debug.DrawLine(transform.position, Vector3.zero, Color.cyan);

	}

	void WeaponController()
	{
		//Weapon chooser
		if(Input.GetKeyDown(KeyCode.Alpha1) && weapons[0] != null) 
		{
			weapons[WeaponChoice].SetActive(false);
			WeaponChoice=0;
			weapons[WeaponChoice].SetActive(true);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha2) && weapons[1] != null) 
		{
			weapons[WeaponChoice].SetActive(false);
			WeaponChoice=1;
			weapons[WeaponChoice].SetActive(true);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha3) && weapons[2] != null) 
		{
			weapons[WeaponChoice].SetActive(false);
			WeaponChoice=2;
			weapons[WeaponChoice].SetActive(true);

		}
		else if(Input.GetKeyDown(KeyCode.Alpha4) && weapons[3] != null)
		{
			weapons[WeaponChoice].SetActive(false);
			WeaponChoice=3;
			weapons[WeaponChoice].SetActive(true);
		}

		//Attack controller
		if(Input.GetButton("Fire1")) weapons[WeaponChoice].SendMessage("MainActionController");
		
		if(Input.GetButton("Fire2")) weapons[WeaponChoice].SendMessage("SecondaryActionController");
	}

	// Regenerates health based on distance from crystal. Separate from and stacks with an Entity's regen float.
	void healthRegenController()
	{
		float counter = (GameObject.Find("Game Controller").GetComponent<GameController>().sphereScale/2)-
					Vector3.Distance(gameObject.transform.position, 
			                 GameObject.Find("Crystal").GetComponentInChildren<Crystal>().transform.position);

		if(counter > 0) inLitArea = true;
		else inLitArea = false;

		if(inLitArea && health < maxHealth)
			health += counter / 1000;
		else if (!inLitArea)
			health += counter / 100;
	}

	//Controls respawn timer and respawn position.
	void DeathController()
	{
		if(respawnTimer == -99) 
		{
			Debug.Log ("you are dying");
			respawnTimer = deathCounter+1 * 10f;
		}
		else if(health > 0)
		{
			respawnTimer = -99;
			dying=false;
			Debug.Log("someone helped you up");
			InvokeRepeating("healthRegenController",1,1);
			CancelInvoke("DeathController");
		}
		else if( respawnTimer > 0) respawnTimer--;
		else
		{
			respawnTimer = -99;
			deathCounter++;
			this.transform.position = respawnPoint;
			treasures = 0;
			health = maxHealth;
			dying=false;
			Debug.Log("you got better");
			InvokeRepeating("healthRegenController",1,1);
			CancelInvoke("DeathController");
		}
	}

	// OnTriggerEnter and Exit are called when entering and leaving triggers.
	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag == "Treasure")
		{
			treasures++;
			Destroy(col.gameObject);
		}
	}

	public bool InLitArea 
	{
		get 
		{
			return inLitArea;
		}
		set 
		{
			inLitArea = value;
		}
	}

	public bool Dying 
	{
		get 
		{
			return dying;
		}
		set 
		{
			dying = value;
		}
	}
}
