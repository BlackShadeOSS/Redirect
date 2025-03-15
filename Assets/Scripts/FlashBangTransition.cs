using System.Collections;
using UnityEngine;

public class FlashBangTransition : MonoBehaviour
{
    public float fadeDuration = 2f;
    public float delayAppear = 1.7f;
    public float appearDuration = 2f;
    
    public SpriteRenderer fadeInSprite;
    public SpriteRenderer appearSprite;

    private void Start()
    {
        if (fadeInSprite != null)
        {
            StartCoroutine(Flashbang());
        }
    }

    private IEnumerator Flashbang()
    {
        // zanikaj pierwszego
        float elapsedTime = 0f;
        Color initialColor = fadeInSprite.color;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadeInSprite.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // zniknij go na 0%
        fadeInSprite.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

        // poof 
        fadeInSprite.gameObject.SetActive(false);

        // czkekaj dilej
        yield return new WaitForSeconds(delayAppear);

        // pojaw drugiego
        elapsedTime = 0f;
        Color appearSpriteInitialColor = appearSprite.color;

        // Make appearSprite fade out instead of disappearing immediately
        while (elapsedTime < appearDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / appearDuration);
            appearSprite.color = new Color(appearSpriteInitialColor.r, appearSpriteInitialColor.g, appearSpriteInitialColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Fully fade out the appearSprite after the fade duration
        appearSprite.color = new Color(appearSpriteInitialColor.r, appearSpriteInitialColor.g, appearSpriteInitialColor.b, 0f);
    
        // Optionally disable the sprite after fading out (if needed)
        appearSprite.gameObject.SetActive(false);
    }

}
