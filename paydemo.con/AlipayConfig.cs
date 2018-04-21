namespace paydemo.con
{
    public class AlipayConfig
    {
        /// <summary>
        /// 商户ID
        /// </summary>
        /// <returns></returns>
        public static string Pid=>"1001";
        //----------------
        public static string AppId => "";


        #region 属性
        /// <summary>
        /// 获取合作者身份ID
        /// </summary>
        public static string Partner => "";


        /// <summary>
        /// 获取商户的私钥
        /// </summary>
        public static string Private_key => "";

        /// <summary>
        /// 获取支付宝的公钥
        /// </summary>
        public static string Public_key => "";

        /// <summary>
        /// 获取字符编码格式
        /// </summary>
        public static string Input_charset => "utf-8";

        /// <summary>
        /// 获取签名方式
        /// </summary>
        public static string Sign_type => "RSA";


        /// <summary>
        /// 商户号
        /// </summary>
        public static string Seller => "";

        #endregion
    }
}