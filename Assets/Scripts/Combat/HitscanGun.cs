﻿using UnityEngine;

namespace Combat
{
	public class HitscanGun : BaseGun
	{
	    public HitscanGun(WeaponController wp, GunController gc) : base(wp, gc)
	    {
	    }

	    protected override void Reset()
	    {
	    }

	    protected override void Shoot()
	    {
	        var fwd = GunController.FirePoint.TransformDirection(Vector3.forward);

	        Debug.DrawRay(GunController.FirePoint.position, fwd * WeaponController.MaxShotDistance, Color.green, 5.0f);

	        RaycastHit hit;
	        if (Physics.Raycast(GunController.FirePoint.position, fwd, out hit, WeaponController.MaxShotDistance))
	        {
	            Debug.Log("Raycast hit: " + hit.collider);
	        }
	    }
	}
}

