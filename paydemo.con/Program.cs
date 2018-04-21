using System;

namespace paydemo.con
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("彩虹易支付==>");
            var message=string.Empty;
            Pay.WithPay("alipay","20160806151343349","http://www.cccyun.cc/notify_url.php","http://www.cccyun.cc/return_url.php","VIP会员","0.01",out message);
            Console.WriteLine("请求结果：");
        }
    }
}
