using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotableSprite : MonoBehaviour
{
    Color32[] savedColors;
	Texture2D savedTexture;
    void Start()
    {
		savedTexture = GetComponent<SpriteRenderer>().sprite.texture;
        savedColors = GetComponent<SpriteRenderer>().sprite.texture.GetPixels32();
    }
    void Update()
    {
        if (Input.GetMouseButton(1) && Input.GetKeyDown(KeyCode.LeftControl))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            //If something was hit, the RaycastHit2D.collider will not be null.
            if (hit.collider != null)
            {
                Vector2Int blastPosition = PositionOnTheSprite(PercentageClickPosition(hit.point));
                Debug.Log("BlastPosition(pixel):" + blastPosition);
                float blastRadius = WorldLengthToPixelLength(GameCursor.instance.gizmoSize);
                Debug.Log("BlastRadius(pixel):" + blastRadius);
                BoomOnSprite(blastPosition, blastRadius);
            }
        }
    }

    Vector2 PercentageClickPosition(Vector2 hitpoint)
    {
        Vector2 min = GetComponent<SpriteRenderer>().bounds.min;
        Vector2 max = GetComponent<SpriteRenderer>().bounds.max;
        //Mathf.Lerp (output.min, output.max, Mathf.InverseLerp (input.min, input.max, input));
        float x = Mathf.Lerp(0, 100, Mathf.InverseLerp(min.x, max.x, hitpoint.x));
        float y = Mathf.Lerp(0, 100, Mathf.InverseLerp(min.y, max.y, hitpoint.y));
        return new Vector2(x, y);
    }

    Vector2Int PositionOnTheSprite(Vector2 percentage)
    {
        Texture texture = GetComponent<SpriteRenderer>().sprite.texture;
        Vector2 spriteSize = new Vector2(texture.width, texture.height);
        int x = Mathf.RoundToInt(Mathf.Lerp(0, texture.width, Mathf.InverseLerp(0, 100, percentage.x)));
        int y = Mathf.RoundToInt(Mathf.Lerp(0, texture.height, Mathf.InverseLerp(0, 100, percentage.y)));
        return new Vector2Int(x, y);
    }

    float WorldLengthToPixelLength(float worldLength)
    {
        Vector2 boundSize = GetComponent<SpriteRenderer>().bounds.size;
        Texture texture = GetComponent<SpriteRenderer>().sprite.texture;
        Vector2 spriteSize = new Vector2(texture.width, texture.height);
        // spriteSize (Pixel) => boundSize (WorldPos)
        // 1Pixel => boundSize / spriteSize (WorldPosBoundLength)
        Vector2 onePixelLengthInWorld = boundSize / spriteSize;
        // worldLength => (1Pixel x X)
        // X => worldLength / 1Pixel
        return worldLength / onePixelLengthInWorld.x;
    }

    public void BoomOnSprite(Vector2Int blastPosition, float blastRadius)
    {
        Texture2D texture = GetComponent<SpriteRenderer>().sprite.texture;

        List<Vector2> vectors = new List<Vector2>();

        Color32[] colors = texture.GetPixels32();

        int startX = Mathf.Clamp(blastPosition.x - (int)blastRadius, 0, texture.width);
        int endX = Mathf.Clamp(blastPosition.x + (int)blastRadius, 0, texture.width);
        int startY = Mathf.Clamp(blastPosition.y - (int)blastRadius, 0, texture.height);
        int endY = Mathf.Clamp(blastPosition.y + (int)blastRadius, 0, texture.height);
        // Loop pixels
        for (int X = startX; X <= endX; X++)
        {
            for (int Y = startY; Y < endY; Y++)
            {
                //pixel in blast circle => discard
                Vector2Int checkPos = new Vector2Int(X, Y);
                if (Vector2Int.Distance(checkPos, blastPosition) <= blastRadius)
                {
                    //discard pixel
                    texture.SetPixel(X, Y, Color.clear);
                }
            }
        }
        //texture.Apply();
        Destroy(GetComponent<PolygonCollider2D>());
		Texture2D newTexture = new Texture2D(texture.width, texture.height, texture.format, false);
		Color[] newColors = texture.GetPixels(0, 0, texture.width, texture.height);
		newTexture.SetPixels(newColors);
		newTexture.Apply();
		Sprite sprite = GetComponent<SpriteRenderer>().sprite;
		GetComponent<SpriteRenderer>().sprite = Sprite.Create(newTexture, new Rect(0,0,newTexture.width, newTexture.height), new Vector2(0.5f,0.5f), sprite.pixelsPerUnit);
        gameObject.AddComponent<PolygonCollider2D>();
    }

    void OnDisable()
    {
        Texture2D texture = GetComponent<SpriteRenderer>().sprite.texture;
        for (int X = 0; X < texture.width; X++)
        {
            for (int Y = 0; Y < texture.height; Y++)
            {
                texture.SetPixel(X, Y, savedColors[X + Y * texture.width]);
				savedTexture.SetPixel(X, Y, savedColors[X + Y * texture.width]);
            }
        }
        texture.Apply();
		savedTexture.Apply();
    }

}
