using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.CSV;
using InputLog.Core.IO;
using InputLog.Core.Analyses.Copytask;
using System.Globalization;


namespace CorpusManager
{
    public static class CorpusWriter
    {
        public static void Process(string corpus_path, string filename, bool anonimize=true) 
        {
            // Create some directories (if they do not exist yet)

            var idfx_path = corpus_path + "idfx/";
            var xml_path = corpus_path + "xml/";
            var bigram_path = corpus_path + "bigrams/";
            var data_path = corpus_path + "data/";

            Directory.CreateDirectory(idfx_path);
            Directory.CreateDirectory(xml_path);
            Directory.CreateDirectory(bigram_path);
            Directory.CreateDirectory(data_path);

            var filename_no_ext = Path.GetFileNameWithoutExtension(filename);
            var ID = Directory.GetFiles(idfx_path).Length + 1;

            // Copy the IDFX to it's new location
            var new_idfx_location = idfx_path + ID + ".idfx";
            File.Copy(filename, new_idfx_location);
			Console.WriteLine("Hello World!");
            Console.WriteLine(filename);

            // Open SessionID & Events.
            var reader = EventLogFactory.CreateFileEventLogReader(new_idfx_location, LogFormat.XML);
            var sessionID = reader.ReadSessionIdentification();
            var evnts = reader.ReadEvents();

            sessionID.SessionInfo["Gender"] = CorpusWriter.TranslateGender(sessionID);
            if (anonimize)
                sessionID.SessionInfo["Participant"] = CorpusWriter.CalculateUUID(sessionID.SessionInfo["Participant"], sessionID.SessionInfo["Age"].ToString(), sessionID.SessionInfo["Gender"]);
            sessionID.MetaInfo["__LogFileName"] = ID + ".idfx"; // Anonimise the LogFileName
            var questions = evnts.FirstOrDefault(s => s.Type == "questions");

            if (questions is null)
            {
                questions = new InputLog.Core.Events.Event();
                questions.Type = "questions";
                var question_part = new InputLog.Core.Events.WinLog.Questions();
                question_part.Computer = "";
                question_part.Keyboard = "";
                question_part.Browser = "";
                question_part.Education = "";
                question_part.Handedness = "";
                question_part.Language = "";
                questions.Parts.Add(question_part);
                evnts.Add(questions);
            }

            if (!(questions is null))
            {
                var questions_part = questions.Parts[0] as InputLog.Core.Events.WinLog.Questions;
                questions_part.Computer = CorpusWriter.TranslateComputer(questions_part.Computer);
                questions_part.Keyboard = CorpusWriter.TranslateFamiliarity(questions_part.Keyboard);
                questions_part.Browser = CorpusWriter.TranslateBrowser(questions_part.Browser);
                questions_part.Education = CorpusWriter.TranslateEducation(questions_part.Education);
                questions.Parts[0] = questions_part;
            }

            // Write updated IDFX.
            var evnt_writer = EventLogFactory.CreateFileEventLogWriter(new_idfx_location, LogFormat.XML);
            evnt_writer.WriteExistingFile(sessionID, evnts);

            // Get the CopyTask Analysis Summary (Do the actual analysis)
            var analysis = new CopytaskAnalysis("CT", evnts, sessionID);
            CopytaskAnalysisSummary summary = analysis.DoAnalysis() as CopytaskAnalysisSummary;

            // Save the analysis files.
            var analysis_writer = new CopytaskAnalysisXMLWriter(xml_path + ID.ToString() + ".xml");
            analysis_writer.WriteDocument(sessionID, null, summary);
            analysis_writer.Dispose();
            Directory.Delete(xml_path + "Images", true); // Remove useless directories
            Directory.Delete(xml_path + "Style", true);

            // Save the bigram files.
            ICSVLineGetter rawBigrams = new CSVLineGetter(summary.RawBigrams.AsReadOnly());
            ICSVLineGetter sessionInfo = new CSVRepeatGetter(sessionID);
            var bigram_writer = new CSVMergeWriter(new ICSVLineGetter[] { rawBigrams, sessionInfo });
            bigram_writer.WriteToFile(bigram_path + ID.ToString() + "_bigram.csv");

            // Write to the corpus CSV's.
            CorpusWriter.Write(data_path, summary, sessionID, ID);
        }

