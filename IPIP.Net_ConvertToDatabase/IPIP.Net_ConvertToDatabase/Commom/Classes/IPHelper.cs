using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IPIP.Net_ConvertToDatabase.Commom.Classes
{
    public class IPHelper
    {
        public IPAddress IPAdd(IPAddress ip, int addnum)
        {
            byte[] ipbytes = ip.GetAddressBytes();

            int uppernum = -1;
            int overnum = -1;
            
            //从小到达遍历IP段  来获得增加后的IP
            for(int x=3;x>=0;x--)
            {
                if(x ==3)
                {
                    byte b = ipbytes[x];

                    overnum = (Convert.ToInt32(b) + addnum) % 256;
                    uppernum = (Convert.ToInt32(b) + addnum) / 256;

                    ipbytes[x] = Convert.ToByte(overnum);

                    overnum = -1;
                }
                else
                {
                    byte b = ipbytes[x];
                    overnum = (Convert.ToInt32(b) + uppernum) % 256;
                    uppernum = (Convert.ToInt32(b) + uppernum) / 256;
                    ipbytes[x] = Convert.ToByte(overnum);
                    overnum = -1;
                }
            }

            return new IPAddress(ipbytes);
        }

        public IPAddress IPDelete(IPAddress ip, int deletenum)
        {
            byte[] ipbytes = ip.GetAddressBytes();

            int uppernum = -1;
            int overnum = -1;

            //从小到达遍历IP段  来获得增加后的IP
            for (int x = 3; x >= 0; x--)
            {
                if (x == 3)
                {
                    byte b = ipbytes[x];

                    if((Convert.ToInt32(b) - deletenum)>0)
                    {
                        overnum = (Convert.ToInt32(b) - deletenum) % 256;
                    }
                    else
                    {
                        overnum = (256 - (Math.Abs(Convert.ToInt32(b) - deletenum) % 256)) %256;
                    }

                    if ((Convert.ToInt32(b) - deletenum) >= 0)
                    {
                        uppernum = 0;
                    }
                    else
                    {
                        uppernum = (Math.Abs(Convert.ToInt32(b) - deletenum) - 1) / 256 + 1;
                    }


                    ipbytes[x] = Convert.ToByte(overnum);
                    overnum = -1;
                }
                else
                {
                    byte b = ipbytes[x];

                    if ((Convert.ToInt32(b) - uppernum) > 0)
                    {
                        overnum = (Convert.ToInt32(b) - uppernum) % 256;
                    }
                    else
                    {
                        overnum = (256 - (Math.Abs(Convert.ToInt32(b) - uppernum) % 256)) % 256;
                    }

                    if ((Convert.ToInt32(b) - uppernum) >= 0)
                    {
                        uppernum = 0;
                    }
                    else
                    {
                        uppernum = (Math.Abs(Convert.ToInt32(b) - uppernum) - 1) / 256 + 1;
                    }

                    ipbytes[x] = Convert.ToByte(overnum);
                    overnum = -1;
                }
            }

            return new IPAddress(ipbytes);
        }

        public static long IpToInt(string ip)
        {
            char[] separator = new char[] { '.' };
            string[] items = ip.Split(separator);
            return long.Parse(items[0]) << 24
                    | long.Parse(items[1]) << 16
                    | long.Parse(items[2]) << 8
                    | long.Parse(items[3]);
        }


        public static string IntToIp(long ipInt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((ipInt >> 24) & 0xFF).Append(".");
            sb.Append((ipInt >> 16) & 0xFF).Append(".");
            sb.Append((ipInt >> 8) & 0xFF).Append(".");
            sb.Append(ipInt & 0xFF);
            return sb.ToString();
        }
    }
}
