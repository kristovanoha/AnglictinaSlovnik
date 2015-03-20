using System;
using System.Collections.Generic;
using System.IO;

using System.Net;
using SpeechLib;
using System.Diagnostics;
//using Microsoft.DirectX;

namespace AnglictinaSlovicka
{
    class Pracovni
    {

        #region Constant

        public static string FORBIDDEN_WORDS = "FORBIDDENWORDS";

        #endregion



        #region variable

       /// <summary>
       /// aktualni soubor
       /// </summary>
        public static int souborAkt = 0;


        /// <summary>
        /// aktualni radek v tabulce souboru....pro odznaceni
        /// </summary>
        public static int radekAktTabulkaSouboruSlovnik = 0;
        
        /// <summary>
        /// aktualni cesta k souborum
        /// </summary>
        public static string CESTA = Environment.CurrentDirectory + "\\soubory";


        /// <summary>
        ///  1 - editace slovicka, 2 - vloz nove slovicko
        /// </summary>
        public static int stavAkce = -1;

        /// <summary>
        /// slovicka v souboru ceske
        /// </summary>
        public static List<string>[] slovicka;
       
        /// <summary>
        /// slovicka jednoho souboru anglicka
        /// </summary>
        public static List<string>[] slovickaEN;

        /// <summary>
        /// slovicka rating uspesnosti
        /// </summary>
        public static List<string>[] slovickaRating;

        /// <summary>
        /// slovicka urcena pro test ceska   (nahodne z nich je tahano)
        /// </summary>
        public static List<string> testSlovicka;

        /// <summary>
        /// slovicka urcena pro test anglicka   (nahodne z nich je tahano)
        /// </summary>
        public static List<string> testSlovickaEN;

        /// <summary>
        /// slovicka urcena pro test jejich rating  (nahodne z nich je tahano)
        /// </summary>
        public static List<string> testSlovickaRating;


        /// <summary>
        /// pocet slovicek v jednom listu (souboru)
        /// </summary>
        public static int[] slovickaPocet;


        /// <summary>
        /// slovička, které nechci vyhledávat
        /// </summary>
        public static List<string> forbiddenWords = new List<string>();


        // slovicka ze vsech souboru
        public static List<string> slovickaVseKomplet = new List<string>();
        public static List<string> slovickaVseKompletEN = new List<string>();
        public static List<string> slovickaVseKompletRating = new List<string>();
        public static List<string> slovickaVseKompletSoubor = new List<string>();

        
       /// <summary>
       /// soubory vsechny ceske
       /// </summary>
        public static List<string> souboryVse = new List<string>();
       
        /// <summary>
        /// soubory vse anglicke
        /// </summary>
        public static List<string> souboryVseEN = new List<string>();

        /// <summary>
        /// soubory vsechny rating
        /// </summary>
        public static List<string> souboryVseRating = new List<string>();

        /// <summary>
        /// soubory vybrane pro test v nastaveni
        /// </summary>
        public static List<string> souboryProTest = new List<string>();

        /// <summary>
        /// aktualne zpracovane slovicko
        /// </summary>       
        public static string aktSlovickoUpravaEN;

        /// <summary>
        /// aktualne zpracovany soubor
        /// </summary>       
        public static string aktSouborUprava;

        /// <summary>
        /// aktualne zpracovany soubor pro nová slovička
        /// </summary>       
        public static string aktSouborNoveSlovo;


        //pro test cast

        /// <summary>
        /// aktualni stav testu 0 - napis preklad slovicka, 1 - spravne, 2 -opraveno spravne, 3 - chyba prepis, 4 - restart testu
        /// </summary>
        public static int testAktStav = 0;

        /// <summary>
        /// preklad do anglictiny...co musi uzivatel napsat
        /// </summary>
        public static string testAngPreklad;

        /// <summary>
        /// preklad do anglictiny...co musi uzivatel napsat, posledni slovicko...
        /// </summary>
        public static string testAngPrekladPosledni;

        /// <summary>
        /// oprava slovicka, nepocita se do statistiky...prepis spatne napsaneho slovicka
        /// </summary>
        public static bool testOpravaSlova = false;

        /// <summary>
        /// statistika celkovy pocet pokusu
        /// </summary>
        public static decimal statPocetCelkem = 0;
        
        /// <summary>
        /// statistika celkovy pocet uspesnych pokusu
        /// </summary>
        public static decimal statPocetUspech = 0;
        
        /// <summary>
        /// statistika celkovy pocet neuspesnych pokusu
        /// </summary>
        public static decimal statPocetNeuspech = 0;

        /// <summary>
        /// statistika celkova procentni uspesnost
        /// </summary>
        public static decimal statProcentUspech = 0;


        // nastaveni testu

        /// <summary>
        /// neopakovat jiz uhodnuta slovicka
        /// </summary>
        public static bool nastavTestNeopakovat = false;

