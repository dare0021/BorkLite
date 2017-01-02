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

        /// <summary>
        /// Test whether two line segments intersect. If so, calculate the intersection point.
        /// <see cref="http://stackoverflow.com/a/14143738/292237"/>
        /// </summary>
        /// <param name="p">Vector to the start point of p.</param>
        /// <param name="p2">Vector to the end point of p.</param>
        /// <param name="q">Vector to the start point of q.</param>
        /// <param name="q2">Vector to the end point of q.</param>
        /// <param name="intersection">The point of intersection, if any.</param>
        /// <param name="considerOverlapAsIntersect">Do we consider overlapping lines as intersecting?
        /// </param>
        /// <returns>True if an intersection point was found.</returns>
        public static bool LineSegementsIntersect(Vec2 p, Vec2 p2, Vec2 q, Vec2 q2,
            out Vec2 intersection, bool considerCollinearOverlapAsIntersect = false)
        {
            intersection = null;

            var r = p2 - p;
            var s = q2 - q;
            var rxs = r.Cross(s);
            var qpxr = (q - p).Cross(r);

            // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
            if (Math.Abs(rxs) < 1e-10 && Math.Abs(qpxr) < 1e-10)
            {
                // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
                // then the two lines are overlapping,
                if (considerCollinearOverlapAsIntersect)
                    if ((0 <= (q - p).Dot(r) && (q - p).Dot(r) <= r.Dot(r)) || (0 <= (p - q).Dot(s) && (p - q).Dot(s) <= s.Dot(s)))
                        return true;

                // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
                // then the two lines are collinear but disjoint.
                // No need to implement this expression, as it follows from the expression above.
                return false;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (Math.Abs(rxs) < 1e-10 && !(Math.Abs(qpxr) < 1e-10))
                return false;

            // t = (q - p) x s / (r x s)
            var t = (q - p).Cross(s) / rxs;

            // u = (q - p) x r / (r x s)

            var u = (q - p).Cross(r) / rxs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
            // the two line segments meet at the point p + t r = q + u s.
            if (!(Math.Abs(rxs) < 1e-10) && (0 <= t && t <= 1) && (0 <= u && u <= 1))
            {
                // We can calculate the intersection point using either t or u.
                intersection = p + t * r;

                // An intersection was found.
                return true;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return false;
        }

        /// <summary>
        /// Ray traces from the given object and returns the first object that the ray collides with
        /// 1) use intersection checking to cull the object list to a list of possible objects
        /// 2) check the intersections detected to find the closest one
        /// 
        /// Returns null if nothing found
        /// </summary>
        static public GameDisplayObject rayTrace(GameDisplayObject from, float rayLength, out Vec2 intersection, bool checkProjectilesAlso = false)
        {
            Vec2 origin = from.getPosition();
            Vec2 target = origin + new Vec2(rayLength * Math.Sin(from.getRotation() * Math.PI / 180),
                                            rayLength * Math.Cos(from.getRotation() * Math.PI / 180));
            intersection = null;
            var candidateList = new List<Pair<GameDisplayObject, Vec2>>();
            Console.WriteLine("ROT: " + from.getRotation());
            Console.WriteLine("Origin: " + origin);
            Console.WriteLine("Target: " + target);

            foreach (var child in shipList)
            {
                if (from.Allegiance == child.Allegiance)
                    continue;
                var quad = child.getBoundingBox();
                if (LineSegementsIntersect(origin, target, quad.v0, quad.v1, out intersection))
                    candidateList.Add(new Pair<GameDisplayObject, Vec2>(child, new Vec2(intersection)));
                if (LineSegementsIntersect(origin, target, quad.v1, quad.v2, out intersection))
                    candidateList.Add(new Pair<GameDisplayObject, Vec2>(child, new Vec2(intersection)));
                if (LineSegementsIntersect(origin, target, quad.v2, quad.v3, out intersection))
                    candidateList.Add(new Pair<GameDisplayObject, Vec2>(child, new Vec2(intersection)));
                if (LineSegementsIntersect(origin, target, quad.v3, quad.v0, out intersection))
                    candidateList.Add(new Pair<GameDisplayObject, Vec2>(child, new Vec2(intersection)));
            }
            if (checkProjectilesAlso)
            {
                foreach(var child in projectileList)
                {
                    if (from.Allegiance == child.Allegiance)
                        continue;
                    var quad = child.getBoundingBox();
                    if (LineSegementsIntersect(origin, target, quad.v0, quad.v1, out intersection))
                        candidateList.Add(new Pair<GameDisplayObject, Vec2>(child, new Vec2(intersection)));
                    if (LineSegementsIntersect(origin, target, quad.v1, quad.v2, out intersection))
                        candidateList.Add(new Pair<GameDisplayObject, Vec2>(child, new Vec2(intersection)));
                    if (LineSegementsIntersect(origin, target, quad.v2, quad.v3, out intersection))
                        candidateList.Add(new Pair<GameDisplayObject, Vec2>(child, new Vec2(intersection)));
                    if (LineSegementsIntersect(origin, target, quad.v3, quad.v0, out intersection))
                        candidateList.Add(new Pair<GameDisplayObject, Vec2>(child, new Vec2(intersection)));
                }
            }
            if (candidateList.Count < 1)
                return null;
            double min = float.MaxValue;
            GameDisplayObject outval = null;
            foreach (var pair in candidateList)
            {
                double dist = (pair.Y - from.getPosition()).getLength();
                if (dist < min)
                {
                    outval = pair.X;
                    min = dist;
                    intersection = pair.Y;
                }
            }
            return outval;
        }
    }
}
