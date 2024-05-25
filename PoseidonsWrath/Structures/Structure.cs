using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PoseidonsWrath{

    public abstract class Structure{
        public string name { get; set; }
        public Texture2D texture { get; set; }
        public Rectangle sprite { get; set; }
        public Vector2 location { get; set; }
        public int height { get; set; }
        public int durability { get; set; }
        public int remainingDurability { get; set; }
        public List<Resource> requirements { get; set; }
        public Boolean isSelected { get; set; }
        
        public Structure(string name, Texture2D texture, Rectangle sprite, Vector2 location, int height, int durability, List<Resource> requirements){
            this.name = name;
            this.texture = texture;
            this.sprite = sprite;  
            this.location = location; 
            this.height = height;
            this.durability = durability;
            remainingDurability = durability;
            this.requirements = requirements;
            isSelected = false;
        }

        public abstract void Place(Vector2 location);

         public abstract void Destroy(Vector2 location);

         public abstract void Draw(SpriteBatch spriteBatch);

    }
}