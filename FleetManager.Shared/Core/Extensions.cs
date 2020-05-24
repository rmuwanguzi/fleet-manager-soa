using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Core
{
    public static class Extensions
    {
      
        public static long ToUnixTimestamp(this DateTime value)
        {
            return (long)Math.Truncate((value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }
    }
}
