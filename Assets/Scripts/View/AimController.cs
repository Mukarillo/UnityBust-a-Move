using UnityEngine;
using System;
using System.Collections.Generic;

public class AimController : MonoBehaviour
{
    private const float SPEED = 1f;
    private const float MIN_ANGLE = 70f;
    private const float GUIDE_LINE_VELOCITY = 0.02f;
    private Vector2 mDirection;
    public Vector2 Direction
    {
        get { return mDirection; }
        set
        {
            if (GetAngle(value) < -MIN_ANGLE || GetAngle(value) > MIN_ANGLE)
                return;

            mDirection = value;
            UpdateArrow();
            UpdateGuideLine();
        }
    }

    public float Angle => GetAngle(Direction);
    private Vector3 finalPos => mCenterPoint + Quaternion.AngleAxis(Angle - 90, Vector3.back) * new Vector3(1, 0, 0);

    private Action<Vector2> mOnShoot;
    private Vector3 mCenterPoint;

    private bool mIsDragging;
    private Camera mMainCamera;
    private LineRenderer[] mGuideLines;
    private Material mGuideMaterial;
    private Vector2 mGuideLineOffset = new Vector2(0,0);

    private void Awake()
    {
        mGuideMaterial = Resources.Load<Material>("Material/GuideLine");

        mGuideLines = new LineRenderer[3];
        for(int i = 0; i < 3; i++)
        {
            mGuideLines[i] = new GameObject("GuideLine").AddComponent<LineRenderer>();
            mGuideLines[i].material = mGuideMaterial;
            mGuideLines[i].startWidth = 0.3f;
            mGuideLines[i].endWidth = 0.3f;
            mGuideLines[i].textureMode = LineTextureMode.Tile;
            mGuideLines[i].positionCount = 2;
        }
    }

    public void Initiate(Vector3 centerPoint, Action<Vector2> onShoot)
    {
        mCenterPoint = centerPoint;
        mOnShoot = onShoot;

        mMainCamera = Camera.main;
        Direction = new Vector2(0, 1);
    }

    private void Update()
    {
        //KEYBOARD
        if (Input.GetKey(KeyCode.LeftArrow))
            Direction = new Vector3(Mathf.Sin((Angle - SPEED) * Mathf.Deg2Rad), Mathf.Cos((Angle - SPEED) * Mathf.Deg2Rad), 0);
        if (Input.GetKey(KeyCode.RightArrow))
            Direction = new Vector3(Mathf.Sin((Angle + SPEED) * Mathf.Deg2Rad), Mathf.Cos((Angle + SPEED) * Mathf.Deg2Rad), 0);
        if (Input.GetKeyDown(KeyCode.Space))
            Shoot();

        //MOUSE AND TOUCH
        var inputPosition = Input.mousePosition;

        if(Input.GetMouseButtonDown(0))
            mIsDragging = true;
        if (Input.GetMouseButtonUp(0))
            EndDrag();

        if(Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
                mIsDragging = true;
            else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
                EndDrag();

            inputPosition = Input.GetTouch(0).position;
        }

        if (mIsDragging)
            Direction = (GetWorldPosition(inputPosition) - mCenterPoint).normalized;

        mGuideLineOffset.x -= GUIDE_LINE_VELOCITY;
        mGuideMaterial.SetTextureOffset("_MainTex", mGuideLineOffset);
    }

    private void UpdateGuideLine()
    {
        mCurrentIndex = 0;
        FireGuideLine(new List<Vector3> { mCenterPoint }, mCenterPoint, Direction);
    }

    int mCurrentIndex = 0;
    private void FireGuideLine(List<Vector3> positions, Vector3 pos, Vector2 dir)
    {
        var col = Physics2D.Raycast(pos + (Vector3)dir, dir, 100f, 1 << 9);
        positions.Add(col.point);
        if(positions.Count == 2)
        {
            mGuideLines[mCurrentIndex].SetPositions(positions.ToArray());
            mCurrentIndex++;
            positions.Add(col.point);
        }

        if (col.collider.tag == "Wall" && positions.Count < 4)
        {
            dir.x *= -1;
            FireGuideLine(positions, col.point, dir);
        }
    }

    private void EndDrag()
    {
        mIsDragging = false;

        Shoot();
    }

    private void Shoot()
    {
        mOnShoot.Invoke(Direction);
    }

    private void UpdateArrow()
    {
        transform.rotation = Quaternion.AngleAxis(Angle, Vector3.back);
        transform.position = finalPos;
    }

    private float GetAngle(Vector2 direction) => Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
    private Vector3 GetWorldPosition(Vector2 screenPos)
    {
        var wordPos = mMainCamera.ScreenToWorldPoint(screenPos);
        wordPos.z = 0;
        return wordPos;
    }
}
