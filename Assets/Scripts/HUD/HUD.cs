using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject health1DisplayPoint;
    [SerializeField] private GameObject health2DisplayPoint;
    [SerializeField] private GameObject heartSprite;
    [SerializeField] private GameObject startTitle;

    [Header("Settings")]
    [SerializeField] private float spacing = 16;

    private List<GameObject> health1Display = new List<GameObject>();
    private List<GameObject > health2Display = new List<GameObject>();

    public Color box1Color {  get; set; }
    public Color box2Color { get; set; }

    public GameObject box1 { get; set; }
    public GameObject box2 { get; set; }

    private void Start() {
        health1Display.Clear();
        health2Display.Clear();
    }

    public void showStartTitle(bool show) {
        startTitle.SetActive(show);
    }

    public void updateBox2Health() {
        BoxComponent comp = box2.GetComponent<BoxComponent>();
        if (comp == null) return;

        int maxHealth = comp.maxHealth;
        int health = comp.health;

        if (health < health2Display.Count) {
            // remove extras
            for (int start = health2Display.Count - 1; start >= health; start--) {
                GameObject obj = health2Display[start];
                health2Display.RemoveAt(start); // Use RemoveAt for better performance
                Destroy(obj);
            }

            return;
        }

        if (health2Display.Count != 0) return;

        for (int i = 0; i < maxHealth; i++) {
            Vector3 position = new Vector3(i * spacing, 0, 0); // Use local space offset
            GameObject heart = Instantiate(heartSprite, health2DisplayPoint.transform);

            Image image = heart.GetComponent<Image>();
            image.color = new Color(box2Color.r, box2Color.g, box2Color.b, 1.0f);

            heart.transform.localPosition = position; // Set local position
            health2Display.Add(heart);
        }
    }

    public void updateBox1Health() {
        BoxComponent comp = box1.GetComponent<BoxComponent>();
        if (comp == null) return;

        int maxHealth = comp.maxHealth;
        int health = comp.health;

        if (health < health1Display.Count) {
            // remove extras
            for (int start = health1Display.Count - 1; start >= health; start--) {
                GameObject obj = health1Display[start];
                health1Display.RemoveAt(start); // Use RemoveAt for better performance
                Destroy(obj);
            }

            return;
        }

        if (health1Display.Count != 0) return;

        for (int i = 0; i < maxHealth; i++) {
            Vector3 position = new Vector3(i * spacing, 0, 0); // Use local space offset
            GameObject heart = Instantiate(heartSprite, health1DisplayPoint.transform);

            Image image = heart.GetComponent<Image>();
            image.color = new Color(box1Color.r, box1Color.g, box1Color.b, 1.0f);

            heart.transform.localPosition = position; // Set local position
            health1Display.Add(heart);
        }
    }
}
