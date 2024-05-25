


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PoseidonsWrath{

    public class Resource{
        public string name { get; set;}
        public int quantity { get; set;}
        public Texture2D texture { get; set;}
        public Rectangle sprite { get; set;}

        public Resource(string name, Texture2D texture, Rectangle sprite, int quantity) {
            this.name = name;
            this.quantity = quantity;
            this.texture = texture;
            this.sprite = sprite;
        }
    }
}