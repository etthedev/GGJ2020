﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IThrowable
{
	void Throw(Vector3 direction);
}

public class GenericItem : MonoBehaviour, IInteractable, IThrowable
{
	public Collider ingredientCollider;
	public Rigidbody rb;

	private void Awake()
	{
		if (ingredientCollider == null)
		{
			ingredientCollider = GetComponent<Collider>();
		}
		if (rb == null)
		{
			rb = GetComponent<Rigidbody>();
		}
	}

	public virtual void Interact(GameObject instigator)
	{
		var player = instigator.GetComponent<PlayerController>();

		LetPlayerHoldItem(player);
	}

	protected virtual bool LetPlayerHoldItem(PlayerController player)
	{
		if (player != null)
		{
			if (player.heldItem == null)
			{
				ToggleColliderAndGravity(false);
				player.HoldItem(this);

				return true;
			}

			return false;
		}

		return false;
	}

	public virtual void Throw(Vector3 throwDirection)
	{
		ToggleColliderAndGravity(true);
		rb.AddForce(throwDirection, ForceMode.Impulse);
	}

	public void ToggleColliderAndGravity(bool enable)
	{
		ingredientCollider.enabled = enable;
		rb.useGravity = enable;
	}
}
