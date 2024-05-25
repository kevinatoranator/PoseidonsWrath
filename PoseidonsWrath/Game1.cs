using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PoseidonsWrath;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public SpriteFont defaultFont;

    int displayWidth = 1280;
    int displayHeight = 720;
    //1280x720 40x22.5 32x32

    Texture2D tileMap, resourceMap, UIMap;
    public SoundEffect waveSFX, digSFX;
    Song bgmusic;

    string baseMap;
    Level baseLevel;
    Boolean wasReleased, corePlaced, coreInvalid;
    LevelUI level1UI;
    int currentWave, mineCount, waveInterval, waveShift;
    double waveTimer, blinkTick;

    Structure selected;
    List<Resource> inventory;
    Resource sand, wetSand, shell, rock;
    Building core, lowWall, highWall, tower, mine, moat;
    Button nextWave;
    List<Tile> waveTiles;
    List<double> turnWaveDistance, currentWaveDistance;
    Vector2 mousePosition;
    const int waveTicks = 10;
    const int hTiles = 40;
    const int vTiles = 16;
    const double tickTime = .25;
    Structure selectedStructure;

    Random rand;

    public enum GameState
    {
        MainMenu,
        LevelSelect,
        ConstructionPhase,
        WavePhase,
        GameOver
    }
    GameState previousState;
    GameState currentState = GameState.ConstructionPhase;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = displayWidth;
        _graphics.PreferredBackBufferHeight = displayHeight;
    }

    protected override void Initialize()
    {

        //1280x720 80x45
        baseMap =
        "0000000000000000000000000000000000000000\n" +
        "0000000000000000000000000000000000000000\n" +
        "0000000000000000000000000000000000000000\n" +
        "0000000000000000000000000000000000000000\n" +
        "1111111111111111111111111111111111111111\n" +
        "1111111111111111111111111111111111111111\n" +
        "1111111111111111111111111111111111111111\n" +
        "1111111111111111111111111111111111111111\n" +
        "1111111111111111111111111111111111111111\n" +
        "1111111111111111111111111111111111111111\n" +
        "1111111111111111111111111111111111111111\n" +
        "1111111111111111111111111111111111111111\n" +
        "1111111111111111111111111111111111111111\n" +
        "1111111111111111111111111111111111111111\n" +
        "1111111111111111111111111111111111111111\n" +
        "1111111111111111111111111111111111111111\n";

        //level1UI = new LevelUI(new List<Resource>(){new Resource("Sand", 5, resourceMap, new Rectangle(0, 0, 32, 32))}, new List<Structure>(){}, 0, 1200);
        selected = core;
        currentWave = 1;
        mineCount = 0;
        waveTimer = 0;
        waveInterval = 0;
        wasReleased = true;
        corePlaced = false;
        waveTiles = new List<Tile>() { };
        turnWaveDistance = new List<double>();
        currentWaveDistance = new List<double>();
        rand = new Random();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        tileMap = Content.Load<Texture2D>("terrain_map");
        resourceMap = Content.Load<Texture2D>("resource_map");
        UIMap = Content.Load<Texture2D>("UI_map");
        defaultFont = Content.Load<SpriteFont>("defaultFont");
        waveSFX = Content.Load<SoundEffect>("oceanWaveSFX");
        digSFX = Content.Load<SoundEffect>("sandDigSFX");
        bgmusic = Content.Load<Song>("sandcastleBGM");
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = 0.15f;
        MediaPlayer.Play(bgmusic);
    }

    protected override void Update(GameTime gameTime)
    {
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if(currentState == GameState.ConstructionPhase){
            if(previousState == GameState.WavePhase){
                sand.quantity += 5*mineCount;
                currentWave++;
                previousState = currentState;
            }
            ConstructionUpdate(gameTime);
        }  
        else if (currentState == GameState.WavePhase)
            updateWave(gameTime);
        //DO WAVE STUFF
        foreach (Button b in level1UI.buttons)
        {
            b.Update(this, gameTime);
        }
        foreach(Building b in baseLevel.structures){
            b.Update(baseLevel.structures);
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        baseLevel.Draw(_spriteBatch);
        level1UI.Draw(_spriteBatch, defaultFont, selectedStructure);
        // _spriteBatch.DrawString(defaultFont, "X", new Vector2(selected.location.X, selected.location.Y+500), Color.Black
        foreach(Tile t in waveTiles)
            t.Draw(_spriteBatch);
        TileHighlight(_spriteBatch, new Rectangle((int)Math.Floor(mousePosition.X / 32) * 32, (int)Math.Floor(mousePosition.Y / 32) * 32, 32, 32), Color.White, 1);
        if(selectedStructure != null)
            TileHighlight(_spriteBatch, new Rectangle((int)selectedStructure.location.X, (int)selectedStructure.location.Y, 32, 32), Color.White, 1);
        
        if(coreInvalid){
            blinkTick += gameTime.ElapsedGameTime.TotalSeconds;
            Texture2D _pointTexture = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
            _pointTexture.SetData<Color>(new Color[]{Color.White});
            _spriteBatch.Draw(_pointTexture, new Rectangle(0, 0, hTiles/4 * 32, vTiles*32), Color.Red * 0.5f);//vertl
            _spriteBatch.Draw(_pointTexture, new Rectangle((hTiles - hTiles/4)*32, 0, hTiles/4 * 32, vTiles*32), Color.Red * 0.5f);//vertr
            _spriteBatch.Draw(_pointTexture, new Rectangle(hTiles/4 * 32, 0, hTiles/2*32, 160), Color.Red * 0.5f);//hort
            _spriteBatch.Draw(_pointTexture, new Rectangle(hTiles/4 * 32, (vTiles - vTiles/4)*32, hTiles/2*32, vTiles/4*32), Color.Red * 0.5f);//horb
            if(blinkTick > 2){
                coreInvalid = false;
                blinkTick = 0;
            }
        }
        _spriteBatch.End();
        //FPS
        //Console.WriteLine(1 / gameTime.ElapsedGameTime.TotalSeconds);
        base.Draw(gameTime);
    }


    public void changeState(GameState state)
    {
        currentState = state;
    }

    //Construction Phase update put in own class???

    public void ConstructionUpdate(GameTime gameTime)
    {

        //generates map
        if (baseLevel == null)
        {
            sand = new Resource("Sand", resourceMap, new Rectangle(0, 0, 32, 32), 5);
            wetSand = new Resource("WetSand", resourceMap, new Rectangle(32, 0, 32, 32), 2);
            rock = new Resource("Rock", resourceMap, new Rectangle(96, 0, 32, 32), 1);
            shell = new Resource("Shells", resourceMap, new Rectangle(64, 0, 32, 32), 0);
            core = new Building("Core", tileMap, new Rectangle(64, 0, 32, 32), new Vector2(0, 596), 1, 1, new List<Resource>() { });
            lowWall = new Building("lowWall", tileMap, new Rectangle(96, 0, 32, 32), new Vector2(48, 596), 1, 1, new List<Resource>() { });
            highWall = new Building("highWall", tileMap, new Rectangle(128, 0, 32, 32), new Vector2(96, 596), 2, 1, new List<Resource>() { });
            tower = new Building("Tower", tileMap, new Rectangle(160, 0, 32, 32), new Vector2(144, 596), 3, 1, new List<Resource>() { });
            mine = new Building("Mine", tileMap, new Rectangle(192, 0, 32, 32), new Vector2(192, 596), 1, 1, new List<Resource>() { });
            moat = new Building("Moat", tileMap, new Rectangle(224, 0, 32, 32), new Vector2(240, 596), 1, 1, new List<Resource>() { });
            nextWave = new Button("NextWave", UIMap, new Rectangle(0, 0, 32, 32), defaultFont)
            {
                position = new Vector2(1000, 600)
            };


            inventory = new List<Resource>() { sand, wetSand, rock, shell };
            level1UI = new LevelUI(inventory, new List<Structure>() { core, lowWall, highWall, tower, mine, moat }, 0, 520);
            level1UI.buttons.Add(nextWave);
            int currentMapX = 0;
            int currentMapY = 0;
            List<Tile> levelLayout = new List<Tile>();
            foreach (char c in baseMap)
            {
                Tile ti;
                switch (c)
                {
                    case '0':
                        ti = new Tile("Water", tileMap, new Rectangle(0, 0, 32, 32), new Vector2(currentMapX * 32, currentMapY * 32), false, false);
                        levelLayout.Add(ti);
                        currentMapX++;
                        break;
                    case '1':
                        ti = new Tile("Sand", tileMap, new Rectangle(32, 0, 32, 32), new Vector2(currentMapX * 32, currentMapY * 32), true, false);
                        levelLayout.Add(ti);
                        currentMapX++;
                        break;
                    case '\n':
                        currentMapX = 0;
                        currentMapY++;
                        break;

                }
            }
            List<Structure> levelStructures = new List<Structure>() { };
            baseLevel = new Level("Empty Beach", levelLayout, levelStructures);

        }//end of initial level gen

        mousePosition = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);

        //place objects
        if (Mouse.GetState().LeftButton == ButtonState.Pressed && wasReleased)
        {
            int tileX = (int)Math.Floor(mousePosition.X / 32.0);
            int tileY = (int)Math.Floor(mousePosition.Y / 32.0);
            if (tileY * hTiles >= 192 && tileY * hTiles < 640 && !tileOccupied(baseLevel.structures, tileX, tileY))
            {
                if (selected == core && !corePlaced)
                {
                    if(tileX >= hTiles/4 && tileX < hTiles - hTiles/4 && tileY < vTiles - vTiles/4){//only place in middle half
                        baseLevel.structures.Add(new Building("Core", tileMap, new Rectangle(64, 0, 32, 32), new Vector2(tileX * 32, tileY * 32), 1, 1, new List<Resource>() { }));
                        digSFX.Play(.1f, 0f, 0f);
                        corePlaced = true;//max place 1
                    }else{
                        coreInvalid = true;
                        //invalidPlacement(selected.name);
                    }
                }
                else if (selected == lowWall && sand.quantity > 0)
                {
                    baseLevel.structures.Add(new Building("LowWall", tileMap, new Rectangle(96, 0, 32, 32), new Vector2(tileX * 32, tileY * 32), 2, 5, new List<Resource>() { }));
                    digSFX.Play(.1f, 0f, 0f);
                    sand.quantity -= 1;
                }
                else if (selected == highWall && sand.quantity > 1)
                {
                    baseLevel.structures.Add(new Building("HighWall", tileMap, new Rectangle(128, 0, 32, 32), new Vector2(tileX * 32, tileY * 32), 3, 10, new List<Resource>() { }));
                    digSFX.Play(.1f, 0f, 0f);
                    sand.quantity -= 2;
                }
                else if (selected == tower && sand.quantity > 2 && !checkNeighbors(baseLevel.structures, tower, tileX, tileY))
                {
                    baseLevel.structures.Add(new Building("Tower", tileMap, new Rectangle(160, 0, 32, 32), new Vector2(tileX * 32, tileY * 32), 4, 15, new List<Resource>() { }));
                    digSFX.Play(.1f, 0f, 0f);
                    sand.quantity -= 3;
                }
                else if (selected == mine && mineCount < currentWave)
                {
                    baseLevel.structures.Add(new Building("Mine", tileMap, new Rectangle(192, 0, 32, 32), new Vector2(tileX * 32, tileY * 32), 0, 1, new List<Resource>() { }));
                    digSFX.Play(.1f, 0f, 0f);
                    sand.quantity += 2;
                    mineCount++;
                }
                else if (selected == moat)
                {
                    baseLevel.structures.Add(new Building("Moat", tileMap, new Rectangle(224, 0, 32, 32), new Vector2(tileX * 32, tileY * 32), 0, 1, new List<Resource>() { }));
                    digSFX.Play(.1f, 0f, 0f);
                    sand.quantity += 1;
                }
            }
            //selected from menu
            if (mousePosition.X > core.location.X && mousePosition.X < core.location.X + 32 && mousePosition.Y > core.location.Y && mousePosition.Y < core.location.Y + 32)
            {
                selected = core;
            }
            else if (mousePosition.X > lowWall.location.X && mousePosition.X < lowWall.location.X + 32 && mousePosition.Y > lowWall.location.Y && mousePosition.Y < lowWall.location.Y + 32)
            {
                selected = lowWall;
            }
            else if (mousePosition.X > highWall.location.X && mousePosition.X < highWall.location.X + 32 && mousePosition.Y > highWall.location.Y && mousePosition.Y < highWall.location.Y + 32)
            {
                selected = highWall;
            }
            else if (mousePosition.X > tower.location.X && mousePosition.X < tower.location.X + 32 && mousePosition.Y > tower.location.Y && mousePosition.Y < tower.location.Y + 32)
            {
                selected = tower;
            }
            else if (mousePosition.X > mine.location.X && mousePosition.X < mine.location.X + 32 && mousePosition.Y > mine.location.Y && mousePosition.Y < mine.location.Y + 32)
            {
                selected = mine;
            }
            else if (mousePosition.X > moat.location.X && mousePosition.X < moat.location.X + 32 && mousePosition.Y > moat.location.Y && mousePosition.Y < moat.location.Y + 32)
            {
                selected = moat;
            }
            foreach (Structure s in level1UI.structures)
            {
                s.isSelected = false;
            }
            if (selected != null)
                selected.isSelected = true;
            foreach(Structure s in baseLevel.structures){
                if(mousePosition.X >= s.location.X && mousePosition.X <= s.location.X + 32 && mousePosition.Y >= s.location.Y && mousePosition.Y <= s.location.Y + 32){
                    selectedStructure = s;
                }
            }
            wasReleased = false;
        }//End of left click check
        if (Mouse.GetState().LeftButton == ButtonState.Released)
        {
            wasReleased = true;
        }
    }

    public void updateWave(GameTime gameTime)
    {
        waveTimer += gameTime.ElapsedGameTime.TotalSeconds;//Wave timer to count for each wave step movement
        List<Tile> currentWaves = new List<Tile>() { };
        foreach (Tile t in waveTiles)//generate alt wave list to iterate
            currentWaves.Add(t);
        if (waveInterval == 0)//waveStartup
        {
            waveShift = rand.Next(0, 50);
            for (int i = 0; i < hTiles; i++)
            {
                double radianVal = i * 32 * (2 * Math.PI) / 1248;//Assign each column on map a value between 0->2pi 
                double sineVal = Math.Sin(radianVal + waveShift); //Returns value between 0 and 1 offset by shift
                double turn = Math.Abs(sineVal * (5 + currentWave) / waveTicks); //Multiply by "amplitude" aka max tiles to move toward castle
                currentWaveDistance.Add(0);//initialize 0s to distance wave traveled
                turnWaveDistance.Add(turn);//how far each column moves per tick
            }
        }

        if (waveTimer >= tickTime && waveInterval < waveTicks)//each wave tick
        {
            waveInterval += 1;
            Console.WriteLine("Second: " + waveTimer);
            waveTimer = 0;
            bool locFound = false;
            foreach (Tile t in baseLevel.tiles)
            {//initial start of wave from last water tile
                Vector2 newLoc = new Vector2(t.location.X, t.location.Y + 32);
                if (t.type == "Water" && t.location.Y == 96)
                {//Eventually change to lowest water tile in column
                    foreach (Tile w in currentWaves)
                    {
                        if (newLoc == w.location)
                        {//if next tile already has a wave tile on it increase height
                            w.height += 1;
                            locFound = true;
                            break;
                        }
                    }
                    if (!locFound)
                    {//if next tile is not already wave tile (Probably its own function have to repeat)
                        bool waveLocFound = false;
                        foreach (Structure s in baseLevel.structures.ToArray())
                        {
                            if (newLoc == s.location)
                            {
                                s.remainingDurability -= 1;
                                waveLocFound = true;
                                if (s.remainingDurability <= 0)
                                    baseLevel.structures.Remove(s);
                                break;
                            }
                        }
                        if (!waveLocFound)
                        {
                            waveTiles.Add(new Tile("Water", tileMap, new Rectangle(0, 0, 32, 32), newLoc, false, false));//move water tiles by the designated value floored 
                        }
                    }
                }
            }//end of base tile loop
            foreach (Tile w in currentWaves)
            {//check each wave tile
                locFound = false;
                Vector2 newLoc = new Vector2(w.location.X, w.location.Y + 32);
                int newLocCol = (int)(newLoc.X / 32);
                foreach (Tile x in currentWaves)
                {
                    if (newLoc == x.location)
                    {
                        w.height += 1;
                        locFound = true;
                        break;
                    }
                }
                if (!locFound)
                {//same function as with reg tiles
                    bool waveLocFound = false;
                    foreach (Structure s in baseLevel.structures.ToArray())
                    {
                        if (newLoc == s.location)
                        {
                            if(s.height >= w.height){
                                s.remainingDurability -= 1;
                                //s.height = s.height * s.remainingDurability/s.durability;
                                waveLocFound = true;
                                if (s.remainingDurability <= 0)
                                    baseLevel.structures.Remove(s);
                                break;
                            }
                            if(s.name == "Mine"){
                                baseLevel.structures.Add(new Building("WaterMine", tileMap, new Rectangle(192, 32, 32, 32), s.location, 0, 10, new List<Resource>() { }));
                                baseLevel.structures.Remove(s);
                            }else if(s.name == "Moat"){
                                baseLevel.structures.Add(new Building("WaterMoat", tileMap, new Rectangle(224, 32, 32, 32), s.location, 0, 10, new List<Resource>() { }));
                                waveLocFound = true;
                                baseLevel.structures.Remove(s);
                                break;
                            }
                        }
                        
                    }
                    if (!waveLocFound && currentWaveDistance[newLocCol] >= 1)
                    {
                        waveTiles.Add(new Tile("Water", tileMap, new Rectangle(0, 0, 32, 32), newLoc, false, false));//move water tiles by the designated value floored   
                    }

                    currentWaveDistance[newLocCol] = currentWaveDistance[newLocCol] % 1;
                }
            }
            for (int i = 0; i < hTiles; i++)
            {//update next tick wave tiles
                currentWaveDistance[i] += turnWaveDistance[i];
            }
        }
        if (waveInterval >= waveTicks && waveTimer >= tickTime)
        {//wave end (destruction and hit code needs to be in each tick)
            waveInterval += 1;
            waveTimer = 0;
            Console.WriteLine("wave receding");
            //while(waveTiles.Count > 0){
            foreach (Tile t in waveTiles.ToArray())
            {
                t.height -= 1;
                if (t.height <= 0)
                {
                    waveTiles.Remove(t);
                }
            }
            //}
            if (waveTiles.Count == 0 || waveInterval > 20)
            {
                Console.WriteLine("wave ended");
                currentState = GameState.ConstructionPhase;
                waveInterval = 0;
                currentWaveDistance.Clear();
                turnWaveDistance.Clear();
                waveTiles.Clear();
                previousState = GameState.WavePhase;
            }
        }
    }

    public void TileHighlight(SpriteBatch spriteBatch, Rectangle bounds, Color color, int borderSize)
    {
        Texture2D _pointTexture;

        _pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        _pointTexture.SetData<Color>(new Color[] { Color.White });

        if(bounds.Y > 480)
            bounds.Y = 480;

        spriteBatch.Draw(_pointTexture, new Rectangle(bounds.X, bounds.Y, borderSize, borderSize + bounds.Height), color);
        spriteBatch.Draw(_pointTexture, new Rectangle(bounds.X, bounds.Y, borderSize + bounds.Width, borderSize), color);
        spriteBatch.Draw(_pointTexture, new Rectangle(bounds.X + bounds.Width, bounds.Y, borderSize, borderSize + bounds.Height), color);
        spriteBatch.Draw(_pointTexture, new Rectangle(bounds.X, bounds.Y + bounds.Height, borderSize + bounds.Width, borderSize), color);
    }

    public bool tileOccupied(List<Structure> structures, int tileX, int tileY){
        //Debugger.Break();
        foreach(Structure s in structures){
            if(s.location.X == tileX*32 && s.location.Y == tileY*32)
                return true;
        }
        return false;
    }
    public bool checkNeighbors(List<Structure> structures, Structure structure, int tileX, int tileY){
        foreach(Structure s in structures){
            if(s.name == "Tower"){
                if(s.location.X == tileX*32-32 && s.location.Y == tileY*32)//west
                    return true;
                else if(s.location.X == tileX*32+32 && s.location.Y == tileY*32)//east
                    return true;
                else if(s.location.X == tileX*32 && s.location.Y == tileY*32-32)//north
                    return true;
                else if(s.location.X == tileX*32 && s.location.Y == tileY*32+32)//south
                    return true;
            }
        }
        return false;
    }
    public void invalidPlacement(string name){

    }
}
