using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;

namespace ShootShapesUp
{
    public class GameRoot : Game
    {
        // some helpful static properties
        public static GameRoot Instance { get; private set; }
        public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }
        public static Vector2 ScreenSize { get { return new Vector2(Viewport.Width, Viewport.Height); } }
        public static GameTime GameTime { get; private set; }

        public static Texture2D Player { get; private set; }
        public static Texture2D Seeker { get; private set; }
        public static Texture2D SeekerMissile { get; private set; }
        public static Texture2D Boss { get; private set; }
        public static Texture2D Bullet { get; private set; }
        public static Texture2D Bullet2 { get; private set; }
        public static Texture2D Pointer { get; private set; }
        public static Texture2D Crosshair { get; private set; }
        public static Texture2D Background1 { get; private set; }
        public static Texture2D Background2 { get; private set; }
        public static Texture2D Boss100 { get; private set; }
        public static Texture2D Boss80 { get; private set; }
        public static Texture2D Boss60 { get; private set; }
        public static Texture2D Boss40 { get; private set; }
        public static Texture2D Boss20 { get; private set; }
        public static Texture2D Boss0 { get; private set; }

        public static SpriteFont Font { get; private set; }
        public static SpriteFont Lives { get; private set; }
        public static SpriteFont Score { get; private set; }
        public static SpriteFont Level { get; private set; }
        public static SpriteFont BossHP { get; private set; }
        public static SpriteFont PlasmaAmmo { get; private set; }
        public static SpriteFont Bombs { get; private set; }
        public static SpriteFont Ending { get; private set; }

        public static Song Music { get; private set; }
        public static Song Music2 { get; private set; }
        public static Song Victory { get; private set; }

        private static readonly Random rand = new Random();

        private static SoundEffect[] explosions;
        // return a random explosion sound
        public static SoundEffect Explosion { get { return explosions[rand.Next(explosions.Length)]; } }

        private static SoundEffect[] shots;
        public static SoundEffect Shot { get { return shots[rand.Next(shots.Length)]; } }

        private static SoundEffect[] spawns;
        public static SoundEffect Spawn { get { return spawns[rand.Next(spawns.Length)]; } }

