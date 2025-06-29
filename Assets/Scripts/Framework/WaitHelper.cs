using System.Collections.Generic;
using UnityEngine;

namespace Framework
{public static class WaitHelper
     {
         private static Dictionary<float, WaitForSeconds> waitCache = new();
 
         public static WaitForSeconds GetWait(float seconds)
         {
             if (waitCache.TryGetValue(seconds, out WaitForSeconds wait))
                 return wait;
             
             waitCache.Add(seconds, new WaitForSeconds(seconds));
 
             return waitCache[seconds];
         }
     }
    
}