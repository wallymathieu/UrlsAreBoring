﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace UrlsAreBoring.Core
{
    /// <summary>
    /// http://weblogs.asp.net/bradvincent/helper-class-querystring-builder-chainable
    /// A chainable query string helper class.
    /// Example usage :
    /// string strQuery = QueryString.Current.Add("id", "179").ToString();
    /// string strQuery = new QueryString().Add("id", "179").ToString();
    /// </summary>
    public class QueryString : NameValueCollection
    {
        public QueryString() { }

        public QueryString(string queryString)
        {
            FillFromString(queryString);
        }
#if false
        public static QueryString Current
        {
            get
            {
                return new QueryString().FromCurrent();
            }
        }
#endif
        /// <summary>
        /// extracts a querystring from a full URL
        /// </summary>
        /// <param name="s">the string to extract the querystring from</param>
        /// <returns>a string representing only the querystring</returns>
        public string ExtractQuerystring(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s.Contains("?"))
                    return s.Substring(s.IndexOf("?") + 1);
            }
            return s;
        }

        /// <summary>
        /// returns a querystring object based on a string
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the QueryString object </returns>
        public QueryString FillFromString(string s)
        {
            base.Clear();
            if (string.IsNullOrEmpty(s)) return this;
            foreach (string keyValuePair in ExtractQuerystring(s).Split('&'))
            {
                if (string.IsNullOrEmpty(keyValuePair)) continue;
                string[] split = keyValuePair.Split('=');
                base.Add(split[0],
                    split.Length == 2 ? split[1] : "");
            }
            return this;
        }
#if false
        /// <summary>
        /// returns a QueryString object based on the current querystring of the request
        /// </summary>
        /// <returns>the QueryString object </returns>
        public QueryString FromCurrent()
        {
            if (HttpContext.Current != null)
            {
                return FillFromString(HttpContext.Current.Request.QueryString.ToString());
            }
            base.Clear();
            return this;
        }
#endif
        /// <summary>
        /// add a name value pair to the collection
        /// </summary>
        /// <param name="name">the name</param>
        /// <param name="value">the value associated to the name</param>
        /// <returns>the QueryString object </returns>
        public new QueryString Add(string name, string value)
        {
            return Add(name, value, false);
        }

        /// <summary>
        /// adds a name value pair to the collection
        /// </summary>
        /// <param name="name">the name</param>
        /// <param name="value">the value associated to the name</param>
        /// <param name="isUnique">true if the name is unique within the querystring. This allows us to override existing values</param>
        /// <returns>the QueryString object </returns>
        public QueryString Add(string name, string value, bool isUnique)
        {
            string existingValue = base[name];
            if (string.IsNullOrEmpty(existingValue))
                base.Add(name, HttpUtility.UrlEncodeUnicode(value));
            else if (isUnique)
                base[name] = HttpUtility.UrlEncodeUnicode(value);
            else
                base[name] += "," + HttpUtility.UrlEncodeUnicode(value);
            return this;
        }

        /// <summary>
        /// removes a name value pair from the querystring collection
        /// </summary>
        /// <param name="name">name of the querystring value to remove</param>
        /// <returns>the QueryString object</returns>
        public new QueryString Remove(string name)
        {
            string existingValue = base[name];
            if (!string.IsNullOrEmpty(existingValue))
                base.Remove(name);
            return this;
        }

        /// <summary>
        /// clears the collection
        /// </summary>
        /// <returns>the QueryString object </returns>
        public QueryString Reset()
        {
            base.Clear();
            return this;
        }
#if FALSE
        /// <summary>
        /// Encrypts the keys and values of the entire querystring acc. to a key you specify
        /// </summary>
        /// <param name="key">the key to use in the encryption</param>
        /// <returns>an encrypted querystring object</returns>
        public QueryString Encrypt(string key)
        {
            QueryString qs = new QueryString();
            Utils.Cryptography.Encryption enc = new Utils.Cryptography.Encryption();
            enc.Password = key;
            for (var i = 0; i < base.Keys.Count; i++)
            {
                if (!string.IsNullOrEmpty(base.Keys[i]))
                {
                    foreach (string val in base[base.Keys[i]].Split(','))
                        qs.Add(enc.Encrypt(base.Keys[i]), enc.Encrypt(HttpUtility.UrlDecode(val)));
                }
            }
            return qs;
        }

        /// <summary>
        /// Decrypts the keys and values of the entire querystring acc. to a key you specify
        /// </summary>
        /// <param name="key">the key to use in the decryption</param>
        /// <returns>a decrypted querystring object</returns>
        public QueryString Decrypt(string key)
        {
            QueryString qs = new QueryString();
            Utils.Cryptography.Encryption enc = new Utils.Cryptography.Encryption();
            enc.Password = key;
            for (var i = 0; i < base.Keys.Count; i++)
            {
                if (!string.IsNullOrEmpty(base.Keys[i]))
                {
                    foreach (string val in base[base.Keys[i]].Split(','))
                        qs.Add(enc.Decrypt(HttpUtility.UrlDecode(base.Keys[i])), enc.Decrypt(HttpUtility.UrlDecode(val)));
                }
            }
            return qs;
        }
#endif
        /// <summary>
        /// overrides the default
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the associated decoded value for the specified name</returns>
        public new string this[string name]
        {
            get
            {
                return HttpUtility.UrlDecode(base[name]);
            }
            set
            {
                base[name] = HttpUtility.UrlEncodeUnicode(value);
            }
        }

        /// <summary>
        /// overrides the default indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns>the associated decoded value for the specified index</returns>
        public new string this[int index]
        {
            get
            {
                return HttpUtility.UrlDecode(base[index]);
            }
        }

        /// <summary>
        /// checks if a name already exists within the query string collection
        /// </summary>
        /// <param name="name">the name to check</param>
        /// <returns>a boolean if the name exists</returns>
        public bool Contains(string name)
        {
            string existingValue = base[name];
            return !string.IsNullOrEmpty(existingValue);
        }

        /// <summary>
        /// outputs the querystring object to a string
        /// </summary>
        /// <returns>the encoded querystring as it would appear in a browser</returns>
        public override string ToString()
        {
            if (base.Keys.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();

            for (var i = 0; i < base.Keys.Count; i++)
            {
                if (!string.IsNullOrEmpty(base.Keys[i]))
                {
                    if (i != 0)
                        builder.Append("&");
                    builder
                        .Append(HttpUtility.UrlEncodeUnicode(base.Keys[i])).Append("=").Append(base[base.Keys[i]]);
                }
            }
            return builder.ToString();
        }
    }
}