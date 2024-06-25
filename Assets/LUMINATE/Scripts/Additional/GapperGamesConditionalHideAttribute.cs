using UnityEngine;
using System;
using System.Collections;

namespace GapperGames
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class GapperGamesConditionalHideAttribute : PropertyAttribute
    {
        public string conditionalSourceField;
        public bool showIfTrue;
        public int enumIndex;

        public GapperGamesConditionalHideAttribute(string boolVariableName, bool showIfTrue)
        {
            conditionalSourceField = boolVariableName;
            this.showIfTrue = showIfTrue;
        }

        public GapperGamesConditionalHideAttribute(string enumVariableName, int enumIndex)
        {
            conditionalSourceField = enumVariableName;
            this.enumIndex = enumIndex;
        }

    }

}



