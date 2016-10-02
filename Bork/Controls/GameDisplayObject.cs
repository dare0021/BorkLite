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
        public GameDisplayObject()
        {
            MaxSpeed = double.MaxValue;
            MaxRotationSpeed = double.MaxValue;
        }

        public void Update(double dt)
        {
            var rotation = getRotation();
            var effectiveSpeed = Speed * dt;
            var dx = effectiveSpeed * Math.Sin(rotation * Math.PI / 180);
            var dy = effectiveSpeed * Math.Cos(rotation * Math.PI / 180);
            setPosition(getPosition() + new Vec2(dx, -dy)); //Why -ve? who knows?
            setRotation(rotation + RotationSpeed * dt);
        }

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
                if (hp < 0)
                    justDied(value);
            }
        }
        public double MaxSpeed { get; set; }
        public double MaxRotationSpeed { get; set; }
        public double MaxHP { get; set; }

        protected LinkedList<string> tags;

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
                tags.AddLast("invulnerable");
            }
        }

        /// <summary>
        /// Run when the HP first reaches < 0
        /// </summary>
        /// <param name="dHP">The hp modifier that resulted in this function being called</param>
        protected void justDied(double dHP)
        {

        }
    }
}