        public static bool SongSwitch = false;
        public static bool SongSwitch2 = false;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public GameRoot()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = @"Content";

            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width; //uses the width of the current screen to use as the width of the game window
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height; //gets the height of the current screen to use the height of the game window
            graphics.IsFullScreen = true; // sets the game window to fullscreen, removing the top banner from the window
        }

        protected override void Initialize()
        {
            base.Initialize();

            EntityManager.Add(PlayerShip.Instance);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(GameRoot.Music2);

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Player = Content.Load<Texture2D>("Art/Player");
            Seeker = Content.Load<Texture2D>("Art/Seeker");
            SeekerMissile = Content.Load<Texture2D>("Art/SeekerMissile");
            Boss = Content.Load<Texture2D>("Art/Boss");
            Bullet = Content.Load<Texture2D>("Art/Bullet");
            Bullet2 = Content.Load<Texture2D>("Art/Bullet2");
            Pointer = Content.Load<Texture2D>("Art/Pointer");
            Crosshair = Content.Load<Texture2D>("Art/crosshair");
            Background1 = Content.Load<Texture2D>("Art/Background1");
            Background2 = Content.Load<Texture2D>("Art/Background2");
            Boss100 = Content.Load<Texture2D>("Art/Boss100%");
            Boss80 = Content.Load<Texture2D>("Art/Boss80%");
            Boss60 = Content.Load<Texture2D>("Art/Boss60%");
            Boss40 = Content.Load<Texture2D>("Art/Boss40%");
            Boss20 = Content.Load<Texture2D>("Art/Boss20%");
            Boss0 = Content.Load<Texture2D>("Art/Boss0%");

            Font = Content.Load<SpriteFont>("Font");
            Lives = Content.Load<SpriteFont>("Lives");
            Score = Content.Load<SpriteFont>("Score");
            Level = Content.Load<SpriteFont>("Levels");
            BossHP = Content.Load<SpriteFont>("BossHP");
            PlasmaAmmo = Content.Load<SpriteFont>("PlasmaAmmo");
            Bombs = Content.Load<SpriteFont>("Bombs");
            Ending = Content.Load<SpriteFont>("Ending");

            Music = Content.Load<Song>("Sound/Music");
            Music2 = Content.Load<Song>("Sound/Music2");
            Victory = Content.Load<Song>("Sound/Victory");

            // These linq expressions are just a fancy way loading all sounds of each category into an array.
            explosions = Enumerable.Range(1, 8).Select(x => Content.Load<SoundEffect>("Sound/explosion-0" + x)).ToArray();
            shots = Enumerable.Range(1, 4).Select(x => Content.Load<SoundEffect>("Sound/shoot-0" + x)).ToArray();
            spawns = Enumerable.Range(1, 8).Select(x => Content.Load<SoundEffect>("Sound/spawn-0" + x)).ToArray();
        }

        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            Input.Update();

            // Allows the game to exit
            if (Input.WasButtonPressed(Buttons.Back) || Input.WasKeyPressed(Keys.Escape))
                this.Exit();

            EntityManager.Update();

            if (EntityManager.Lives==0) // Closes the game if the life counter reaches 0
            {
                this.Exit();
            }

            EnemySpawner.Update();

            if (EntityManager.Level == 3 && SongSwitch == false && EntityManager.BossHealth > 1) // Switches songs to boss music once level 3 is reached
            {
                MediaPlayer.Play(GameRoot.Music);
                SongSwitch = true;
            }

            if (SongSwitch2 == false && EntityManager.BossHealth <= 0) // plays a victory tune once level 3 is beaten
            {
                MediaPlayer.Play(GameRoot.Victory);
                SongSwitch2 = true;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            // Draw backgrounds
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            if (EntityManager.Level == 1)
            {
                spriteBatch.Draw(Background1, Vector2.Zero, Color.White); //Draws background1
            }
            if (EntityManager.Level == 2)
            {
                spriteBatch.Draw(Background2, Vector2.Zero, Color.White); //Draws background2
            }
            if (EntityManager.Level == 3 && EntityManager.BossHealth > 81)
            {
                spriteBatch.Draw(Boss100, Vector2.Zero, Color.White); //Draws Boss100 background
            }
            else if (EntityManager.Level == 3 && EntityManager.BossHealth <= 80 && EntityManager.BossHealth >= 61)
            {
                spriteBatch.Draw(Boss80, Vector2.Zero, Color.White); //Draws Boss80 background
            }
            else if (EntityManager.Level == 3 && EntityManager.BossHealth <= 60 && EntityManager.BossHealth >= 41)
            {
                spriteBatch.Draw(Boss60, Vector2.Zero, Color.White); //Draws Boss60 background
            }
            else if (EntityManager.Level == 3 && EntityManager.BossHealth <= 40 && EntityManager.BossHealth >= 21)
            {
                spriteBatch.Draw(Boss40, Vector2.Zero, Color.White); //Draws Boss40 background
            }
            else if (EntityManager.Level == 3 && EntityManager.BossHealth <= 20 && EntityManager.BossHealth >= 1)
            {
                spriteBatch.Draw(Boss20, Vector2.Zero, Color.White); //Draws Boss20 background
            }
            else if (EntityManager.Level == 3 && EntityManager.BossHealth <= 0)
            {
                spriteBatch.Draw(Boss0, Vector2.Zero, Color.White); //Draws Boss0 background
            }
            spriteBatch.End();


            // Draw entities. Sort by texture for better batching.
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
            EntityManager.Draw(spriteBatch);
            spriteBatch.Draw(Crosshair, Input.MousePosition, Color.Red*0.7f); // Draws a crosshair on the mouse position
            spriteBatch.End();


            // Draw user interface
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            spriteBatch.DrawString(Lives, "Lives: " + EntityManager.Lives, new Vector2(50, 10), Color.White); // Displays a life counter 
            spriteBatch.DrawString(Score, "Score: " + EntityManager.Score, new Vector2(1600, 10), Color.White); // Displays a Score counter 
            spriteBatch.DrawString(Level, "Level: " + EntityManager.Level, new Vector2(1600, 60), Color.White); // Displays the Level number
            spriteBatch.DrawString(Level, "Plasma Ammo: " + PlayerShip.PlasmaAmmo, new Vector2(50, 60), Color.White); // Displays the amount of Plasma ammo (Right click weapon)
            spriteBatch.DrawString(Level, "Bombs: " + PlayerShip.Bombs, new Vector2(50, 110), Color.White); // Displays the number of bombs available (Space bar weapon)


            if (EntityManager.Level == 3)
            {
                spriteBatch.DrawString(BossHP, "Boss Health: " + EntityManager.BossHealth + "%", new Vector2(820, 10), Color.White); // Displays the Boss health percentage
            }

            if (EntityManager.BossHealth == 0)
            {
                spriteBatch.DrawString(Ending, "Congratulations!" /*+ System.Environment.NewLine + "press escape to exit"*/, new Vector2(480, 440), Color.White); // new line makes output no centered 
                spriteBatch.DrawString(Ending, "Press Escape To Exit", new Vector2(335, 580), Color.White);
            }

            spriteBatch.End();


            base.Draw(gameTime);
        }

        private void DrawRightAlignedString(string text, float y)
        {
            var textWidth = GameRoot.Font.MeasureString(text).X;
            spriteBatch.DrawString(GameRoot.Font, text, new Vector2(ScreenSize.X - textWidth - 5, y), Color.White);
        }
    }
}
