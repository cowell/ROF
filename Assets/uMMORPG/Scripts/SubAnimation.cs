// Applies another renderer's animation to another sprite sheet.
// => The sprite names have to be exactly the same!
using System;
using System.Collections.Generic;
using UnityEngine;

public class SubAnimation : MonoBehaviour
{
    public SpriteRenderer sourceAnimation;
    public new SpriteRenderer renderer;
    public List<Sprite> spritesToAnimate;

    void LateUpdate()
    {
        renderer.sprite = spritesToAnimate != null
                          ? spritesToAnimate.Find(s => s.name == sourceAnimation.sprite.name)
                          : null;
    }
}
