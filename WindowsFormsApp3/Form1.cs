using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private SqlConnection baglan = new SqlConnection(ConfigurationManager.ConnectionStrings["Connn"].ConnectionString);
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dateEdit1.EditValue = DateTime.Now;
            dateEdit2.EditValue = DateTime.Now;
            gridView1.OptionsBehavior.Editable = false;
            gridView2.OptionsBehavior.Editable = false;
            gridView1.OptionsView.ShowAutoFilterRow = true;
            gridView1.OptionsView.HeaderFilterButtonShowMode = FilterButtonShowMode.Button;
			
		}

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            string tarih = dateEdit1.DateTime.ToString("yyyy.MM.dd");
            string tarih2 = dateEdit2.DateTime.ToString("yyyy.MM.dd");

		
			baglan.Close();
			baglan.Open();
			string kayit = string.Concat(new string[]
			{
		"SELECT convert(VARCHAR(10),H.Tarih,102) as Tarih,s.Stok_Kodu,h.barkod as Barkod,s.Urun_Adi,sum(h.Adet) as Miktar,h.Fiyat as BirimFiyat,sum(tutar) as Toplam FROM POS_URUN S INNER JOIN HAREKET H ON S.Stok_Kodu = H.Stok_Kodu INNER JOIN BELGE B ON B.Belge_ID=H.Belge_ID where h.tarih >= '",
		tarih,
		" 00:00' and h.tarih <= '",
		tarih2,
		" 23:59' AND Iptal not in(1,2) group by s.Stok_Kodu,h.barkod,s.Urun_Adi,h.Fiyat,convert(VARCHAR(10),H.Tarih,102)"
			});
			try
			{
				SqlCommand kdvcmd = new SqlCommand(string.Concat(new string[]
				{
			"SELECT h.kdv as Kdv,sum(tutar) as Toplam FROM POS_URUN S INNER JOIN HAREKET H ON S.Stok_Kodu = H.Stok_Kodu INNER JOIN BELGE B ON B.Belge_ID = H.Belge_ID where h.tarih >= '",
			tarih,
			" 00:00' and h.tarih <= '",
			tarih2,
			" 23:59' AND Iptal not in (1, 2) group by h.Kdv"
				}), baglan);
				SqlDataAdapter kdvda = new SqlDataAdapter(kdvcmd);
				DataTable kdvdt = new DataTable();
				kdvda.Fill(kdvdt);
				gridControl2.DataSource = kdvdt;
			}
			catch (Exception)
			{
				throw;
			}	

			//Nakit
			try
			{
				string sorgu = string.Concat(new string[] { "SELECT SUM(B.Toplam) AS NAKIT  FROM ODEME O LEFT JOIN BELGE B ON B.Belge_ID=O.Belge_ID LEFT JOIN POS_KREDI P ON P.Tus_no=O.Tus_no LEFT JOIN SERVER_AFFILIATE SA ON SA.SERVERAFFILIATEID=B.Sube_No where  B.tarih BETWEEN '", tarih, " 00:00' AND '", tarih2, " 23:59' AND o.tus_no='0' AND B.Belge_tipi IN('FAT','FIS') AND B.Sube_No='1' and Iptal not in (1, 2)" });
				SqlCommand cmd = new SqlCommand(sorgu, baglan);
				SqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					labelControl8.Text = dr["NAKIT"].ToString();
				}
				dr.Close();
			}
			catch (Exception)
			{

				throw;
			}

			//K.kartı
			try
			{
				string sorgu = string.Concat(new string[] { "SELECT SUM(B.Toplam) AS KREDI  FROM ODEME O LEFT JOIN BELGE B ON B.Belge_ID=O.Belge_ID LEFT JOIN POS_KREDI P ON P.Tus_no=O.Tus_no LEFT JOIN SERVER_AFFILIATE SA ON SA.SERVERAFFILIATEID=B.Sube_No where  B.tarih BETWEEN'", tarih, " 00:00' AND'", tarih2, " 23:59' AND o.tus_no IN(1,2,3,4,5,6,7,8,9) AND B.Belge_tipi IN('FAT','FIS') AND B.Sube_No='1' and Iptal not in (1, 2)" });
				SqlCommand cmd = new SqlCommand(sorgu, baglan);
				SqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					labelControl6.Text = dr["KREDI"].ToString();
				}
				dr.Close();
			}
			catch (Exception)
			{

				throw;
			}

			//Toplam tutar
			try
			{
				string sorgu = string.Concat(new string[] { "SELECT SUM(B.Toplam) AS GENEL_CIRO  FROM ODEME O LEFT JOIN BELGE B ON B.Belge_ID=O.Belge_ID LEFT JOIN POS_KREDI P ON P.Tus_no=O.Tus_no where  B.tarih BETWEEN '", tarih, " 00:00' AND '", tarih2, " 23:59' AND o.tus_no IN(0,1,2,3,4,5,6,7,8,9) AND B.Belge_tipi IN('FAT','FIS')" });
				SqlCommand cmd = new SqlCommand(sorgu, baglan);
				SqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					labelControl2.Text = dr["GENEL_CIRO"].ToString();
				}
				dr.Close();
			}
			catch (Exception)
			{
				throw;
			}

			//Müşteri Sayısı
			try
			{
				string sorgu = string.Concat(new string[] { "SELECT COUNT(*) AS MUSTERISAYISI  FROM BELGE B LEFT JOIN SERVER_AFFILIATE SA ON SA.SERVERAFFILIATEID=B.Sube_No where  B.tarih BETWEEN '", tarih, " 00:00' AND '", tarih2, " 23:59' AND B.Sube_No='1'" });
				SqlCommand cmd = new SqlCommand(sorgu, baglan);
				SqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					labelControl10.Text = dr["MUSTERISAYISI"].ToString();
				}
				dr.Close();
			}
			catch (Exception)
			{
				throw;
			}

			try
			{
				SqlCommand komut = new SqlCommand(kayit, baglan);
				SqlDataAdapter da = new SqlDataAdapter(komut);
				DataTable dt = new DataTable();
				da.Fill(dt);
				gridControl1.DataSource = dt;
				baglan.Close();
			}
			catch (Exception)
			{
				throw;
			}
            

		}

        private void ribbonControl1_Click(object sender, EventArgs e)
        {

        }
    }
}
