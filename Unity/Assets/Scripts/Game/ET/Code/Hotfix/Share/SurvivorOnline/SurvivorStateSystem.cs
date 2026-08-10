namespace ET
{
    public static class SurvivorStateSystem
    {
        public static int NextRandom(this SurvivorWorldComponent self)
        {
            self.Data.RandomState = unchecked(self.Data.RandomState * 1103515245 + 12345);
            return self.Data.RandomState & int.MaxValue;
        }

        public static long AllocateStateId(this SurvivorWorldComponent self)
        {
            return self.Data.NextStateId++;
        }
    }
}
