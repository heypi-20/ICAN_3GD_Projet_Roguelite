using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPropa : MonoBehaviour
{
    public float _Chrono = 5f;
    public float Chornotarget = 3f;
    private int Growth;
    public bool CanGrow = true;
    // Update is called once per frame
    void Update()
    {
        if(_Chrono < 0 && CanGrow)
        {
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * 1.5f, 0.4f, gameObject.transform.localScale.z * 1.5f);
            Growth += 1;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Seed"))
        {
            CanGrow = false;
            for(int i = Growth; i >= 0; i--)
            {
                Instantiate(collision.gameObject);
                Growth -= 1;
            }
        }
    }
}
