using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace PoseidonsWrath{

    public class Tile{
        
        public String type { get; set; }
        public Texture2D texture {get; set;}
        public Rectangle sprite {get; set;}
        public Rectangle hitBox {get; set;}
        public int height {get; set;}
        public Vector2 location {get; set;}
        public Boolean isBuildable{get; set;}
        public Boolean isHovered{get; set;}


        public Tile(String type, Texture2D texture, Rectangle map, Vector2 location, Boolean buildable, Boolean isHovered) {
            this.type = type;
            this.texture = texture;
            this.sprite = map;
            this.location = location;
            this.isBuildable = buildable;
            this.isHovered = isHovered;
            this.height = 1;

            hitBox = new Rectangle((int)location.X, (int)location.Y, 32, 32);

        }


        public void Draw(SpriteBatch spriteBatch)
        {
                spriteBatch.Draw(texture, location, sprite, Color.White);

                //draw border
                Color borderColor = Color.Gray;
                //DrawBorder(spriteBatch, hitBox, borderColor, 1);

        }

        public void DrawBorder(SpriteBatch spriteBatch, Rectangle bounds, Color color, int borderSize){//super laggy 60fps to 14 fps
            Texture2D _pointTexture;

            _pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _pointTexture.SetData<Color>(new Color[]{Color.White});

            spriteBatch.Draw(_pointTexture, new Rectangle(bounds.X, bounds.Y, borderSize, borderSize + bounds.Height), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(bounds.X, bounds.Y, borderSize + bounds.Width, borderSize), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(bounds.X + bounds.Width, bounds.Y, borderSize, borderSize + bounds.Height), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(bounds.X, bounds.Y + bounds.Height, borderSize + bounds.Width, borderSize), color);
        }
    }
}