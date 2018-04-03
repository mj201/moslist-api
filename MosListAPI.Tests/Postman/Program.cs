using MosListAPI.Enum;
using MosListAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Postman
{
    class Program
    {
        private static string loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        static Dictionary<string, string> CaptureFlags(string[] args){
            var flags = new Dictionary<string, string>();
            string previousFlag = null;
            foreach(string arg in args){
                if (string.IsNullOrEmpty(arg))
                    continue;
                if (!arg.StartsWith("-") && previousFlag != null){
                    flags[previousFlag] = arg;
                    continue;
                }
                flags.Add(arg, null);
                previousFlag = arg;
            }
            return flags;
        }
        static void PrintFlags(Dictionary<string, string> flags){
            foreach(var key in flags.Keys){
                Console.WriteLine("key: " + key + " value: " + flags[key]);
            }
        }
        static int GetNumberOfAds(Dictionary<string, string> flags){
            int numAds = -1;
            if (!int.TryParse(flags["-n"], out numAds) || numAds < 1){
                Console.WriteLine("Number of ads must be numeric and greater than 0");
            } 
            return numAds;
        }
        // TODO : Write help
        static void DisplayHelp(){
            Console.WriteLine("Help!!");
        }
        static object[] GenerateAds(int numAds){
            /*var properties = typeof(AdPostRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in properties){
                Console.WriteLine("property type: " + propertyInfo.PropertyType);
            }*/
            var retArray = new object[numAds];
            var rand = new Random();
            var states = Enum.GetValues(typeof(State));
            for (int i = 0; i < numAds; i++){
               retArray[i] = new {
                   Body = loremIpsum.Substring(0, rand.Next(1, loremIpsum.Length - 1)),
                   PostDate = DateTime.Now.AddDays(-1 * rand.Next(0, 1000)).ToString("yyyy-MM-ddThh:mm:ss"),
                   State = (State)states.GetValue(rand.Next(states.Length)),
                   Title = loremIpsum.Substring(0, rand.Next(1, loremIpsum.Length - 1)),
                   UserId = Guid.NewGuid(),
                   Response = new {
                       StatusCode = 200
                   }
               };
            }
            return retArray;
        }
        static void Main(string[] args)
        {
            var flags = CaptureFlags(args);
            if (!flags.ContainsKey("-n") || flags.ContainsKey("-h")){
                DisplayHelp();
                return;
            }
            var numAds = GetNumberOfAds(flags);
            if (numAds < 1)
                return;

            var ads = GenerateAds(numAds);
            var json = JsonConvert.SerializeObject(ads, Formatting.Indented);
            if (!flags.ContainsKey("-o"))
                Console.WriteLine(json);
            else
            {
                File.WriteAllText(flags["-o"], json);
            }
        }
    }
}
