using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class CountSpawner : MonoBehaviour
{
    public TextMeshProUGUI particlePrefab;
    public float launchForce = 5f;
    public float fadeDuration = 2f;
    public Vector2 launchDirectionRange = new Vector2(-1, 1);

    private List<TextMeshProUGUI> particlePool = new List<TextMeshProUGUI>();

    public void SpawnParticle(int count)
    {
        TextMeshProUGUI particle = GetPooledParticle();
        if (particle == null)
        {
            particle = Instantiate(particlePrefab, particlePrefab.transform.parent);
            particlePool.Add(particle);
        }

        particle.transform.position = transform.position;
        particle.gameObject.SetActive(true);
        particle.alpha = 1;
        particle.text = "+" + count;

        Vector2 launchDirection = Random.Range(launchDirectionRange.x, launchDirectionRange.y) * Vector2.right;
        Rigidbody2D rb = particle.GetComponent<Rigidbody2D>();
        rb.velocity = launchDirection.normalized * launchForce;

        particle.StartCoroutine(FadeAway(particle));
    }

    private IEnumerator FadeAway(TextMeshProUGUI particle)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / fadeDuration;
            particle.alpha = 1 - t;
            yield return null;
        }

        particle.gameObject.SetActive(false);
    }

    private TextMeshProUGUI GetPooledParticle()
    {
        foreach (TextMeshProUGUI particle in particlePool)
        {
            if (!particle.gameObject.activeInHierarchy)
            {
                return particle;
            }
        }
        return null;
    }
}