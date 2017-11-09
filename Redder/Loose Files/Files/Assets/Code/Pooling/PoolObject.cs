using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    //Ovveride void for use by the poolManager.
	public virtual void OnObjectReuse()
    {

    }

    //Same override void with overload for planes.
    public virtual void OnObjectReuse(GameObject obj)
    {

    }

    protected void Destroy()
    {
        gameObject.SetActive(false); //When any poolObject uses a Destroy() function is it disabled instead. Otherwise I'd be destroying my pooled objects.
    }
}
