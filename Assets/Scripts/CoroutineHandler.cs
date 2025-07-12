using System.Collections;
using UnityEngine;

public class CoroutineHandler: MonoBehaviour
{
    public void SwitchCoroutine(IEnumerator newRoutine, ref Coroutine variableToSwitch)
    {
        if (variableToSwitch == null)
        {
            StopCoroutine(variableToSwitch);
            variableToSwitch = null;
        }

        variableToSwitch = StartCoroutine(newRoutine);
    }
} 

