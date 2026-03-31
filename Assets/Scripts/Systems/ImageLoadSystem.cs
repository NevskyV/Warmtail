using System.IO;
using UnityEngine;

namespace Systems
{
    public class ImageLoadSystem
    {
        public static Sprite LoadNewSprite(string filePath, float pixelsPerUnit = 100.0f)
        {
            Texture2D spriteTexture = LoadTexture(filePath);
            if (spriteTexture != null)
            {
                Sprite newSprite = Sprite.Create(
                    spriteTexture,
                    new Rect(0, 0, spriteTexture.width, spriteTexture.height),
                    new Vector2(0.5f, 0.5f),
                    pixelsPerUnit
                );
                return newSprite;
            }

            return null;
        }

        private static Texture2D LoadTexture(string filePath)
        {
            Texture2D tex2D = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex2D = new Texture2D(2, 2);
                if (tex2D.LoadImage(fileData))
                    return tex2D;
            }

            return null;
        }
    }
}