        /// <summary>
        /// nastaveni testu ...dat test uplne ze vsech slovicek
        /// </summary>
        public static bool nastavTestVsechnaSlovicka = false;

        /// <summary>
        /// nastaveni testu, kolik je celkove v testu slovicek
        /// </summary>
        public static int nastavTestPocetSlovicek = 0;

        /// <summary>
        /// nastaveni testu, kolik je celkove v testu slovicek
        /// </summary>
        public static bool nastavTestRatingSlovicek = false;
        
        /// <summary>
        /// zda se ma zobrazit tooltipova napoveda
        /// </summary>
        public static bool nastavTestZobrazToolTip = false;       
        
        /// <summary>
        /// kolik je celkove slovicek ....v databazi...
        /// </summary>
        public static int nastavPocetSlovicekCelkem = 0;
        
        /// <summary>
        /// pri restartu testu, vynulovani statistiky
        /// </summary>
        public static bool nastavRestartStatistiky = false;

        /// <summary>
        /// spousteni zvuku prekladu slovicka
        /// </summary>
        public static bool nastavSpousteniZvuku = false;

        /// <summary>
        /// spousteni zvuku prekladu slovicka
        /// </summary>
        public static int nastavRychlostZvuku = 0;

        /// <summary>
        /// cas spusten...test bezi, cas tedy taky....
        /// </summary>
        public static bool casSpusten = false;

        /// <summary>
        /// cesta kde se bude vytvaret aktualni zaloha....
        /// </summary>
        public static string cestaZalohyAdresar = string.Empty;


        /// <summary>
        /// casovac pro napisy
        /// </summary>
        public static int pocetCasu = 0;
        public static int pocetCasu2 = 0;
        public static int pocetCasu3 = 0;

        /// <summary>
        /// spravna odpoved v testu, kdy se hleda cesky preklad
        /// </summary>
        public static int spravnaOdpovedTestCeska = 0;


        /// <summary>
        /// jedna li se o opravu testu vyber spravny preklade do cestiny....
        /// </summary>
        public static bool opravaOdpovedTestCeska = false;


        #endregion


        #region metody


        /// <summary>
        /// zjisteni obsahu adresare vsechny soubory
        /// nactou se pouze ty ktere maji stejny s priponou EN
        /// </summary>
        public static List<string> seznamSouboru(string novySoubor)
        {

            string[] souboryVseV = null;
            souboryVse.Clear();
            souboryVseEN.Clear();
            souboryVseRating.Clear();
          //  souboryProTest.Clear();

            //existuje adresar ?
            if (Directory.Exists(CESTA))
            {
                souboryVseV = Directory.GetFiles(CESTA);

                List<string> souboryVsePom = new List<string>();
                
                foreach (string ss in souboryVseV)
                {
                    souboryVsePom.Add(Path.GetFileName(ss));

                }

                // pouze soubory ,ktere maji EN podobny soubor
                for (int index = 0; index < souboryVsePom.Count; index++)
                {
                    string retezec = souboryVsePom[index].Replace(".txt", "");
                    if (!souboryVsePom[index].EndsWith("EN"))
                    {
                        if (souboryVsePom.IndexOf(retezec + "EN.txt") != -1)
                        {
                            souboryVse.Add(souboryVsePom[index]);
                            souboryVseEN.Add(souboryVsePom[index].Replace(".txt", "EN.txt"));
                            souboryVseRating.Add(souboryVsePom[index].Replace(".txt", "RAT.txt"));
                        }
                    }
                }
            }

            return souboryVse;
        }


        /// <summary>
        /// restartovaní všech proměných....
        /// </summary>
        public static void restartVsechPromenych()
        {  
            slovickaVseKomplet.Clear();
            slovickaVseKompletEN.Clear();
            slovickaVseKompletRating.Clear();
            slovickaVseKompletSoubor.Clear();
            souboryVse.Clear();
            souboryVseEN.Clear();
            souboryVseRating.Clear();
            souboryProTest.Clear();
            forbiddenWords.Clear();
        }


        /// <summary>
        /// vytvoreni noveho souboru ceskeho i anglickeho
        /// </summary>
        public static bool vytvorSoubor(string nazevSouboru)
        {
            //Console.WriteLine("vytvoreni souboru");
            if (File.Exists(Pracovni.CESTA + "\\" + nazevSouboru + ".txt") || File.Exists(Pracovni.CESTA + "\\" + nazevSouboru + "EN.txt"))
            {
                return false;
            }
            else
            {  
                stavAkce = 2;

                StreamWriter sw = new StreamWriter(CESTA + "\\" + nazevSouboru + ".txt");            
                sw.Close();

                StreamWriter swR = new StreamWriter(CESTA + "\\" + nazevSouboru + "RAT.txt");
                swR.Close();

                StreamWriter swen = new StreamWriter(CESTA + "\\" + nazevSouboru + "EN.txt");     
                swen.Close();
                seznamSouboru(string.Empty);
                SlovickaDoListuForbidden();
            }
      
             return true;
        }