        public static void Write(string corpus_path, CopytaskAnalysisSummary summary, SessionIdentification sessionID, int ID)
        {
            var i = 0;

            var component_folder = Path.Combine(corpus_path, "components");
            var trial_folder = Path.Combine(corpus_path, "trials");
            
            Directory.CreateDirectory(component_folder);
            Directory.CreateDirectory(trial_folder);
            
            var correctness = Math.Round(100 * summary.CorrectnessStatistics["Overall"].AggregatedCorrectness, 1);
            var cpm = Convert.ToInt32(summary.GroupData["InterKey Intervals (IKI)"]["Targeted Bigrams"].CPM);
            WriteSession(corpus_path, sessionID, correctness, cpm);

            // Write the different groups
            foreach (var pair in summary.GroupData)
            {
                if (pair.Key.Contains("InterKey Intervals") || pair.Key.Contains(">") || pair.Key.Contains("10%"))  // These are not saved in the database.
                    continue;
                i += 1;
                if (pair.Key == "Overall")
                {
                    var names = new String[] { "tapping", "sentence", "words1", "words2", "words3", "words4", "consonants" };
                    var j = 0;
                    foreach (var title in pair.Value.Values)
                    {
                        var file_path = Path.Combine(component_folder, names[j] + ".csv");
                        WriteComponent(pair.Value[title], file_path, ID);
                        j++;
                    }
                    if (j < 7)
                        Console.WriteLine("This IDFX file might be malformatted!");
                }
                else if (i <= 7 && i >= 4)  // Trials
                {
                    var names = new String[] { "words1", "words2", "words3", "words4" };
                    var file_path = Path.Combine(trial_folder, names[i - 4]);
                    Directory.CreateDirectory(file_path);
                    WriteTrials(pair.Value, file_path, ID);
                }
                else if (i >= 9)  // Other entries such as Hand combinations & Frequencies
                {
                    var dir_name = pair.Key.ToLower();
                    if (dir_name.Contains("hand"))
                        dir_name = "hands";
                    var directory = Path.Combine(corpus_path, dir_name);
                    Directory.CreateDirectory(directory);
                    if (pair.Key == "Frequency")
                    {
                        WriteMisc(pair.Value, directory, new string[] { "high", "low" }, ID);
                    }
                    else
                    {
                        var names = pair.Value.Values.Select(s => s.ToLower()).ToArray();
                        WriteMisc(pair.Value, directory, names, ID);
                    }
                }
            }
            foreach (var name in new string[] { "words1", "words2", "words3", "words4" })
            {
                if (File.Exists(Path.Combine(trial_folder, name, "overall.csv")))
                    File.Delete(Path.Combine(trial_folder, name, "overall.csv"));

                File.Copy(Path.Combine(component_folder, name + ".csv"), Path.Combine(trial_folder, name, "overall.csv"));
            }

        }

