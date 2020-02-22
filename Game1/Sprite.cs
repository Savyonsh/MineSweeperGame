using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    class Sprite
    {
        Texture2D spriteTexture2D;
        Rectangle spriteRectangle;
        Vector2 spriteVector2D;
        bool hasBomb;
        bool discovered;
        int howManyNear;

        public Sprite(Vector2 spriteVector2D, Texture2D spriteTexture2D,
        Rectangle spriteRectangle)
        {
            this.spriteRectangle = spriteRectangle;
            this.spriteTexture2D = spriteTexture2D;
            this.spriteVector2D = spriteVector2D;
            this.hasBomb = false;
            this.discovered = false;
            this.howManyNear = 0;
        }

        public Texture2D GetSetSpriteTexture2D
        {
            get { return spriteTexture2D; }
            set { spriteTexture2D = value; }
        }

        public Rectangle GetSetSpriteRectangle
        {
            get { return spriteRectangle; }
            set { spriteRectangle = value; }
        }

        public Vector2 GetSetSpriteVector2D
        {
            get { return spriteVector2D; }
            set { spriteVector2D = value; }
        }

        public void placeBomb() { hasBomb = true; }

        public bool Bombed() { return hasBomb; }

        public bool isDicovered
        {
            get { return discovered; }
            set { discovered = value; }
        }

        public void incNumberNearMe() { howManyNear++; }

        public void clearFeild() {
            hasBomb = false;
            howManyNear = 0;
            isDicovered = false;
        }

        public int getNumberNearMe() { return howManyNear; }
    }
}
