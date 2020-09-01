using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public SprigganSheet sprigganSheet;

    public SprigganAnimation currentAnimation;
    public int currentAnimationFrameIndex;
    public int currentAnimationFrameDelayRemaining;
    public int currentAnimationCyclesFinished;

    public const float defaultScale = 0.5f;

    public int health;
    public int maxHealth;

    public void LoadSprigganSheet(SprigganSheet s)
    {
        this.sprigganSheet = s;
        s.transform.parent = transform;
        Scale(defaultScale);
        if (s.animations.Length == 1)
        {
            StartAnimation(0); // If there's only one animation, default to it
        }
    }

    public void Scale(float value)
    {
        sprigganSheet.transform.localScale = new Vector3(value, value, value);
        sprigganSheet.transform.localPosition = new Vector3(0.0f - sprigganSheet.sprigganWidth / 2 * value, 0.0f - sprigganSheet.sprigganHeight / 2 * value, 0.0f - Spriggan.backOffset / 2.0f * value);

    }

    public void StartAnimation(int index)
    {
        SprigganAnimation anim = sprigganSheet.animations[index];
        currentAnimation = anim;
        currentAnimationFrameIndex = -1;
        AdvanceAnimationFrame();
        currentAnimationCyclesFinished = 0;
    }

    public void AdvanceAnimationFrame()
    {
        currentAnimationFrameIndex++;
        if (currentAnimationFrameIndex >= currentAnimation.frames.Length)
        {
            currentAnimationFrameIndex = currentAnimation.frames.Length - 1; // Revert to final frame
            currentAnimationCyclesFinished++;
        }
        else if (currentAnimation.frames[currentAnimationFrameIndex] == -1)
        {
            currentAnimationFrameIndex = 0; // Reset animation
            currentAnimationCyclesFinished++;
        }
        int currentAnimationFrame = currentAnimation.frames[currentAnimationFrameIndex];
        sprigganSheet.Show(currentAnimationFrame);
        currentAnimationFrameDelayRemaining = currentAnimation.delays[currentAnimationFrameIndex];
    }

    public void Update()
    {
        if (currentAnimation == null) return;
        currentAnimationFrameDelayRemaining -= (int)(Time.deltaTime * 1000);
        if (currentAnimationFrameDelayRemaining <= 0)
        {
            AdvanceAnimationFrame();
        }
    }
}
