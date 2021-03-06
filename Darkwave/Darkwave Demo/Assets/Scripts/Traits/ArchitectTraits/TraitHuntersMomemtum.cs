﻿using UnityEngine;
using System.Collections;

public class TraitHuntersMomemtum : Trait
{
	public float cooldown; // The trait's base cooldown.
	private float currentCooldown; // The trait's current cooldown timer.

	public int focusIncrease; // Seconds of focus that is granted.
    private Effect tempEff;

	// Manages the cooldown of the trait.
	void Update()
	{
		if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
		if (currentCooldown < 0) currentCooldown = 0;
	}

	// Runs Effect() if the cooldown has expired.
	public void StartEffect()
	{
		if (playerScript.causedHeadShot && currentCooldown == 0)
		{
			Effect();
			currentCooldown = cooldown;
		}
	}

	// Increases the player's focus by the value of focusIncrease.
	void Effect()
	{
        tempEff = gameObject.AddComponent<Focused>();
        tempEff.EffectStart(focusIncrease, playerScript, playerScript);
        playerScript.NewEffect(tempEff);
    }
}
