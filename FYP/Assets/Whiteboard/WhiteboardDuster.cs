using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class WhiteboardDuster : MonoBehaviour
{
    [SerializeField] private Transform _sponge;
    [SerializeField] private int _dusterSizeX = 130;
    [SerializeField] private int _dusterSizeZ = 30;
    [SerializeField] private Vector3 raycastOffset;

    [SerializeField] private XRBaseController leftController;
    [SerializeField] private XRBaseController rightController;
    [SerializeField] private float vibrationIntensity = 0.5f;
    [SerializeField] private float vibrationInterval = 0.1f;
    private bool _isTouching;
    private float _vibrationTimer;

    private Renderer _renderer;
    private float _spongeHeight;

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
        _renderer = _sponge.GetComponent<Renderer>();
        _spongeHeight = _sponge.localScale.y * 2;
    }

    void Update()
    {
        rightGrabbing = rightGrab.action.IsPressed();
        leftGrabing = leftGrab.action.IsPressed();
        Erase();
    }
    private void OnEnable()
    {
        rightGrab.action.Enable();
    }
    private void OnDisable()
    {
        leftGrab.action.Disable();
    }
    private void Erase()
    {
        Vector3 raycastOrigin = _sponge.position + raycastOffset;
        Debug.DrawRay(raycastOrigin, transform.up, Color.green);

        if (Physics.Raycast(_sponge.position, transform.up, out _touch, _spongeHeight) && (rightGrabbing || leftGrabing))
        {
            if (_touch.transform.CompareTag("Whiteboard"))
            {
                if (_whiteboard == null)
                {
                    _whiteboard = _touch.transform.GetComponent<Whiteboard>();
                }

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                var x = (int)(_touchPos.x * _whiteboard.textureSize.x - (_dusterSizeX / 2));
                var y = (int)(_touchPos.y * _whiteboard.textureSize.y - (_dusterSizeZ / 2));

                if (y < 0 || y > _whiteboard.textureSize.y || x < 0 || x > _whiteboard.textureSize.x) return;

                if (_touchedLastFrame)
                {
                    Color[] whitePixels = Enumerable.Repeat(Color.white, _dusterSizeX * _dusterSizeZ).ToArray();
                    _whiteboard.texture.SetPixels(x, y, _dusterSizeX, _dusterSizeZ, whitePixels);

                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        _whiteboard.texture.SetPixels(lerpX, lerpY, _dusterSizeX, _dusterSizeZ, whitePixels);
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
