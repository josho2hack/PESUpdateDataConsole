using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;

namespace PESUpdateData
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Offices> office;
            using (var _context = new EPESEntities())
            {
                office = _context.Offices.Where(d => d.Code != "00000000" && d.Code.Substring(5, 3) == "000").ToList();
            }
            DateTime yearForRequest;
            if (DateTime.Now.Month == 10 || DateTime.Now.Month == 11 || DateTime.Now.Month == 12)
            {
                yearForRequest = new DateTime(DateTime.Now.AddYears(1).Year, 1, 1);
            }
            else
            {
                yearForRequest = new DateTime(DateTime.Now.Year, 1, 1);
            }

            foreach (var item in office)
            {
                using (var db = new EPESEntities())
                {
                    DataForEvaluations dataForEvaluation = null;
                    string url = "";

                    if (item.Code == "00009000" || (item.Code.Substring(5, 3) == "000" && item.Code.Substring(0, 3) != "000"))
                    {
                        var m = DateTime.Now.AddMonths(-1).Month;
                        if (m == 10 || m == 11 || m == 12)
                        {
                            for (int i = 10; i <= m; i++)
                            {
                                if (item.Code == "00009000")
                                {
                                    dataForEvaluation = db.DataForEvaluations.Where(d => d.Offices.Code == "00009000" && d.Month == i && d.PointOfEvaluations.Year == yearForRequest && d.PointOfEvaluations.Name.Contains("ผลการจัดเก็บภาษี") && d.PointOfEvaluations.Unit == 0).Include(d => d.PointOfEvaluations).FirstOrDefault();
                                }
                                else
                                {
                                    dataForEvaluation = db.DataForEvaluations.Where(d => d.Offices.Code == item.Code && d.Month == i && d.PointOfEvaluations.Year == yearForRequest && d.PointOfEvaluations.Name.Contains("ผลการจัดเก็บภาษี") && d.PointOfEvaluations.Unit == 0).Include(d => d.PointOfEvaluations).FirstOrDefault();
                                }

                                if (item.Code == "00009000" || item.Code.Substring(2, 6) == "000000")
                                {
                                    url = "http://10.20.37.11:7072/serviceTier/webapi/All/officeId/00000000" + "/year/" + (yearForRequest.Year + 543).ToString("D4") + "/month/" + i.ToString("D2");
                                }
                                else
                                {
                                    url = "http://10.20.37.11:7072/serviceTier/webapi/All/officeId/" + item.Code.Substring(0, 2) + "000000" + "/year/" + (yearForRequest.Year + 543).ToString("D4") + "/month/" + i.ToString("D2");
                                }

                                var webRequest = WebRequest.Create(url) as HttpWebRequest;
                                if (webRequest == null)
                                {
                                    return;
                                }
                                webRequest.ContentType = "application/json";
                                webRequest.UserAgent = "Nothing";
                                using (var s = webRequest.GetResponse().GetResponseStream())
                                {
                                    using (var sr = new StreamReader(s))
                                    {
                                        var taxCollectionsAsJson = sr.ReadToEnd();
                                        var taxCollections = JsonConvert.DeserializeObject<Rootobject>(taxCollectionsAsJson);
                                        if (item.Code == "00009000")
                                        {
                                            var tax = taxCollections.taxCollection.FirstOrDefault(t => t.officeCode == "00000722");
                                            if (tax != null)
                                            {
                                                if (dataForEvaluation != null)
                                                {
                                                    dataForEvaluation.Expect = tax.CMCYforcast;
                                                    dataForEvaluation.Result = tax.CMcurrentYear;
                                                    dataForEvaluation.Approve = 1;
                                                    db.SaveChanges();
                                                }
                                                else
                                                {
                                                    var p = db.PointOfEvaluations.Where(poe => poe.OwnerOfficeId == item.Id && poe.Year == yearForRequest && poe.Name.Contains("ผลการจัดเก็บภาษี") && poe.Unit == 0).FirstOrDefault();

                                                    dataForEvaluation = new DataForEvaluations();
                                                    dataForEvaluation.Expect = tax.CMCYforcast;
                                                    dataForEvaluation.Result = tax.CMcurrentYear;
                                                    dataForEvaluation.Approve = 1;
                                                    dataForEvaluation.PointOfEvaluationId = p.Id;
                                                    dataForEvaluation.OfficeId = item.Id;
                                                    dataForEvaluation.Month = i;
                                                    db.DataForEvaluations.Add(dataForEvaluation);
                                                    db.SaveChanges();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var tax = taxCollections.taxCollection.FirstOrDefault(t => t.officeCode == item.Code);
                                            if (tax != null)
                                            {
                                                if (dataForEvaluation != null)
                                                {
                                                    dataForEvaluation.Expect = tax.CMCYforcast;
                                                    dataForEvaluation.Result = tax.CMcurrentYear;
                                                    dataForEvaluation.Approve = 1;
                                                    db.SaveChanges();
                                                }
                                                else
                                                {
                                                    var p = db.PointOfEvaluations.Where(poe => poe.OwnerOfficeId == item.Id && poe.Year == yearForRequest && poe.Name.Contains("ผลการจัดเก็บภาษี") && poe.Unit == 0).FirstOrDefault();

                                                    dataForEvaluation = new DataForEvaluations();
                                                    dataForEvaluation.Expect = tax.CMCYforcast;
                                                    dataForEvaluation.Result = tax.CMcurrentYear;
                                                    dataForEvaluation.Approve = 1;
                                                    dataForEvaluation.PointOfEvaluationId = p.Id;
                                                    dataForEvaluation.OfficeId = item.Id;
                                                    dataForEvaluation.Month = i;
                                                    db.DataForEvaluations.Add(dataForEvaluation);
                                                    db.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 10; i <= 12; i++)
                            {
                                if (item.Code == "00009000")
                                {
                                    dataForEvaluation = db.DataForEvaluations.Where(d => d.Offices.Code == "00009000" && d.Month == i && d.PointOfEvaluations.Year == yearForRequest && d.PointOfEvaluations.Name.Contains("ผลการจัดเก็บภาษี") && d.PointOfEvaluations.Unit == 0).Include(d => d.PointOfEvaluations).FirstOrDefault();
                                }
                                else
                                {
                                    dataForEvaluation = db.DataForEvaluations.Where(d => d.Offices.Code == item.Code && d.Month == i && d.PointOfEvaluations.Year == yearForRequest && d.PointOfEvaluations.Name.Contains("ผลการจัดเก็บภาษี") && d.PointOfEvaluations.Unit == 0).Include(d => d.PointOfEvaluations).FirstOrDefault();
                                }

                                if (item.Code == "00009000" || item.Code.Substring(2, 6) == "000000")
                                {
                                    url = "http://10.20.37.11:7072/serviceTier/webapi/All/officeId/00000000" + "/year/" + (yearForRequest.Year + 543).ToString("D4") + "/month/" + m.ToString("D2");
                                }
                                else
                                {
                                    url = "http://10.20.37.11:7072/serviceTier/webapi/All/officeId/" + item.Code.Substring(0, 2) + "000000" + "/year/" + (yearForRequest.Year + 543).ToString("D4") + "/month/" + m.ToString("D2");
                                }
                                var webRequest = WebRequest.Create(url) as HttpWebRequest;
                                if (webRequest == null)
                                {
                                    return;
                                }
                                webRequest.ContentType = "application/json";
                                webRequest.UserAgent = "Nothing";
                                using (var s = webRequest.GetResponse().GetResponseStream())
                                {
                                    using (var sr = new StreamReader(s))
                                    {
                                        var taxCollectionsAsJson = sr.ReadToEnd();
                                        var taxCollections = JsonConvert.DeserializeObject<Rootobject>(taxCollectionsAsJson);
                                        if (item.Code == "00009000")
                                        {
                                            var tax = taxCollections.taxCollection.FirstOrDefault(t => t.officeCode == "00000722");
                                            if (tax != null)
                                            {
                                                if (dataForEvaluation != null)
                                                {
                                                    dataForEvaluation.Expect = tax.CMCYforcast;
                                                    dataForEvaluation.Result = tax.CMcurrentYear;
                                                    dataForEvaluation.Approve = 1;
                                                    db.SaveChanges();
                                                }
                                                else
                                                {
                                                    var p = db.PointOfEvaluations.Where(poe => poe.OwnerOfficeId == item.Id && poe.Year == yearForRequest && poe.Name.Contains("ผลการจัดเก็บภาษี") && poe.Unit == 0).FirstOrDefault();

                                                    dataForEvaluation = new DataForEvaluations();
                                                    dataForEvaluation.Expect = tax.CMCYforcast;
                                                    dataForEvaluation.Result = tax.CMcurrentYear;
                                                    dataForEvaluation.Approve = 1;
                                                    dataForEvaluation.PointOfEvaluationId = p.Id;
                                                    dataForEvaluation.OfficeId = item.Id;
                                                    dataForEvaluation.Month = i;
                                                    db.DataForEvaluations.Add(dataForEvaluation);
                                                    db.SaveChanges();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var tax = taxCollections.taxCollection.FirstOrDefault(t => t.officeCode == item.Code);
                                            if (tax != null)
                                            {
                                                if (dataForEvaluation != null)
                                                {
                                                    dataForEvaluation.Expect = tax.CMCYforcast;
                                                    dataForEvaluation.Result = tax.CMcurrentYear;
                                                    dataForEvaluation.Approve = 1;
                                                    db.SaveChanges();
                                                }
                                                else
                                                {
                                                    var p = db.PointOfEvaluations.Where(poe => poe.OwnerOfficeId == item.Id && poe.Year == yearForRequest && poe.Name.Contains("ผลการจัดเก็บภาษี") && poe.Unit == 0).FirstOrDefault();

                                                    dataForEvaluation = new DataForEvaluations();
                                                    dataForEvaluation.Expect = tax.CMCYforcast;
                                                    dataForEvaluation.Result = tax.CMcurrentYear;
                                                    dataForEvaluation.Approve = 1;
                                                    dataForEvaluation.PointOfEvaluationId = p.Id;
                                                    dataForEvaluation.OfficeId = item.Id;
                                                    dataForEvaluation.Month = i;
                                                    db.DataForEvaluations.Add(dataForEvaluation);
                                                    db.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                }

                            }

                            for (int i = 1; i <= m; i++)
                            {
                                if (item.Code == "00009000")
                                {
                                    dataForEvaluation = db.DataForEvaluations.Where(d => d.Offices.Code == "00009000" && d.Month == i && d.PointOfEvaluations.Year == yearForRequest && d.PointOfEvaluations.Name.Contains("ผลการจัดเก็บภาษี") && d.PointOfEvaluations.Unit == 0).Include(d => d.PointOfEvaluations).FirstOrDefault();
                                }
                                else
                                {
                                    dataForEvaluation = db.DataForEvaluations.Where(d => d.Offices.Code == item.Code && d.Month == i && d.PointOfEvaluations.Year == yearForRequest && d.PointOfEvaluations.Name.Contains("ผลการจัดเก็บภาษี") && d.PointOfEvaluations.Unit == 0).Include(d => d.PointOfEvaluations).FirstOrDefault();
                                }

                                if (item.Code == "00009000" || item.Code.Substring(2, 6) == "000000")
                                {
                                    url = "http://10.20.37.11:7072/serviceTier/webapi/All/officeId/00000000" + "/year/" + (yearForRequest.Year + 543).ToString("D4") + "/month/" + m.ToString("D2");
                                }
                                else
                                {
                                    url = "http://10.20.37.11:7072/serviceTier/webapi/All/officeId/" + item.Code.Substring(0, 2) + "000000" + "/year/" + (yearForRequest.Year + 543).ToString("D4") + "/month/" + m.ToString("D2");
                                }

                                var webRequest = WebRequest.Create(url) as HttpWebRequest;
                                if (webRequest == null)
                                {
                                    return;
                                }
                                webRequest.ContentType = "application/json";
                                webRequest.UserAgent = "Nothing";
                                using (var s = webRequest.GetResponse().GetResponseStream())
                                {
                                    using (var sr = new StreamReader(s))
                                    {
                                        var taxCollectionsAsJson = sr.ReadToEnd();
                                        var taxCollections = JsonConvert.DeserializeObject<Rootobject>(taxCollectionsAsJson);
                                        if (item.Code == "00009000")
                                        {
                                            var tax = taxCollections.taxCollection.FirstOrDefault(t => t.officeCode == "00000722");
                                            if (tax != null)
                                            {
                                                if (dataForEvaluation != null)
                                                {
                                                    dataForEvaluation.Expect = tax.CMCYforcast;
                                                    dataForEvaluation.Result = tax.CMcurrentYear;
                                                    dataForEvaluation.Approve = 1;
                                                    db.SaveChanges();
                                                }
                                                else
                                                {
                                                    var p = db.PointOfEvaluations.Where(poe => poe.OwnerOfficeId == item.Id && poe.Year == yearForRequest && poe.Name.Contains("ผลการจัดเก็บภาษี") && poe.Unit == 0).FirstOrDefault();

                                                    dataForEvaluation = new DataForEvaluations();
                                                    dataForEvaluation.Expect = tax.CMCYforcast;
                                                    dataForEvaluation.Result = tax.CMcurrentYear;
                                                    dataForEvaluation.Approve = 1;
                                                    dataForEvaluation.PointOfEvaluationId = p.Id;
                                                    dataForEvaluation.OfficeId = item.Id;
                                                    dataForEvaluation.Month = i;
                                                    db.DataForEvaluations.Add(dataForEvaluation);
                                                    db.SaveChanges();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var tax = taxCollections.taxCollection.FirstOrDefault(t => t.officeCode == item.Code);
                                            if (tax != null)
                                            {
                                                if (dataForEvaluation != null)
                                                {
                                                    dataForEvaluation.Expect = tax.CMCYforcast;
                                                    dataForEvaluation.Result = tax.CMcurrentYear;
                                                    dataForEvaluation.Approve = 1;
                                                    db.SaveChanges();
                                                }
                                                else
                                                {
                                                    var p = db.PointOfEvaluations.Where(poe => poe.OwnerOfficeId == item.Id && poe.Year == yearForRequest && poe.Name.Contains("ผลการจัดเก็บภาษี") && poe.Unit == 0).FirstOrDefault();

                                                    dataForEvaluation = new DataForEvaluations();
                                                    dataForEvaluation.Expect = tax.CMCYforcast;
                                                    dataForEvaluation.Result = tax.CMcurrentYear;
                                                    dataForEvaluation.Approve = 1;
                                                    dataForEvaluation.PointOfEvaluationId = p.Id;
                                                    dataForEvaluation.OfficeId = item.Id;
                                                    dataForEvaluation.Month = i;
                                                    db.DataForEvaluations.Add(dataForEvaluation);
                                                    db.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            //}  
        }
    }
}