        public static string CalculateUUID(string name, string age, string gender)
        {
        /*
         Calculate the UUID of a user (Unique User IDentifier) by taking a hash of the name, age and gender.
         This has the drawback that if two people have the same name, age, gender that they will be matched.
         Furthermore, if a user does the task at multiple times at different ages, they will also have a different UUID.
         */
            string text = name + age + gender;

            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var sha = new SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty).Substring(0, 16);
            }
        }

        private static void WriteMisc(CopytaskAnalysisSummary.GroupStatistics stats, string path, string[] names, int ID)
        {
            var i = 0;
            foreach (var title in stats.Values)
            {
                var file_path = Path.Combine(path, names[i] + ".csv");
                WriteComponent(stats[title], file_path, ID);
                i += 1;
            }
        }

        private static void WriteTrials(CopytaskAnalysisSummary.GroupStatistics stats, string path, int ID)
        {
            var i = 0;
            foreach (var title in stats.Values)
            {
                var file_path = Path.Combine(path, "trial" + (i + 1).ToString() + ".csv");
                WriteComponent(stats[title], file_path, ID);
                i += 1;
            }
            if (i < 7 || i > 8)
                Console.WriteLine("This IDFX file might be malformatted!");
        }

        private static void WriteComponent(StatisticsAccumulator stats, string path, int ID)
        {
            var keys = new string[] { "id", "targetted", "non_targetted", "cpm", "median", "stdev", "logmean", "mean_iki" };

            var values = new string[] { ID.ToString(), stats.CountTargetted.ToString(), stats.CountNotTargetted.ToString(), UnNaN(stats.CPM), stats.Median.ToString(), UnNaN(stats.StdDev, 1), UnNaN(Math.Exp(stats.LogMean), 1).ToString(), UnNaN(stats.Mean, 1) };
            WriteCSV(path, keys, values);
        }

        private static string UnNaN(double d, int round = 0)
        {
            if (Double.IsNaN(d))
                return "";
            else
                return Math.Round(d, round).ToString();
        }

        // Escape Session & test_group --> can contain comma's and will mess with the CSV
        private static string Get(Dictionary<string, string> dict, String key, String def = "", bool escape = true)
        {
            if (dict.ContainsKey(key))
            {
                if (escape)
                    return Escape(dict[key]);
                else
                    return dict[key];
            }
            else
                return def;
        }

        public static string TranslateFamiliarity(string s)
        {
            var familiarity = s.Trim();
            if (familiarity == "very familiar" || familiarity == "yn gyfarwydd iawn" || familiarity == "sehr vertraut" || familiarity == "muy familiar" || familiarity == "très familier" || familiarity == "una tastiera che conosco molto bene" || familiarity == "zeer vertrouwd" || familiarity == "godt kjent" || familiarity == "bardzo znanej" || familiarity == "muito familiar" || familiarity == "çok tanıdık" || familiarity == "zeer bekend")
                return "100";

            if (familiarity == "familiar" || familiarity == "gyfarwydd" || familiarity == "vertraut" || familiarity == "familiar" || familiarity == "familier" || familiarity == "una tastiera che conosco più o meno bene" || familiarity == "vertrouwd" || familiarity == "kjent" || familiarity == "znanej" || familiarity == "familiar" || familiarity == "tanıdık")
                return "50";

            if (familiarity == "not familiar" || familiarity == "yn gyfarwydd" || familiarity == "nicht vertraut" || familiarity == "no familiar" || familiarity == "pas familier" || familiarity == "una tastiera che non conosco bene" || familiarity == "niet vertrouwd" || familiarity == "ikke kjent" || familiarity == "nieznanej" || familiarity == "nada familiar" || familiarity == "tanıdık değil" || familiarity == "nieznany")
                return "0";

            return "";
        }

        public static string TranslateEducation(string s)
        {
            var education = s.Trim();

            if (education == "primary education" || education == "Addysg gynradd" || education == "Grundschule" || education == "educación primaria" || education == "Ecole primaire" || education == "scuola elementare" || education == "lagere school" || education == "grunnskole, trinn 1-7" || education == "podstawowe" || education == "educação primária" || education == "ilköğretim")
                return "primary";

            if (education == "secondary education" || education == "Addysg uwchradd" || education == "Weiterführende Schule" || education == "educación secundaria" || education == "Collège" || education == "scuola media" || education == "middelbaar algemeen of technisch onderwijs" || education == "grunnskole, trinn 8-10 (ungdomsskole)" || education == "videregående skole" || education == "zasadnicze zawodowe" || education == "educação secundária" || education == " ortaöğretim" || education == "middelbaar algemeen van technisch onderwijs")
                return "secondary";

            if (education == "post-secondary education not leading to a degree" || education == "Addysg drydyddol (alwadigaethol)" || education == "Berufsausbildung" || education == "educación profesional (vocacional)" || education == "Bac professionnel" || education == "Filière professionnelle post-bac" || education == "scuola superiore" || education == "middelbaar beroepsonderwijs" || education == "eksamener og emner i høyere utdanning uten at det utgjør en grad" || education == "średnie" || education == "educação pós-secundária não conducente a grau" || education == " mesleki yükseköğretim")
                return "professional";

            if (education == "undergraduate degree" || education == "Bagloriaeth" || education == "Bachelorabschluss" || education == "grado, diplomatura o licenciatura" || education == "Bac général" || education == "laurea triennale" || education == "bachelor" || education == "lavere grad (bachelorgrad og cand.mag.-grad)" || education == "wyższe: licencjat" || education == "wyższe: inżynier" || education == "licenciatura" || education == " lisans derecesi" || education == "bachelor opleiding" || education == "bachelordiploma")
                return "undergraduate";

            if (education == "postgraduate degree" || education == "Gradd meistr" || education == "Master" || education == "master" || education == "Maîtrise" || education == "laurea magistrale" || education == "master" || education == "mastergrad/ hovedfag" || education == "wyższe: magister" || education == "wyższe: magister inżynier" || education == "mestrado ou outro grau pós-graduado" || education == "yüksek lisans derecesi")
                return "postgraduate";

            if (education == "doctoral degree" || education == "Doethuriaeth" || education == "Doktor" || education == "doctorado" || education == "Doctorat" || education == "dottorato di ricerca" || education == "PhD" || education == "doktorgrad" || education == "wyższe: doktorat" || education == "doutoramento" || education == " doktora derecesi")
                return "PhD";

            return "Missing";
        }

        public static string TranslateGender(SessionIdentification sessionID)
        {
            var gender = Get(sessionID.SessionInfo, "Gender", escape: false);

            if (gender == "M (male)" || gender == "G (gwryw)" || gender == "M (männlich)" || gender == "H (hombre)" || gender == "M (masculin)" || gender == "M (maschio)" || gender == "M (man)" || gender == "M (mann)" || gender == "M (mężczyzna)" || gender == "M (masculino)" || gender == "E (erkek)" || gender == "Man" || gender == "Male" || gender == "Masculin" || gender == "männlich" || gender == "hombre" || gender == "Erkek" || gender == "Mężczyzna")
                return "M (Male)";

            if (gender == "F (female)" || gender == "B (benyw)" || gender == "W (weiblich)" || gender == "M (mujer)" || gender == "F (féminin)" || gender == "F (femmina)" || gender == "V (vrouw)" || gender == "K (kvinne)" || gender == "K (Kobieta)" || gender == "F (feminino)" || gender == "K (kız)" || gender == "Vrouw" || gender == "Kobieta" || gender == "weiblich" || gender == "Female" || gender == "mujer" || gender == "Féminin" || gender == "Kiz" || gender == "F (vrouwelijk)" || gender == "Benyw")
                return "F (Female)";

            if (gender == "X (other)" || gender == "X (Eraill)" || gender == "X (anderes)" || gender == "X (otros)" || gender == "X (autre)" || gender == "X (altro)" || gender == "X (ander)" || gender == "X (annet)" || gender == "X (inna)" || gender == "X (outro)" || gender == "X(diğer)")
                return "X (Other)";

            return "Missing";
        }

        public static string TranslateComputer(string s)
        {
            var computer = s.Trim();

            if (computer.ToLower() == "desktop" || computer == "sobremesa" || computer == "De bureau" || computer == "stasjonær PC" || computer == "komputerze stacjonarnym" || computer == "secretária" || computer == "masaüstü bilgisayarı" || computer == "bureaublad")
                return "desktop";

            if (computer.ToLower() == "laptop" || computer == "gluniadur" || computer == "portátil" || computer == "Portable" || computer == "bærbar PC" || computer == "laptopie" || computer == "portátil")
                return "laptop";

            return "Missing";
        }

        public static string TranslateBrowser(string s)
        {
            var browser = s.Trim();

            if (browser == "ander" || browser == "annen" || browser == "Autre" || browser == "other" || browser == "otro" || browser == "arall" || browser == "andere" || browser == "altro" || browser == "inna" || browser == "outro" || browser == "diğer")
                return "other";

            if (browser == "don't know" || browser == "ik weet het niet" || browser == "vet ikke" || browser == "weiß nicht" || browser == "nid wyf yn sicr" || browser == "no lo sé" || browser == "Je ne sais pas" || browser == "non so" || browser == "nie wiem" || browser == "não sei" || browser == "bilmiyorum")
                return "don't know";

            if (browser == "Edge")
                return "Microsoft Edge";

            if (browser == "")
                return "Missing";

            return browser;
        }

        private static string Escape(string s)
        {
            return "\"" + s + "\"";
        }

        private static void WriteSession(string path, SessionIdentification sessionID, double correctness = 0.0, int cpm = 0)
        {
            /*
             Write the session data + the total correctness + the cpm to a file.
             */
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var timestamp = DateTime.ParseExact(sessionID.MetaInfo["__LogCreationDate"], "dd'/'MM'/'yy HH':'mm':'ss'.'FFF", CultureInfo.InvariantCulture);
            String[] keys = new String[] { "uuid", "language", "age", "gender", "session", "keyboard", "test_group", "experience", "handedness", "computer", "familiarity", "browser", "disorder", "education", "repetition", "correctness", "cpm", "creation_time" };
            String[] values = new string[] { Get(sessionID.SessionInfo, "Participant"), Get(sessionID.SessionInfo,"Text Language"), Get(sessionID.SessionInfo,"Age", escape:false), Get(sessionID.SessionInfo, "Gender"), Get(sessionID.SessionInfo,"Session"), Get(sessionID.SessionInfo,"Keyboard"), Get(sessionID.SessionInfo,"Group"),
                                             Get(sessionID.SessionInfo,"Experience"), Get(sessionID.SessionInfo,"Handedness Score", escape:false), Get(sessionID.SessionInfo, "Computer"), Get(sessionID.SessionInfo, "Keyboard Familiarity", escape:false), Get(sessionID.SessionInfo, "Browser"),
                                             Get(sessionID.SessionInfo,"Disorder").ToUpper(), Get(sessionID.SessionInfo, "Education"), Get(sessionID.SessionInfo,"Repetition").ToUpper() , correctness.ToString(), cpm.ToString(), "\""+timestamp.ToString("yyyy'-'MM'-'dd HH:mm:ss")+"\""};

            WriteCSV(Path.Combine(path, "session.csv"), keys, values);
        }

        private static void WriteCSV(string path, String[] keys, String[] values)
        {
            /*
             Write the data to a CSV.
             If the CSV already exists, we will simply append the data, else we will create a new CSV.
             */
            var list = new List<ArrayList>();
            list.Add(new ArrayList());
            if (File.Exists(path))
            {
                // Append to CSV.
                var lines = File.ReadAllLines(path);
                foreach (var line in lines)
                {
                    list[0].Add(line.Split(',') as String[]);
                }
                File.Delete(path);
            }
            else
            {
                // Create new CSV.
                list[0].Add(keys);
            }
            var writer = new CSVTextWriter();
            list[0].Add(values);
            writer.WriteToFile(list, path, ",");
        }
    }
}
