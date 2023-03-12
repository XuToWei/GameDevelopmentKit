namespace ET
{
    public interface ITimeNow
    {
        //需要保证线程安全
        long GetUtcNowTicks();

        void Update();
    }
}
