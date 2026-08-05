using GTA;
using IMNOTEPIX.Framework.fExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMNOTEPIX.Framework.fExtension
{
    internal class EntityExtensionTick : Script
    {
        public EntityExtensionTick() {
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            EntityExtensions.Update();
        }
    }
    internal class BlipExtensionTick : Script
    {
        public BlipExtensionTick()
        {
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            BlipExtensions.Update();
        }
    }
}
