using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMNOTEPIX.Framework.fCore
{
    public class Misc
    {
        /// <param name="p0">Known values: 0, 8 (used in SinglePlayer), 7 (used in GTA Online)</param>
        public static void SetLowerRichPresence(int p0, string textLabel)
            => Function.Call(Hash.NETWORK_​SET_​RICH_​PRESENCE_​STRING, p0, textLabel);
    }
}
