using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bork.Helpers;

namespace Bork.Controls
{
    /// <summary>
    /// Parent class for all game objects (ships, misiles, etc)
    /// </summary>
    class GameDisplayObject : RichImage
    {
        public GameDisplayObject(string path, Modules.CollisionDetection.CollisionTypes collisionType = Modules.CollisionDetection.CollisionTypes.None,  bool animated = false, int frameCount = 0, double duration = 0, int from = 0) 
            : base(path, animated, frameCount, duration, from)
        {
            RotationMode = Common.RotationMode.Manual;

            MaxSpeed = double.MaxValue;
            MaxRotationSpeed = double.MaxValue;
            Allegiance = "";

            CollisionType = collisionType;
            if (collisionType != Modules.CollisionDetection.CollisionTypes.None)
            {
                Modules.CollisionDetection.addObject(this);
            }
        }

        ~GameDisplayObject()
        {
            if (CollisionType != Modules.CollisionDetection.CollisionTypes.None)
            {
                Modules.CollisionDetection.removeObject(this);
            }
        }

        new public void Update(double dt)
        {
            ((RichImage)this).Update(dt);
            var rotation = getRotation();

            if (RotationMode == Common.RotationMode.Tracking && TrackingTarget != null)
            {
                var dr = Common.getAngleBetween(getPosition(), TrackingTarget.getPosition());
                var target1 = dr + 180;
                var target2 = dr - 180;
                RotationTarget = Math.Abs(rotation - target1) < Math.Abs(rotation - target2) ?
                                    target1 : target2;
            }
            if (RotationMode == Common.RotationMode.TargetRotation|| RotationMode == Common.RotationMode.Tracking)
            {
                RotationSpeed = RotationTarget - rotation;
            }

            var effectiveSpeed = Speed * dt;
            var dx = effectiveSpeed * Math.Sin(rotation * Math.PI / 180);
            var dy = effectiveSpeed * Math.Cos(rotation * Math.PI / 180);
            var effectiveRotationSpeed = RotationSpeed * dt; 
            setPosition(getPosition() + new Vec2(dx, dy));
            setRotation(rotation + effectiveRotationSpeed);
        }

        public Common.RotationMode RotationMode { get; set; }

        private double speed;
        public double Speed
        {
            get
            {
                return speed;
            }
            set
            {
                if (value > MaxSpeed)
                {
                    value = MaxSpeed;
                }
                else if (value < -MaxSpeed)
                {
                    value = -MaxSpeed;
                }
                else
                {
                    speed = value;
                }
            }
        }
        private double rotationSpeed;
        public double RotationSpeed
        {
            get
            {
                return rotationSpeed;
            }
            set
            {
                if (value > MaxRotationSpeed)
                    value = MaxRotationSpeed;
                if (value < -MaxRotationSpeed)
                    value = -MaxRotationSpeed;
                rotationSpeed = value;
            }
        }
        private double hp;
        public double HP
        {
            get
            {
                var retval = hp < 0 ? 0 : hp;
                return retval;
            }
            set
            {
                hp = value > MaxHP ? MaxHP : value;
                if (hp <= 0)
                    kill(value);
            }
        }
        public double MaxSpeed { get; set; }
        public double MaxRotationSpeed { get; set; }
        public double MaxHP { get; set; }
        public double RotationTarget { get; set; }

        private RichImage trackingTarget;
        public RichImage TrackingTarget
        {
            get
            {
                return trackingTarget;
            }
            set
            {
                trackingTarget = value;
            }
        }

        public bool isCollidable()
        {
            return Modules.CollisionDetection.contains(this);
        }

        protected List<string> tags = new List<string>();

        public bool isInvulnerable()
        {
            return tags.Contains("invulnerable");
        }
        public void isInvulnerable(bool v)
        {
            if (tags.Contains("invulnerable") && !v)
            {
                tags.Remove("invulnerable");
            }
            else if (!tags.Contains("invulnerable") && v)
            {
                tags.Add("invulnerable");
            }
        }

        public bool isKilled()
        {
            return tags.Contains("killed");
        }

        public Modules.CollisionDetection.CollisionTypes CollisionType { get; set; }
        /// <summary>
        /// Prevent friendly fire / shooting yourself when firing
        /// Depending on how it's used
        /// </summary>
        public string Allegiance { get; set; }

        /// <summary>
        /// Called by Modules.CollisionDetection
        /// Calls both objects
        /// </summary>
        /// <param name="other">the object this collided with</param>
        public void collision(GameDisplayObject other)
        {
            if (CollisionType == Modules.CollisionDetection.CollisionTypes.Projectile)
            {
                HP = 0;
            }
        }

        /// <summary>
        /// Run when the HP first reaches < 0
        /// </summary>
        /// <param name="dHP">The hp modifier that resulted in this function being called</param>
        public void kill(double dHP)
        {
            tags.Add("killed");
        }
    }
}
