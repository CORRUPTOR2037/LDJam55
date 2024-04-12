using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Entity : MonoBehaviour {

    protected SpriteRenderer _spr;

    public static Tilemap tilemap;
    protected new SpriteRenderer renderer {
        get {
            if (_spr == null) _spr = GetComponent<SpriteRenderer>();
            return _spr;
        }
    }


    public int x { get; protected set; }
    public int y { get; protected set; }
    public float height { get; protected set; }

    public delegate void OnTrigger (Collider2D collider);

    public OnTrigger onTrigger;

    public virtual void SetPosition (int x, int y, float height) {
        this.x = x;
        this.y = y;
        this.height = height;
        transform.position = tilemap.layoutGrid.GetCellCenterLocal(new Vector3Int(x, y, 0)) + HeightVector(height);
        if (renderer != null)
            renderer.sortingOrder = 100 - y;
    }

    protected Vector3 HeightVector (float value) {
        return new Vector3(0, value, 0);
    }

    public void SetSprite (Sprite sprite) {
        renderer.sprite = sprite;
    }

    public void SetColor (Color color) {
        renderer.color = color;
    }

    public virtual void OnTriggerEnter2D (Collider2D collider) {
        if (onTrigger != null)
            onTrigger.Invoke(collider);
    }
}
