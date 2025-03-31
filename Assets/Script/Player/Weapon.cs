using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePoint, fireParent;
    public GameObject bulletPrefab; 

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Shoot();
        }
    }

    void Shoot () 
    {
        GameObject gm = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        gm.transform.parent = fireParent;

        Destroy(gm, 3f);
    }

    
}