        public static bool VytvorJednoduchySoubor(string nazevSouboru)
        {
            if (File.Exists(Pracovni.CESTA + "\\" + nazevSouboru + ".txt"))
            {
                return false;
            }
            else
            {
                StreamWriter sw = new StreamWriter(CESTA + "\\" + nazevSouboru + ".txt");
                sw.Close();
            }
            return true;
        }


        /// <summary>
        /// podrobny obsah jednotlivého souboru
        /// </summary>
        /// <returns>vraci list slovicek souboru</returns>
        private static List<string> PodrobnyObsahSouboru(string soubor)
        {
            List<string> obsahSouboru = new List<string>();
            string line = string.Empty;
            string cestaCela = CESTA + "\\" + soubor;

            StreamReader sr = new StreamReader(cestaCela);

            while ((line = sr.ReadLine()) != null)
            {
                Console.WriteLine(line);
                obsahSouboru.Add(line);
            }
            sr.Close();
            return obsahSouboru;
        }


        /// <summary>
        /// seznam vsech slovicek do listu ze vsech souboru  POTREBUJI
        /// </summary>
        /// <param name="novySoubor">novy soubor soubor je potreba pridat jednu skupinu</param>
        public static void SlovickaDoListuForbidden()
        {
           {             
               forbiddenWords = SlovickaDoListuJednotlive(FORBIDDEN_WORDS + ".txt");

                slovicka = new List<string>[souboryVse.Count];
                slovickaEN = new List<string>[souboryVse.Count];
                slovickaRating = new List<string>[souboryVse.Count];
                slovickaPocet = new int[souboryVse.Count];

                nastavPocetSlovicekCelkem = 0;

                for (int index = 0; index < souboryVse.Count; index++)
                {
                    slovicka[index] = PodrobnyObsahSouboru(souboryVse[index]);
                    slovickaEN[index] = PodrobnyObsahSouboru(souboryVseEN[index]);
                    // neexistuje rating soubor...zalozieme novy
                    if (File.Exists(Pracovni.CESTA + "\\" + souboryVseRating[index]) == false)
                        VytvoreniRatingSouboru(souboryVseRating[index], slovicka[index].Count);
                   
                    slovickaRating[index] = PodrobnyObsahSouboru(souboryVseRating[index]);

                    slovickaPocet[index] = slovickaEN[index].Count;
                    nastavPocetSlovicekCelkem = nastavPocetSlovicekCelkem + slovickaPocet[index];
                }                
            }
        }


        /// <summary>
        /// Načteni jednotlivé souboru do listu
        /// </summary>
        /// <param name="nameFile"></param>
        /// <returns></returns>
        public static List<string> SlovickaDoListuJednotlive(string nameFile)
        {
            List<string> result = new List<string>();
            result = PodrobnyObsahSouboru(nameFile);
            return result;
        }


        /// <summary>
        /// vytvoreni rating souboru, pripad kdy uzivatel smaze, zalozi se novy s nulovymi zaznamy
        /// </summary>
        /// <param name="nazev">nazev souboru</param>
        /// <param name="pocet">pocet zaznamu,ktery by mel mit</param>
        private static void VytvoreniRatingSouboru(string nazev, int pocet)
        {
            string cestaCela = CESTA + "\\" + nazev;

            StreamWriter sw = new StreamWriter(cestaCela);

            while (pocet > 0)
            {
                pocet--;
                sw.WriteLine("0");
            }
            sw.Close();
        }


        /// <summary>
        /// sestaveni vsech souboru do celkoveho listu
        /// </summary>
        public static void VytvoreniSeznamuVsechna()
        {
            slovickaVseKomplet.Clear();
            slovickaVseKompletEN.Clear();
            slovickaVseKompletSoubor.Clear();
            slovickaVseKompletRating.Clear();

            int celkovyPocet = 0;
            for (int index = 0; index < souboryVse.Count; index++)
            {
                slovickaVseKomplet.AddRange(PodrobnyObsahSouboru(souboryVse[index]));
                slovickaVseKompletEN.AddRange(PodrobnyObsahSouboru(souboryVseEN[index]));
                slovickaVseKompletRating.AddRange(PodrobnyObsahSouboru(souboryVseRating[index]));

                while (celkovyPocet + slovicka[index].Count > slovickaVseKompletSoubor.Count)
                    slovickaVseKompletSoubor.Add(souboryVse[index]);
                celkovyPocet =  slovickaVseKompletSoubor.Count;
            }            
        }


