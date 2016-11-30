// /** 
//  * Functions.cs
//  * Will Hart
//  * 20161126
// */

namespace GameGHJ.AI
{
    #region Dependencies
    
    using UnityEngine;

    #endregion

    public static class Functions
    {
        public static float Logistic(float x, float a = 1.03f, float b = 55f, float c = 8f)
        {
            return Mathf.Clamp01(a/(1 + b*Mathf.Exp(-c*x)));
        }

        public static float ReverseLogistic(float x, float mul = 2f, float offset = 0.5f)
        {
            // basically I want a "reverse logistic function" here, something that (2x-1)^{11}+0.5 provides an ok estimation
            // just not sure of performance
            return ToPower(mul * x - 1, 11) + offset;
        }

        public static float Quadratic(float x) => ToPower(x, 2);
        public static float Quartic(float x) => ToPower(x, 4);
        public static float Sextic(float x) => ToPower(x, 6);
        public static float Octic(float x) => ToPower(x, 8);
        public static float OneIfTrue(bool condition) => condition ? 1 : 0;
        public static float OneIfFalse(bool condition) => 1 - OneIfTrue(condition);

        private static float ToPower(float x, float pow)
        {
            return Mathf.Clamp01(Mathf.Pow(x, pow));
        }
    }
}