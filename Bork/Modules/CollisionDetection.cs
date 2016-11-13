using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Bork.Controls;
using Bork.Helpers;

namespace Bork.Modules
{
    /// <summary>
    /// Handles and stores collision detection
    /// TODO: Lasers
    /// </summary>
    static class CollisionDetection
    {
        public enum CollisionTypes
        {
            None, Ship, Projectile
        }

        /// <summary>
        /// Collision detection with all
        /// </summary>
        static LinkedList<GameDisplayObject> shipList = new LinkedList<GameDisplayObject>();

        /// <summary>
        /// Collision detection with ships but not each other
        /// </summary>
        static LinkedList<GameDisplayObject> projectileList = new LinkedList<GameDisplayObject>();

        static Dictionary<Tuple<GameDisplayObject, GameDisplayObject>, bool> state = new Dictionary<Tuple<GameDisplayObject, GameDisplayObject>, bool>();

        static public bool addObject(GameDisplayObject gdo)
        {
            switch (gdo.CollisionType)
            {
                case CollisionTypes.Ship:
                    if (shipList.Contains(gdo))
                        return false;
                    foreach (var ship in shipList)
                    {
                        state.Add(new Tuple<GameDisplayObject, GameDisplayObject>(gdo, ship), shipCollisionDetection(gdo, ship));
                    }
                    foreach (var projectile in projectileList)
                    {
                        state.Add(new Tuple<GameDisplayObject, GameDisplayObject>(gdo, projectile), projectileCollisionDetection(projectile, gdo));
                    }
                    shipList.AddLast(gdo);
                    break;
                case CollisionTypes.Projectile:
                    if (projectileList.Contains(gdo))
                        return false;
                    foreach (var projectile in shipList)
                    {
                        state.Add(new Tuple<GameDisplayObject, GameDisplayObject>(gdo, projectile), projectileCollisionDetection(gdo, projectile));
                    }
                    projectileList.AddLast(gdo);
                    break;
                default:
                    Debug.Assert(false && "No such type" != "");
                    return false;
            }
            return true;
        }

        static public bool removeObject(GameDisplayObject gdo)
        {
            var retval = false;
            switch (gdo.CollisionType)
            {
                case CollisionTypes.Ship:
                    retval = shipList.Remove(gdo);
                    break;
                case CollisionTypes.Projectile:
                    retval = projectileList.Remove(gdo);
                    break;
                default:
                    Debug.Assert(false && "No such type" != "");
                    break;
            }
            if (retval == true)
            {
                List<Tuple<GameDisplayObject, GameDisplayObject>> toRemove = new List<Tuple<GameDisplayObject, GameDisplayObject>>();
                foreach (var key in state.Keys)
                {
                    if (key.Item1 == gdo || key.Item2 == gdo)
                        toRemove.Add(key);
                }
                foreach (var child in toRemove)
                {
                    state.Remove(child);
                }
            }
            return retval;
        }

        static public bool shipCollisionDetection(GameDisplayObject s1, GameDisplayObject s2)
        {
            return s1.boundingBoxContainsPoint(s2.getPosition()) || s2.boundingBoxContainsPoint(s1.getPosition());
        }

        static public bool projectileCollisionDetection(GameDisplayObject projectile, GameDisplayObject gdo)
        {
            return gdo.boundingBoxContainsPoint(projectile.getPosition());
        }

        static public void Update()
        {
            foreach (var key in state.Keys.ToList())
            {
                if (key.Item1.Allegiance == key.Item2.Allegiance)
                {
                    state[key] = false;
                }
                else if (key.Item1.CollisionType == CollisionTypes.Ship && key.Item2.CollisionType == CollisionTypes.Ship)
                {
                    state[key] = shipCollisionDetection(key.Item1, key.Item2);
                }
                else if (key.Item1.CollisionType == CollisionTypes.Projectile)
                {
                    state[key] = projectileCollisionDetection(key.Item1, key.Item2);
                }
                else if (key.Item2.CollisionType == CollisionTypes.Projectile)
                {
                    state[key] = projectileCollisionDetection(key.Item2, key.Item1);
                }
                else
                {
                    Debug.Assert(false && "Invalid combination" != "");
                }

                if (state[key] == true)
                {
                    Console.WriteLine("COLLISION: " + key.Item1.id + " & " + key.Item2.id);
                    key.Item1.collision(key.Item2);
                    key.Item2.collision(key.Item1);
                }
            }
        }

        static public bool getCollision(GameDisplayObject g1, GameDisplayObject g2)
        {
            var key = new Tuple<GameDisplayObject, GameDisplayObject>(g1, g2);
            if (state.ContainsKey(key))
            {
                return state[key];
            }
            key = new Tuple<GameDisplayObject, GameDisplayObject>(g2, g1);
            if (state.ContainsKey(key))
            {
                return state[key];
            }
            Console.WriteLine("WARN: getCollision() on a non-existent pair");
            return false;
        }

        static public bool contains(GameDisplayObject gdo)
        {
            switch (gdo.CollisionType)
            {
                case CollisionTypes.Ship:
                    return shipList.Contains(gdo);
                case CollisionTypes.Projectile:
                    return projectileList.Contains(gdo);
                default:
                    Debug.Assert(false && "No such type" != "");
                    break;
            }
            return false;
        }
    }
}
