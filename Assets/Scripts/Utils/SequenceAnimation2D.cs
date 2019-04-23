using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SequenceAnimation2D : MonoBehaviour
{
    protected SpriteRenderer mSpriteRenderer;
    private Sprite[] mSprites;
    private int mFramesToChange;
    protected int mFrameCounter;

    protected int mCurrentFrame;

    void Awake()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initiate(Sprite[] sprites, int startFrom = 0, int framesToChange = 12)
    {
        mSprites = sprites;
        mFramesToChange = framesToChange;
        mCurrentFrame = startFrom;

        mSpriteRenderer.sprite = mSprites[mCurrentFrame];
    }

    protected virtual void ResetAnimation()
    {
        mCurrentFrame = 0;
    }

    protected virtual void Update()
    {
        if(++mFrameCounter % mFramesToChange == 0 && mFrameCounter != 0)
        {
            mFrameCounter = 0;
            UpdateFrame();
        }
    }

    protected virtual void UpdateFrame()
    {
        if (mCurrentFrame >= mSprites.Length)
            ResetAnimation();
        mSpriteRenderer.sprite = mSprites[mCurrentFrame++];
    }
}