        public static void UlozeniDoSouboru(List<string> list, string file)
        {
            StreamWriter sw = new StreamWriter(CESTA + "\\" + file + ".txt");

            foreach (var item in list)
            {
                sw.WriteLine(item);
            }


            sw.Close();

        }


        /// <summary>
        /// zmena slovicka v souboru ,nebo nove slovicka.... True - uprava, False - nove
        /// </summary>
        /// <param name="soubor">soubor,ktereho se tyka zmena</param>
        /// <param name="puvAng">puvodni anglicke slovicko</param>
        /// <param name="novAng">nove anglicke slovicko</param>
        /// <param name="novCes">nove ceske slovicko</param>
        /// <param name="upravaSlova">jedna se o upravu slova</param>
        /// <param name="smazaniSlova">jedna se o smazani</param>
        /// <param name="ratingNew">novy rating...kdyz je jina nez 0, uprava je false, i smazani je false</param>
        /// <returns>probehlo v poradku true, jinak false</returns>
        public static bool UpravaNovaSlovicka(string soubor, string puvAng, string novAng, string novCes, bool upravaSlova, bool smazaniSlova, int ratingNew)
        {
            string souborZmeny = string.Empty;
            
            //TODO kontroly zda nic neni empty..... a existuje soubor
            if ((soubor == string.Empty || novAng == string.Empty || novCes == string.Empty) && ratingNew == 0)
                return false;

            // TODO lze pouzit metodu
            // nalezeni souboru....
            int souborZmenyIndex = 0;
            for (int index = 0; index < souboryVse.Count; index++)
            {
                if (souboryVse[index].Replace(".txt", "") == soubor)
                {
                    souborZmenyIndex = index;
                    souborZmeny = souboryVse[index];
                    break;
                }
            }
         

            if (smazaniSlova == false)
            {
                if (upravaSlova)  // uprava slovicka
                {

                    // zmena listu
                    for (int index = 0; index < slovickaEN[souborZmenyIndex].Count; index++)
                    {
                        if (slovickaEN[souborZmenyIndex][index] == puvAng)
                        {
                            slovickaEN[souborZmenyIndex][index] = OdstraneniMezerViceSlovnehoAng(novAng);
                            slovicka[souborZmenyIndex][index] = novCes;
                            break;
                        }
                    }
                }
                else if (ratingNew != 0)  // zmena ratingu, 
                {
                    int indexSlovicko = slovickaEN[souborZmenyIndex].IndexOf(puvAng);
                    if (indexSlovicko > -1)
                    {
                        int ratingPuvodni = Convert.ToInt32(slovickaRating[souborZmenyIndex][indexSlovicko]);
                        ratingPuvodni = ratingPuvodni + ratingNew;

                        slovickaRating[souborZmenyIndex][indexSlovicko] = Convert.ToString(ratingPuvodni);
                    }
                }
                else           //nove slovicko
                {
                    // vklad slovicka
                   // slovickaEN[souborZmenyIndex].Add(novAng);

                    slovickaEN[souborZmenyIndex].Add(OdstraneniMezerViceSlovnehoAng(novAng));
                    slovicka[souborZmenyIndex].Add(novCes);
                    slovickaRating[souborZmenyIndex].Add("0");

                }
            }
            else   // smazani slovicka
            {
                int kdeSmazat = slovickaEN[souborZmenyIndex].IndexOf(puvAng);
                if (kdeSmazat == -1)
                    return false;
                slovickaEN[souborZmenyIndex].RemoveAt(kdeSmazat);
                slovicka[souborZmenyIndex].RemoveAt(kdeSmazat);
                slovickaRating[souborZmenyIndex].RemoveAt(kdeSmazat);

            }


            // zapis  souboru ...zmeny

                string cestaCela = CESTA + "\\" + souboryVse[souborZmenyIndex];
                
               StreamWriter sw = new StreamWriter(cestaCela);


                SetrideniSlovicek(ref slovicka[souborZmenyIndex], ref slovickaEN[souborZmenyIndex], ref slovickaRating[souborZmenyIndex]);

                foreach (string radek in slovicka[souborZmenyIndex])
                {
                    sw.WriteLine(radek);                
                }
                sw.Close();

            //agnlicke slovicka
                StreamWriter swen = new StreamWriter(cestaCela.Replace(".txt", "EN.txt"));
                foreach (string radek in slovickaEN[souborZmenyIndex])
                {
                    swen.WriteLine(radek);                
                }
                swen.Close();
                

            // zapis ratingu
                StreamWriter rat = new StreamWriter(cestaCela.Replace(".txt", "RAT.txt"));
                foreach (string radek in slovickaRating[souborZmenyIndex])
                {
                    rat.WriteLine(radek);
                }
                rat.Close();
                return true;
        }

        
        /// <summary>
        /// uprava slovicek, vice traru, odstarneni mezer, ktere se muzou vyskytnou u carky
        /// </summary>
        /// <param name="slovicko">cele slovicko,,,</param>
        /// <returns>odstraneny mezeryd</returns>
        private static string OdstraneniMezerViceSlovnehoAng(string slovicko)
        {
            string vysledek = string.Empty;
            string text = slovicko;

            if (text.IndexOf(",") != -1)
            {
                List<string> slovicka = new List<string>();

                while (text.IndexOf(",") != -1)
                {
                    slovicka.Add(text.Substring(0, text.IndexOf(",")).Trim());
                    text = text.Substring(text.IndexOf(",") + 1, text.Length - text.IndexOf(",") - 1);
                }
                slovicka.Add(text.Substring(0, text.Length).Trim());

                while (slovicka.Count > 0)
                {
                    vysledek = vysledek + slovicka[0];
                    slovicka.RemoveAt(0);
                    if (slovicka.Count > 0)
                        vysledek += ",";
                }
            }
            else
            {
                vysledek = slovicko;
            }
            return vysledek;
        }
        

