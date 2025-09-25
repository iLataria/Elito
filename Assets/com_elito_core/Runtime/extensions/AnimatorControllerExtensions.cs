using System.Collections.Generic;
using System.Linq;

namespace Bunker.Core.Extensions
{
    public static class AnimatorControllerExtensions 
    {
        public static IEnumerable<string> GetStateNames(this UnityEditor.Animations.AnimatorController ac)
        {
            return ac.layers.SelectMany(o => o.stateMachine.states.Select(oo => oo.state.name));
        }
    }
}
