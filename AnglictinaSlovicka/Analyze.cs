using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnglictinaSlovicka
{
    public class Analyze
    {

        #region Constructor

        public Analyze(List<string> nameFiles, bool onlyNew)
        {

            FindWords = analyze(nameFiles, onlyNew);
        }

        #endregion


        #region Properties


        /// <summary>
        /// Počet všech slov (nové, známe, zakázané)
        /// </summary>
        public int TotalWords
        {
            get;
            set;
        }


        /// <summary>
        /// Známá slovička
        /// </summary>
        public int KnowWords
        {
            get;
            set;
        }

        /// <summary>
        /// Nová slova
        /// </summary>
        public int NewWords
        {
            get;
            set;
        }


        /// <summary>
        /// Slovička známa, názvy, the apod. všechny i filtrovana
        /// </summary>
        public int ForbidensAllWords
        {
            get;
            set;
        }

        /// <summary>
        /// Počet všech slov (nové, známe, zakázané)
        /// </summary>
        public int OneTotalWords
        {
            get
            {
                return FindOneTotal.Count;
            }
        }


        /// <summary>
        /// Známá slovička
        /// </summary>
        public int OneKnowWords
        {
            get
            {
                return FindOneKnow.Count;
            }
        }

        /// <summary>
        /// Nová slova
        /// </summary>
        public int OneNewWords
        {
            get
            {
                return FindOneNew.Count;
            }
        }


        /// <summary>
        /// Slovička známa, názvy, the apod.
        /// </summary>
        public int OneForbidensWords
        {
            get
            {
                return FindOneForbidden.Count + FindOneForbFilter1.Count + FindOneForbFilter2.Count;
            }
        }

        #endregion
        

        #region Variables

        public Dictionary<string, int> FindWords = new Dictionary<string, int>();

        public Dictionary<string, int> FindOneTotal = new Dictionary<string, int>();
        public Dictionary<string, int> FindOneForbidden = new Dictionary<string, int>();
        public Dictionary<string, int> FindOneKnow = new Dictionary<string, int>();
        public Dictionary<string, int> FindOneNew = new Dictionary<string, int>();

        /// <summary>
        /// filter čarka 
        /// </summary>
        public Dictionary<string, int> FindOneForbFilter1 = new Dictionary<string, int>();

        /// <summary>
        /// filter s
        /// </summary>
        public Dictionary<string, int> FindOneForbFilter2 = new Dictionary<string, int>();


        #endregion
        //
        
        #region Methods

        private string fisrtUpper(string word)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(word))
            {
                result = word.Substring(0, 1).ToUpper() + word.Substring(1);
            }

            return result;
        }


        private void addWordToDictionary(string word, Dictionary<string, int> dictionary)
        {
            if (!dictionary.ContainsKey(word.ToLower()))
            {
                dictionary.Add(word.ToLower(), 1);
            }
            else
            {
                dictionary[word.ToLower()] = dictionary[word.ToLower()] + 1;
            }
        }


        /// <summary>
        /// Filtrovane slova - false neni dovoleno
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        private bool addForbiddenWorldsWithFilter(string world, bool save)
        {
            bool result = false;

            if (Pracovni.forbiddenWords.Contains(world.ToLower()))
            {
                if (save)
                    addWordToDictionary(world, FindOneForbidden);
                return true;
            }
            else if (world.Contains("\'"))
            {
                if (save)
                    addWordToDictionary(world, FindOneForbFilter1);
                return true;
            }
            if (world[world.Length - 1] == 's')
            {
                string withoutS = (world.Substring(0, world.Length - 1));
                if (Pracovni.ExistujeSlovicko(withoutS.ToLower()) || Pracovni.forbiddenWords.Contains(withoutS.ToLower()))
                {
                    if (save)
                      addWordToDictionary(world, FindOneForbFilter2);
                    return true;
                }
            }


            return result;
        }



        /// <summary>
        /// Main function for analyze
        /// </summary>
        /// <param name="nameFile"></param>
        /// <param name="onlyNew"></param>
        /// <returns></returns>
        private Dictionary<string, int>  analyze(List<string> nameFile, bool onlyNew)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            Dictionary<string, int> findWords = new Dictionary<string, int>();

            
            string line;
            TotalWords = 0;KnowWords = 0;ForbidensAllWords = 0;NewWords = 0;
           
            FindOneForbidden.Clear();
            FindOneKnow.Clear();
            FindOneNew.Clear();
            FindOneTotal.Clear();
            FindOneForbFilter1.Clear();
            FindOneForbFilter2.Clear();

            foreach (var item in nameFile)
            {

                System.IO.StreamReader sr = new System.IO.StreamReader(item);
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    while (!string.IsNullOrEmpty(line))
                    {
                        string slovo = getOneWord(ref line);
                        if (slovo.Length > 1)
                        {
                            TotalWords++;
                            addWordToDictionary(slovo, FindOneTotal);

                            if (Pracovni.ExistujeSlovicko(slovo.ToLower()))
                            {
                                KnowWords++;
                                addWordToDictionary(slovo, FindOneKnow);
                            }
                            else if (addForbiddenWorldsWithFilter(slovo, true))
                            {
                                ForbidensAllWords++;
                                //addWordToDictionary(slovo, FindOneForbidden);
                            }
                            else
                            {
                                NewWords++;
                                addWordToDictionary(slovo, FindOneNew);
                            }


                            if (findWords.ContainsKey(slovo))
                            {
                                findWords[slovo] = findWords[slovo] + 1;
                            }
                            else if (!Pracovni.forbiddenWords.Contains(slovo.ToLower()) && !addForbiddenWorldsWithFilter(slovo,false))
                            {

                                if (!onlyNew || (onlyNew && !Pracovni.ExistujeSlovicko(slovo)))
                                {
                                    findWords.Add(slovo, 1);
                                    int pocet = 0;
                                    if (findWords.ContainsKey(slovo.ToLower()) && findWords.ContainsKey(fisrtUpper(slovo)))
                                    {
                                        pocet += findWords[fisrtUpper(slovo)];
                                        findWords.Remove(fisrtUpper(slovo));
                                        findWords[slovo.ToLower()] += pocet;
                                    }
                                }
                            }
                        }
                    }
                }

                sr.Close();

            }

            foreach (var item in findWords.OrderBy(i => i.Key))
            {
                result.Add(item.Key, item.Value);
            }
            

            return result;
        }


        private string getOneWord(ref string line)
        {
            string result = string.Empty;
            bool isWord = false;

            for (int i = 0; i < line.Length; i++)
            {
                Char ch = line[i];
                if (Char.IsLetter(ch))
                {
                    result += ch;
                    isWord = true;
                }
                else if (isWord && (ch == '-' || ch == '\''))
                {
                    result += ch;
                }
                else if (isWord )
                {
                    line = line.Substring( i, line.Length - i  );
                    return result;
                }
            }

            line = string.Empty;
            return result;
        }


        #endregion




    }
}
