using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspectable : Interactable {
    public bool isPoster;

    public bool destroyOnInspect;
    public int id;
    public bool[] pieces;

    [HideInInspector]
    public Sprite sprite;

    new void Start() {
        base.Start();
        sprite = UpdateSprite(GameManager.manager.pagePieceImages);
    }

    public void PosterCheck() {
        bool isInspectable = false;
        foreach (bool piece in pieces) {
            if (piece) {
                isInspectable = true;
                break;
            }
        }

        if (!isInspectable) {
            gameObject.layer = LayerMask.NameToLayer("Uninteractable");
        }
    }

    public override bool CanInteract() {
        if (!player.freeFall && !GameManager.manager.isInspecting) { return true; }
        return false;
    }

    public override void Interact() {
        GameManager.manager.Inspect(sprite, this);
        if (player.transform.position.x > transform.position.x) { player.spriteRenderer.flipX = true; }
        else if (player.transform.position.x < transform.position.x) { player.spriteRenderer.flipX = false; }
        player.interactor.RemoveInteractable();
    }

    public Sprite UpdateSprite(Sprite[] sprites) {
        int l = sprites[0].texture.width;
        Sprite merged = Sprite.Create(new Texture2D(l, l), new Rect(0.0f, 0.0f, l, l), new Vector2(0.5f, 0.5f), 32);
        merged.texture.filterMode = FilterMode.Point;

        for (int y = 0; y < 224; y++) {
            for (int x = 0; x < 224; x++) {
                merged.texture.SetPixel(x, y, Color.clear);
            }
        }

        for (int i = 0; i < 5; i++) {
            if (pieces[i]) {
                Sprite spriteToAdd = sprites[i];
                for (int y = 0; y < 224; y++) {
                    for (int x = 0; x < 224; x++) {
                        if (merged.texture.GetPixel(x, y).a < 1) {
                            merged.texture.SetPixel(x, y, spriteToAdd.texture.GetPixel(x, y));
                        }
                    }
                }
            }
        }

        merged.texture.Apply(true);
        return merged;
    }

    public void UpdatePoster() {
        pieces = GameManager.manager.pagePieces;
        GetComponent<SpriteRenderer>().sprite = UpdateSprite(GameManager.manager.posterPieceImages);
        sprite = UpdateSprite(GameManager.manager.pagePieceImages);
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
