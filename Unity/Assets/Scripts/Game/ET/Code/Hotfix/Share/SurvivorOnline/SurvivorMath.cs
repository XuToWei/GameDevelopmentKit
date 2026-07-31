namespace ET
{
    public static class SurvivorMath
    {
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        public static int Abs(int value)
        {
            return value < 0 ? -value : value;
        }

        public static int Sign(int value)
        {
            if (value < 0)
            {
                return -1;
            }

            if (value > 0)
            {
                return 1;
            }

            return 0;
        }
    }
}
