using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace AppSheetProject
{
    class Program
    {
        // declare static constants
        static readonly string LISTREQUESTURL = "https://appsheettest1.azurewebsites.net/sample/list";
        static readonly string DETAILREQUESTURL = "https://appsheettest1.azurewebsites.net/sample/detail/";
        static readonly int MAXLISTCOUNT = 5;
        

        static void Main(string[] args)
        {
            List<Person> answer = getYoungest();
            if (answer == null)
            {
                Console.WriteLine("Unable to get any queryable IDs from the list method");
            }
            else
            {
                foreach (Person p in answer)
                {
                    Console.WriteLine(p);
                }
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// The method used to call for the details of each person who's id 
        /// we got from the list API call
        /// </summary>
        /// <param name="id">the id of the person being retrieved</param>
        /// <returns>a person object populated with all the data for the person</returns>
        private static Person getPerson(int id)
        {
            try
            {
                Person person;
                string requestURL = DETAILREQUESTURL + id;
                WebRequest request = WebRequest.Create(requestURL);
                WebResponse response = request.GetResponse();
                if (((HttpWebResponse)response).StatusDescription != "OK")
                {
                    response.Close();
                    return null;
                }
                using (Stream datastream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(datastream);
                    string responseData = reader.ReadToEnd();
                    person = new JavaScriptSerializer().Deserialize<Person>(responseData);
                }
                response.Close();
                return person;
            }
            catch (System.Net.WebException e)
            {
                //Console.WriteLine("Caught Web exception: " + e);
                return null;
            }

        }

        /// <summary>
        /// The main method that runs through the lists of ids and gets 
        /// up to MAXLISTCOUNT of the youngest entries
        /// 
        /// </summary>
        /// <returns>A list of the MAXLISTCOUNT youngest people</returns>
        private static List<Person> getYoungest()
        {
            string token = null;
            Person person = null;
            ListResponse listResponse = null;
            string requestURL = LISTREQUESTURL;
            List<Person> sortedList = new List<Person>();
            do
            {
                WebRequest request = WebRequest.Create(requestURL);
                WebResponse response = request.GetResponse();
                if (((HttpWebResponse)response).StatusDescription != "OK")
                    return null;

                using (Stream datastream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(datastream);
                    string responseData = reader.ReadToEnd();
                    listResponse = new JavaScriptSerializer().Deserialize<ListResponse>(responseData);
                    foreach(int i in listResponse.result)
                    {
                        person = getPerson(i);
                        if (person == null)
                        {
                            continue;
                        }
                        sortedList.Add( person);
                        sortedList.Sort((x, y) => x.age.CompareTo(y.age));
                        if (sortedList.Count > MAXLISTCOUNT)
                        {
                            sortedList.RemoveAt(MAXLISTCOUNT);
                        }
                    }
                    token = listResponse.token;
                    requestURL = LISTREQUESTURL + "?token=" + token;
                }
                response.Close();

            } while (token != null);
            

            return sortedList;
        }
    }
}