        /// <summary>
        /// setrideni slovicek v souboru...cz i en a rating
        /// </summary>
        private static void SetrideniSlovicek(ref List<string> ceska, ref List<string> anglicka, ref List<string> rating)
        {
            List<string> dohromady = new List<string>();

            for (int index = 0; index < ceska.Count; index++)
            {
                dohromady.Add(anglicka[index] + "|" + ceska[index] + "|" + rating[index]);
            }
            dohromady.Sort();
            ceska.Clear();
            anglicka.Clear();
            rating.Clear();

            for (int index = 0; index < dohromady.Count; index++)
            {
                int delka = dohromady[index].Length;
                int znak = dohromady[index].IndexOf("|");
                int znakPosle = dohromady[index].LastIndexOf("|");
        
                anglicka.Add(dohromady[index].Substring(0, dohromady[index].IndexOf("|")));
                ceska.Add(dohromady[index].Substring( (znak + 1), (znakPosle - znak - 1 )  ));
                rating.Add(dohromady[index].Substring((znakPosle + 1), (delka - znakPosle - 1 )));
            }
        }


        /// <summary>
        /// kontrola zda slovicko jiz existuje... prohleda celkovy slovnik, hleda se v podle anglickych
        /// </summary>
        /// <param name="noveSlovicko"></param>
        /// <returns>true - existuje </returns>
        public static bool ExistujeSlovicko(string noveSlovicko)
        {
            bool vysledek = false;

            for (int index = 0; index < slovickaVseKompletEN.Count; index++)
            {
                if (slovickaVseKompletEN[index] == noveSlovicko)
                {
                    vysledek = true;
                    break;
                }
            }

            return vysledek;

        }


        /// <summary>
        /// kontrola zda slovicko jiz existuje... prohleda celkovy slovnik, hleda se v podle anglickych
        /// </summary>
        /// <param name="noveSlovicko"></param>
        /// <returns>vraci cislo radku ve slovniku </returns>
        public static int ExistujeSlovickoRadek(string noveSlovicko)
        {
            int vysledek = 0;

            for (int index = 0; index < slovickaVseKompletEN.Count; index++)
            {
                if (slovickaVseKompletEN[index] == noveSlovicko)
                {
                    vysledek = index;
                    break;
                }
            }

            return vysledek;

        }


        public static string PrekladSlovicka(string noveSlovicko)
        {
            string result = string.Empty;

            if (ExistujeSlovickoRadek(noveSlovicko) != 0)
            {
                result = slovickaVseKomplet[ExistujeSlovickoRadek(noveSlovicko)];
            }

            return result;
        }


        /// <summary>
        /// u existujiciho slovicka vrati nazev souboru, kde se na nachazi....kdyz neexistuje tak vrati null
        /// </summary>
        /// <param name="slovicko">slovicko,ktere hledam</param>
        /// <returns></returns>
        public static string ZdrojovySoubor(string slovicko)
        {
            string vysledek = string.Empty;

            if (slovicko == string.Empty)
                return string.Empty;

            int index = slovickaVseKompletEN.IndexOf(slovicko);
            if (index == -1)
                return string.Empty;

            vysledek = slovickaVseKompletSoubor[index];

            return vysledek.Replace(".txt","");
        }


        /// <summary>
        /// zmena ratingu u jednotliveho slovicka...
        /// </summary>
        /// <param name="angSlovicko">slovicko</param>
        /// <param name="rating">rating o kolik se ma zmenit</param>
        /// <returns>true - probehlo v poradku, false - neprobehlo</returns>
        public static bool ZmenaRatinguSlovicka(string angSlovicko, int rating)
        {
            if (angSlovicko == string.Empty)
                return false;
            string souborZmeny = ZdrojovySoubor(angSlovicko);

           // int indexSoubor = IndexniCisloSlovnikSoub(souborZmeny);

            UpravaNovaSlovicka(souborZmeny,angSlovicko,string.Empty,string.Empty, false, false, rating);

            return true;
        }

