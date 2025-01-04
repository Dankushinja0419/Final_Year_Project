using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickInput : MonoBehaviour
{
    [Header("Progression UI")]
    [SerializeField] private GameObject progressionPanel; // Progression UI canvas

    [SerializeField] private InputActionProperty buttonB; // InputAction to display progression UI

    private bool canPress = true;

    void Update()
    {
        if (buttonB.action.triggered && canPress)
        {
            progressionPanel.SetActive(!progressionPanel.activeSelf);
            canPress = false;
            StartCoroutine(PanelCd());
        }
    }

    private void OnEnable()
    {
        buttonB.action.Enable();
    }
    private void OnDisable()
    {
        buttonB.action.Disable();
    }

    private IEnumerator PanelCd()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        canPress = true;
    }

}
