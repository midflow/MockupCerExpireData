using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Bogus;

namespace MookupCerExpireData
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                //prepare data
                var subjects = new[]
                {
                "CN=XXXXXX.dv2.bbswrs.aze2.cloud.geico.net, OU=API Access, O=GEICO, L=Chevy Chase, S=Maryland, C=US",
                "CN=XXXXXX.dv1.bbswrs.aze1.cloud.geico.net, OU=API Access, O=GEICO, L=Chevy Chase, S=Maryland, C=US",
                "CN=XXXXXX.dv1.bbswrs.aze1.cloud.geico.net, OU=API Access, O=GEICO, L=Chevy Chase, S=Maryland, C=US, OU=API Access, O=GEICO, L=Chevy Chase, S=Maryland, C=US",
                "CN=XXXXXX.dv2.bbswrs.aze2.cloud.geico.net, OU=API Access, O=GEICO, L=Chevy Chase, S=Maryland, C=US, OU=API Access, O=GEICO, L=Chevy Chase, S=Maryland, C=US"
                };
                var issuer = new[]
                {
                "CN=GEICO SHA256 SUB CA 01, DC=GEICO, DC=corp, DC=net",
                "CN=GEICODDC Private Primary Issuing CA, O=GEICO",
                "DC=Windows Azure CRP Certificate Generator"
                };
                var ResourceTypes = new[]
                {
                "Cloud service (classic)",
                "Virtual Machines (classic)",
                "Virtual Machines"
                };
                var Regions = new[]
                {
                "East US",
                "West US",
                "North Europe",
                "North Central US"
                };

                //Get Location of generated file
                var filesPath = System.Configuration.ConfigurationManager.AppSettings["FilesPath"];
                //Create some mock Audits   

                //1. Certificates           
                var certificate = new Faker<Certificates>()
                        .RuleFor(c => c.Subject, f => f.PickRandom(subjects))
                        .RuleFor(c => c.Issuer, f => f.PickRandom(issuer))
                        .RuleFor(c => c.NoOfDaysToExpire, f => f.Random.Number(10, 99))
                        .RuleFor(c => c.NotAfter, f => f.Date.Future().ToString("MM/dd/yyy hh:mm:ss tt"))
                        .RuleFor(c => c.SerialNumber, f => f.Random.Replace("????????????????????????????????????"))
                        .RuleFor(c => c.Thumbprint, f => f.Random.Replace("????????????????????????????????????"));

                //2. Datas
                var datas = new Faker<Data>()
                       .RuleFor(d => d.Name, f => f.Random.Replace("???-??????-??#-???-??????-###"))
                       .RuleFor(d => d.Certificates, f => certificate.Generate(2).ToList());

                //3. Resources
                var resource = new Faker<Resources>()
                    .RuleFor(r => r.ResourceType, f => f.PickRandom(ResourceTypes))
                    .RuleFor(r => r.Data, f => datas.Generate(1).ToList());

                //4. Regions
                var regions = new Faker<Region>()
                    .RuleFor(ri => ri.Name, f => f.PickRandom(Regions))
                    .RuleFor(ri => ri.Resources, f => resource.Generate(3).ToList());

                //5. AuditCreteria
                var auditCriteria = new Faker<AuditCriterias>()
                    .RuleFor(ac => ac.NoOfDaysToExpire, f => f.Random.Number(10, 99));

                //6.Audit
                var audits = new Faker<Audits>()
                    .RuleFor(a => a.AuditId, f => f.Random.Replace("####-#####-????-????"))
                    .RuleFor(a => a.SubscriptionName, f => f.Random.Replace("??-??-??-##"))
                    .RuleFor(a => a.SubscriptionId, f => f.Random.Replace("???????????????"))
                    .RuleFor(a => a.AuditTimeStamp, f => f.Date.Past().ToString("MM_dd_yyyy_hh_mm_ss"))
                    .RuleFor(a => a.AuditSubcategoryType, f => f.Random.Replace("CCCP####"))
                    .RuleFor(a => a.AuditCriteria, f => auditCriteria.Generate(1).SingleOrDefault())
                    .RuleFor(a => a.Region, f => regions.Generate(2).ToList());

                var rnd = new Random();

                List<Audits> fakeAudits;
                //number of Audits to gen
                int noAudit = 0;
                if (!String.IsNullOrEmpty(args[0]) && int.TryParse(args[0], out noAudit))
                {
                    fakeAudits = audits.Generate(noAudit);
                }
                else
                {
                    fakeAudits = audits.Generate(rnd.Next(5, 15));
                }

                //Gen Json files for each Audit
                foreach (var fakeAudit in fakeAudits)
                {
                    string json = JsonConvert.SerializeObject(fakeAudit);

                    //write Json string to file
                    System.IO.File.WriteAllText(filesPath + fakeAudit.AuditId + ".json", json);

                    Console.WriteLine(json);
                }

                Console.WriteLine("Press any key to exit!");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
    }
}
