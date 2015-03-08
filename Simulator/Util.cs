using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Atmosphere.BattleSimulator
{
    static class Util
    {
        public static List<T> Filter<T>(List<T> s, Predicate<T> p)
        {
            List<T> t = new List<T>();
            foreach (T a in s) if (p(a)) t.Add(a);
            return t;
        }

        /**<summary>Generic higher-order function for filtering out records from a dictionary
         *      based on given Predicate.</summary>
         * <returns>A new Dictionary&lt;string, T&gt; that contains only those Key/Value pairs
         *      from s for which p(Value) holds true</returns>
         */
        public static Dictionary<U, T> Filter<U, T>(Dictionary<U, T> s, Predicate<T> p)
        {
            Dictionary<U, T> t = new Dictionary<U, T>();

            foreach (KeyValuePair<U, T> a in s)
            {
                if (p(a.Value)) t.Add(a.Key, a.Value);
            }

            return t;
        }

        /**<summary>Method for "uniquing" a list of strings.</summary>
         * <param name="s">The list of strings to unique</param>
         * <param name="sort">Whether or not the list needs to be sorted. If you are passing
         *      a non-sorted list, set this to TRUE. If the list is already sorted, pass FALSE.</param>
         * <returns>A new list of only those strings from s which are unique.</returns>
         */
        public static List<string> Unique(List<string> s, bool sort)
        {
            if (sort)
            {
                s.Sort(String.Compare);
            }

            List<string> t = new List<string>();
            string val_hold = "";

            for (int i = 0; i < s.Count; i++)
            {
                if (val_hold.CompareTo(s[i]) != 0)
                    t.Add(s[i]);
                val_hold = s[i];
            }

            return t;
        }

        /**<summary>Generic method for filtering out records from a dictionary using a list of 
         *      acceptable keys. MUCH faster than the generic Filter&lt;T&gt;(Dictionary&lt;U,T&gt;) method.</summary>
         * <param name="s">This is the dictionary to be filtered.</param>
         * <param name="good">This is the list of keys.</param>
         * <remarks>For this method to work, good MUST be a strict subset of s.</remarks>
         * <returns>A new Dictionary with only the elements of s whose keys are in good.</returns>
         */
        public static Dictionary<string, T> FilterWithKeyList<T>(Dictionary<string, T> s, List<string> good)
        {
            List<string> all = new List<string>();
            Dictionary<string, T> t = new Dictionary<string, T>();

            foreach (string key in s.Keys)
            {
                all.Add(key);
            }

            good.Sort(string.Compare);
            all.Sort(string.Compare);

            int a = 0, g = 0;

            while (g < good.Count)
            {
                int test = (all[a].CompareTo(good[g]));
                if (test == 0)
                {
                    t.Add(good[g], s[good[g]]);
                    g++;
                    a++;
                }
                else if (test < 0)
                { // all[a] < good[g]
                    a++;
                }
                // all[a] can never be > good[g] 
            }

            // We've passed by all the goods, so we're done.

            return t;
        }


        public static XmlDocument GetXmlFromResource(string id)
        {
            XmlDocument doc = null;

            using (Stream resource = Assembly.GetCallingAssembly().GetManifestResourceStream(id))
            {
                using (StreamReader reader = new StreamReader(resource))
                {
                    doc = new XmlDocument();
                    doc.LoadXml(reader.ReadToEnd());
                }
            }

            return doc;
        }
        
        public static StreamReader GetStreamReaderFromResource(string id)
        {       
            Stream resource = Assembly.GetCallingAssembly().GetManifestResourceStream(id);

            return new StreamReader(resource);
        }
        
        public static string GetTextFromResource(string id)
        {
            string text = null;
            
            using (Stream resource = Assembly.GetCallingAssembly().GetManifestResourceStream(id))
            {
                using (StreamReader reader = new StreamReader(resource))
                {
                    text = reader.ReadToEnd();
                }
            }
            
            return text;
        }
    }
}
