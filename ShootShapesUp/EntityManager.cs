using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootShapesUp
{
    static class EntityManager
    {
        static List<Entity> entities = new List<Entity>();
        static List<Enemy> enemies = new List<Enemy>();
        static List<Bullet> bullets = new List<Bullet>();

        static bool isUpdating;
        static List<Entity> addedEntities = new List<Entity>();

        public static int Count { get { return entities.Count; } }
        public static int Lives = 3; //Life counter
        public static int Score = 0; //Score counter
        public static int Level = 1; //Level Number
        public static int Health;
        public static int BossHealth = 100;
        public static bool Level3Reset = true;
        static bool Stop = false;

        public static void Add(Entity entity)
        {
            if (!isUpdating)
                AddEntity(entity);
            else
                addedEntities.Add(entity);
        }

        private static void AddEntity(Entity entity)
        {
            entities.Add(entity);
            if (entity is Bullet)
                bullets.Add(entity as Bullet);
            else if (entity is Enemy)
                enemies.Add(entity as Enemy);
        }

        public static void Update()
        {
            isUpdating = true;
            HandleCollisions();

            foreach (var entity in entities)
                entity.Update();

            isUpdating = false;

            foreach (var entity in addedEntities)
                AddEntity(entity);

            addedEntities.Clear();

            entities = entities.Where(x => !x.IsExpired).ToList();
            bullets = bullets.Where(x => !x.IsExpired).ToList();
            enemies = enemies.Where(x => !x.IsExpired).ToList();
            if (Score>=500 && Score<=1000) //500,1000
            {
                Level = 2;
            }
            else if (Score>=500 && Score>=1001) //500,1001
            {
                Level = 3;
                if (Level3Reset==true)
                {
                    enemies.ForEach(x => x.WasShot());
                    Level3Reset = false;
                }
            }
            if (BossHealth==0 && Stop == false)
            {
                enemies.ForEach(x => x.WasShot());
                Stop = true;
            }
        }

        static void HandleCollisions()
        {
            // handle collisions between enemies
            for (int i = 0; i < enemies.Count; i++)
                for (int j = i + 1; j < enemies.Count; j++)
                {
                    if (IsColliding(enemies[i], enemies[j]))
                    {
                        enemies[i].HandleCollision(enemies[j]);
                        enemies[j].HandleCollision(enemies[i]);
                    }
                }

            // handle collisions between bullets and enemies
            for (int i = 0; i < enemies.Count; i++)
                for (int j = 0; j < bullets.Count; j++)
                {
                    if (Level <= 2)
                    {
                        if (IsColliding(enemies[i], bullets[j]))
                        {
                            enemies[i].WasShot();
                            bullets[j].IsExpired = true;
                            Score += 5; //Increments score by 5 points per kill
                        }
                    }
                    else if (Level == 3 && IsColliding(enemies[i], bullets[j]))
                    {                       
                        bullets[j].IsExpired = true;
                        Health--;
                        if (Health<=0)
                        {
                            enemies[i].WasShot();
                            Score += 10;
                            if (BossHealth>=1) // will not continue reducing boss hp% if the hp % reaches 0%
                            {
                                BossHealth = BossHealth - 2; // reduces boss hp% by specified number per enemy kill
                            }
                        }
                    }
                }

            // handle collisions between the player and enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].IsActive && IsColliding(PlayerShip.Instance, enemies[i]))
                {
                    PlayerShip.Instance.Kill();
                    enemies.ForEach(x => x.WasShot());
                    Lives--; //Removes 1 life per player death
                    EnemySpawner.Reset();
                    break;
                    
                }
            }
        }

        private static bool IsColliding(Entity a, Entity b)
        {
            float radius = a.Radius + b.Radius;
            return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in entities)
                entity.Draw(spriteBatch);
        }
        public static void Kill()
        {
            enemies.ForEach(x => x.WasShot());
        }

    }
}
