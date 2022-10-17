namespace ET
{
    public static class AccountHelper
    {
        /// <summary>
        /// 检查账号密码格式合法性
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <returns>是否合法，非法返回错误码</returns>
        public static int? CheckAccountPassword(string account, string password)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                return 1;
            }

            if (account.Length is < 1 or > 18)
            {
                return 2;
            }

            if (password.Length is < 1 or > 18)
            {
                return 3;
            }
            
            return null;
        }
    }
}
