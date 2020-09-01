using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprigganAnimation : MonoBehaviour
{
    public int index;
    public int[] frames; // Put a -1 at the end to make the animation repeat, otherwise it holds on the last frame
    public int[] delays; // How long each frame is held, in ms
}
