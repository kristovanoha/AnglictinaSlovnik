using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Diagnostics;
//using AdvancedTools;
//using AdvancedTools;

namespace AnglictinaSlovicka
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            cas.Start();
            this.tblSearchWords.CellContentClick += new DataGridViewCellEventHandler(vyberPouzeJednaMoznost);           
        }



        #region Variables

        public string _fileNameAnalysis = string.Empty;

        public List<string> _filesNameAnalysis = new List<string>();

        #endregion


        Stopwatch stopwatch = new Stopwatch();
        ToolTip toolTipy = new ToolTip();

        /// <summary>
        /// nacteni formulare nacteni vsech dat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            Pracovni.restartVsechPromenych();
            Cursor = Cursors.WaitCursor;
            //TODO ...nacitani
           // nacteniSouboruSlovniku(string.Empty);
           // Pracovni.souboryVse = Pracovni.seznamSouboru(string.Empty);

            //zalozeni souboru forbiden
            Pracovni.VytvorJednoduchySoubor(Pracovni.FORBIDDEN_WORDS);

            Pracovni.seznamSouboru(string.Empty);
            Pracovni.SlovickaDoListuForbidden();

            
            Pracovni.VytvoreniSeznamuVsechna();
            NacteniVsechSlovicekDoSlovniku();
            nacteniSouboruSlovniku(string.Empty);
            ZmenaStavu(-1);

            //nachystani testu...
            if (Pracovni.nactiNastaveniProgramu())
                NastaveniCheckBoxu();

            //Pracovni.SestaveniSlovicekProTest(1);
            SpusteniTestuPriStartu();
            StatistickeCisla();
            txbTestParPocetSlov.Text = Pracovni.PocetTestovanychSlov(false).ToString();


            fillToFileComboBox(false);

            //For test
            

            toolTipy.SetToolTip(btnRestartTestu, "Provede restartování testu.");
            Cursor = Cursors.Arrow;
            stopwatch.Start();
        }


        //zalozeni noveho souboru....musi byt vyplnen nazev
        private void btnNovySoubor_Click(object sender, EventArgs e)
        {
            zalozNovySoubor();           
    
        }


        // nazev noveho souboru
        private void txbNovySoubor_TextChanged(object sender, EventArgs e)
        {
            if (txbNovySoubor.Text != string.Empty)
                btnNovySoubor.Enabled = true;
            else
                btnNovySoubor.Enabled = false;
            }


        // klikanec na tabulku souboru
        private void viewSouborySlovnik_MouseClick(object sender, MouseEventArgs e)
        {
            
            viewSouborySlovnik[0, Pracovni.radekAktTabulkaSouboruSlovnik].Value = 0;
            viewSouborySlovnik[0, viewSouborySlovnik.CurrentRow.Index].Value = 1;
            Pracovni.radekAktTabulkaSouboruSlovnik = viewSouborySlovnik.CurrentRow.Index;
            ZmenaStavu(0);
        }


        /// <summary>
        /// nacteni obsahu souboru do slovniku soubory
        /// </summary>
        private void nacteniSouboruSlovniku(string novySoubor)
        {
            
            //nacteni souboru do listu ...vsechny soubory

            if (Pracovni.radekAktTabulkaSouboruSlovnik > -1 && viewSouborySlovnik.RowCount>0)
                viewSouborySlovnik[0, Pracovni.radekAktTabulkaSouboruSlovnik].Value = 0;

            List<string> obsahAdresare = Pracovni.seznamSouboru(string.Empty);

            viewSouborySlovnik.RowCount = obsahAdresare.Count;
                        
            viewSouboryNastaveni.RowCount = obsahAdresare.Count;

            for (int index = 0; index < obsahAdresare.Count; index++)
            {
                if (novySoubor == Path.GetFileName(obsahAdresare[index]).Replace(".txt",""))
                {
                    Pracovni.radekAktTabulkaSouboruSlovnik = index;
                }

                viewSouborySlovnik.Rows[index].Cells[1].Value = Path.GetFileName(obsahAdresare[index].Replace(".txt", ""));
                //nacteni souboru do nastaveni
                 viewSouboryNastaveni.Rows[index].Cells[1].Value = Path.GetFileName(obsahAdresare[index].Replace(".txt", ""));
                 NacteniPoctuSlovicekNastaveni();
            }
            

            // odznaceni slovnik soubory...zadny radek na zacatku nesmi byt oznacen
            viewSouborySlovnik.ClearSelection();

            if (novySoubor != string.Empty)
            {
                viewSouborySlovnik[0,Pracovni.radekAktTabulkaSouboruSlovnik].Value = 1;
                viewSouborySlovnik.Rows[Pracovni.radekAktTabulkaSouboruSlovnik].Selected = true;
                viewSouborySlovnik.FirstDisplayedScrollingRowIndex = Pracovni.radekAktTabulkaSouboruSlovnik;
            }

        }


        /// <summary>
        /// nacteni vsech slovicek do slovniku
        /// </summary>
        private void NacteniVsechSlovicekDoSlovniku()
        {

            viewSlovickaSlovnik.RowCount = Pracovni.slovickaVseKomplet.Count;
            for (int index = 0; index < Pracovni.slovickaVseKomplet.Count; index++)
            {
                viewSlovickaSlovnik.Rows[index].Cells[1].Value = Pracovni.slovickaVseKomplet[index];
                viewSlovickaSlovnik.Rows[index].Cells[0].Value = Pracovni.slovickaVseKompletEN[index];
                viewSlovickaSlovnik.Rows[index].Cells[2].Value = Pracovni.slovickaVseKompletRating[index];
                viewSlovickaSlovnik.Rows[index].Cells[3].Value = Pracovni.slovickaVseKompletSoubor[index].Replace(".txt","");
            }

        }


        //enter na textfildu novy soubor
        private void txbNovySoubor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && txbNovySoubor.Text != string.Empty)
            {
                zalozNovySoubor();
                e.Handled = true;
            }
        }


        /// <summary>
        /// zalozeni noveho souboru slovicek
        /// </summary>
        private void zalozNovySoubor()
        {

            if (Pracovni.vytvorSoubor(txbNovySoubor.Text))
            {
                
                string novySoubor = txbNovySoubor.Text;
                txbPopisSlovnik.Text = "Byl úspěšně založen soubor " + txbNovySoubor.Text + ".txt ";
                btnNovySoubor.Enabled = false;
                txbNovySoubor.Text = string.Empty;

                txbAnglickyPreklad.Text = string.Empty;
                txbCeskyPreklad.Text = string.Empty;

                Pracovni.aktSouborNoveSlovo = novySoubor;

                nacteniSouboruSlovniku(novySoubor);
                viewSlovickaSlovnik.ClearSelection();
            }
            else
                txbPopisSlovnik.Text = "Nepodařilo se založit " + txbNovySoubor.Text + ".txt ";

        }


        /// <summary>
        /// tlacitko obsah TEST
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Pracovni.spravnaOdpovedTestCeska = nastavAnglickePrekladyTestu();
        }


        /// <summary>
        /// vyber slovicka ve slovniku... pro editaci
        /// </summary>
        /// <param name="sender"></param>       
        private void viewSlovickaSlovnik_MouseClick(object sender, MouseEventArgs e)
        {   
            txbAnglickyPreklad.Text = viewSlovickaSlovnik.Rows[viewSlovickaSlovnik.CurrentRow.Index].Cells[0].Value + "";
            txbCeskyPreklad.Text = viewSlovickaSlovnik.Rows[viewSlovickaSlovnik.CurrentRow.Index].Cells[1].Value + "";

            Pracovni.aktSlovickoUpravaEN = viewSlovickaSlovnik.Rows[viewSlovickaSlovnik.CurrentRow.Index].Cells[0].Value + "";
            Pracovni.aktSouborUprava = viewSlovickaSlovnik.Rows[viewSlovickaSlovnik.CurrentRow.Index].Cells[3].Value + "";
            ZmenaStavu(1);
            //viewSouborySlovnik.ClearSelection();
        }


        /// <summary>
        /// zmena stavu na novy, upravuje tlacitko
        /// </summary>       
        private void ZmenaStavu(int stavNovy)
        {
            Pracovni.stavAkce = stavNovy;
            if (Pracovni.stavAkce == -1)
            {
                Pracovni.stavAkce = 0;
              viewSouborySlovnik.ClearSelection();

                if (viewSouborySlovnik.RowCount > 0)
                {
                    viewSouborySlovnik[0,0].Value = 1;
                    viewSouborySlovnik.Rows[0].Selected = true;
                }
            }
            else if (Pracovni.stavAkce == 0)
            {
                btnVlozitSlovnik.Text = "Vložit nové slovíčko";
                btnSmaz.Enabled = false;

                viewSouborySlovnik.Rows[Pracovni.radekAktTabulkaSouboruSlovnik].Selected = true;
                viewSlovickaSlovnik.ClearSelection();
            }
            else if (Pracovni.stavAkce == 1)
            {
                btnVlozitSlovnik.Text = "Upravit slovíčko";
                btnSmaz.Enabled = true;
                viewSouborySlovnik.ClearSelection();
            }
            else
            {
                btnVlozitSlovnik.Text = "Vložit nové slovíčko";
                btnSmaz.Enabled = false;
                viewSlovickaSlovnik.ClearSelection();
            }
        }


        /// <summary>
        /// vlozeni ci zmena slovicka....do slovniku a konkretniho souboru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVlozitSlovnik_Click(object sender, EventArgs e)
        {
            VkladEditaceSlovicka();
        }


        /// <summary>
        /// vklad nebo zmena slovicka do slovniku...
        /// </summary>
        private void VkladEditaceSlovicka()
        {

            if (Pracovni.aktSlovickoUpravaEN == null)
                Pracovni.aktSlovickoUpravaEN = " ";

            //skrol na to slovicko ve slovniku
            if (Pracovni.ExistujeSlovicko(txbAnglickyPreklad.Text.Trim()))
            {
                int rowExistSlovicko = Pracovni.ExistujeSlovickoRadek(txbAnglickyPreklad.Text.Trim());

                viewSlovickaSlovnik.Rows[rowExistSlovicko].Selected = true;
                viewSlovickaSlovnik.FirstDisplayedScrollingRowIndex = rowExistSlovicko;
            }

            //TODO kontrola zda neexistuje ,ci zda neni prazdne...
            if (Pracovni.ExistujeSlovicko(txbAnglickyPreklad.Text.Trim()) 
                && txbAnglickyPreklad.Text.Trim() != Pracovni.aktSlovickoUpravaEN.Trim())
            {
                txbPopisSlovnik.Text = "Toto anglické slovíčko již ve slovníku existuje!  Nelze uložit!";
                Pracovni.pocetCasu2 = 0;


            }
            else
            {  // muzeme provest
                bool ok = false;
                if (Pracovni.stavAkce == 1)
                {
                    string slovicko = Pracovni.aktSlovickoUpravaEN;
                    ok = Pracovni.UpravaNovaSlovicka(Pracovni.aktSouborUprava, Pracovni.aktSlovickoUpravaEN, txbAnglickyPreklad.Text.Trim(), txbCeskyPreklad.Text.Trim(), true, false, 0);
                    if (ok)
                         txbPopisSlovnik.Text = "Bylo upraveno slovíčko - " + slovicko;
                    Pracovni.pocetCasu2 = 0;
                }
                if (Pracovni.stavAkce == 2)
                {
                    string slovicko = txbAnglickyPreklad.Text.Trim();
                    ok = Pracovni.UpravaNovaSlovicka(Pracovni.aktSouborNoveSlovo, null, txbAnglickyPreklad.Text.Trim(), txbCeskyPreklad.Text.Trim(), false, false, 0);
                    if (ok)
                        txbPopisSlovnik.Text = "Bylo vloženo nové slovíčko: " + slovicko;
                    Pracovni.pocetCasu2 = 0;
                }
                if (ok)
                {
                    //opetovne nacteni ... jeste zmenit
                    Pracovni.SlovickaDoListuForbidden();
                    Pracovni.VytvoreniSeznamuVsechna();
                    NacteniVsechSlovicekDoSlovniku();

                    txbAnglickyPreklad.Text = string.Empty;
                    txbCeskyPreklad.Text = string.Empty;
                    viewSlovickaSlovnik.ClearSelection();
                   // Pracovni.stavAkce = 0;
                    ZmenaStavu(0);
                    NacteniPoctuSlovicekNastaveni();
                }
            }

        }


        /// <summary>
        /// Znovu načteni všech sloviček
        /// </summary>
        public void ZnovuNacteniVsechSlov()
        {
            Pracovni.SlovickaDoListuForbidden();
            Pracovni.VytvoreniSeznamuVsechna();
            NacteniVsechSlovicekDoSlovniku();
            NacteniPoctuSlovicekNastaveni();
        }



        // změna textu u anglického výkladu.... jedna se o nove slovo tak stav 2
        private void txbAnglickyPreklad_TextChanged(object sender, EventArgs e)
        {
            if (txbAnglickyPreklad.Text != string.Empty && Pracovni.stavAkce != 1)
            {
                btnVlozitSlovnik.Text = "Vložit nové slovíčko";
                Pracovni.stavAkce = 2;
              //  ZmenaStavu(0);
              
            }


        }


        //kliknuti na soubory kvuli novemu slovicku
        private void viewSouborySlovnik_Click(object sender, EventArgs e)
        {
            Pracovni.aktSouborNoveSlovo = viewSouborySlovnik.Rows[viewSouborySlovnik.CurrentRow.Index].Cells[1].Value + "";
         //  viewSlovickaSlovnik.ClearSelection();
            txbAnglickyPreklad.Text = string.Empty;
            txbCeskyPreklad.Text = string.Empty;
            //Pracovni.stavAkce = 2;
            ZmenaStavu(2);
        }


        // smazani slovicka
        private void btnSmaz_Click(object sender, EventArgs e)
        {
            bool ok = false;
            if (Pracovni.stavAkce == 1)
            {
                string slovicko = Pracovni.aktSlovickoUpravaEN;
                ok = Pracovni.UpravaNovaSlovicka(Pracovni.aktSouborUprava, Pracovni.aktSlovickoUpravaEN, txbAnglickyPreklad.Text.Trim(), txbCeskyPreklad.Text.Trim(), true, true, 0);
                if (ok)
                    txbPopisSlovnik.Text = "Bylo smazáno slovíčko: " + slovicko;
                Pracovni.pocetCasu2 = 0;
            }
                //opetovne nacteni ... jeste zmenit
            if (ok)
            {
                Pracovni.SlovickaDoListuForbidden();
                Pracovni.VytvoreniSeznamuVsechna();
                NacteniVsechSlovicekDoSlovniku();

                txbAnglickyPreklad.Text = string.Empty;
                txbCeskyPreklad.Text = string.Empty;
                viewSlovickaSlovnik.ClearSelection();
               // Pracovni.stavAkce = 0;
                ZmenaStavu(0);
                NacteniPoctuSlovicekNastaveni();
            }
        }


        // vyber souboru pro test
        private void viewSouboryNastaveni_MouseClick(object sender, MouseEventArgs e)
        {
            if ((viewSouboryNastaveni[0, viewSouboryNastaveni.CurrentRow.Index].FormattedValue.ToString()) == "True")
            {
                viewSouboryNastaveni[0, viewSouboryNastaveni.CurrentRow.Index].Value = 0;
                Pracovni.SouboryProTest(viewSouboryNastaveni[1, viewSouboryNastaveni.CurrentRow.Index].FormattedValue.ToString(), false);
            }
            else
            {
                viewSouboryNastaveni[0, viewSouboryNastaveni.CurrentRow.Index].Value = 1;
                Pracovni.SouboryProTest(viewSouboryNastaveni[1, viewSouboryNastaveni.CurrentRow.Index].FormattedValue.ToString(), true);          
            }

            chbNastavVsechnaSlova.Checked = false;
            Pracovni.nastavTestVsechnaSlovicka = false;

            txbNastavPocetSlovTest.Text = Pracovni.PocetTestovanychSlov(true).ToString();
        }


        /// <summary>
        /// nacteni poctu slovicek v nastaveni a slovnik do view....... kdyz zmazu,nebo pridam slovicko
        /// </summary>
        private void NacteniPoctuSlovicekNastaveni()
        {

            List<string> obsahAdresare = Pracovni.seznamSouboru(string.Empty);

            for (int index = 0; index < obsahAdresare.Count; index++)
            {
                 viewSouboryNastaveni.Rows[index].Cells[2].Value = Pracovni.slovickaPocet[index].ToString() + "";
                 viewSouborySlovnik.Rows[index].Cells[2].Value = Pracovni.slovickaPocet[index].ToString() + "";                 
                
            }
        }


        /// <summary>
        /// restartovani testu....
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVyhodnoceni_Click(object sender, EventArgs e)
        {
            RestartTestuSpusteni();
        }


        /// <summary>
        /// pri startu aplikace se spusti test...jestli jsou nejeke slovicka
        /// </summary>
        private void SpusteniTestuPriStartu()
        {
            Pracovni.SestaveniSlovicekProTest(1);

            string ang = string.Empty;
            string ces = string.Empty;
            Pracovni.NahodnaSlovicka(ref ang, ref ces);
            Pracovni.testAngPreklad = ang;
            txbTestCesky.Text = ces;
            txbTestCeskySlov.Text = ces;
            StatistickeCisla();

            string popisTestu = string.Empty;
            if (Pracovni.nastavTestVsechnaSlovicka)
                popisTestu = popisTestu + "Všechna slovíčka v testu. ";
            else
                popisTestu = popisTestu + "Výběr slovíček podle souborů. ";

            if (Pracovni.nastavTestNeopakovat)
                popisTestu = popisTestu + "Slovíčka se neopakují. ";
            else
                popisTestu = popisTestu + "Slovíčka se opakují. ";


            if (Pracovni.nastavTestRatingSlovicek)
                popisTestu = popisTestu + "Výběr ovlivňuje rating slovíček. ";
            else
                popisTestu = popisTestu + "Výběr náhodný. ";


            txbTestPopisTestu.Text = popisTestu;

            if (Pracovni.PocetTestovanychSlov(false) == 0)
            {
                if (Pracovni.PocetTestovanychSlov(true) == 0)
                    txbTestUpozorneni.Text = "V testu nejsou vybrány žádná slovíčka !";
            }
            txbTestParPocetSlov.Text = Pracovni.PocetTestovanychSlov(false).ToString();
            if (Pracovni.testAngPreklad != string.Empty && txbTestUpozorneni.Text == string.Empty)
            {
                lblVyhodnoceniTest.Text = "Napiš anglický překlad";                
            }
        }


        /// <summary>
        /// restartovani testu
        /// </summary>
        private void RestartTestuSpusteni()
        {
            Pracovni.SestaveniSlovicekProTest(1);

            string ang = string.Empty;
            string ces = string.Empty;
            Pracovni.NahodnaSlovicka(ref ang, ref ces);
            Pracovni.testAngPreklad = ang;
            txbTestAngVzor.Text = string.Empty;
            txbTestAngVzorSlov.Text = string.Empty;
            txbTestAnglicky.Text = string.Empty;
            txbTestCesky.Text = ces;
            txbTestCeskySlov.Text = ces;
            txbTestUpozorneni.Text = string.Empty;

            string popisTestu = string.Empty;
            Pracovni.testAktStav = 0;
            Pracovni.testOpravaSlova = false;

            if (Pracovni.nastavTestVsechnaSlovicka)
                popisTestu = popisTestu + "Všechna slovíčka v testu. ";
            else
                popisTestu = popisTestu + "Výběr slovíček podle souborů. ";

            if (Pracovni.nastavTestNeopakovat)
                popisTestu = popisTestu + "Slovíčka se neopakují. ";
            else
                popisTestu = popisTestu + "Slovíčka se opakují. ";


            if (Pracovni.nastavTestRatingSlovicek)
                popisTestu = popisTestu + "Výběr ovlivňuje rating slovíček. ";
            else
                popisTestu = popisTestu + "Výběr náhodný. ";


            txbTestPopisTestu.Text = popisTestu;
            txbTestParPocetSlov.Text = Pracovni.PocetTestovanychSlov(false).ToString();
            if (Pracovni.testAngPreklad != string.Empty && txbTestUpozorneni.Text == string.Empty)
            {
                lblVyhodnoceniTest.Text = "Napiš anglický překlad";               
            }
            if (txbTestUpozorneni.Text != string.Empty)
            {
                lblVyhodnoceniTest.Text = string.Empty;               
            }
            if (Pracovni.nastavRestartStatistiky)
            {
                Pracovni.statPocetCelkem = 0;
                Pracovni.statPocetNeuspech = 0;
                Pracovni.statPocetUspech = 0;
                Pracovni.statProcentUspech = 0;
            }
            StatistickeCisla();


            // anglicky do cestiny
            txbTestCeskyAngSlov.Text = Pracovni.testAngPreklad;

            Pracovni.opravaOdpovedTestCeska = false;
            nastavAnglickePrekladyTestu();
            testPouzeMozneOdpovediCesky(666);


            // upozorneni na restart
            //if (Pracovni.PocetTestovanychSlov(false) == 0)
            //{
            //    if (Pracovni.PocetTestovanychSlov(true) == 0)
            //        txbTestUpozorneni.Text = "V testu nejsou žádná slovíčka! Nastav Test!";
            //    else
            //        txbTestUpozorneni.Text = "V testu již nejsou slovíčka (neopakují se). Restartuj test !";

            //}
            vycerpaniSlovicekVTestu();
        }


        /// <summary>
        ///  zmena tabulatoru ....test, slovnik , nastaveni....
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabHlavniPanel_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (tabHlavniPanel.SelectedIndex == 0) // test
            {
                RestartTestuSpusteni();
                stopwatch.Start();
            }
            else
            {
                stopwatch.Stop();
            }

            if (tabHlavniPanel.SelectedIndex == 1) // slovnik
            {
                // obnoveni slovniku...muze byt zmena ratingu z testu
                Pracovni.seznamSouboru(string.Empty);
                Pracovni.SlovickaDoListuForbidden();
                Pracovni.VytvoreniSeznamuVsechna();
                NacteniVsechSlovicekDoSlovniku();
            }
            if (tabHlavniPanel.SelectedIndex == 2) // nastaveni
            {
                txbNastavPocetSlovCelkem.Text = Pracovni.nastavPocetSlovicekCelkem.ToString();
                txbNastavPocetSlovTest.Text = Pracovni.PocetTestovanychSlov(true).ToString();
            }
        }


        //zmena nebo nove slovicko ENTER
        private void txbAnglickyPreklad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 )
            {
                VkladEditaceSlovicka();
                e.Handled = true;
            }
        }


        // zmena nebo nove slovicko ENTER
        private void txbCeskyPreklad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 )
            {
                VkladEditaceSlovicka();
                e.Handled = true;
                txbAnglickyPreklad.Focus();
            }
        }


        /// <summary>
        /// hadani anglickeho prekladu... ENTER
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbTestAnglicky_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                VyhodnoceniTestSlovicka(false, false);  
                e.Handled = true;
 
            }
        }


        /// <summary>
        /// vypis statistickych cisel v testu ....uspechy, neuspechy... atd
        /// </summary>
        private void StatistickeCisla()
        {
            txbStatCelkem.Text = Pracovni.statPocetCelkem + "";
            txbStatUspech.Text = Pracovni.statPocetUspech + "";
            txbStatNeuspech.Text = Pracovni.statPocetNeuspech + "";
            txbStatProcento.Text = Pracovni.statProcentUspech.ToString("0%") ;
  
        }


        /// <summary>
        /// vyhodnoceni slovicka zda jsme uhadli...
         /// </summary>
         /// <param name="slovniHodnoceni">true - jedna se slovni hodnoceni</param>
         /// <param name="spravnostOdpovedi">true - spravna odpoved, false - spatna odpoved</param>
        private void VyhodnoceniTestSlovicka(bool slovniHodnoceni, bool spravnostOdpovedi)
        {
            //TODO co velikost pismen...nechat, ci zmenit
            if ((txbTestAnglicky.Text != string.Empty || slovniHodnoceni) && Pracovni.testAngPreklad != null)
            {
                if (txbTestAnglicky.Text.Trim().ToLower() == Pracovni.testAngPreklad.ToLower()
                    || (slovniHodnoceni  )
                    ) //spravne
                {

                    if (Pracovni.testOpravaSlova )
                    {
                        Pracovni.testOpravaSlova = false;
                        lblVyhodnoceniTest.Text = "Správně opraveno.";
                       // lblVyhodnoceniTestCesky.Text = "Správně opraveno";
                        Pracovni.testAktStav = 2;
                        Pracovni.ZvukPrehrajSlovicko(Pracovni.testAngPreklad, false);
                    }
                    else
                    {
                        lblVyhodnoceniTest.Text = "Správně !";
                        lblVyhodnoceniTestCesky.Text = "Správně !";
                        Pracovni.statPocetCelkem++;                       

                        if (slovniHodnoceni && spravnostOdpovedi == false)
                        {
                            Pracovni.statPocetNeuspech++;
                        }
                        else
                        {
                            Pracovni.statPocetUspech++;
                        }

                        if (Pracovni.statPocetCelkem > 0)
                        {
                            Pracovni.statProcentUspech = Pracovni.statPocetUspech / Pracovni.statPocetCelkem;
                        }

                        if (slovniHodnoceni && spravnostOdpovedi == false)
                        {
                            Pracovni.ZmenaRatinguSlovicka(Pracovni.testAngPreklad, -2);
                        }
                        else
                        {
                            Pracovni.ZmenaRatinguSlovicka(Pracovni.testAngPreklad, 1);
                            Pracovni.OdebraniSlovaTest(Pracovni.testAngPreklad);
                        }

                        Pracovni.testAktStav = 1;
                        
                        Pracovni.ZvukPrehrajSlovicko(Pracovni.testAngPreklad, false);
                    }


                    
                    string ang = string.Empty;
                    string ces = string.Empty;
                    Pracovni.NahodnaSlovicka(ref ang, ref ces);
                    Pracovni.testAngPreklad = ang;
                    txbTestCesky.Text = ces;
                    txbTestCeskySlov.Text = ces;

                    txbTestAnglicky.Text = string.Empty;
                    txbTestAngVzor.Text = string.Empty;
                    txbTestAngVzorSlov.Text = string.Empty;
                    txbTestParPocetSlov.Text = Pracovni.PocetTestovanychSlov(false).ToString();


                    vycerpaniSlovicekVTestu();
                    //if (Pracovni.PocetTestovanychSlov(false) == 0)
                    //{
                    //    if (Pracovni.PocetTestovanychSlov(true) == 0)
                    //        txbTestUpozorneni.Text = "V testu nejsou žádná slovíčka! Nastav Test!";
                    //    else
                    //        txbTestUpozorneni.Text = "V testu již nejsou slovíčka (neopakují se). Restartuj test !";
                    //}
                }
                else // chyba
                {
                    txbTestAngVzor.Text = Pracovni.testAngPreklad;
                   // txbTestAngVzorSlov.Text = Pracovni.testAngPreklad;

                    if (Pracovni.testOpravaSlova == false)
                    {
                        Pracovni.statPocetCelkem++;
                        Pracovni.statPocetNeuspech++;
                        if (Pracovni.statPocetCelkem > 0)
                        {
                            Pracovni.statProcentUspech = Pracovni.statPocetUspech / Pracovni.statPocetCelkem;
                        }
                        Pracovni.ZmenaRatinguSlovicka(Pracovni.testAngPreklad, -2);
                    }

                        Pracovni.testAktStav = 3; 
                        Pracovni.testOpravaSlova = true;

                  
                       
                    lblVyhodnoceniTest.Text = "Chyba. Oprav anglický překlad !";                    
                }


                StatistickeCisla();
            }
        }


        /// <summary>
        /// nastaveni testu...vsechna slovicka
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbNastavVsechnaSlova_CheckedChanged(object sender, EventArgs e)
        {
            if (Pracovni.nastavTestVsechnaSlovicka)
                Pracovni.nastavTestVsechnaSlovicka = false;
            else
                Pracovni.nastavTestVsechnaSlovicka = true;

            txbNastavPocetSlovTest.Text = Pracovni.PocetTestovanychSlov(true).ToString();
        }


        /// <summary>
        /// nastaveni testu - neopakovat jiz uhodnuta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (Pracovni.nastavTestNeopakovat)
                Pracovni.nastavTestNeopakovat = false;
            else
                Pracovni.nastavTestNeopakovat = true;
        }

        private void chbNastavRestartStatistiky_CheckedChanged(object sender, EventArgs e)
        {
            if (Pracovni.nastavRestartStatistiky)
                Pracovni.nastavRestartStatistiky = false;
            else
                Pracovni.nastavRestartStatistiky = true;
        }


        //casovac zobrazeni hlasek pri vyplnovani
        private void cas_Tick(object sender, EventArgs e)
        {
            txbCasTest.Text = stopwatch.Elapsed.Hours + ":" + stopwatch.Elapsed.Minutes + ":" + stopwatch.Elapsed.Seconds;


            if (Pracovni.testAktStav == 0)
                if (txbTestUpozorneni.Text != string.Empty)
                {
                    lblVyhodnoceniTest.Text = string.Empty;
                    lblVyhodnoceniTestCesky.Text = string.Empty;
                }

            if (Pracovni.testAktStav == 1 || Pracovni.testAktStav == 2)
            {
                Pracovni.pocetCasu++;
                if (Pracovni.pocetCasu == 3)
                {
                    lblVyhodnoceniTest.Text = "Napiš anglický překlad";
                    if (txbTestUpozorneni.Text != string.Empty)
                    {
                        lblVyhodnoceniTest.Text = string.Empty;
                        lblVyhodnoceniTestCesky.Text = string.Empty;
                    }
                    Pracovni.pocetCasu = 0;
                }
            }

            // vynulovani ceskeho testu
            if (lblVyhodnoceniTestCesky.Text != string.Empty)
                Pracovni.pocetCasu3++;
            if (Pracovni.pocetCasu3 > 3)
            {
                Pracovni.pocetCasu3 = 0;
                lblVyhodnoceniTestCesky.Text = string.Empty;
            }


            if (txbPopisSlovnik.Text != string.Empty)
                Pracovni.pocetCasu2++;
            if (Pracovni.pocetCasu2 > 6)
            {
                Pracovni.pocetCasu2 = 0;
                txbPopisSlovnik.Text = string.Empty;
            }
        }


        // zohlednovat rating pri testu
        private void chbNastavRating_CheckedChanged(object sender, EventArgs e)
        {
            if (Pracovni.nastavTestRatingSlovicek)
                Pracovni.nastavTestRatingSlovicek = false;
            else
                Pracovni.nastavTestRatingSlovicek = true;
        }


        //zavreni formulare
        private void Formular_FormClosing(object sender, FormClosingEventArgs e)
        {
            Pracovni.ulozNastaveniProgramu();
        }


        /// <summary>
        /// nastaveni checkboxu ...kdyz se nastaveni nacte ze souboru
        /// </summary>
        private void NastaveniCheckBoxu()
        {
            if (Pracovni.nastavTestNeopakovat)
            {
                Pracovni.nastavTestNeopakovat = false;
                chbNastavNeopakovatSlova.Checked = true;
            }
            if (Pracovni.nastavTestRatingSlovicek)
            {
                Pracovni.nastavTestRatingSlovicek = false;
                chbNastavRating.Checked = true;
            }
            if (Pracovni.nastavRestartStatistiky)
            {
                Pracovni.nastavRestartStatistiky = false;
                chbNastavRestartStatistiky.Checked = true;
            }
            if (Pracovni.nastavTestVsechnaSlovicka)
            {
                Pracovni.nastavTestVsechnaSlovicka = false;
                chbNastavVsechnaSlova.Checked = true;
            }

            if (Pracovni.nastavTestZobrazToolTip)
            {
                Pracovni.nastavTestZobrazToolTip = false;
                chbNastavZobrazTool.Checked = true;
            }

            if (Pracovni.nastavSpousteniZvuku)
            {
                Pracovni.nastavSpousteniZvuku = false;
                chbNastavPrehratZvuk.Checked = true;
            }

            txbCestaZalohy.Text = Pracovni.cestaZalohyAdresar;
         
            trackBarRychlostZvuku.Value = Pracovni.nastavRychlostZvuku;

            //nastaveni
            for (int index = 0; index < viewSouboryNastaveni.RowCount; index++)
            {                
                if (Pracovni.souboryProTest.IndexOf(viewSouboryNastaveni.Rows[index].Cells[1].FormattedValue.ToString())!= -1 )
                {
                    viewSouboryNastaveni.Rows[index].Cells[0].Value = true;
                }
            }
           

        }

        //zobrazit napovedu tooltipovou...
        private void chbNastavZobrazTool_CheckedChanged(object sender, EventArgs e)
        {
            if (Pracovni.nastavTestZobrazToolTip)
            {
                Pracovni.nastavTestZobrazToolTip = false;
                toolTipy.Active = false;
            }
            else
            {
                Pracovni.nastavTestZobrazToolTip = true;
                toolTipy.Active = true;
            }
        }

        private void btnZvuk_Click(object sender, EventArgs e)
        {
          //  Pracovni.PrehrajZvuk();
        }

        //prehravat zvukove slovicko
        private void chbNastavPrehratZvuk_CheckedChanged(object sender, EventArgs e)
        {
            if (Pracovni.nastavSpousteniZvuku)
            {
                Pracovni.nastavSpousteniZvuku = false;
                chbTestZvukZaply.Checked = false;
            }
            else
            {
                Pracovni.nastavSpousteniZvuku = true;
                chbTestZvukZaply.Checked = true;
            }
        }


        //prehrani slovicka
        private void btnPrehrajZvuk_Click(object sender, EventArgs e)
        {
            Pracovni.ZvukPrehrajSlovicko(txbAnglickyPreklad.Text, true);
        }

        //nastaveni rychlosti zvuku
        private void trackBarRychlostZvuku_ValueChanged(object sender, EventArgs e)
        {          
            Pracovni.nastavRychlostZvuku = trackBarRychlostZvuku.Value;
        }


        // zapnuti zvuku v testu
        private void chbTestZvukZaply_CheckedChanged(object sender, EventArgs e)
        {
            if (chbTestZvukZaply.Checked == true)
                chbNastavPrehratZvuk.Checked = true;
            else
                chbNastavPrehratZvuk.Checked = false;
        }


        /// <summary>
        /// zobrazeni anglickeho slovicka
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestAngUkaz_Click(object sender, EventArgs e)
        {
            txbTestAngVzorSlov.Text = Pracovni.testAngPreklad;
        }


        /// <summary>
        /// spravne uhodnul slovicko...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestAngSpravne_Click(object sender, EventArgs e)
        {
            VyhodnoceniTestSlovicka(true, true);
        }


        /// <summary>
        /// spatne uhodnul slovicko
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestAngSpatne_Click(object sender, EventArgs e)
        {
            VyhodnoceniTestSlovicka(true, false);
        }


        /// <summary>
        /// zmena druhu testu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zmenaDruhuTestu(object sender, EventArgs e)
        {
            txbTestCeskyAngSlov.Text = Pracovni.testAngPreklad;
            
            Pracovni.opravaOdpovedTestCeska = false;
            nastavAnglickePrekladyTestu();
            testPouzeMozneOdpovediCesky(666);
        }


        /// <summary>
        /// kdyz dojde k vycerpani vsech slovicek v testu je treba zamezit moznostem vyberu v testech....d
        /// a naopak, kdyz jiz se restartuje test tak znovu obnovit..
        /// true - v poradku, false - nejsou slovicka
        /// </summary>
        private bool vycerpaniSlovicekVTestu()
        {
            // upozorneni na restart
            if (Pracovni.PocetTestovanychSlov(false) == 0)
            {
                if (Pracovni.PocetTestovanychSlov(true) == 0)
                    txbTestUpozorneni.Text = "V testu nejsou žádná slovíčka! Nastav Test!";
                else
                    txbTestUpozorneni.Text = "V testu již nejsou slovíčka (neopakují se). Restartuj test !";


                // nepristupen tlacitek
                btnTestAngUkaz.Enabled = false;
                btnTestAngSpravne.Enabled = false;
                btnTestAngSpatne.Enabled = false;
                btnTestCeskyPreklad1.Enabled = false;
                btnTestCeskyPreklad2.Enabled = false;
                btnTestCeskyPreklad3.Enabled = false;
                btnTestCeskyPreklad4.Enabled = false;
                btnTestCeskyPreklad5.Enabled = false;

                return false;

            }
            else
            {
                // nepristupen tlacitek
                btnTestAngUkaz.Enabled = true;
                btnTestAngSpravne.Enabled = true;
                btnTestAngSpatne.Enabled = true;
                btnTestCeskyPreklad1.Enabled = true;
                btnTestCeskyPreklad2.Enabled = true;
                btnTestCeskyPreklad3.Enabled = true;
                btnTestCeskyPreklad4.Enabled = true;
                btnTestCeskyPreklad5.Enabled = true;

                return true;
            }

            return true;
        }

        /// <summary>
        /// nastaveni anglickeho prekladu (pet slovicek), v testu cesky do anglictiny...
        /// </summary>
        /// <returns>vraci spravnou odpoved</returns>
        private int nastavAnglickePrekladyTestu()
        {

            int pocetSlovVerzi = 5;

            if (pocetSlovVerzi > Pracovni.PocetTestovanychSlov(true))
            {
                pocetSlovVerzi = Pracovni.PocetTestovanychSlov(true);
            }

            Random ran = new Random();

            int spravnaOdpoved = ran.Next(pocetSlovVerzi);

            int falesnaOdpoved = 1;

            List<int> dohromady = new List<int>();
            dohromady.Add(Pracovni.slovickaVseKomplet.IndexOf(txbTestCesky.Text));


            while (dohromady.Count < pocetSlovVerzi) //|| dohromady.Count == Pracovni.nastavPocetSlovicekCelkem)
            {
                falesnaOdpoved = ran.Next(Pracovni.nastavPocetSlovicekCelkem);

                if (dohromady.IndexOf(falesnaOdpoved) == -1)
                {
                    dohromady.Add(falesnaOdpoved);
                }
            }


            int posledni = dohromady.Count - 1;
                
            for (int index = 0; index < dohromady.Count; index++)
            {                
                switch (index)
                {
                    case 0:
                        if (spravnaOdpoved == index)
                        {
                            btnTestCeskyPreklad1.Text = txbTestCesky.Text;
                        }
                        else
                        {
                            btnTestCeskyPreklad1.Text = Pracovni.slovickaVseKomplet[dohromady[posledni]];
                            posledni--;
                        }
                        break;
                    case 1:
                        if (spravnaOdpoved == index)
                        {
                            btnTestCeskyPreklad2.Text = txbTestCesky.Text;
                        }
                        else
                        {
                            btnTestCeskyPreklad2.Text = Pracovni.slovickaVseKomplet[dohromady[posledni]];
                            posledni--;
                        }
                        break;
                    case 2:
                        if (spravnaOdpoved == index)
                        {
                            btnTestCeskyPreklad3.Text = txbTestCesky.Text;
                        }
                        else
                        {
                            btnTestCeskyPreklad3.Text = Pracovni.slovickaVseKomplet[dohromady[posledni]];
                            posledni--;
                        }
                        break;
                    case 3:
                        if (spravnaOdpoved == index)
                        {
                            btnTestCeskyPreklad4.Text = txbTestCesky.Text;
                        }
                        else
                        {
                            btnTestCeskyPreklad4.Text = Pracovni.slovickaVseKomplet[dohromady[posledni]];
                            posledni--;
                        }
                        break;
                    case 4:
                        if (spravnaOdpoved == index)
                        {
                            btnTestCeskyPreklad5.Text = txbTestCesky.Text;
                        }
                        else
                        {
                            btnTestCeskyPreklad5.Text = Pracovni.slovickaVseKomplet[dohromady[posledni]];
                            posledni--;
                        }
                        break;
                    default:
                        break;
                }
            }



            Pracovni.spravnaOdpovedTestCeska = spravnaOdpoved;
            return spravnaOdpoved;
        }


        /// <summary>
        /// vybira pouze mozne odpovedi (ceske), ostatni mizi dokud nevybere spravnou odpovede
        /// </summary>
        private void testPouzeMozneOdpovediCesky(int tlaco)
        {
            if (Pracovni.opravaOdpovedTestCeska)
            {
                switch (tlaco)
                {
                    case 0:
                        btnTestCeskyPreklad1.Enabled = false;
                        break;
                    case 1:
                        btnTestCeskyPreklad2.Enabled = false;
                        break;
                    case 2:
                        btnTestCeskyPreklad3.Enabled = false;
                        break;
                    case 3:
                        btnTestCeskyPreklad4.Enabled = false;
                        break;
                    case 4:
                        btnTestCeskyPreklad5.Enabled = false;
                        break;
                    default:
                        break;
                }

            }
            else
            {
                btnTestCeskyPreklad1.Enabled = true;
                btnTestCeskyPreklad2.Enabled = true;
                btnTestCeskyPreklad3.Enabled = true;
                btnTestCeskyPreklad4.Enabled = true;
                btnTestCeskyPreklad5.Enabled = true;

                vycerpaniSlovicekVTestu();
            }

            
        }


        /// <summary>
        /// vyhodnoceni odpovedi spravneho ceskeho prekladu z vyberu peti
        /// </summary>
        private void testVyhodnoceniCeskehoPrekladu(int cisloTlacitka)
        {
            if (cisloTlacitka == Pracovni.spravnaOdpovedTestCeska)
            {
                if (Pracovni.opravaOdpovedTestCeska)
                {
                    Pracovni.opravaOdpovedTestCeska = false;
                    lblVyhodnoceniTestCesky.Text = "Správně opraveno";
                    testPouzeMozneOdpovediCesky(666);
                }
                else
                {
                    VyhodnoceniTestSlovicka(true, true);
                    lblVyhodnoceniTestCesky.Text = "Správně";
                }
                nastavAnglickePrekladyTestu();
                txbTestCeskyAngSlov.Text = Pracovni.testAngPreklad;
            }
            else
            {
                if (Pracovni.opravaOdpovedTestCeska == false)
                {
                    VyhodnoceniTestSlovicka(true, false);                    
                }
                    lblVyhodnoceniTestCesky.Text = "Špatně! Zkus jinou možnost.";
                    Pracovni.opravaOdpovedTestCeska = true;

            }
            
           
        }

        // tlacitko z hledani spravneho ceskeho prekladu
        private void btnTestCeskyPreklad1_Click(object sender, EventArgs e)
        {
            if (btnTestCeskyPreklad1.Text != string.Empty)
            {
                testVyhodnoceniCeskehoPrekladu(0);
                testPouzeMozneOdpovediCesky(0);
            }
        }

        // tlacitko z hledani spravneho ceskeho prekladu
        private void btnTestCeskyPreklad2_Click(object sender, EventArgs e)
        {
            if (btnTestCeskyPreklad2.Text != string.Empty)
            {
                testVyhodnoceniCeskehoPrekladu(1);
                testPouzeMozneOdpovediCesky(1);
            }
        }

        // tlacitko z hledani spravneho ceskeho prekladu
        private void btnTestCeskyPreklad3_Click(object sender, EventArgs e)
        {
            if (btnTestCeskyPreklad3.Text != string.Empty)
            {
                testVyhodnoceniCeskehoPrekladu(2);
                testPouzeMozneOdpovediCesky(2);
            }
        }

        // tlacitko z hledani spravneho ceskeho prekladu
        private void btnTestCeskyPreklad4_Click(object sender, EventArgs e)
        {
            if (btnTestCeskyPreklad4.Text != string.Empty)
            {
                testVyhodnoceniCeskehoPrekladu(3);
                testPouzeMozneOdpovediCesky(3);
            }
        }

        // tlacitko z hledani spravneho ceskeho prekladu
        private void btnTestCeskyPreklad5_Click(object sender, EventArgs e)
        {
            if (btnTestCeskyPreklad5.Text != string.Empty)
            {
                testVyhodnoceniCeskehoPrekladu(4);
                testPouzeMozneOdpovediCesky(4);
            }
        }
     

        /// <summary>
        /// provedeni zalohy slovniku
        /// </summary>
        private void ProvedeniZalohySlovicek()
        {
 
            //vytvoreni adresare struktura - rok mesic den - (pocet slovicek)  napr. 20120215-(356)
            string jmenoAdresare = string.Empty;
            DateTime aktualniDatum = new DateTime();
            aktualniDatum = DateTime.Now;
           
            if (Pracovni.cestaZalohyAdresar != string.Empty)
            {

                jmenoAdresare = Pracovni.cestaZalohyAdresar + "\\" + aktualniDatum.Date.ToString("yyyyMMdd") + "-(" + Pracovni.nastavPocetSlovicekCelkem + ")";
                if (!Directory.Exists(jmenoAdresare))
                {
                    Directory.CreateDirectory(jmenoAdresare);
                }

                string[] files = Directory.GetFiles(Pracovni.CESTA);
                string destFile = string.Empty;
                string fileName = string.Empty;

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = System.IO.Path.GetFileName(s);
                    destFile = System.IO.Path.Combine(jmenoAdresare, fileName);
                    File.Copy(s, destFile,true);
                }
                // hlaska zaloha byla vytvorena kde ...
                System.Windows.Forms.MessageBox.Show("Zaloha byla vytvořena v adresáři - " + jmenoAdresare);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Nepovedla se provedení zálohy !!!" );
                // nejaka upozornovaci hlaska ze neni vybrana cesta, kde se ma vytvorit zaloha
            }

        }
        

        /// <summary>
        /// provede zalohu slovicek....
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProvestZalohu_Click(object sender, EventArgs e)
        {
            ProvedeniZalohySlovicek();
            
        }


        /// <summary>
        /// restartovani casu na nulu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCasRestart_Click(object sender, EventArgs e)
        {
            stopwatch.Restart();
        }


        /// <summary>
        /// vybrani cesty zalohy pomoci dialogu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCestaZalohyDialog_Click(object sender, EventArgs e)
        {
            dlgCestaZalohyA.SelectedPath = txbCestaZalohy.Text;
            dlgCestaZalohyA.ShowDialog();
            Pracovni.cestaZalohyAdresar = dlgCestaZalohyA.SelectedPath.ToString();
            txbCestaZalohy.Text = dlgCestaZalohyA.SelectedPath.ToString();

        }


        /// <summary>
        /// kontrola zda existuje vubec takovy adresar, do ktereho chceme delat zalohu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbCestaZalohy_Validated(object sender, EventArgs e)
        {
            if (!Directory.Exists(txbCestaZalohy.Text))
            {
                dlgCestaZalohyA.ShowDialog();
                Pracovni.cestaZalohyAdresar = dlgCestaZalohyA.SelectedPath.ToString();
                txbCestaZalohy.Text = dlgCestaZalohyA.SelectedPath.ToString();
            }
            if (!Directory.Exists(txbCestaZalohy.Text))
            {
                txbCestaZalohy.Text = string.Empty;
            }
            else
            {
                Pracovni.cestaZalohyAdresar = dlgCestaZalohyA.SelectedPath.ToString();
            }
        }
   

        //Kontrola zda jiz neexistuje napsane slovicko
        private void txbAnglickyPreklad_Validating(object sender, CancelEventArgs e)
        {
            if (Pracovni.ExistujeSlovicko(txbAnglickyPreklad.Text))
            {
                int rowExistSlovicko = Pracovni.ExistujeSlovickoRadek(txbAnglickyPreklad.Text);

                viewSlovickaSlovnik.Rows[rowExistSlovicko].Selected = true;
                viewSlovickaSlovnik.FirstDisplayedScrollingRowIndex = rowExistSlovicko;
            }
        }



        private OpenFileDialog openFileDialog1 = new OpenFileDialog();
        

        /// <summary>
        /// Výběr souboru k prohledání
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "titulky (*.srt)|*.srt|textove soubory (*.txt)|*.txt";
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _fileNameAnalysis = openFileDialog1.FileName;

                _filesNameAnalysis.Clear();
                foreach (var item in openFileDialog1.FileNames)
                {                   

                    _filesNameAnalysis.Add(item);
                }

                txbCountFiles.Text = _filesNameAnalysis.Count.ToString();
                analyseAndFillTable();

            }
            txbSelectFile.Text = _fileNameAnalysis;
        }


        /// <summary>
        /// analyza a nacteni do tabulky
        /// </summary>
        private void analyseAndFillTable()
        {
            //designForEdit(false);

            //if (!string.IsNullOrEmpty(_fileNameAnalysis))
            if (_filesNameAnalysis.Count> 0)
            {
                Analyze analyze = new Analyze(_filesNameAnalysis, chbOnlyNew.Checked);

                tblSearchWords.RowCount = 0;
                tblSearchWords.RowCount = analyze.FindWords.Count;                

                int index = 0;
                foreach (var item in analyze.FindWords)
                {
                    if (Pracovni.ExistujeSlovicko(item.Key))
                    {
                        tblSearchWords.Rows[index].Cells[0].Value = true;
                        tblSearchWords.Rows[index].Cells[2].ReadOnly = true;
                    }
                    
                    tblSearchWords.Rows[index].Cells[3].Value = (index + 1).ToString();
                    tblSearchWords.Rows[index].Cells[4].Value = item.Key;
                    tblSearchWords.Rows[index].Cells[5].Value = item.Value;
                    tblSearchWords.Rows[index].Cells[7].Value = Pracovni.PrekladSlovicka(item.Key);
                    tblSearchWords.Rows[index].Cells[6].Value = Pracovni.ZdrojovySoubor(item.Key);
   
                    index++;
                }

                txbTotalWords.Text = analyze.TotalWords.ToString();
                txbKnowWords.Text = analyze.KnowWords.ToString();
                txbNewWords.Text = analyze.NewWords.ToString();
                txbForbiddenWords.Text = analyze.ForbidensAllWords.ToString();

                //jednotlive vyskyty
                txbOneTotalWords.Text = analyze.OneTotalWords.ToString();
                txbOneKnowWords.Text = analyze.OneKnowWords.ToString();
                txbOneNewWords.Text = analyze.OneNewWords.ToString();
                txbOneForbiddenWords.Text = analyze.OneForbidensWords.ToString();

                System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog1.FileName);
               
                sr.Close();
            }
        }


        /// <summary>
        /// naplneni tabulky z existujich prekladovych souboru
        /// </summary>
        private void fillTableByFile()
        {           

            tblSearchWords.RowCount = 0;
            List<string> slova = new List<string>();
            string nazevSouboru = cmbIntoFile.Text;
            if (!string.IsNullOrEmpty(cmbIntoFile.Text))
            {
                if (Pracovni.FORBIDDEN_WORDS == nazevSouboru)
                {
                    nazevSouboru += ".txt";
                }
                else
                {
                    nazevSouboru += "EN.txt";
                }
                slova = Pracovni.SlovickaDoListuJednotlive(nazevSouboru);
            }

            int index = 0;
            tblSearchWords.RowCount = slova.Count;
            foreach (var item in slova)
            {
               tblSearchWords.Rows[index].Cells[3].Value = (index + 1).ToString();
               tblSearchWords.Rows[index].Cells[4].Value = item;
               tblSearchWords.Rows[index].Cells[6].Value = Pracovni.ZdrojovySoubor(item);
               tblSearchWords.Rows[index].Cells[7].Value = Pracovni.PrekladSlovicka(item);
               index++;
            }

            txbTotalWords.Text = slova.Count.ToString();
            //designForEdit(true);
        }


        /// <summary>
        /// Fill table filter words not edit
        /// </summary>
        private void fillTableByFilter()
        {
            tblSearchWords.RowCount = 0;

            Analyze analyze = new Analyze(_filesNameAnalysis, true);

            int index = 0;

            if (chbFiltrForbid.Checked)
            {
                tblSearchWords.RowCount = analyze.FindOneForbidden.Count;
                foreach (var item in analyze.FindOneForbidden)
                {
                    tblSearchWords.Rows[index].Cells[3].Value = (index + 1).ToString();
                    tblSearchWords.Rows[index].Cells[4].Value = item.Key;
                    tblSearchWords.Rows[index].Cells[5].Value = item.Value;
                    tblSearchWords.Rows[index].Cells[6].Value = "FORBIDDEN";
                    // tblSearchWords.Rows[index].Cells[7].Value = Pracovni.PrekladSlovicka(item);
                    index++;
                }
            }

            if (chbFiltrS.Checked) // s filter
            {
                tblSearchWords.RowCount += analyze.FindOneForbFilter2.Count;
                foreach (var item in analyze.FindOneForbFilter2)
                {
                    tblSearchWords.Rows[index].Cells[3].Value = (index + 1).ToString();
                    tblSearchWords.Rows[index].Cells[4].Value = item.Key;
                    tblSearchWords.Rows[index].Cells[5].Value = item.Value;
                    tblSearchWords.Rows[index].Cells[6].Value = "Množné";
                    index++;
                }
            }

            if (chbFiltrCarka.Checked) // carka
            {
                tblSearchWords.RowCount += analyze.FindOneForbFilter1.Count;
                foreach (var item in analyze.FindOneForbFilter1)
                {
                    tblSearchWords.Rows[index].Cells[3].Value = (index + 1).ToString();
                    tblSearchWords.Rows[index].Cells[4].Value = item.Key;
                    tblSearchWords.Rows[index].Cells[5].Value = item.Value;
                    tblSearchWords.Rows[index].Cells[6].Value = "čárka";                   
                    index++;
                }
            }

           // txbTotalWords.Text = slova.Count.ToString();
        }


        /// <summary>
        /// Napln combo box vyberu souboru kde se bude ukladat
        /// </summary>
        private void fillToFileComboBox(bool useForbidden)
        {
            cmbIntoFile.Items.Clear();
            if (useForbidden)
            {
                cmbIntoFile.Items.Add(Pracovni.FORBIDDEN_WORDS);
            }

            foreach (var item in Pracovni.souboryVse)
            {
                cmbIntoFile.Items.Add(item.ToString().Replace(".txt", string.Empty));
            }

            if (useForbidden)
            {
                cmbIntoFile.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Uložení změn.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnalyzeSave_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (!chbEditaceFiles.Checked)  //uprava z nahodneho souboru
            {
                if (cmbIntoFile.SelectedItem == null)
                {
                    MessageBox.Show("Vyber soubor kde se budou ukladat nové slovička");

                }
                else
                {
                    string souborNovychSlov = cmbIntoFile.SelectedItem.ToString();

                    for (int i = 0; i < tblSearchWords.RowCount; i++)
                    {

                        string slovo = tblSearchWords.Rows[i].Cells[4].Value.ToString().ToLower();

                        //zakázaná slova
                        bool? isForbidden = (bool?)((DataGridViewCheckBoxCell)tblSearchWords.Rows[i].Cells[2]).Value;

                        if (isForbidden == true)
                        {
                            // var forbiddenWord = tblSearchWords.Rows[i].Cells[4].Value;
                            Pracovni.forbiddenWords.Add(slovo);
                        }

                        //uprava existujicich
                        bool? isNewUpdated = (bool?)((DataGridViewCheckBoxCell)tblSearchWords.Rows[i].Cells[1]).Value;
                        if (isNewUpdated == true)
                        {
                            bool? isExist = (bool?)((DataGridViewCheckBoxCell)tblSearchWords.Rows[i].Cells[0]).Value;
                            var newUpdatedWorld = tblSearchWords.Rows[i].Cells[4].Value.ToString().ToLower();
                            string preklad = tblSearchWords.Rows[i].Cells[7].Value.ToString().ToLower();

                            if (isExist == true)
                            {
                                string soubor = tblSearchWords.Rows[i].Cells[6].Value.ToString();
                                Pracovni.UpravaNovaSlovicka(soubor, slovo, slovo, preklad, true, false, 0);
                            }
                            else
                            {
                                Pracovni.UpravaNovaSlovicka(souborNovychSlov, slovo, slovo, preklad, false, false, 0);
                            }
                        }
                    }

                    Pracovni.UlozeniDoSouboru(Pracovni.forbiddenWords, Pracovni.FORBIDDEN_WORDS);

                    ZnovuNacteniVsechSlov();
                    analyseAndFillTable();
                }
            }
            else  // uprava ze zdrojovych souboru
            {
                string souborNovychSlov = cmbIntoFile.SelectedItem.ToString();
                bool isForbidenFile = souborNovychSlov == Pracovni.FORBIDDEN_WORDS ? true : false;


                for (int i = 0; i < tblSearchWords.RowCount; i++)
                {
                    string slovo = tblSearchWords.Rows[i].Cells[4].Value.ToString().ToLower();

                    //zakázaná slova
                    bool? isForbidden = (bool?)((DataGridViewCheckBoxCell)tblSearchWords.Rows[i].Cells[2]).Value;

                    if (isForbidden == true)  //vymazat
                    {
                        if (isForbidenFile)
                        {
                            Pracovni.forbiddenWords.Remove(slovo);
                        }
                        else
                        {
                            Pracovni.UpravaNovaSlovicka(souborNovychSlov, slovo, slovo, "neco", false, true, 0);
                        }
                    }                   

                    //uprava existujicich
                    bool? isNewUpdated = (bool?)((DataGridViewCheckBoxCell)tblSearchWords.Rows[i].Cells[1]).Value;
                    if (isNewUpdated == true && !isForbidenFile)
                    {
                        string preklad = tblSearchWords.Rows[i].Cells[7].Value.ToString().ToLower();
                        string soubor = tblSearchWords.Rows[i].Cells[6].Value.ToString();
                        Pracovni.UpravaNovaSlovicka(soubor, slovo, slovo, preklad, true, false, 0);
                    }
                }

                if (isForbidenFile)
                {
                    Pracovni.UlozeniDoSouboru(Pracovni.forbiddenWords, Pracovni.FORBIDDEN_WORDS);
                }

                ZnovuNacteniVsechSlov();
                fillTableByFile();
            }
            Cursor.Current = Cursors.Default;
        }


        /// <summary>
        /// Výběr pouze nově objevených slov....
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbOnlyNew_CheckedChanged(object sender, EventArgs e)
        {
            analyseAndFillTable();
        }


        /// <summary>
        /// Vybirani check boxu vyradit/nové
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vyberPouzeJednaMoznost(object sender, DataGridViewCellEventArgs e)
        {
            int selectedRowIndex = int.Parse(e.RowIndex.ToString());
            int action = e.ColumnIndex;


            selectRowForChange(action, selectedRowIndex);
        }


        /// <summary>
        /// Vyber slova a co s nim bude  Action - 1 uprava,  2 - odstranit
        /// </summary>
        /// <param name="e"></param>
        /// <param name="selectedRowIndex"></param>
        private void selectRowForChange(int action, int selectedRowIndex)
        {
            if (action == 1)  //uprava slova
            {
                bool? isChecked = (bool?)((DataGridViewCheckBoxCell)tblSearchWords.Rows[selectedRowIndex].Cells[1]).Value;              
                if (isChecked != true)
                {
                    tblSearchWords.Rows[selectedRowIndex].Cells[1].Value = true;
                    tblSearchWords.Rows[selectedRowIndex].Cells[2].Value = false;
                    changeRowColor(System.Drawing.Color.SkyBlue, selectedRowIndex);
                }
                else
                {
                    tblSearchWords.Rows[selectedRowIndex].Cells[1].Value = false;
                    changeRowColor(System.Drawing.Color.White, selectedRowIndex);
                }
            }
            else if (action == 2 && !tblSearchWords.Rows[selectedRowIndex].Cells[2].ReadOnly) //vyrazeni
            {
                
                bool? isChecked = (bool?)((DataGridViewCheckBoxCell)tblSearchWords.Rows[selectedRowIndex].Cells[2]).Value;
                if (isChecked != true)
                {
                    tblSearchWords.Rows[selectedRowIndex].Cells[2].Value = true;
                    tblSearchWords.Rows[selectedRowIndex].Cells[1].Value = false;
                    changeRowColor(System.Drawing.Color.Silver, selectedRowIndex);
                }
                else
                {
                    tblSearchWords.Rows[selectedRowIndex].Cells[2].Value = false;
                    changeRowColor(System.Drawing.Color.White, selectedRowIndex);
                }
            }

        }


        /// <summary>
        /// Změna barvy řádku        
        /// </summary>
        /// <param name="color"></param>
        /// <param name="row"></param>
        private void changeRowColor(Color color, int row)
        {
            for (int i = 0; i < 8; i++)
            {
                tblSearchWords.Rows[row].Cells[i].Style.BackColor = color;
            }
        }
        

        private void MainForm_Resize(object sender, EventArgs e)
        {
            int velikost = this.Height;
            tblSearchWords.Size = new System.Drawing.Size(tblSearchWords.Size.Width, (velikost - 220));
            tabHlavniPanel.Size = new System.Drawing.Size(tabHlavniPanel.Size.Width, (velikost - 130));
        }


        /// <summary>
        /// odchyt klavesovych zkratek na 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tblSearchWords_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'v')
            {
                selectRowForChange(1, tblSearchWords.CurrentRow.Index);
            }
            else if (e.KeyChar == ' ')
            {
                selectRowForChange(2, tblSearchWords.CurrentRow.Index);
            }
        }

        //zapnutí editace souboru
        private void chbEditaceFiles_CheckedChanged(object sender, EventArgs e)
        {
            if (chbEditaceFiles.Checked)
            {               
                changeEditable(EditableSituaon.Edit);

            }
            else
            {               
                changeEditable(EditableSituaon.Normal);
               
            }
        }


        //zmena souboru nové načtení
        private void cmbIntoFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chbEditaceFiles.Checked)
            {
                fillTableByFile();
            }
        }

        /// <summary>
        /// oznaceni radku kde se neco zmeni
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tblSearchWords_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 7)
            {                
                selectRowForChange(1, tblSearchWords.CurrentRow.Index);
                if ((bool?)((DataGridViewCheckBoxCell)tblSearchWords.Rows[tblSearchWords.CurrentRow.Index].Cells[1]).Value == false)
                {
                    selectRowForChange(1, tblSearchWords.CurrentRow.Index);
                }
            }
        }


        /// <summary>
        /// editační situace
        /// </summary>
        public enum EditableSituaon  { Normal = 0, Edit = 1, ShowFilter = 2};
       
        private void changeEditable(EditableSituaon editableSituation)
        {

            if (editableSituation == EditableSituaon.Edit)
            {

                txbKnowWords.Text = string.Empty;
                txbNewWords.Text = string.Empty;
                txbForbiddenWords.Text = string.Empty;

                //jednotlive vyskyty
                txbOneTotalWords.Text = string.Empty;
                txbOneKnowWords.Text = string.Empty;
                txbOneNewWords.Text = string.Empty;
                txbOneForbiddenWords.Text = string.Empty;

                tblSearchWords.Columns[0].Visible = false;
                tblSearchWords.Columns[5].Visible = false;

                btnSelectFile.Enabled = false;
                chbOnlyNew.Enabled = false;
                fillToFileComboBox(true);
                fillTableByFile();

            }
            else if (editableSituation == EditableSituaon.ShowFilter)
            {
                tblSearchWords.Columns[0].Visible = false;
                tblSearchWords.Columns[1].Visible = false;
                tblSearchWords.Columns[2].Visible = false;
                tblSearchWords.Columns[5].Visible = true;
                tblSearchWords.Columns[7].Visible = false;

                btnSelectFile.Enabled = false;
                chbOnlyNew.Enabled = false;
                cmbIntoFile.Enabled = false;
                chbEditaceFiles.Enabled = false;
                btnAnalyzeSave.Enabled = false;
                chbFiltrForbid.Visible = true;
                chbFiltrCarka.Visible = true;
                chbFiltrS.Visible = true;
                chbFilt1.Visible = true;
                chbFilt2.Visible = true;
                chbFilt3.Visible = true;

                fillTableByFilter();
            }
            else if (editableSituation == EditableSituaon.Normal)
            {
                tblSearchWords.Columns[0].Visible = true;
                tblSearchWords.Columns[1].Visible = true;
                tblSearchWords.Columns[2].Visible = true;
                tblSearchWords.Columns[5].Visible = true;
                tblSearchWords.Columns[7].Visible = true;

                btnSelectFile.Enabled = true;
                chbOnlyNew.Enabled = true;
                cmbIntoFile.Enabled = true;
                chbEditaceFiles.Enabled = true;
                btnAnalyzeSave.Enabled = true;
                chbEditaceFiles.Checked = false;
                chbFiltrForbid.Visible = false;
                chbFiltrCarka.Visible = false;
                chbFiltrS.Visible = false;
                chbFilt1.Visible = false;
                chbFilt2.Visible = false;
                chbFilt3.Visible = false;

                fillToFileComboBox(false);
                analyseAndFillTable();
            }
        }

        private void chbFilterShow_CheckedChanged(object sender, EventArgs e)
        {
            if (chbFilterShow.Checked)
            {
                changeEditable(EditableSituaon.ShowFilter);                
            }
            else
            {
                changeEditable(EditableSituaon.Normal);
            }
        }


        /// <summary>
        /// Zapnutí filtru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbFiltrForbid_CheckedChanged(object sender, EventArgs e)
        {
            fillTableByFilter();
        }
    }
}