        #endregion


        #region Test

        /// <summary>
        /// pridani, ubráni souboru pro test....pouziti pri rozšiřeni, nebo ubrani
        /// </summary>
        /// <param name="soubor">nazev souboru</param>
        /// /// <param name="pridani">pridani - true, odebrani false</param>
        /// <returns></returns>
        public static bool SouboryProTest(string soubor, bool pridani)
        {
            if (soubor == string.Empty)
                return false;

            if (pridani)
            {
                if (souboryProTest.IndexOf(soubor) == - 1)
                    souboryProTest.Add(soubor);
                }
            else
            {
                if (souboryProTest.IndexOf(soubor) != -1) 
                  souboryProTest.RemoveAt(souboryProTest.IndexOf(soubor));
            }
            return true;
        }


        /// <summary>
        /// vraci indexni cislo slovnikoveho souboru podle jmena
        /// </summary>
        /// <param name="jmenoSouboru">nazev souboru, ktere se vetsinou vezme z tabulky</param>
        /// <returns></returns>
        private static int IndexniCisloSlovnikSoub(string jmenoSouboru)
        {
            if (jmenoSouboru == string.Empty)
                return -1;
            
            int vysledek = -1;
                        
            for (int index = 0; index < souboryVse.Count; index++)
            {
                if (souboryVse[index].Replace(".txt", "") == jmenoSouboru)
                {
                    vysledek = index;                   
                    break;
                }
            }

            return vysledek;
        }


        /// <summary>
        /// sestaveni slovicek, ktere se budou nahodne tahat ....zavisí na nastaveni
        /// </summary>
        /// <param name="typTestu">typ testu jaky bude zvolen</param>
        /// <returns></returns>
        public static bool SestaveniSlovicekProTest(int typTestu)
        {
            testSlovicka = new List<string>();
            testSlovickaEN = new List<string>();
            testSlovickaRating = new List<string>();

            if (nastavTestVsechnaSlovicka) // vsechna slovicka
            {
                for (int index = 0; index < souboryVse.Count; index++)
                {
                    for (int ind = 0; ind < slovicka[index].Count; ind++)
                    {
                        testSlovicka.Add(slovicka[index][ind]);
                        testSlovickaEN.Add(slovickaEN[index][ind]);
                        testSlovickaRating.Add(slovickaRating[index][ind]);

                        if (nastavTestRatingSlovicek == true && nastavTestNeopakovat == false )
                        {
                            int pocet = Convert.ToInt32(slovickaRating[index][ind]);
                            while (pocet < 0)
                            {
                                testSlovicka.Add(slovicka[index][ind]);
                                testSlovickaEN.Add(slovickaEN[index][ind]);
                                testSlovickaRating.Add(slovickaRating[index][ind]);
                                pocet++;
                            }
                        }
                    }
                }
            }
            else
            {
                // nacteni slovicek do testu  -- v pripade ze je vyber
                for (int index = 0; index < souboryProTest.Count; index++)
                {
                    // Console.WriteLine(" " + souboryProTest[index] + IndexniCisloSlovnikSoub(souboryProTest[index]));

                    int indexSoubor = IndexniCisloSlovnikSoub(souboryProTest[index]);

                    // doplneni jednotlivych slovicek z vybranych souboru
                    for (int ind = 0; ind < slovicka[indexSoubor].Count; ind++)
                    {
                        testSlovicka.Add(slovicka[indexSoubor][ind]);
                        testSlovickaEN.Add(slovickaEN[indexSoubor][ind]);
                        testSlovickaRating.Add(slovickaRating[indexSoubor][ind]);

                        if (nastavTestRatingSlovicek == true && nastavTestNeopakovat == false )
                        {
                            int pocet = Convert.ToInt32(slovickaRating[indexSoubor][ind]);
                            while (pocet < 0)
                            {
                                testSlovicka.Add(slovicka[indexSoubor][ind]);
                                testSlovickaEN.Add(slovickaEN[indexSoubor][ind]);
                                testSlovickaRating.Add(slovickaRating[indexSoubor][ind]);
                                pocet++;
                            }
                        }
                    }

                }
            }
            return true;
        }


        /// <summary>
        /// v pripade, že je uhadnuto slovicko a je zapnuto bez opakovani uhadlych...tak se odstrani z testu
        /// </summary>
        /// <param name="slovicko">hledane slovicko v ANG</param>
        public static void OdebraniSlovaTest(string slovicko)
        {
            if (nastavTestNeopakovat)
            {
                while (testSlovickaEN.IndexOf(slovicko) != -1)
                {
                    int indexSlov = testSlovickaEN.IndexOf(slovicko);
                    testSlovickaEN.RemoveAt(indexSlov);
                    testSlovicka.RemoveAt(indexSlov);
                    testSlovickaRating.RemoveAt(indexSlov);
                }

            }
        }


