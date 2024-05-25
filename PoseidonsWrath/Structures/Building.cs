

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PoseidonsWrath{


    public class Building : Structure
    {
        [Flags]
        enum Sides{
            None = 0,
            East = 1,
            West = 2,
            North = 4,
            South = 8
        }
        Sides sides = 0;
        private int _tileIndexX, _tileIndexY;
        public Building(string name, Texture2D texture, Rectangle sprite, Vector2 location, int height, int durability, List<Resource> requirements) : base(name, texture, sprite, location, height, durability, requirements)
        {
            _tileIndexX = (int)(location.X/32);
            _tileIndexY = (int)(location.Y/32);
        }

        public override void Destroy(Vector2 location)
        {
            throw new System.NotImplementedException();
        }

        public override void Place(Vector2 location)
        {
            throw new System.NotImplementedException();
        }

        public void Update(List<Structure> structures){
            sides = 0;
            foreach(Structure s in structures){
                if(s.name == name && name =="LowWall"){
                    if((int)s.location.X/32 == _tileIndexX-1 && (int)s.location.Y/32 == _tileIndexY)//West
                        sides = sides | Sides.West;
                    else if((int)s.location.X/32 == _tileIndexX+1 && (int)s.location.Y/32 == _tileIndexY)//East
                        sides = sides | Sides.East;
                    else if((int)s.location.X/32 == _tileIndexX && (int)s.location.Y/32 == _tileIndexY - 1)//North
                        sides = sides | Sides.North;
                    else if((int)s.location.X/32 == _tileIndexX && (int)s.location.Y/32 == _tileIndexY + 1)//South
                        sides = sides | Sides.South;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            switch(sides){
                case Sides.East:
                    sprite = new Rectangle(64, 32, 32, 32);
                    break;
                case Sides.West:
                    sprite = new Rectangle(128, 32, 32, 32);
                    break;
                case Sides.North:
                    sprite = new Rectangle(32, 96, 32, 32);
                    break;
                case Sides.South:
                    sprite = new Rectangle(32, 32, 32, 32);
                    break;
                case Sides.East | Sides.West:
                    sprite = new Rectangle(96, 32, 32, 32);
                    break;
                case Sides.North | Sides.South:
                    sprite = new Rectangle(0, 32, 32, 32);
                    break;
                case Sides.North | Sides.East:
                    sprite = new Rectangle(0, 64, 32, 32);
                    break;
                case Sides.North | Sides.West:
                    sprite = new Rectangle(64, 64, 32, 32);
                    break;
                case Sides.South | Sides.East:
                    sprite = new Rectangle(0, 96, 32, 32);
                    break;
                case Sides.South | Sides.West:
                    sprite = new Rectangle(64, 96, 32, 32);
                    break;
                case Sides.North | Sides.South | Sides.East | Sides.West:
                    sprite = new Rectangle(32, 64, 32, 32);
                    break;
            }

            spriteBatch.Draw(texture, location, sprite,Color.White);
            if(isSelected){
                Texture2D _pointTexture;

                _pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pointTexture.SetData<Color>(new Color[]{Color.White});

                spriteBatch.Draw(_pointTexture, new Rectangle((int)location.X+1, (int)location.Y+1, 1, 31), Color.White);
                spriteBatch.Draw(_pointTexture, new Rectangle((int)location.X+1, (int)location.Y+1, 31, 1), Color.White);
                spriteBatch.Draw(_pointTexture, new Rectangle((int)location.X + 31, (int)location.Y-1, 1, 31), Color.White);
                spriteBatch.Draw(_pointTexture, new Rectangle((int)location.X-1, (int)location.Y + 31, 31, 1), Color.White);
            }
        }
    }
}