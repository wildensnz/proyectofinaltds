using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InventarioHerramienta
{
    public class Generic
    {
        public static bool generateLog(string texto, string file)
        {
            if (!File.Exists(file))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(file))
                {
                    sw.WriteLine(texto);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(file))
                {
                    sw.WriteLine(texto);
                }
            }
            return true;
        }
        public static string GetPath(HttpRequest request)
        {
            string Url = string.Format("{0}://{1}", request.Scheme, request.Host);
            if (!string.IsNullOrEmpty(request.PathBase))
            {
                Url += request.PathBase + "/";
            }
            else
            {
                Url = string.Format("{0}://{1}/", request.Scheme, request.Host);
            }
            return Url;
        }

        public static string GetValueClaim(HttpContext currentUser, string tipo)
        {
            try
            {
                if (currentUser.User.HasClaim(c => c.Type == tipo))
                {
                    return currentUser.User.Claims.FirstOrDefault(c => c.Type == tipo)!.Value.ToString();
                }
            }
            catch
            {

            }
            return "";
        }

        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