        /// <summary>
        /// Vraci náhodné slovíčko z testovaných slovíček.
        /// </summary>
        /// <param name="anglicke"></param>
        /// <param name="ceske"></param>
        /// <returns></returns>
        public static bool NahodnaSlovicka(ref string anglicke, ref string ceske)
        {
            if (testSlovicka.Count < 1)
                return false;
            
            Random ran = new Random();
            if (nastavTestNeopakovat == true && nastavTestRatingSlovicek == true )    // neopakujici se,ale bere se v potaz rating 
            {
                int pocetMin = 0;
                int minimum = 999999999;
                List<int> pozice = new List<int>();

                // spocitani nejnizsich
                for (int index = 0; index < testSlovicka.Count; index++)
                {
                    if (Convert.ToInt32(testSlovickaRating[index]) < minimum)
                    {
                        minimum = Convert.ToInt32(testSlovickaRating[index]);
                        pozice.Clear();
                        pozice.Add(index);
                        pocetMin = 1;
                    }
                    else if (minimum == Convert.ToInt32(testSlovickaRating[index]))
                    {
                        pocetMin++;
                        pozice.Add(index);
                    }
                }

                int nahCislo = ran.Next(pozice.Count);
  
                anglicke = testSlovickaEN[pozice[nahCislo]];
                ceske = testSlovicka[pozice[nahCislo]];

                // pokud to jde tak at neni stejne slovicko hned v poradi
                if (pozice.Count > 1 && anglicke == testAngPrekladPosledni)
                    while (anglicke == testAngPrekladPosledni)
                    {
                        nahCislo = ran.Next(pozice.Count);
                        anglicke = testSlovickaEN[pozice[nahCislo]];
                        ceske = testSlovicka[pozice[nahCislo]];
                    }
            }
            else
            {
                int nahCislo = ran.Next(testSlovicka.Count);
               // Console.WriteLine("rando " + testSlovicka.Count + "   " +nahCislo);

                anglicke = testSlovickaEN[nahCislo];
                ceske = testSlovicka[nahCislo];

               if (testSlovicka.Count > 1 && anglicke == testAngPrekladPosledni)
                   while (anglicke == testAngPrekladPosledni)
                   {
                        nahCislo = ran.Next(testSlovicka.Count);
                        anglicke = testSlovickaEN[nahCislo];
                        ceske = testSlovicka[nahCislo];
                   }

            }


            testAngPrekladPosledni = anglicke;

            return true;
        }


        /// <summary>
        /// pocet slov, ktere jsou v testu...
        /// </summary>
        ///  <param name="skutecny">true - skutecny stav ze souboru, false - vraci z aktualniho listu testovanych slovicek </param>
        /// <returns>pocet slov v testu</returns>
        public static int PocetTestovanychSlov(bool skutecny)
        {
            int vysledek = 0;
            if (slovickaPocet != null)
            {
                if (skutecny) // skutecny ze souboru
                {
                    if (nastavTestVsechnaSlovicka) //vsechna slovicka
                        vysledek = slovickaVseKomplet.Count;
                    else
                    {
                        for (int index = 0; index < souboryProTest.Count; index++)
                        {
                            // Heg VYZKOUSET VYJIMKU
                            try
                            {
                                vysledek += slovickaPocet[IndexniCisloSlovnikSoub(souboryProTest[index])];
                            }

                            catch
                            {
                                Console.WriteLine("chyba souboru nacteni ");
                            }
                        }
                    }
                }
                else // aktualni z listu
                {
                    vysledek = testSlovicka.Count;
                }
            }
            return vysledek;
        }

        #endregion


        #region ulozeniNastaveni

        /// <summary>
        /// ulozeni nastaveni souboru ....
        /// </summary>
        public static void ulozNastaveniProgramu()
        {
         // zapsani nastaveni do souboru

            // zapsani nastaveni do souboru

            StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "\\" + "nastaveniProgramu.txt");

            sw.WriteLine(nastavTestNeopakovat);
            sw.WriteLine(nastavTestVsechnaSlovicka);
            sw.WriteLine(nastavTestRatingSlovicek);
            sw.WriteLine(nastavRestartStatistiky);
            sw.WriteLine(nastavTestZobrazToolTip);
            sw.WriteLine(nastavSpousteniZvuku);
            sw.WriteLine(cestaZalohyAdresar);
            sw.WriteLine(nastavRychlostZvuku);


            //oznacene soubory
            for (int index = 0; index < Pracovni.souboryProTest.Count; index++)
            {
                sw.WriteLine(Pracovni.souboryProTest[index]);
            }

            sw.Close();
        }


