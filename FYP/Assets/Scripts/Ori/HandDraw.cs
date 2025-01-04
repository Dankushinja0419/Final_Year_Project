using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandDraw : MonoBehaviour
{
    [Header("Refrence")]
    [SerializeField] private Whiteboard whiteboard;
    [SerializeField] private HiraganaChecker hiraganaChecker;
    [SerializeField] private GameData difficultyData;

    [Header("Draw")]
    [SerializeField] private Transform _hand;
    [SerializeField] private int _drawSizeX = 40;
    [SerializeField] private int _drawSizeZ = 15;
    [SerializeField] private Vector3 raycastOffset;
    [SerializeField] private Color markerColor = Color.black;   

    [Header("Particle")]
    [SerializeField] private ParticleSystem[] drawingParticles;
    [SerializeField] private int particleIdx;

    [Header("Controller")]
    [SerializeField] private XRBaseController controller;
    [SerializeField] private float vibrationIntensity = 0.5f;
    [SerializeField] private float vibrationInterval = 0.1f;
    [SerializeField] public float _rayLength = 10;
    [SerializeField] public InputActionProperty trigger; // InputAction for select
    [SerializeField] public InputActionProperty grab; // InputAction for draw

    private Renderer _renderer;
    private Whiteboard _whiteboard;
    private RaycastHit _touch;
    private Vector2 _touchPos, _lastTouchPos;
    private Quaternion _lastTouchRot;
    private bool _isTouching;
    private bool _touchedLastFrame;
    private bool wasDrawingLastFrame = false;
    private bool _isFirstTouch = true;
    private float _vibrationTimer;

    private bool isDrawing;
    private bool checkDrawing;

    private void Awake()
    {
        markerColor = difficultyData.drawColor;
        particleIdx = difficultyData.particleIdx;

        foreach (ParticleSystem ps in drawingParticles)
        {
            if (ps.isPlaying)
            {
                ps.Stop();
            }
        }
    }

    void Start()
    {
        _renderer = _hand.GetComponent<Renderer>();
    }

    void Update()
    {
        isDrawing = grab.action.IsPressed();
        if (isDrawing)
        {
            Draw(); 
            checkDrawing = true;
        }
        else
        {
            // Reset first touch when not drawing
            _isFirstTouch = true;
        }

        // Check if drawing just ended this frame
        if (checkDrawing && !isDrawing)
        {
            hiraganaChecker?.CheckDrawing();
            checkDrawing = false;
            drawingParticles[particleIdx].Stop();
        }

        if (trigger.action.triggered)
        {
            Clearboard();
        }
    }

    private void OnEnable()
    {
        trigger.action.Enable();
        grab.action.Enable();
    }

    private void OnDisable()
    {
        trigger.action.Disable();
        grab.action.Disable();
    }

    private void Draw()
    {
        if (!isDrawing)
        {
            return;
        }

        Vector3 raycastOrigin = _hand.position + raycastOffset;

        if (Physics.Raycast(raycastOrigin, transform.forward, out _touch, _rayLength))
        {
            if (_touch.transform.CompareTag("Whiteboard"))
            {
                if (_whiteboard == null)
                {
                    _whiteboard = _touch.transform.GetComponent<Whiteboard>();
                }

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                // Calculate pixel position
                var x = (int)(_touchPos.x * _whiteboard.textureSize.x - (_drawSizeX / 2));
                var y = (int)(_touchPos.y * _whiteboard.textureSize.y - (_drawSizeZ / 2));

                // Prevent writing outside bounds
                if (y < 0 || y > _whiteboard.textureSize.y || x < 0 || x > _whiteboard.textureSize.x) return;
                
                if (_isFirstTouch)
                {
                    _lastTouchPos = new Vector2(x, y);
                    _isFirstTouch = false;
                }

                if (_touchedLastFrame)
                {
                    DrawLineBresenham((int)_lastTouchPos.x, (int)_lastTouchPos.y, x, y);
                    _whiteboard.texture.Apply();
                }

                _lastTouchPos = new Vector2(x, y);
                _touchedLastFrame = true;

                // Start particle effect at the drawing position
                if (!drawingParticles[particleIdx].isPlaying && particleIdx > 0)
                {
                    drawingParticles[particleIdx].Play();
                }

                drawingParticles[particleIdx].transform.position = _touch.point; // Place particles at the touch position


                if (_isTouching)
                {
                    if (isDrawing)
                    {
                        HandVibration();
                    }
                }
                _isTouching = true;

                return;
            }
        }

        _whiteboard = null;
        _touchedLastFrame = false;
        _isTouching = false;
    }

    private void DrawLineBresenham(int x0, int y0, int x1, int y1)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        Color[] colors = Enumerable.Repeat(markerColor, _drawSizeX * _drawSizeZ).ToArray();

        while (true)
        {
            // Draw at the calculated position
            _whiteboard.texture.SetPixels(x0, y0, _drawSizeX, _drawSizeZ, colors);

            // Stop if the end point is reached
            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    private void Clearboard()
    {
        Vector3 raycastOrigin = _hand.position + raycastOffset;

        if (Physics.Raycast(raycastOrigin, transform.forward, out _touch, _rayLength))
        {
            if (_touch.transform.CompareTag("Duster"))
            {
                whiteboard.ClearBoard();
            }
        }
    }

    private void HandVibration()
    {
        // Only vibrate at intervals
        if (_vibrationTimer <= 0 && _isTouching)
        {
            controller?.SendHapticImpulse(0.5f, vibrationInterval);
            _vibrationTimer = vibrationInterval; // Reset timer
        }

        // Decrease timer   
        _vibrationTimer -= Time.deltaTime;
    }
}
