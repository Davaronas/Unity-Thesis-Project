using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UpgradeStats
{
    public static class ShadowDash
    {
        public static class Distance
        {
            public static float def = 7.5f;
            public static float distance1 = 12f;
            public static float distance2 = 16f;
        }

        public static class Cooldown
        {
            public static float def = 2f;
            public static float decreased = 1.2f;
        }

        public static class ManaCost
        {
            public static int def = 2;
            public static int decreased = 1;
        }
    }

    public static class PredatorVision
    {
        public static class Distance
        {
            public static float def = 27f;
            public static float distance1 = 35f;
            public static float distance2 = 43f;
        }

        public static class Duration
        {
            public static float def = 8.5f;
            public static float duration1 = 13f;
            public static float duration2 = 18f;
        }

        public static class ManaCost
        {
            public static int def = 2;
            public static int decreased = 1;
        }
    }

    public static class ShadowForm
    {
        public static class Duration
        {
            public static float def = 5f;
            public static float duration1 = 8f;
            public static float duration2 = 12f;
        }

        public static class Speed
        {
            public static float def = 1f;
            public static float increasedSpeed = 1.3f;
        }

        public static class Jump
        {
            public static float def = 1f;
            public static float increasedJump = 1.5f;
        }
    }

    public static class ShadowSentinel
    {
        public static class Quantity
        {
            public static int def = 1;
            public static int increased = 3;
        }

        public static class ManaCost
        {
            public static int def = 3;
            public static int decreased = 2;
        }
    }

    public static class Mirage
    {
        public static class ManaCost
        {
            public static int def = 1;
            public static float decreased = 0.5f;
        }
    }

    public static class DarkHaven
    {
        public static class Quantity
        {
            public static int def = 1;
            public static int increased = 2;
        }

        public static class Size
        {
            public static Vector3 def = new Vector3(1.2f, 1.2f, 1.2f);
            public static Vector3 increased = new Vector3(2.2f, 1.7f, 2.2f);
            public static float tunnelIncreased = 1.5f;
        }
    }



    public static class ManaSlots
    {
        public static int def = 2;
        public static int ManaSlots3 = 3;
        public static int ManaSlots4 = 4;
        public static int ManaSlots5 = 5;
        public static int ManaSlots6 = 6;
        public static int ManaSlots7 = 7;
        public static int ManaSlots8 = 8;
    }

    public static class ManaRegen
    {
        public static float def = 2.5f;
        public static float ManaRegen1 = 5f;
        public static float ManaRegen2 = 7.5f;
        public static float ManaRegen3 = 10f;
        public static float ManaRegen4 = 15f;
    }
}
