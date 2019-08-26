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

            string url = "";
            var m = DateTime.Now.AddMonths(-1).Month;

            if (m == 10 || m == 11 || m == 12)
            {
                for (int i = 10; i <= m; i++)
                {
                    for (int pak = 0; pak <= 12; pak++)
                    {
                        url = "http://10.20.37.11:7072/serviceTier/webapi/All/officeId/" + pak.ToString("D2") + "000000" + "/year/" + (yearForRequest.Year + 543).ToString("D4") + "/month/" + i.ToString("D2");
                        GetTCL(url, office, yearForRequest, i);
                    }
                }
            }
            else
            {
                for (int i = 10; i <= 12; i++)
                {
                    for (int pak = 0; pak <= 12; pak++)
                    {
                        url = "http://10.20.37.11:7072/serviceTier/webapi/All/officeId/" + pak.ToString("D2") + "000000" + "/year/" + (yearForRequest.Year + 543).ToString("D4") + "/month/" + i.ToString("D2");
                        GetTCL(url, office, yearForRequest, i);
                    }
                }
                for (int i = 1; i <= m; i++)
                {
                    for (int pak = 0; pak <= 12; pak++)
                    {
                        url = "http://10.20.37.11:7072/serviceTier/webapi/All/officeId/" + pak.ToString("D2") + "000000" + "/year/" + (yearForRequest.Year + 543).ToString("D4") + "/month/" + i.ToString("D2");
                        GetTCL(url, office, yearForRequest, i);
                    }
                }
            }
        }

        private static void GetTCL(string url, List<Offices> office, DateTime yearForRequest,int i)
        {
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
                    foreach (var t in taxCollections.taxCollection)
                    {
                        if (t.officeCode != "00000000")
                        {
                            if (t.officeCode == "00000722")
                            {
                                using (var db = new EPESEntities())
                                {
                                    var dataForEvaluation = db.DataForEvaluations.Where(d => d.Offices.Code == "00009000" && d.Month == i && d.PointOfEvaluations.Year == yearForRequest && d.PointOfEvaluations.Name.Contains("ผลการจัดเก็บภาษี") && d.PointOfEvaluations.Unit == 0).Include(d => d.PointOfEvaluations).FirstOrDefault();

                                    if (dataForEvaluation != null)
                                    {
                                        dataForEvaluation.Expect = t.CMCYforcast;
                                        dataForEvaluation.Result = t.CMcurrentYear;
                                        dataForEvaluation.Approve = 1;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        dataForEvaluation = new DataForEvaluations();
                                        dataForEvaluation.Expect = t.CMCYforcast;
                                        dataForEvaluation.Result = t.CMcurrentYear;
                                        dataForEvaluation.Approve = 1;
                                        dataForEvaluation.OfficeId = office.Where(o => o.Code == "00009000").FirstOrDefault().Id;
                                        dataForEvaluation.PointOfEvaluationId = db.PointOfEvaluations.Where(poe => poe.OwnerOfficeId == dataForEvaluation.OfficeId && poe.Year == yearForRequest && poe.Name.Contains("ผลการจัดเก็บภาษี") && poe.Unit == 0).FirstOrDefault().Id;
                                        dataForEvaluation.Month = i;
                                        db.DataForEvaluations.Add(dataForEvaluation);
                                        db.SaveChanges();
                                    }
                                }
                            } // ภญ.
                            else
                            {
                                using (var db = new EPESEntities())
                                {
                                    var dataForEvaluation = db.DataForEvaluations.Where(d => d.Offices.Code == t.officeCode && d.Month == i && d.PointOfEvaluations.Year == yearForRequest && d.PointOfEvaluations.Name.Contains("ผลการจัดเก็บภาษี") && d.PointOfEvaluations.Unit == 0).Include(d => d.PointOfEvaluations).FirstOrDefault();

                                    if (dataForEvaluation != null)
                                    {
                                        dataForEvaluation.Expect = t.CMCYforcast;
                                        dataForEvaluation.Result = t.CMcurrentYear;
                                        dataForEvaluation.Approve = 1;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        dataForEvaluation = new DataForEvaluations();
                                        dataForEvaluation.Expect = t.CMCYforcast;
                                        dataForEvaluation.Result = t.CMcurrentYear;
                                        dataForEvaluation.Approve = 1;
                                        dataForEvaluation.OfficeId = office.Where(o => o.Code == t.officeCode).FirstOrDefault().Id;
                                        dataForEvaluation.PointOfEvaluationId = db.PointOfEvaluations.Where(poe => poe.OwnerOfficeId == dataForEvaluation.OfficeId && poe.Year == yearForRequest && poe.Name.Contains("ผลการจัดเก็บภาษี") && poe.Unit == 0).FirstOrDefault().Id;
                                        dataForEvaluation.Month = i;
                                        db.DataForEvaluations.Add(dataForEvaluation);
                                        db.SaveChanges();
                                    }
                                }
                            } // สภ.
                        }
                    }
                }
            }
        } //End getTCL
    }
}
