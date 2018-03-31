using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CurtainController : MonoBehaviour
{
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void ChangeColor(Color color)
    {
        GetComponent<Image>().color = color;
    }

   public IEnumerator CheckCurtainDisable()
    {
        while (!Input.GetMouseButtonDown(0))
            yield return null;

        Disable();
    }
}
