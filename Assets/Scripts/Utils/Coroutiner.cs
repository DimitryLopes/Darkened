using System.Collections;
using UnityEngine;

public class Coroutiner : MonoBehaviour
{
    public void RunCoroutine(IEnumerator enumerator)
    {
        StartCoroutine(enumerator);
    }
}
