namespace ET.Server
{
    public static class SurvivorRoomCodeUtility
    {
        public static string Normalize(string roomCode)
        {
            return roomCode?.Trim().ToUpperInvariant();
        }

        public static bool IsValid(string roomCode)
        {
            if (string.IsNullOrEmpty(roomCode) || roomCode.Length < 4 || roomCode.Length > 12)
            {
                return false;
            }

            for (int index = 0; index < roomCode.Length; ++index)
            {
                if (roomCode[index] >= 'A' && roomCode[index] <= 'Z')
                {
                    continue;
                }

                if (roomCode[index] >= '0' && roomCode[index] <= '9')
                {
                    continue;
                }

                return false;
            }

            return true;
        }
    }

    public static class SurvivorCoroutineLockType
    {
        public const int RoomDirectory = 50;
    }
}