        /// <summary>
        /// nacti nastaveni programu
        /// </summary>
        public static bool nactiNastaveniProgramu()
        {
            if (File.Exists(Environment.CurrentDirectory + "\\" + "nastaveniProgramu.txt") == false)
                return false;
            // zapsani nastaveni do souboru
            string vlastnost = string.Empty;
            StreamReader sw = new StreamReader(Environment.CurrentDirectory + "\\" + "nastaveniProgramu.txt");

            vlastnost = sw.ReadLine();
            if (vlastnost == "False")
                nastavTestNeopakovat = false;
            else
                nastavTestNeopakovat = true;

            vlastnost = sw.ReadLine();
            if (vlastnost == "False")
                nastavTestVsechnaSlovicka = false;
            else
                nastavTestVsechnaSlovicka = true;

         
            vlastnost = sw.ReadLine();
            if (vlastnost == "False")
                nastavTestRatingSlovicek = false;
            else
                nastavTestRatingSlovicek = true;
           
            vlastnost = sw.ReadLine();
            if (vlastnost == "False")
                nastavRestartStatistiky = false;
            else
                nastavRestartStatistiky = true;

            vlastnost = sw.ReadLine();
            if (vlastnost == "False")
                nastavTestZobrazToolTip = false;
            else
                nastavTestZobrazToolTip = true;

            vlastnost = sw.ReadLine();
            if (vlastnost == "False")
                nastavSpousteniZvuku = false;
            else
                nastavSpousteniZvuku = true;

            vlastnost = sw.ReadLine();
            Pracovni.cestaZalohyAdresar = vlastnost;

            try
            {
                vlastnost = sw.ReadLine();
                nastavRychlostZvuku = Convert.ToInt32(vlastnost);
            }
            catch
            {
                nastavRychlostZvuku = 0;
            }
            //nacteni oznaceni soubory v testu
            string radek = sw.ReadLine();
            //string souborCesta = Pracovni.cesta + "\\" + radek + ".txt";
            //string souborCestaAng = Pracovni.cesta + "\\" + radek + "EN.txt";
            
            while (radek != null)
            {
                string souborCesta = Pracovni.CESTA + "\\" + radek + ".txt";
                string souborCestaAng = Pracovni.CESTA + "\\" + radek + "EN.txt";
                // kontrola kdyby nahodou uzivatel smazal soubory a byly stale v testu...
               if (File.Exists(souborCesta) && File.Exists(souborCestaAng))
                    Pracovni.souboryProTest.Add(radek);
                radek = sw.ReadLine();
            }

            sw.Close();

            return true;
        }

        #endregion



        #region zvukovyPreklad


        /// <summary>
        /// zvukovy preklad pomoci goolu  TEST
        /// </summary>
        /// <param name="slovicko">text,ktery se ma prelozit</param>
        /// <returns></returns>
        public static bool ZvukovyPreklad(string slovicko)
        {
            if (slovicko == string.Empty)
                return false;

            WebClient web = new WebClient();

            web.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 9.0; Windows;)");

            string encstr = string.Empty;

            string filename = "preklad.mp3"; //could also be tts.wav

            string s = slovicko;

            encstr = Uri.EscapeDataString(s);

            Console.WriteLine(encstr);

            web.DownloadFile("http://translate.google.com/translate_tts?tl=en&q=" + encstr, ".\\" + filename);
                    
           
            return true;
        }


        /// <summary>
        /// prehrani slovicka pomoci syntetickeho hlasu
        /// </summary>
        /// <param name="slovicko">slovicko co se ma prehrat</param>
        /// <param name="spust">prehraje se i kdyz neni zaply zvuk</param>
        public static void ZvukPrehrajSlovicko(string slovicko, bool spust)
        {
            if (nastavSpousteniZvuku || spust)
            {
                SpVoice voice = new SpVoice();
                voice.Rate = nastavRychlostZvuku;   
                
                     
              

                if (slovicko.IndexOf(",")!= -1)
                {
                    List<string> slovicka = new List<string>();
                    string text = slovicko;

                    while (text.IndexOf(",") != -1)
                    {
                        slovicka.Add(text.Substring(0,text.IndexOf(",")).Trim());
                        text = text.Substring(text.IndexOf(",") + 1, text.Length - text.IndexOf(",") -1  );
                    }
                    slovicka.Add(text.Substring(0, text.Length  ).Trim());

                    while (slovicka.Count > 0)
                    {
                        voice.Speak(slovicka[0], SpeechVoiceSpeakFlags.SVSFDefault);
                        slovicka.RemoveAt(0);
                    }
                }
                else
                {
                    //voice.Voice = voice.GetVoices("Name=Microsoft mary", "Language=409").Item(0);
                    voice.Speak(slovicko, SpeechVoiceSpeakFlags.SVSFDefault);
                }
            }

        }


          #endregion
    }
}
