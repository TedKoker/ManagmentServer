using MoneySystemServer.Contacts.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySystemServer.Models
{
    public class OrderLinqList
    {
        public UserMoneyDetaleResponse UserMoneyDetaleResponse { get; set; }
        public OrderLinqList FatherNode { get; set; }
        public OrderLinqList SonNode { get; set; }
        public OrderLinqList GrandFather { get; set; }
        public OrderLinqList GrandSon { get; set; }

        public OrderLinqList()
        {

        }

        public OrderLinqList (OrderLinqList fatherNode, OrderLinqList childNode)
        {
            FatherNode = fatherNode;
            SonNode = childNode;
            GrandFather = fatherNode!=null ? fatherNode.FatherNode : null;
            GrandSon = childNode!=null ? childNode.SonNode : null;
            if(childNode!=null)
            {
                childNode.ChangeFather(this);
            }
            if (fatherNode != null)
            {
                fatherNode.ChangeSon(this);
            }
        }

        public void ChangeSon(OrderLinqList newSon)
        {
            SonNode = newSon;
            GrandSon = newSon.SonNode;
        }

        public void ChangeFather(OrderLinqList newFather)
        {
            FatherNode = newFather;
            GrandFather = newFather.FatherNode;
        }
    }
}
