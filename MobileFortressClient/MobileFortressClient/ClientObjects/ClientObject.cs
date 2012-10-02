using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MobileFortressClient.ClientObjects
{
    abstract class ClientObject
    {
        public abstract void Update(float dt);
        public abstract void Draw();
    }
}
