using UnityEngine;

public class PieceGlow : SequenceAnimation2D
{
    private float mWaitTimeBetweenPlays;
    private float mCounter;

    private bool mCanPlay = false;

    public void Initiate(float waitTimeBetweenPlays = 2f)
    {
        mSpriteRenderer.sortingOrder = 1;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;

        mWaitTimeBetweenPlays = waitTimeBetweenPlays;

        base.Initiate(AssetController.ME.GlowAnimation, 0, 8);
        mSpriteRenderer.sprite = null;
    }

    protected override void ResetAnimation()
    {
        mCanPlay = false;
        mCounter = 0f;
        base.ResetAnimation();
    }

    protected override void UpdateFrame()
    {
        base.UpdateFrame();
        if (!mCanPlay)
            mSpriteRenderer.sprite = null;
    }

    protected override void Update()
    {
        if (!mCanPlay)
        {
            mCounter += Time.deltaTime;
            if (mCounter >= mWaitTimeBetweenPlays)
            {
                mCanPlay = true;
                mCurrentFrame = 0;
                mCounter = 0;
                UpdateFrame();
            }
        }
        else
            base.Update();
    }
}
