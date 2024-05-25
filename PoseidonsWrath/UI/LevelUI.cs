

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PoseidonsWrath{


    public class LevelUI{

        public List<Resource> resources { get; set; }
        public List<Structure> structures { get; set; }

        public List<Button> buttons { get; set; }

        public int x { get; set; }
        public int y { get; set; }

        public LevelUI(List<Resource> resources, List<Structure> structures, int x, int y){
            this.resources = resources;
            this.structures = structures;
            this.x = x;
            this.y = y;
            buttons = new List<Button>(){};
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont defaultFont, Structure selected){
            int index = 0;
            foreach (Resource r in resources){
                spriteBatch.Draw(r.texture, new Vector2(x + 48 * index, y), r.sprite, Color.White);
                spriteBatch.DrawString(defaultFont, r.quantity.ToString(), new Vector2(x + 48 * index +8, y + 40), Color.Black);
                index++;
            }
            index = 0;
            foreach (Structure s in structures){
                s.Draw(spriteBatch);
                index++;
            }
            foreach (Button b in buttons){
                b.Draw(spriteBatch);
            }
            if(selected != null){
                spriteBatch.DrawString(defaultFont, "Building: " + selected.name, new Vector2(x + 800, y + 16), Color.Black);
                spriteBatch.DrawString(defaultFont, "Health: " + selected.remainingDurability + "/" + selected.durability.ToString(), new Vector2(x + 800, y + 42), Color.Black);
                spriteBatch.DrawString(defaultFont, "Height: " + selected.height.ToString(), new Vector2(x + 800, y + 74), Color.Black);
            }
        }

    }
}