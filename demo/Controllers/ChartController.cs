using demo.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace demo.Controllers
{
    public class ChartController : Controller
    {
        public ActionResult Chart(string chart)
        {
			shopEntities db = new shopEntities();
			
			if (chart == "prochart")
			{
				List<DataPoint> dataPoints = new List<DataPoint>();
				var pros = db.Products;
                int JL = 0;
				int RL = 0;
				int AP = 0;
				int D = 0;
				int CS = 0;
				int DW = 0;
				foreach (var pro in pros)
				{
					if (pro.Brand == "JL") JL += int.Parse(pro.Quantity.ToString());
					else if (pro.Brand=="RL") RL += int.Parse(pro.Quantity.ToString());
					else if(pro.Brand=="AP") AP += int.Parse(pro.Quantity.ToString());
					else if(pro.Brand=="D") D += int.Parse(pro.Quantity.ToString());
					else if(pro.Brand=="CS") CS += int.Parse(pro.Quantity.ToString());
					else DW += int.Parse(pro.Quantity.ToString());
				}
				int total = JL + RL + AP + D + CS;

				float jl = (float.Parse(JL.ToString()) / total )*100;
				float rl = (float.Parse(RL.ToString()) / total) * 100;
				float ap = (float.Parse(AP.ToString()) / total) * 100;
				float d = (float.Parse(D.ToString()) / total) * 100;
				float cs = (float.Parse(CS.ToString()) / total) * 100;
				float dw = (float.Parse(DW.ToString()) / total) * 100;
				dataPoints.Add(new DataPoint("Jacques Lemans", jl));
				dataPoints.Add(new DataPoint("Rolex", rl));
				dataPoints.Add(new DataPoint("Apple", ap));
				dataPoints.Add(new DataPoint("Dior", d));
				dataPoints.Add(new DataPoint("Casio", cs));
				dataPoints.Add(new DataPoint("Daniel Wellington", dw));

				ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
				ViewBag.Message = chart;
			}
			else if(chart=="revenue")
            {
				List<DataPointLine> dataPoints = new List<DataPointLine>();
				var order = db.Orders.Where(o => o.status == "Finish").OrderBy(o=>o.Date_delivery).ToList();
				foreach(var o in order)
                {
					double price = 0;
					var detail = o.OrderDetails.ToList();
					foreach(var d in detail)
                    {
						price += d.Price.Value * d.Quantity.Value;
                    }
					var time = o.Date_delivery.Value.ToString("yyyyMMddHHmmssffff");
					dataPoints.Add(new DataPointLine(double.Parse(time.ToString()), price));
				}
				ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
				ViewBag.Message = chart;
			}			
			else if(chart=="topcustomers")
			{
				List<DataPoint> dataPoints = new List<DataPoint>();
				List<User> users = new List<User>();
				List<double> prices = new List<double>();
				var user = db.Users.Where(u=>u.isAdmin==false).ToList();
				var orders = db.Orders.ToList();
				foreach(var u in user)
                {
					double price = 0;
					foreach(var o in orders)
                    {
                        if (o.ID_Customer == u.Username)
                        {
							var detail = o.OrderDetails.ToList();
							foreach(var d in detail)
                            {
								price += d.Price.Value * d.Quantity.Value;
							}								
                        }
                    }
					if(prices.Count<3)
                    {
						prices.Add(price);
						users.Add(u);
                    }
                    else
                    {
						double min = prices[0];
						for(int i=1;i<3;i++)
                        {
							if (prices[i] < min)
								min = prices[i];
                        }
						for(int i=0;i<3;i++)
                        {
							if(price>min && prices[i]==min)
                            {
								prices[i] = price;
								users[i] = u;
								break;
                            }								
                        }							
						
                    }
                }
				for(int i=0;i<3;i++)
                {
					for(int j=i+1;j<3;j++)
                    {
						if(prices[i]<prices[j])
                        {
							var tmp = prices[i];
							prices[i] = prices[j];
							prices[j] = tmp;

							var utmp = users[i];
							users[i] = users[j];
							users[j] = utmp;
                        }							
                    }						
                }					
				for(int i=0;i<3;i++)
                {
					dataPoints.Add(new DataPoint(users[i].Name.ToString(), prices[i]));
				}					
				ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
				ViewBag.Message = chart;
			}
			else if(chart=="topproducts")
            {
				List<DataPoint> dataPoints = new List<DataPoint>();
				var pros = db.Products.ToList();
				List<Product> products = new List<Product>();
				foreach(var p in pros)
                {
					int tmp = 0;
					if (products.Count < 20)
						products.Add(p);
					else
					{
						int mintop = int.Parse(products[0].TopHot.ToString());
						for(int i=0;i< products.Count;i++)
                        {
							if (int.Parse(products[i].TopHot.ToString()) < mintop)
							{
								mintop = int.Parse(products[i].TopHot.ToString());
								tmp = i;
							}
                        }
						if (int.Parse(p.TopHot.ToString()) > mintop)
							products[tmp] = p;
					}						
                }
				for (int i = 0; i < products.Count; i++)
				{
					for (int j = i + 1; j < products.Count; j++)
					{
						if (int.Parse(products[i].TopHot.ToString()) < int.Parse(products[j].TopHot.ToString()))
						{
							var tmp = products[i];
							products[i] = products[j];
							products[j] = tmp;
						}
					}
				}
				for (int i = 0; i < products.Count; i++)
				{
					dataPoints.Add(new DataPoint(products[i].Name.ToString(), int.Parse(products[i].TopHot.ToString())));
				}
				ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
				ViewBag.Message = chart;
			}				 
			return View();
		}
    }
}