using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootShapesUp
{
    static class EnemySpawner
    {
        static Random rand = new Random();
        static float inverseSpawnChance = 60;
        static int spawn;
        static bool SpawnCap = false; // Boolean which enables or disables spawns in the boss level, to keep the level from becoming cluttered
        static bool HardStop = false;

        public static void Update()
        {

            if (EntityManager.BossHealth == 0)
            {
                HardStop = true;
            }

            if (SpawnCap == false)
            {
                if (!PlayerShip.Instance.IsDead && EntityManager.Count < 200 && EntityManager.Level == 3) // if the boss level is reached and less than 20 enemies are present, more are spawned
                {
                    if (EntityManager.Count != 30)
                    {
                        if (HardStop==true)
                        {
                            SpawnCap = true;
                            if (EntityManager.Count == 500)
                            {
                                EntityManager.Add(Enemy.CreateEnemy(GetSpawnPosition()));
                            }
                        }
                        else if 
                            (rand.Next((int)inverseSpawnChance) == 0)
                            EntityManager.Add(Enemy.CreateEnemy(GetSpawnPosition()));
                        if (EntityManager.Count >= 30)
                        {
                            SpawnCap = true; // spawns are stopped of 20 or more enemies are present in order to avoid clutter
                        }
                    }
                }
            }
            if (EntityManager.Count < 20)
            {
                SpawnCap = false;
            }

            if (!PlayerShip.Instance.IsDead && EntityManager.Count < 200 && EntityManager.Level <= 2)
            {
                if (rand.Next((int)inverseSpawnChance) == 0)
                    EntityManager.Add(Enemy.CreateEnemy(GetSpawnPosition()));
            }

            // slowly increase the spawn rate as time progresses
            if (inverseSpawnChance > 20 && EntityManager.Level == 1)
            {
                inverseSpawnChance -= 0.1f;
            }// default is 0.005f
            else if (inverseSpawnChance > 20 && EntityManager.Level == 2)
            {
                inverseSpawnChance -= 0.2f;
            }
            else if (inverseSpawnChance > 20 && EntityManager.Level == 3)
            {
                inverseSpawnChance -= 0.3f;
            }
        }

        private static Vector2 GetSpawnPosition()
        {
            if (EntityManager.Level == 1)
            {
                spawn = 10;
            }
            else if (EntityManager.Level == 2)
            {
                spawn = rand.Next((int)GameRoot.ScreenSize.Y);
            }
            else if (EntityManager.Level == 3)
            {
                spawn = 250;
            }


            Vector2 pos;

            if(EntityManager.Level == 3)               
            {
                pos = new Vector2(rand.Next((int)GameRoot.ScreenSize.X), spawn);
                return pos;                
            }
            else

            do
            {
                pos = new Vector2(rand.Next((int)GameRoot.ScreenSize.X), spawn);  //use varying values to change spawn locations instead of rand numbers
            }
            while (Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < 250 * 250);

            return pos;
            
 
        }

        public static void Reset()
        {
            inverseSpawnChance = 60;
        }
    }
}
