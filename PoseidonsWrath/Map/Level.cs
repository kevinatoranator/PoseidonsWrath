using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


namespace PoseidonsWrath{

    public class Level{

        public String name {get; set;}
        public List<Tile> tiles{get; set;}
        public List<Structure> structures{get; set;}


        public Level(String name, List<Tile> tiles, List<Structure> structures){
            this.name = name;
            this.tiles = tiles;
            this.structures = structures;
        }

        public void Draw(SpriteBatch spriteBatch){
            foreach (Tile tile in tiles){
                tile.Draw(spriteBatch);
            }
            foreach (Structure s in structures){
                s.Draw(spriteBatch);
            }   
        }
    }
}