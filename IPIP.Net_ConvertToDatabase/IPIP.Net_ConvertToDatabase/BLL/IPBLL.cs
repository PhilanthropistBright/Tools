using IPIP.Net_ConvertToDatabase.Commom.Classes;
using IPIP.Net_ConvertToDatabase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IPIP.Net_ConvertToDatabase.BLL
{
    public class IPBLL
    {
        testEntities db = new testEntities();




        public bool Add(string district, string provinces, string city, string country, IPAddress ipaddress)
        {
            try
            {
                //添加IP的时候要做判定，数据库中是否存在跟当前地址相同IP区域是在临近的IP段，如果存在将不添加而转为修改
                IPHelper iPHelper = new IPHelper();
                IPAddress tempIP = iPHelper.IPAdd(ipaddress, 256);

                if(db.ip_addresstemp.Count()>0)
                {
                    string ip = tempIP.ToString();

                    if(db.ip_addresstemp.Where(s => s.ip3 == ip && s.address == country + city + provinces + district).Count()>0)
                    {
                        //查看邻近的IP地址是否所属地相同
                        ip_addresstemp temp = db.ip_addresstemp.Single(s => s.ip3 == ip && s.address == country + city + provinces + district);
                        if (temp != null)
                        {
                            temp.ip3 = ipaddress.ToString();
                            temp.ip1 = IPHelper.IpToInt(ipaddress.ToString());
                            db.SaveChanges();
                            return true;
                        }
                    }
                    

                    tempIP = iPHelper.IPDelete(ipaddress, 1);
                    ip = tempIP.ToString();
                    if (db.ip_addresstemp.Where(s => s.ip4 == ip && s.address == country + city + provinces + district).Count()>0)
                    {
                        ip_addresstemp temp = db.ip_addresstemp.Single(s => s.ip4 == ip && s.address == country + city + provinces + district);
                        if (temp != null)
                        {
                            temp.ip4 = iPHelper.IPAdd(ipaddress, 255).ToString();
                            temp.ip2 = IPHelper.IpToInt(iPHelper.IPAdd(ipaddress, 255).ToString());
                            db.SaveChanges();
                            return true;
                        }
                    }
                    
                }


                db.ip_addresstemp.Add(new ip_addresstemp()
                {
                    city = city,
                    country = country,
                    provinces = provinces,
                    district = district,
                    address = country + city + provinces + district,
                    ip1 = IPHelper.IpToInt(ipaddress.ToString()),
                    ip2 = IPHelper.IpToInt(iPHelper.IPAdd(ipaddress, 255).ToString()),
                    ip3 = ipaddress.ToString(),
                    ip4 = iPHelper.IPAdd(ipaddress, 255).ToString()
                });
                db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
