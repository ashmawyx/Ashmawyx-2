using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace AshmawyX
{
    public static class ProfileManager
    {
        private static string folder = Application.StartupPath + "\\Profiles";

        static ProfileManager()
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        public static void SaveProfile(Profile profile)
        {
            string path = folder + "\\" + profile.Name + ".json";

            string json =
                "{\n" +
                $"  \"Name\": \"{profile.Name}\",\n" +
                "  \"MirroredKeys\": [\n" +
                BuildKeyArray(profile.MirroredKeys) +
                "  ],\n" +
                $"  \"DelayFrom\": {profile.DelayFrom},\n" +
                $"  \"DelayTo\": {profile.DelayTo}\n" +
                "}";

            File.WriteAllText(path, json);
        }

        private static string BuildKeyArray(List<string> keys)
        {
            string result = "";
            for (int i = 0; i < keys.Count; i++)
            {
                result += $"    \"{keys[i]}\"";
                if (i < keys.Count - 1)
                    result += ",";
                result += "\n";
            }
            return result;
        }

        public static Profile LoadProfile(string name)
        {
            string path = folder + "\\" + name + ".json";
            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path);

            Profile p = new Profile();
            p.Name = name;

            // Extract mirrored keys
            int mkStart = json.IndexOf("[");
            int mkEnd = json.IndexOf("]");
            if (mkStart != -1 && mkEnd != -1)
            {
                string mkRaw = json.Substring(mkStart + 1, mkEnd - mkStart - 1);
                string[] lines = mkRaw.Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    string cleaned = line.Trim().Replace("\"", "");
                    if (cleaned.Length > 0)
                        p.MirroredKeys.Add(cleaned);
                }
            }

            // Extract DelayFrom
            p.DelayFrom = ExtractNumber(json, "\"DelayFrom\":");

            // Extract DelayTo
            p.DelayTo = ExtractNumber(json, "\"DelayTo\":");

            return p;
        }

        private static double ExtractNumber(string json, string key)
        {
            int idx = json.IndexOf(key);
            if (idx == -1)
                return 0;

            idx += key.Length;
            string rest = json.Substring(idx).Trim();

            string number = "";
            foreach (char c in rest)
            {
                if ((c >= '0' && c <= '9') || c == '.' || c == '-')
                    number += c;
                else
                    break;
            }

            double result;
            double.TryParse(number, out result);
            return result;
        }

        public static void DeleteProfile(string name)
        {
            string path = folder + "\\" + name + ".json";
            if (File.Exists(path))
                File.Delete(path);
        }

        public static List<string> GetProfiles()
        {
            List<string> list = new List<string>();

            foreach (string file in Directory.GetFiles(folder, "*.json"))
                list.Add(Path.GetFileNameWithoutExtension(file));

            return list;
        }
    }
}
