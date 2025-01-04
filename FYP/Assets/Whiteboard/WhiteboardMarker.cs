using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class WhiteboardMarker : MonoBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField] private int _raySizeMultiplier = 2;
    [SerializeField] private int _penSizeX = 40;
    [SerializeField] private int _penSizeZ = 15;
    [SerializeField] private Vector3 raycastOffset;
    [SerializeField] private Color markerColor = Color.blue;

    [SerializeField] private XRBaseController leftController;
    [SerializeField] private XRBaseController rightController;
    [SerializeField] private float vibrationIntensity = 0.5f;
    [SerializeField] private float vibrationInterval = 0.1f;
    private bool _isTouching;
    private float _vibrationTimer;

    private Renderer _renderer;
    private float _tipHeight;

    private RaycastHit _touch;
    private Whiteboard _whiteboard;
    private Vector2 _touchPos, _lastTouchPos;
    private bool _touchedLastFrame;
    private Quaternion _lastTouchRot;

    [SerializeField] public InputActionProperty rightGrab; // InputAction for grab
    [SerializeField] public InputActionProperty leftGrab; // InputAction for grab
    private bool rightGrabbing;
    private bool leftGrabing;

    void Start()
    {
        _renderer = _tip.GetComponent<Renderer>();
        _tipHeight = _tip.localScale.y * _raySizeMultiplier;
    }

    void Update()
    {
        rightGrabbing = rightGrab.action.IsPressed();
        leftGrabing = leftGrab.action.IsPressed();
        Draw();
    }
    private void OnEnable()
    {
        rightGrab.action.Enable();
    }
    private void OnDisable()
    {
        leftGrab.action.Disable();
    }
    private void Draw()
    {
        Vector3 raycastOrigin = _tip.position + raycastOffset;

        if (Physics.Raycast(raycastOrigin, transform.up, out _touch, _tipHeight))// && (rightGrabbing || leftGrabing))
        {
            if (_touch.transform.CompareTag("Whiteboard"))
            {
                if (_whiteboard == null)
                {
                    _whiteboard = _touch.transform.GetComponent<Whiteboard>();
                }

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                // Calculate pixel position
                var x = (int)(_touchPos.x * _whiteboard.textureSize.x - (_penSizeX / 2));
                var y = (int)(_touchPos.y * _whiteboard.textureSize.y - (_penSizeZ / 2));

                // Prevent writing outside bounds
                if (y < 0 || y > _whiteboard.textureSize.y || x < 0 || x > _whiteboard.textureSize.x) return;

                if (_touchedLastFrame)
                {
                    // Define color array and apply drawing
                    Color[] _colors = Enumerable.Repeat(markerColor, _penSizeX * _penSizeZ).ToArray();
                    _whiteboard.texture.SetPixels(x, y, _penSizeX, _penSizeZ, _colors);

                    // Smooth the lines
                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        _whiteboard.texture.SetPixels(lerpX, lerpY, _penSizeX, _penSizeZ, _colors);
                    }

                    //transform.rotation = _lastTouchRot;
                    
                    _whiteboard.texture.Apply();
                }

                _lastTouchPos = new Vector2(x, y);
                //_lastTouchRot = transform.rotation;
                _touchedLastFrame = true;

                _isTouching = true;

                if (rightGrabbing)
                {
                    RightHandVibration();
                }

                if (leftGrabing)
                {
                    LeftHandVibration();
                }


                return;
            }
        }

        _whiteboard = null;
        _touchedLastFrame = false;
        _isTouching = false;
    }

    private void RightHandVibration()
    {
        // Only vibrate at intervals
        if (_vibrationTimer <= 0 && _isTouching)
        {
            rightController?.SendHapticImpulse(0.5f, vibrationInterval);
            _vibrationTimer = vibrationInterval; // Reset timer
        }

        // Decrease timer   
        _vibrationTimer -= Time.deltaTime;
    }

    private void LeftHandVibration()
    {
        // Only vibrate at intervals
        if (_vibrationTimer <= 0 && _isTouching)
        { 
            leftController?.SendHapticImpulse(0.5f, vibrationInterval);
            _vibrationTimer = vibrationInterval; // Reset timer
        }

        // Decrease timer
        _vibrationTimer -= Time.deltaTime;
    }
}
