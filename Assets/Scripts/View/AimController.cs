using UnityEngine;
using System;

public class AimController : MonoBehaviour
{
    private const float SPEED = 1f;
    private const float MIN_ANGLE = 55f;
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
        }
    }

    public float Angle => GetAngle(Direction);

    private Action<Vector2> mOnShoot;
    private Vector3 mCenterPoint;

    private bool mIsDragging;
    private Camera mMainCamera;

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
        transform.position = mCenterPoint + Quaternion.AngleAxis(Angle - 90, Vector3.back) * new Vector3(2, 0, 0);
    }

    private float GetAngle(Vector2 direction) => Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
    private Vector3 GetWorldPosition(Vector2 screenPos)
    {
        var wordPos = mMainCamera.ScreenToWorldPoint(screenPos);
        wordPos.z = 0;
        return wordPos;
    }
}
