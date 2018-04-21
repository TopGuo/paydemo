using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace paydemo.con {
    /// <summary>
    /// 
    /// </summary>
    public class Pay {
        private const string url = "http://pay.vpsdl.cn/submit.php?";

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="type">alipay:支付宝,tenpay:财付通,qqpay:QQ钱包,wxpay:微信支付</param>
        /// <param name="tradeno"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="returnUrl"></param>
        /// <param name="name"></param>
        /// <param name="money"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool WithPay (string type, string tradeno, string notifyUrl, string returnUrl, string name, string money, out string message) {

            SortedDictionary<string, string> dict = new SortedDictionary<string, string> ();
            dict.Add ("pid", AlipayConfig.Pid);
            dict.Add ("type", type);
            dict.Add ("out_trade_no", Guid.NewGuid ().ToString ());
            dict.Add ("notify_url", notifyUrl);
            dict.Add ("return_url", returnUrl);
            dict.Add ("name", name);
            dict.Add ("money", money);
            dict.Add ("sign_type", "MD5");
            //var sign = RSAFromPkcs8.sign (string.Join ("&", (dict.Select (t => $"{t.Key}={(t.Value)}"))), AlipayConfig.Private_key, "utf-8");
            var sign= Security.GetMD5(string.Join ("&", (dict.Select (t => $"{t.Key}={(t.Value)}"))));
            dict.Add ("sign", sign);
            var query = dict.ToQueryString ();
            string ReqStr = HttpUtility.GetString ($"{url}{query}");
                
            var obj = JsonConvert.DeserializeObject<JObject> (ReqStr);
            // string returnCode = obj.SelectToken ("alipay_fund_trans_toaccount_transfer_response.code").ToObject<string> ();
            // if (returnCode == "10000") {
            //     message = ReqStr;
            //     return true;
            // } else {
            //     message = obj.SelectToken ("alipay_fund_trans_toaccount_transfer_response.sub_msg").ToObject<string> ();
            //     return false;
            // }
            message="ok";
            return true;

        }

    }
}