using DnsClient;
using System.Text;

public class MainClass{
    public static void Main(String[] argv){

        // Initial user prompt
        Console.WriteLine("Enter domain name (exit to close, multiple domains seperated by a space, or a file by entering file):");
        string domains = Console.ReadLine();

        // Loops until exit is called
        while (domains != "exit")
        {

            string[] domArr;

            // Checks if file was entered
            if (domains == "file")
            {

                // Enter filename, then parses file into space separated string
                Console.WriteLine("Enter file name:");
                string fileName = Console.ReadLine();
                var temp = new StringBuilder();
                temp.Append(File.ReadAllText(fileName));
                temp.Replace(System.Environment.NewLine, " ");
                domains = temp.ToString();
            }

            // Turns space seperated string into an array of domains
            domArr = domains.Split(' ');
            
            // Creates dns lookup object, enables caching
            var client = new LookupClient();
            client.UseCache = true;

            // Creates threads so that domains are run concurrently
            Parallel.ForEach(domArr, domain =>
            {

                // Queries for the MX record
                var result = client.Query(domain, QueryType.MX);

                // Takes each Exchange domain from the record and produces an IP
                foreach (var MxRecord in result.Answers.MxRecords())
                {

                    // Prints the exchange name to show multithreading
                    Console.Write("{0}: ", MxRecord.Exchange);
                    var result2 = client.Query(MxRecord.Exchange.ToString(), QueryType.A);
                    foreach (var ARecord in result2.Answers.ARecords())
                    {

                        // Prints the resolved IP
                        Console.WriteLine(ARecord.Address);
                    }
                }
            });

            // Continues the loop
            domains = Console.ReadLine();
        }
    }
 }